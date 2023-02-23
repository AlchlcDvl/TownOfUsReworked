using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.TaskmasterMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    internal class TaskDone
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;

        private static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(ObjectifierEnum.Taskmaster))
                return;

            if (__instance.Data.IsDead)
                return;

            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var role = Objectifier.GetObjectifier<Taskmaster>(__instance);

            if (role == null)
                return;

            switch (tasksLeft)
            {
                case 1:
                    if (tasksLeft == CustomGameOptions.TMTasksRemaining)
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
                            renderer.sprite = Sprite;
                            arrow.image = renderer;
                            gameObj.layer = 5;
                            role.ImpArrows.Add(arrow);
                        }
                    }

                    break;
                case 0:
                    if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Taskmaster))
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                    
                    role.WinTasksDone = true;
                    break;
            }
        }
    }
}