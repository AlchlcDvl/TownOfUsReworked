namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
public static class RecomputeTaskCounts
{
    public static bool Prefix(GameData __instance)
    {
        if (IsHnS())
            return true;

        __instance.TotalTasks = 0;
        __instance.CompletedTasks = 0;

        if (IsTaskRace())
        {
            PlayerControl most = null;

            foreach (var playerInfo in __instance.AllPlayers)
            {
                var pc = playerInfo.Object;

                if (!pc || !playerInfo || playerInfo.Tasks == null || pc.HasDied())
                    continue;

                var mostRole = most.GetRole();
                var pcRole = pc.GetRole();

                if (!most || (pcRole && mostRole && pcRole.TasksLeft >= mostRole.TasksLeft))
                    most = pc;
            }

            var mostRole2 = most.GetRole();

            if (mostRole2)
                __instance.CompletedTasks = mostRole2.TasksCompleted;

            __instance.TotalTasks = TaskSettings.ShortTasks + TaskSettings.CommonTasks;
        }
        else if (IsCustomHnS())
        {
            foreach (var playerInfo in __instance.AllPlayers)
            {
                var pc = playerInfo.Object;

                if (!playerInfo.Disconnected && playerInfo.Tasks != null && pc.Is<Hunted>())
                {
                    foreach (var task in playerInfo.Tasks)
                    {
                        __instance.TotalTasks++;

                        if (task.Complete)
                            __instance.CompletedTasks++;
                    }
                }
            }
        }
        else
        {
            foreach (var playerInfo in __instance.AllPlayers)
            {
                var pc = playerInfo.Object;

                if (!playerInfo.Disconnected && playerInfo.Tasks != null && pc.CanDoTasks() && pc.Is(Faction.Crew) && !pc.Is<Revealer>() && (!playerInfo.IsDead ||
                    CrewSettings.GhostTasksCountToWin))
                {
                    foreach (var task in playerInfo.Tasks)
                    {
                        __instance.TotalTasks++;

                        if (task.Complete)
                            __instance.CompletedTasks++;
                    }
                }
            }
        }

        return false;
    }
}