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

                if (!playerInfo.Disconnected && playerInfo.Tasks != null && pc.Is(LayerEnum.Hunted))
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

                if (!playerInfo.Disconnected && playerInfo.Tasks != null && pc.CanDoTasks() && pc.Is(Faction.Crew) && !pc.Is(LayerEnum.Revealer) && (!playerInfo.IsDead ||
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

[HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__141), nameof(PlayerControl._CoSetTasks_d__141.MoveNext))]
public static class PlayerControl_SetTasks
{
    public static void Postfix(PlayerControl._CoSetTasks_d__141 __instance)
    {
        if (!IsHnS() && __instance.__1__state == -1)
            __instance.__4__this.RegenTask();
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
public static class CompleteTasksPatch
{
    public static void Postfix(PlayerControl __instance, uint idx)
    {
        if (__instance.Data.Role is LayerHandler handler)
            handler.UponTaskComplete(idx);

        if (HUD().TaskPanel)
        {
            var text = "";

            if (__instance.CanDoTasks())
            {
                var color = "FF00";
                var role = __instance.GetRole();

                if (role.TasksDone)
                    color = "00FF";
                else if (role.TasksCompleted > 0)
                    color = "FFFF";

                text = $"Tasks <#{color}00FF>({role.TasksCompleted}/{role.TotalTasks})</color>";
            }
            else
                text = "<#FF0000FF>Fake Tasks</color>";

            HUD().TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TextMeshPro>().SetText(text);
        }
    }
}