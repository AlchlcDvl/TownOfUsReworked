using System;
using HarmonyLib;
using Hazel;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PhantomMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => SetPhantom.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class SetPhantom
    {
        #pragma warning disable
        public static PlayerControl WillBePhantom;
        #pragma warning restore

        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.exiled?.Object;

            if (WillBePhantom?.Data.IsDead == false && exiled.Is(Faction.Neutral) && !LayerExtentions.NeutralHasUnfinishedBusiness(exiled))
                WillBePhantom = exiled;

            if (!PlayerControl.LocalPlayer.Data.IsDead && exiled != PlayerControl.LocalPlayer)
                return;

            if (PlayerControl.LocalPlayer != WillBePhantom)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
            {
                var former = Role.GetRole(PlayerControl.LocalPlayer);
                var role = new Phantom(PlayerControl.LocalPlayer);
                role.Player.RegenTask();
                role.RoleUpdate(former);
                RemoveTasks(PlayerControl.LocalPlayer);
                PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                Utils.LogSomething("Becoming");
                PlayerControl.LocalPlayer.gameObject.layer = LayerMask.NameToLayer("Players");
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PhantomDied, SendOption.Reliable);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            if (Role.GetRole<Phantom>(PlayerControl.LocalPlayer).Caught)
                return;

            var startingVent = ShipStatus.Instance.AllVents[Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];

            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPos, SendOption.Reliable);
            writer2.Write(PlayerControl.LocalPlayer.PlayerId);
            writer2.Write(startingVent.transform.position.x);
            writer2.Write(startingVent.transform.position.y + 0.3636f);
            AmongUsClient.Instance.FinishRpcImmediately(writer2);

            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(startingVent.transform.position.x, startingVent.transform.position.y + 0.3636f));
            PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance.currentNormalGameOptions.MapId != 5)
                return;

            if (obj.name.Contains("ExileCutscene"))
                ExileControllerPostfix(ConfirmEjects.lastExiled);
        }

        public static void RemoveTasks(PlayerControl player)
        {
            foreach (var task in player.myTasks)
            {
                if (task.TryCast<NormalPlayerTask>() != null)
                {
                    var normalPlayerTask = task.Cast<NormalPlayerTask>();

                    var updateArrow = normalPlayerTask.taskStep > 0;

                    normalPlayerTask.taskStep = 0;
                    normalPlayerTask.Initialize();

                    if (normalPlayerTask.TaskType == TaskTypes.PickUpTowels)
                    {
                        foreach (var console in Object.FindObjectsOfType<TowelTaskConsole>())
                            console.Image.color = Color.white;
                    }

                    normalPlayerTask.taskStep = 0;

                    if (normalPlayerTask.TaskType == TaskTypes.UploadData)
                        normalPlayerTask.taskStep = 1;

                    if ((normalPlayerTask.TaskType == TaskTypes.EmptyGarbage || normalPlayerTask.TaskType == TaskTypes.EmptyChute) &&
                        (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 0 || GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3 ||
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId == 4))
                    {
                        normalPlayerTask.taskStep = 1;
                    }

                    if (updateArrow)
                        normalPlayerTask.UpdateArrow();

                    var taskInfo = player.Data.FindTaskById(task.Id);
                    taskInfo.Complete = false;
                }
            }
        }
    }
}