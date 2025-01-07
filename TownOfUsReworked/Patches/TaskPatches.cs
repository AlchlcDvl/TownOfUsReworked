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

                if (!most || pcRole?.TasksLeft >= mostRole?.TasksLeft)
                    most = pc;
            }

            if (most.TryGetLayer<Role>(out var mostRole2))
                __instance.CompletedTasks = mostRole2.TasksCompleted;

            __instance.TotalTasks = TaskSettings.ShortTasks + TaskSettings.CommonTasks + TaskSettings.LongTasks;
        }
        else if (IsCustomHnS())
        {
            foreach (var playerInfo in __instance.AllPlayers)
            {
                var pc = playerInfo.Object;

                if (!playerInfo.Disconnected && playerInfo.Tasks != null && pc.Is<Hunted>() && (!playerInfo.IsDead || TaskSettings.GhostTasksCountToWin))
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
                    TaskSettings.GhostTasksCountToWin))
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

        CheckEndGame.CheckEnd();
        return false;
    }
}