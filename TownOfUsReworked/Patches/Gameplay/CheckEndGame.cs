namespace TownOfUsReworked.Patches.Gameplay;

[HarmonyPatch(typeof(LogicGameFlowNormal))]
public static class CheckEndGame
{
    [HarmonyPatch(nameof(LogicGameFlowNormal.CheckEndCriteria))]
    public static bool Prefix(LogicGameFlowNormal __instance)
    {
        CheckEnd(__instance);
        return false;
    }

    private static void CheckEnd(LogicGameFlowNormal __instance)
    {
        if (!AmongUsClient.Instance.AmHost)
            return;

        if (Sabotaged())
        {
            WinState = BadGuysSettings.IlluminatiUnleashed ? WinLose.IlluminatiWins : (BadGuysSettings.MainBadGuys switch
            {
                Faction.Intruder => WinLose.IntrudersWin,
                Faction.Compliance => WinLose.ComplianceWins,
                Faction.Syndicate => WinLose.SyndicateWins,
                Faction.Apocalypse => WinLose.ApocalypseWins,
                Faction.Pandorica => WinLose.PandoricaWins,
                _ => WinLose.IlluminatiWins,
            });
            var winners = AllPlayers().Where(x => x.Is(BadGuysSettings.MainBadGuys));
            winners.Do(x => LayerHandler.Handlers[x.PlayerId].Winner = true);
            CallRpc(CustomRPC.Misc, [MiscRPC.WinLose, WinState, .. winners]);
        }

        if (WinState == WinLose.None)
            return;

        if (IsFreePlay())
        {
            HUD().ShowPopUp("The game would have normally ended here, but since this is practice mode, we've reversed everything!");
            __instance.Manager.ReviveEveryoneFreeplay();
            WinState = WinLose.None;
            Ship().Begin();
        }
        else
            EndGame();
    }

    public static void CheckPlayerWins()
    {
        // Only run the subsequent checks if and only if no previous condition as been fulfilled
        var winnerIds = new HashSet<byte>();

        if (WinState == WinLose.None)
            DetectStalemate();

        if (WinState == WinLose.None)
            CheckFactionWin(winnerIds);

        if (WinState == WinLose.None)
            PlayerLayer.GetLayers<Role>().Do(x => x.GameEnd(winnerIds));

        if (WinState == WinLose.None)
            PlayerLayer.GetLayers<Disposition>().Do(x => x.GameEnd(winnerIds));

        if (WinState == WinLose.None)
            return;

        winnerIds.Do(x => LayerHandler.Handlers[x].Winner = true);
        CallRpc(CustomRPC.Misc, [ MiscRPC.WinLose, WinState, .. winnerIds ]);
    }

    public static void CheckSpellWin(Spellslinger hexer)
    {
        if (hexer.Player.HasDied())
            return;

        var faction = hexer.Faction;
        Func<PlayerControl, bool> factionCheck = faction switch
        {
            Faction.Outcast => hexer.Handler.CurrentDisposition switch
            {
                Mafia => x => x.Is(LayerEnum.Mafia),
                Lovers => x => x.IsOtherLover(hexer.Player),
                Linked => x => x.IsOtherLink(hexer.Player),
                _ => x => x == hexer.Player
            },
            _ => x => x.Is(faction)
        };

        var players = AllPlayers();

        if (hexer.Spelled.Count != players.Count(plr => !plr.HasDied() && !factionCheck(plr)))
            return;

        WinState = faction switch
        {
            Faction.Crew => WinLose.CrewWins,
            Faction.Intruder => WinLose.IntrudersWin,
            Faction.Illuminati => WinLose.IlluminatiWins,
            Faction.Pandorica => WinLose.PandoricaWins,
            Faction.Compliance => WinLose.ComplianceWins,
            Faction.Syndicate => WinLose.SyndicateWins,
            Faction.Apocalypse => WinLose.ApocalypseWins,
            Faction.Cabal => WinLose.CabalWins,
            Faction.Undead => WinLose.UndeadWins,
            Faction.Reanimated => WinLose.ReanimatedWins,
            Faction.Cult => WinLose.CultWins,
            Faction.Followers => WinLose.FollowersWin,
            Faction.Outcast => hexer.Player.GetDisposition() switch
            {
                Mafia => WinLose.MafiaWins,
                Lovers => WinLose.LoveWins,
                Defector => WinLose.DefectorWins,
                _ => WinLose.NobodyWins
            },
            _ => WinLose.NobodyWins
        };
        CallRpc(CustomRPC.Misc, [ MiscRPC.WinLose, WinState, .. players.Where(factionCheck) ]);
    }

