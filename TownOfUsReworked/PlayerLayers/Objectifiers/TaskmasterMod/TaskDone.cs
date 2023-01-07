using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using System.Linq;
using Reactor.Utilities;
using UnityEngine;
using Hazel;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;

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
                        else if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKill))
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
                        
                    break;
            }

            if (tasksLeft == 0)
            {
                role.WinTasksDone = true;

                if (AmongUsClient.Instance.AmHost)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.TaskmasterWin,
                        SendOption.Reliable, -1);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
            }
        }
    }
}