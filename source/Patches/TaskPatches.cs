using HarmonyLib;
using TownOfUs.Extensions;

namespace TownOfUs
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
                    if (!playerInfo.Disconnected && playerInfo.Tasks != null && playerInfo.Object &&
                        (PlayerControl.GameOptions.GhostsDoTasks || !playerInfo.IsDead) && !playerInfo.IsImpostor() &&
                        !(playerInfo._object.Is(Alignment.NeutralKill) || playerInfo._object.Is(Alignment.NeutralBen) ||
                            playerInfo._object.Is(Alignment.PostmortalNeutral) || playerInfo._object.Is(RoleEnum.Executioner) ||
                            playerInfo._object.Is(Alignment.PostmortalCrew) || playerInfo._object.Is(RoleEnum.Jester) ||
                            playerInfo._object.Is(RoleEnum.Cannibal)))
                    {
                        for (var j = 0; j < playerInfo.Tasks.Count; j++)
                        {
                            __instance.TotalTasks++;
                            if (playerInfo.Tasks.ToArray()[j].Complete) __instance.CompletedTasks++;
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

                var flag = playerControl.Is(Alignment.NeutralKill)
                           || playerControl.Is(RoleEnum.Jester)
                           || playerControl.Is(RoleEnum.Executioner)
                           || playerControl.Is(RoleEnum.Cannibal)
                           || playerControl.Is(Alignment.NeutralBen)
                           || playerControl.Is(SubFaction.Vampires)
                           || (playerControl.Is(Faction.Crewmates) && playerControl.Is(ModifierEnum.Lover));

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