namespace TownOfUsReworked.Patches;

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
            WinState = GameModifiers.IlluminatiUnleashed ? WinLose.IlluminatiWins : (BadGuysSettings.MainBadGuys switch
            {
                Faction.Intruder => WinLose.IntrudersWin,
                Faction.Compliance => WinLose.ComplianceWins,
                Faction.Syndicate => WinLose.SyndicateWins,
                Faction.Apocalypse => WinLose.ApocalypseWins,
                Faction.Pandorica => WinLose.PandoricaWins,
                _ => WinLose.IlluminatiWins,
            });
            var winners = AllPlayers().Where(x => x.Is(BadGuysSettings.MainBadGuys));
            winners.ForEach(x => x.GetLayers().ForEach(y => y.Winner = true));
            CallRpc(CustomRPC.Misc, [ MiscRPC.WinLose, WinState, .. winners.Distinct() ]);
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
        var winnerIds = new List<byte>();

        if (WinState == WinLose.None)
            DetectStalemate();

        if (WinState == WinLose.None)
            CheckFactionWin(winnerIds);

        if (WinState == WinLose.None)
            CheckSubFactionWin(winnerIds);

        if (WinState == WinLose.None)
            CheckNeutralKillers(winnerIds);

        if (WinState == WinLose.None)
            PlayerLayer.GetLayers<Role>().ForEach(x => x.GameEnd(winnerIds));

        if (WinState == WinLose.None)
            PlayerLayer.GetLayers<Disposition>().ForEach(x => x.GameEnd(winnerIds));

        if (WinState != WinLose.None)
        {
            winnerIds.Select(x => PlayerById(x)).ForEach(x => x.GetLayers().ForEach(y => y.Winner = true));
            CallRpc(CustomRPC.Misc, [ MiscRPC.WinLose, WinState, .. winnerIds.Distinct() ]);
        }
    }

    public static void CheckSpellWin(Spellslinger hexer)
    {
        if (hexer.Player.HasDied())
            return;

        var faction = hexer.Faction;
        Func<PlayerControl, bool> factionCheck = faction switch
        {
            Faction.Syndicate or Faction.Crew or Faction.Intruder or Faction.Illuminati or Faction.Compliance or Faction.Pandorica => x => x.Is(faction),
            _ => hexer.LinkedDisposition switch
            {
                LayerEnum.Mafia => x => x.Is(LayerEnum.Mafia),
                LayerEnum.Lovers => x => x.IsOtherLover(hexer.Player),
                _ => x => x == hexer.Player
            }
        };

        var players = AllPlayers();

        if (hexer.Spelled.Count == players.Count(plr => !plr.HasDied() && !factionCheck(plr)))
        {
            WinState = hexer.Faction switch
            {
                Faction.Crew => WinLose.CrewWins,
                Faction.Intruder => WinLose.IntrudersWin,
                Faction.Illuminati => WinLose.IlluminatiWins,
                Faction.Pandorica => WinLose.PandoricaWins,
                Faction.Compliance => WinLose.ComplianceWins,
                Faction.Syndicate => WinLose.SyndicateWins,
                Faction.Apocalypse => WinLose.ApocalypseWins,
                Faction.Neutral => hexer.Player.GetDisposition() switch
                {
                    Mafia => WinLose.MafiaWins,
                    Lovers => WinLose.LoveWins,
                    Defector => WinLose.DefectorWins,
                    _ => WinLose.NobodyWins
                },
                _ => WinLose.NobodyWins
            };
        }
    }

    private static void CheckFactionWin(List<byte> winnerIds)
    {
        var winner = FactionsToKill.FirstOrDefault(CheckFactionWin);

        if (winner != Faction.None)
        {
            WinState = WinLoseGroupMappings[winner];
            winnerIds.AddRange(AllPlayers().Where(x => x.Is(winner)).Select(x => x.PlayerId));
        }
    }

    private static void CheckSubFactionWin(List<byte> winnerIds)
    {
        var winner = SubFactionsToKill.FirstOrDefault(CheckSubFactionWin);

        if (winner != SubFaction.None)
        {
            WinState = WinLoseGroupMappings[winner];
            winnerIds.AddRange(AllPlayers().Where(x => x.Is(winner)).Select(x => x.PlayerId));
        }
    }

    private static void CheckNeutralKillers(List<byte> winnerIds)
    {
        if (GameModifiers.OrderOfCompliance && GameModifiers.ComplianceType == ComplianceType.Killers)
            return;

        var winner = NksToKill.FirstOrDefault(CheckNkWin);

        if (winner != LayerEnum.None)
        {
            WinState = WinLoseGroupMappings[winner];
            winnerIds.AddRange(AllPlayers().Where(x => x.Is(winner)).Select(x => x.PlayerId));
        }
    }

    private static readonly Faction[] FactionsToKill = [ .. Enum.GetValues<Faction>().Except([ Faction.None, Faction.Neutral, Faction.GameMode ]) ];
    private static readonly Alignment[] AlignmentsToKill = [ Alignment.Killing, Alignment.Neophyte, Alignment.Proselyte ];
    private static readonly LayerEnum[] NksToKill = [ LayerEnum.Arsonist, LayerEnum.Cryomaniac, LayerEnum.Glitch, LayerEnum.Juggernaut, LayerEnum.Murderer, LayerEnum.Werewolf, LayerEnum.Betrayer,
        LayerEnum.SerialKiller ];
    private static readonly SubFaction[] SubFactionsToKill = [ .. Enum.GetValues<SubFaction>().Except([ SubFaction.None ]) ];
    private static readonly Dictionary<Enum, WinLose> WinLoseGroupMappings = FactionsToKill.Cast<Enum>()
        .Concat(SubFactionsToKill.Cast<Enum>())
        .Concat(NksToKill.Cast<Enum>())
        .ToDictionary(
            x => x,
            x => Enum.GetValues<WinLose>().FirstOrDefault(y => y.ToString().StartsWith(x.ToString())));

    private static bool CheckFactionWin(Faction faction)
    {
        if (faction == Faction.Compliance && !GameModifiers.OrderOfCompliance)
            return false;

        if (faction == Faction.Pandorica && !GameModifiers.PandoricaOpens)
            return false;

        if (faction == Faction.Illuminati && !GameModifiers.IlluminatiUnleashed)
            return false;

        var toKill = FactionsToKill.Except([ faction ]);

        foreach (var player in AllPlayers())
        {
            if (player.HasDied())
                continue;

            if (toKill.Any(player.Is) || SubFactionsToKill.Any(player.Is) || (player.Is(Faction.Neutral) && AlignmentsToKill.Any(player.Is)))
                return false;
        }

        return true;
    }

    private static bool CheckSubFactionWin(SubFaction subFaction)
    {
        var toKill = SubFactionsToKill.Except([ subFaction ]);

        foreach (var player in AllPlayers())
        {
            if (player.HasDied())
                continue;

            if (FactionsToKill.Any(player.Is) || toKill.Any(player.Is) || (player.Is(Faction.Neutral) && AlignmentsToKill.Any(player.Is) && !player.Is(subFaction)))
                return false;
        }

        return true;
    }

    private static bool CheckNkWin(LayerEnum nk)
    {
        var toKill = NksToKill.Except([ nk ]);
        var nonNk = AlignmentsToKill.Except([ Alignment.Killing ]);
        var count = 0;

        foreach (var player in AllPlayers())
        {
            if (player.HasDied())
                continue;

            if (FactionsToKill.Any(player.Is) || toKill.Any(player.Is) || (player.Is(Faction.Neutral) && nonNk.Any(player.Is)))
                return false;

            if (player.Is(nk))
                count++;
        }

        return NeutralKillingSettings.WinSolo ? count == 1 : count > 1;
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

            // NK vs. NK when neither can kill each other and Neutrals don't win together
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
        if (TaskSettings.LongTasks + TaskSettings.CommonTasks + TaskSettings.ShortTasks == 0)
            return IsCustomHnS() || IsTaskRace();

        if (IsTaskRace())
            return PlayerLayer.GetLayers<Runner>().First(x => x.TasksDone);

        var allCrew = new List<PlayerControl>();
        var crewWithNoTasks = new List<PlayerControl>();

        foreach (var role in IsCustomHnS() ? PlayerLayer.GetLayers<Hunted>() : Role.GetRoles(Faction.Crew).Where(x => x.Player.CanDoTasks()))
        {
            if (role.Dead && !TaskSettings.GhostTasksCountToWin)
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

        if (ship?.Systems == null)
            return false;

        foreach (var sab in ship.Systems.Values)
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