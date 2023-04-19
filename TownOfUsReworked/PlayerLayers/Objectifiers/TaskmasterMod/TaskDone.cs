using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.TaskmasterMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public static class TaskDone
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(ObjectifierEnum.Taskmaster))
                return;

            if (__instance.Data.IsDead)
                return;

            var role = Objectifier.GetObjectifier<Taskmaster>(__instance);

            if (role.TasksLeft == CustomGameOptions.TMTasksRemaining)
            {
                if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Taskmaster))
                    Utils.Flash(role.Color);
                else if (PlayerControl.LocalPlayer.Is(Faction.Crew) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralBen) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralEvil))
                    Utils.Flash(role.Color);
                else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) || PlayerControl.LocalPlayer.Is(Faction.Syndicate) ||
                    PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralNeo) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralPros))
                {
                    Utils.Flash(role.Color);
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = __instance.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = AssetManager.Arrow;
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    role.ImpArrows.Add(arrow);
                }
            }
            else if (role.TasksDone)
            {
                if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Taskmaster))
                    Utils.Flash(role.Color);

                role.WinTasksDone = true;
            }
        }
    }
}