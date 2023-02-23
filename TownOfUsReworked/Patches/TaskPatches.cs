using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using System;
using UnityEngine;

namespace TownOfUsReworked.Patches
{
    internal static class TaskPatches
    {
        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        private class GameData_RecomputeTaskCounts
        {
            private static bool Prefix(GameData __instance)
            {
                __instance.TotalTasks = 0;
                __instance.CompletedTasks = 0;

                for (var i = 0; i < __instance.AllPlayers.Count; i++)
                {
                    var playerInfo = __instance.AllPlayers.ToArray()[i];

                    if (!playerInfo.Disconnected && playerInfo.Tasks != null && playerInfo.Object != null && (playerInfo._object.CanDoTasks() &&
                        !(playerInfo._object.Is(ObjectifierEnum.Lovers) || (playerInfo._object.Is(RoleEnum.Revealer) && playerInfo.IsDead))))
                    {
                        for (var j = 0; j < playerInfo.Tasks.Count; j++)
                        {
                            __instance.TotalTasks++;

                            if (playerInfo.Tasks.ToArray()[j].Complete)
                                __instance.CompletedTasks++;
                        }
                    }
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
        private class Console_CanUse
        {
            private static bool Prefix(Console __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref float __result)
            {
                var playerControl = playerInfo.Object;

                var flag = !playerControl.CanDoTasks();

                //If the console is not a sabotage repair console
                if (flag && !__instance.AllowImpostor)
                {
                    __result = float.MaxValue;
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__113), nameof(PlayerControl._CoSetTasks_d__113.MoveNext))]
        private static class PlayerControl_SetTasks
        {
            private static void Postfix(PlayerControl._CoSetTasks_d__113 __instance)
            {
                if (__instance == null)
                    return;

                var player = __instance.__4__this;
                var text = player.GetTaskList();

                try
                {
                    var firstText = player.myTasks.ToArray()[0].Cast<ImportantTextTask>();

                    if (firstText.Text.Contains("Sabotage and kill everyone"))
                        player.myTasks.Remove(firstText);

                    firstText = player.myTasks.ToArray()[0].Cast<ImportantTextTask>();
                    
                    if (firstText.Text.Contains("Fake"))
                        player.myTasks.Remove(firstText);
                } catch (InvalidCastException) {}
                
                var task = new GameObject("DetailTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = text;
                player.myTasks.Insert(0, task);
            }
        }
    }
}