using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.TaskmasterMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    internal class TaskDone
    {
        private static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(ObjectifierEnum.Taskmaster))
                return;

            if (__instance.Data.IsDead)
                return;

            var role = Objectifier.GetObjectifier<Taskmaster>(__instance);

            if (role == null)
                return;

            if (role.TasksLeft == CustomGameOptions.TMTasksRemaining)
            {
                if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Taskmaster))
                    Coroutines.Start(Utils.FlashCoroutine(Color.green));
                else if (PlayerControl.LocalPlayer.Is(Faction.Crew))
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));
                else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill) || PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                {
                    Coroutines.Start(Utils.FlashCoroutine(role.Color));
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

            if (role.TasksDone)
            {
                if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Taskmaster))
                    Coroutines.Start(Utils.FlashCoroutine(Color.green));
                
                role.WinTasksDone = true;
            }
        }
    }
}