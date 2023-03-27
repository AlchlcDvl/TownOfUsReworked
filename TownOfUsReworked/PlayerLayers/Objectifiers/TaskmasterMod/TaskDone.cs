using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using UnityEngine;

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
                    Utils.Flash(role.Color, "You are almost done with tasks!");
                else if (PlayerControl.LocalPlayer.Is(Faction.Crew) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralBen) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralEvil))
                    Utils.Flash(role.Color, "There is a <color=#ABABFFFF>Taskmaster</color> on the loose!");
                else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) || PlayerControl.LocalPlayer.Is(Faction.Syndicate) ||
                    PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralNeo) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralPros))
                {
                    Utils.Flash(role.Color, "There is a <color=#ABABFFFF>Taskmaster</color> on the loose!");
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
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
                    Utils.Flash(role.Color, "Done!");

                role.WinTasksDone = true;
            }
        }
    }
}