    private static void CheckFactionWin(HashSet<byte> winnerIds)
    {
        var winner = FactionsToKill.FirstOrDefault(CheckFactionWin);

        if (winner == Faction.None)
            return;

        WinState = WinLoseGroupMappings[winner];
        winnerIds.AddRange(AllPlayers().Where(x => x.Is(winner)).Select(x => x.PlayerId));
    }

    private static readonly Faction[] FactionsToKill = [ .. Enum.GetValues<Faction>().Except([ Faction.None, Faction.Outcast, Faction.GameMode ]) ];
    private static readonly Dictionary<Enum, WinLose> WinLoseGroupMappings = FactionsToKill.Cast<Enum>()
        .ToDictionary(
            x => x,
            x => Enum.GetValues<WinLose>().FirstOrDefault(y => y.ToString().StartsWith(x.ToString())));

    private static bool CheckFactionWin(Faction faction)
    {
        switch (faction)
        {
            case Faction.Compliance when !BadGuysSettings.OrderOfCompliance:
            case Faction.Pandorica when !BadGuysSettings.PandoricaOpens:
            case Faction.Illuminati when !BadGuysSettings.IlluminatiUnleashed:
                return false;
        }

        var toKill = FactionsToKill.Except([ faction ]);

        return AllPlayers().Where(player => !player.HasDied()).All(player => !toKill.Any(player.Is) && (!player.Is(Faction.Outcast)));
    }

    // Stalemate detector for unwinnable situations
    private static void DetectStalemate()
    {
        var players = AllPlayers().Where(x => !x.HasDied());

        if (players.Count() == 2)
        {
            var player1 = players.First();
            var player2 = players.Last();
            var noButtons1 = player1.RemainingEmergencies == 0;
            var noButtons2 = player2.RemainingEmergencies == 0;
            var noButtons = noButtons1 && noButtons2;
            var knighted1 = player1.IsKnighted() || player1.Is<Tiebreaker>();
            var knighted2 = player2.IsKnighted() || player2.Is<Tiebreaker>();
            var neitherKnighted = (knighted1 && knighted2) || (!knighted1 && !knighted2);
            var canKill1 = player1.CanKill();
            var canKill2 = player2.CanKill();
            var cantKill = !(canKill1 || canKill2);
            var rival1 = player1.Is<Rivals>();
            var rival2 = player2.Is<Rivals>();
            var rivals = rival1 && rival2;

            // NK vs. NK when neither can kill each other and Outcasts don't win together
            if ((noButtons && neitherKnighted) || rivals || (cantKill && noButtons))
                PerformStalemate();
        }
        else if (!players.Any())
            PerformStalemate();
    }

    public static void PerformStalemate()
    {
        WinState = WinLose.NobodyWins;
        CallRpc(CustomRPC.Misc, MiscRPC.WinLose, WinState);
    }

    public static bool TasksDone()
    {
        if (TaskOptions.LongTasks + TaskOptions.CommonTasks + TaskOptions.ShortTasks == 0)
            return IsCustomHnS() || IsTaskRace();

        if (IsTaskRace())
            return PlayerLayer.GetLayers<Runner>().First(x => x.TasksDone);

        var allCrew = new List<PlayerControl>();
        var crewWithNoTasks = new List<PlayerControl>();

        foreach (var role in IsCustomHnS() ? PlayerLayer.GetLayers<Hunted>() : Role.GetRoles(Faction.Crew).Where(x => x.Player.CanDoTasks()))
        {
            if (role.Dead && !TaskOptions.GhostTasksCountToWin)
                continue;

            allCrew.Add(role.Player);

            if (role.TasksDone)
                crewWithNoTasks.Add(role.Player);
        }

        return allCrew.Count == crewWithNoTasks.Count;
    }

    private static bool Sabotaged()
    {
        if (!BadGuysSettings.MainBadGuysCanSabotage)
            return false;

        var ship = Ship();

        if (ship?.Systems is null)
            return false;

        foreach (var sab in ship.Systems.values)
        {
            if (sab.TryCast<LifeSuppSystemType>(out var life) && life.Countdown <= 0f)
                return true;

            if (sab.TryCast<ICriticalSabotage>(out var crit) && crit.Countdown <= 0f)
                return true;
        }

        return false;
    }

    [HarmonyPatch(nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    public static bool Prefix(ref bool __result) => __result = false;

    [HarmonyPatch(nameof(LogicGameFlowNormal.EndGameForSabotage))]
    public static bool Prefix() => false;
}

[HarmonyPatch(typeof(GameManager))]
public static class OverrideTaskEndGame
{
    [HarmonyPatch(nameof(GameManager.CheckEndGameViaTasks))]
    [HarmonyPatch(nameof(GameManager.CheckTaskCompletion))]
    public static bool Prefix(ref bool __result)
    {
        if (IsHnS())
            return true;

        GameData.Instance.RecomputeTaskCounts();
        return __result = false;
    }
}