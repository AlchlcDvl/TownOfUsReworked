namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
public static class RecomputeTaskCounts
{
    public static bool Prefix(GameData __instance)
    {
        if (IsHnS)
            return true;

        __instance.TotalTasks = 0;
        __instance.CompletedTasks = 0;

        if (IsTaskRace)
        {
            PlayerControl most = null;

            foreach (var playerInfo in __instance.AllPlayers)
            {
                var pc = playerInfo.Object;

                if (pc == null || playerInfo == null || playerInfo.Tasks == null || pc.HasDied())
                    continue;

                var mostRole = most.GetRole();
                var pcRole = pc.GetRole();

                if (most == null || (pcRole && mostRole && mostRole.TasksLeft >= pcRole.TasksLeft))
                    most = pc;
            }

            var mostRole2 = most.GetRole();

            if (mostRole2)
                __instance.CompletedTasks = mostRole2.TasksCompleted;

            __instance.TotalTasks = CustomGameOptions.ShortTasks + CustomGameOptions.CommonTasks;
        }
        else if (IsCustomHnS)
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
                    CustomGameOptions.GhostTasksCountToWin))
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

[HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__126), nameof(PlayerControl._CoSetTasks_d__126.MoveNext))]
public static class PlayerControl_SetTasks
{
    public static void Postfix(PlayerControl._CoSetTasks_d__126 __instance)
    {
        if (__instance != null && !IsHnS)
            __instance.__4__this.RegenTask();
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
public static class CompleteTasksPatch
{
    public static void Postfix(PlayerControl __instance, ref uint idx)
    {
        var id = idx;
        PlayerLayer.AllLayers.ForEach(x => x.UponTaskComplete(__instance, id));

        if (__instance.Is(LayerEnum.Snitch) && !__instance.Data.IsDead)
        {
            var role = __instance.GetAbility<Snitch>();

            if (role.TasksLeft == CustomGameOptions.SnitchTasksRemaining)
            {
                if (CustomPlayer.Local == __instance)
                    Flash(role.Color);
                else if (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || (CustomPlayer.Local.Is(Alignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals))
                {
                    Flash(role.Color);
                    Role.LocalRole.AllArrows.Add(role.PlayerId, new(CustomPlayer.Local, role.Color));
                }
            }
            else if (CustomPlayer.Local.Is(LayerEnum.Snitch) && role.TasksDone)
            {
                if (CustomPlayer.Local == __instance)
                {
                    Flash(UColor.green);
                    CustomPlayer.AllPlayers.Where(x => x.GetFaction() is Faction.Intruder or Faction.Syndicate || (x.Is(Alignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals))
                        .ForEach(x => Role.LocalRole.AllArrows.Add(x.PlayerId, new(__instance, role.Color)));
                }
                else if (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || (CustomPlayer.Local.Is(Alignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals))
                    Flash(UColor.red);
            }
        }

        if (__instance.Is(LayerEnum.Traitor) && !__instance.Data.IsDead && AmongUsClient.Instance.AmHost)
        {
            var traitor = __instance.GetObjectifier<Traitor>();

            if (traitor.TasksDone)
            {
                Traitor.GetFactionChoice(out var syndicate, out var intruder);
                CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, false, traitor, syndicate, intruder);
                traitor.TurnTraitor(syndicate, intruder);
            }
        }
        else if (__instance.Is(LayerEnum.Taskmaster) && !__instance.Data.IsDead)
        {
            var role = __instance.GetObjectifier<Taskmaster>();

            if (role.TasksLeft == CustomGameOptions.TMTasksRemaining)
            {
                if (CustomPlayer.Local == __instance || CustomPlayer.Local.Is(Faction.Crew) || CustomPlayer.Local.GetAlignment() is Alignment.NeutralBen or Alignment.NeutralEvil)
                    Flash(role.Color);
                else if (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || CustomPlayer.Local.GetAlignment() is Alignment.NeutralKill or Alignment.NeutralNeo or
                    Alignment.NeutralPros)
                {
                    Flash(role.Color);
                    Role.LocalRole.AllArrows.Add(role.PlayerId, new(CustomPlayer.Local, role.Color));
                }
            }
            else if (role.TasksDone)
            {
                if (CustomPlayer.Local.Is(LayerEnum.Taskmaster))
                    Flash(role.Color);

                role.WinTasksDone = true;
            }
        }

        if (__instance.Is(LayerEnum.Phantom))
        {
            var role = __instance.GetRole<Phantom>();

            if (role.TasksLeft == CustomGameOptions.PhantomTasksRemaining && CustomGameOptions.PhantomPlayersAlerted && !role.Caught)
                Flash(role.Color);
        }
        else if (__instance.Is(LayerEnum.Runner))
        {
            var role = __instance.GetRole<Runner>();

            if (role.TasksLeft == 1)
                Flash(role.Color);
        }
        else if (__instance.Is(LayerEnum.Revealer))
        {
            var role = __instance.GetRole<Revealer>();

            if (role.TasksLeft == CustomGameOptions.RevealerTasksRemainingAlert && !role.Caught)
            {
                if (CustomPlayer.Local.Is(LayerEnum.Revealer))
                    Flash(role.Color);
                else if (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || (CustomPlayer.Local.Is(Alignment.NeutralKill) &&
                    CustomGameOptions.RevealerRevealsNeutrals))
                {
                    role.Revealed = true;
                    Flash(role.Color);
                    Role.LocalRole.DeadArrows.Add(role.PlayerId, new(CustomPlayer.Local, role.Color));
                }
            }
            else if (role.TasksDone && !role.Caught)
            {
                if (CustomPlayer.Local.Is(LayerEnum.Revealer) || CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || (CustomPlayer.Local.Is(Alignment.NeutralKill) &&
                    CustomGameOptions.RevealerRevealsNeutrals))
                {
                    Flash(role.Color);
                }
            }
        }

        foreach (var button in AllButtons.Where(x => x.Owner.Player == __instance))
        {
            if (button.HasUses)
            {
                button.Uses++;
                button.MaxUses++;
            }
        }
    }
}