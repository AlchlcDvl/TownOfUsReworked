using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.Patches;
using Random = UnityEngine.Random;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GhoulMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => SetGhoul.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class SetGhoul
    {
        #pragma warning disable
        public static PlayerControl WillBeGhoul;
        #pragma warning restore

        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.exiled?.Object;

            if (WillBeGhoul?.Data.IsDead == false && exiled.Is(Faction.Intruder))
                WillBeGhoul = exiled;

            if (!PlayerControl.LocalPlayer.Data.IsDead && exiled != PlayerControl.LocalPlayer)
                return;

            if (PlayerControl.LocalPlayer != WillBeGhoul)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Ghoul))
            {
                var former = Role.GetRole(PlayerControl.LocalPlayer);
                var role = new Ghoul(PlayerControl.LocalPlayer);
                role.Player.RegenTask();
                role.RoleUpdate(former);
                PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                PlayerControl.LocalPlayer.gameObject.layer = LayerMask.NameToLayer("Players");
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GhoulDied, SendOption.Reliable);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            if (Role.GetRole<Ghoul>(PlayerControl.LocalPlayer).Caught)
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
                ExileControllerPostfix(MiscPatches.ExileControllerPatch.lastExiled);
        }
    }
}