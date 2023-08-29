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

        return false;
    }
}

[HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
public static class CanUse
{
    public static bool Prefix(Console __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref float __result)
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

[HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__114), nameof(PlayerControl._CoSetTasks_d__114.MoveNext))]
public static class PlayerControl_SetTasks
{
    public static void Postfix(PlayerControl._CoSetTasks_d__114 __instance)
    {
        if (__instance == null || IsHnS)
            return;

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
            var role = Ability.GetAbility<Snitch>(__instance);

            if (role.TasksLeft == CustomGameOptions.SnitchTasksRemaining)
            {
                if (CustomPlayer.Local == __instance)
                    Flash(role.Color);
                else if (CustomPlayer.Local.Is(Faction.Intruder) || (CustomPlayer.Local.Is(RoleAlignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals) ||
                    CustomPlayer.Local.Is(Faction.Syndicate))
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
                    CustomPlayer.AllPlayers.Where(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || (x.Is(RoleAlignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals))
                        .ToList().ForEach(x => Role.LocalRole.AllArrows.Add(x.PlayerId, new(__instance, role.Color)));
                }
                else if (CustomPlayer.Local.Is(Faction.Intruder) || (CustomPlayer.Local.Is(RoleAlignment.NeutralKill) && CustomGameOptions.SnitchSeesNeutrals) ||
                    CustomPlayer.Local.Is(Faction.Syndicate))
                {
                    Flash(UColor.red);
                }
            }
        }

        if (__instance.Is(LayerEnum.Traitor) && !__instance.Data.IsDead && AmongUsClient.Instance.AmHost)
        {
            var traitor = Objectifier.GetObjectifier<Traitor>(__instance);

            if (traitor.TasksDone)
            {
                Traitor.GetFactionChoice(out var syndicate, out var intruder);
                CallRpc(CustomRPC.Change, TurnRPC.TurnTraitor, traitor, syndicate, intruder);
                traitor.TurnTraitor(syndicate, intruder);
            }
        }
        else if (__instance.Is(LayerEnum.Taskmaster) && !__instance.Data.IsDead)
        {
            var role = Objectifier.GetObjectifier<Taskmaster>(__instance);

            if (role.TasksLeft == CustomGameOptions.TMTasksRemaining)
            {
                if (CustomPlayer.Local == __instance || CustomPlayer.Local.Is(Faction.Crew) || CustomPlayer.Local.GetAlignment() is RoleAlignment.NeutralBen or
                    RoleAlignment.NeutralEvil)
                {
                    Flash(role.Color);
                }
                else if (CustomPlayer.Local.Is(Faction.Intruder) || CustomPlayer.Local.Is(RoleAlignment.NeutralKill) || CustomPlayer.Local.Is(Faction.Syndicate) ||
                    CustomPlayer.Local.Is(RoleAlignment.NeutralNeo) || CustomPlayer.Local.Is(RoleAlignment.NeutralPros))
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
            var role = Role.GetRole<Phantom>(__instance);

            if (role.TasksLeft == CustomGameOptions.PhantomTasksRemaining && CustomGameOptions.PhantomPlayersAlerted && !role.Caught)
                Flash(role.Color);

            if (role.TasksDone && !role.Caught)
                role.CompletedTasks = true;
        }
        else if (__instance.Is(LayerEnum.Revealer))
        {
            var role = Role.GetRole<Revealer>(__instance);

            if (role.TasksLeft == CustomGameOptions.RevealerTasksRemainingAlert && !role.Caught)
            {
                if (CustomPlayer.Local.Is(LayerEnum.Revealer))
                    Flash(role.Color);
                else if (CustomPlayer.Local.Is(Faction.Intruder) || CustomPlayer.Local.Is(Faction.Syndicate) || (CustomPlayer.Local.Is(RoleAlignment.NeutralKill) &&
                    CustomGameOptions.RevealerRevealsNeutrals))
                {
                    role.Revealed = true;
                    Flash(role.Color);
                    Role.LocalRole.DeadArrows.Add(role.PlayerId, new(CustomPlayer.Local, role.Color));
                }
            }
            else if (role.TasksDone && !role.Caught)
            {
                role.CompletedTasks = role.TasksDone;

                if (CustomPlayer.Local.Is(LayerEnum.Revealer) || CustomPlayer.Local.Is(Faction.Intruder) || CustomPlayer.Local.Is(Faction.Syndicate) ||
                    (CustomPlayer.Local.Is(RoleAlignment.NeutralKill) && CustomGameOptions.RevealerRevealsNeutrals))
                {
                    Flash(role.Color);
                }
            }
        }

        var role1 = Role.GetRole(__instance);

        if (role1.TasksDone)
        {
            if (__instance.Is(LayerEnum.Tracker))
                ((Tracker)role1).UsesLeft++;
            else if (__instance.Is(LayerEnum.Transporter))
                ((Transporter)role1).UsesLeft++;
            else if (__instance.Is(LayerEnum.Vigilante))
                ((Vigilante)role1).UsesLeft++;
            else if (__instance.Is(LayerEnum.Veteran))
                ((Veteran)role1).UsesLeft++;
            else if (__instance.Is(LayerEnum.Altruist))
                ((Altruist)role1).UsesLeft++;
            else if (__instance.Is(LayerEnum.Monarch))
                ((Monarch)role1).UsesLeft++;
            else if (__instance.Is(LayerEnum.Chameleon))
                ((Chameleon)role1).UsesLeft++;
            else if (__instance.Is(LayerEnum.Engineer))
                ((Engineer)role1).UsesLeft++;
            else if (__instance.Is(LayerEnum.Retributionist))
                ((Retributionist)role1).UsesLeft++;
        }
    }
}