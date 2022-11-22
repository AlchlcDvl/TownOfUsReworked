using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;

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

                    if (!playerInfo.Disconnected && playerInfo.Tasks != null && playerInfo.Object && (PlayerControl.GameOptions.GhostsDoTasks ||
                        !playerInfo.IsDead) && !playerInfo.IsImpostor() && !(playerInfo._object.Is(RoleEnum.Jester) ||
                        playerInfo._object.Is(RoleEnum.Amnesiac) || playerInfo._object.Is(RoleEnum.Survivor) ||
                        playerInfo._object.Is(RoleEnum.GuardianAngel) || playerInfo._object.Is(RoleEnum.Executioner) ||
                        playerInfo._object.Is(RoleAlignment.NeutralKill) || playerInfo._object.Is(ObjectifierEnum.Phantom) ||
                        playerInfo._object.Is(AbilityEnum.Revealer)))

                        for (var j = 0; j < playerInfo.Tasks.Count; j++)
                        {
                            __instance.TotalTasks++;

                            if (playerInfo.Tasks.ToArray()[j].Complete)
                                __instance.CompletedTasks++;
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

                var flag = playerControl.Is(RoleEnum.Jester) || playerControl.Is(RoleEnum.Executioner) || playerControl.Is(RoleEnum.Survivor)
                    || playerControl.Is(RoleEnum.GuardianAngel) || playerControl.Is(Faction.Syndicate) || playerControl.Is(RoleAlignment.NeutralKill);

                // If the console is not a sabotage repair console
                if (flag && !__instance.AllowImpostor)
                {
                    __result = float.MaxValue;
                    return false;
                }

                return true;
            }
        }
    }
}