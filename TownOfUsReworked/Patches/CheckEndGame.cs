namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(LogicGameFlowNormal))]
public static class CheckEndGame
{
    [HarmonyPatch(nameof(LogicGameFlowNormal.CheckEndCriteria)), HarmonyPrefix]
    public static bool CheckEndCriteriaPrefix()
    {
        if (IsFreePlay() || IsHnS() || !AmongUsClient.Instance.AmHost)
            return false;

        if (WinState != WinLose.None)
        {
            EndGame();
            return false;
        }

        var spell = PlayerLayer.GetLayers<Spellslinger>().Find(x => !x.Dead && !x.Disconnected && x.Spelled.Count == AllPlayers().Count(y => !y.HasDied()));
        var reb = PlayerLayer.GetLayers<PromotedRebel>().Find(x => !x.Dead && !x.Disconnected && x.Spelled.Count == AllPlayers().Count(y => !y.HasDied()));

        if (TasksDone())
        {
            WinState = IsCustomHnS() ? WinLose.HuntedWin : WinLose.CrewWins;
            CallRpc(CustomRPC.WinLose, WinState);
        }
        else if (spell)
        {
            WinState = spell.Faction switch
            {
                Faction.Crew => WinLose.CrewWins,
                Faction.Intruder => WinLose.IntrudersWin,
                Faction.Neutral => NeutralSettings.NoSolo switch
                {
                    NoSolo.AllNeutrals => WinLose.AllNeutralsWin,
                    NoSolo.AllNKs => WinLose.AllNKsWin,
                    _ => spell.LinkedDisposition switch
                    {
                        LayerEnum.Mafia => WinLose.MafiaWins,
                        LayerEnum.Lovers => WinLose.LoveWins,
                        _ => WinLose.DefectorWins
                    }
                },
                Faction.Syndicate => WinLose.SyndicateWins,
                _ => WinLose.NobodyWins
            };

            CallRpc(CustomRPC.WinLose, WinState);
        }
        else if (reb)
        {
            WinState = reb.Faction switch
            {
                Faction.Crew => WinLose.CrewWins,
                Faction.Intruder => WinLose.IntrudersWin,
                Faction.Neutral => NeutralSettings.NoSolo switch
                {
                    NoSolo.AllNeutrals => WinLose.AllNeutralsWin,
                    NoSolo.AllNKs => WinLose.AllNKsWin,
                    _ => spell.LinkedDisposition switch
                    {
                        LayerEnum.Mafia => WinLose.MafiaWins,
                        LayerEnum.Lovers => WinLose.LoveWins,
                        _ => WinLose.DefectorWins
                    }
                },
                Faction.Syndicate => WinLose.SyndicateWins,
                _ => WinLose.NobodyWins
            };

            CallRpc(CustomRPC.WinLose, WinState);
        }
        else if (Sabotaged() && IntruderSettings.IntrudersCanSabotage)
        {
            WinState = SyndicateSettings.AltImps ? WinLose.SyndicateWins : WinLose.IntrudersWin;
            CallRpc(CustomRPC.WinLose, WinState);
        }
        else
        {
            Disposition.AllDispositions().ForEach(x => x.GameEnd());
            Role.AllRoles().ForEach(x => x.GameEnd());
            DetectStalemate();
        }

        return false;
    }

    // Stalemate detector for unwinnable situations
    private static void DetectStalemate()
    {
        var players = AllPlayers().Where(x => !x.HasDied()).ToList();

        if (players.Count == 2)
        {
            var player1 = players[0];
            var player2 = players[1];
            var nosolo = NeutralSettings.NoSolo == NoSolo.Never;
            var nobuttons1 = player1.RemainingEmergencies == 0;
            var nobuttons2 = player2.RemainingEmergencies == 0;
            var nobuttons = nobuttons1 && nobuttons2;
            var onehasbutton = !nobuttons1 || !nobuttons2;
            var knighted1 = player1.IsKnighted() || player1.Is(LayerEnum.Tiebreaker);
            var knighted2 = player2.IsKnighted() || player2.Is(LayerEnum.Tiebreaker);
            var neitherknighted = (knighted1 && knighted2) || (!knighted1 && !knighted2);
            var onisknighted = !knighted1 || !knighted2;
            var pol1 = player1.Is(LayerEnum.Politician);
            var pol2 = player2.Is(LayerEnum.Politician);
            var cankill1 = player1.CanKill();
            var cankill2 = player2.CanKill();
            var cantkill = !cankill1 && !cankill2;
            var rival1 = player1.Is(LayerEnum.Rivals);
            var rival2 = player2.Is(LayerEnum.Rivals);
            var rivals = rival1 && rival2;

            // NK vs NK when neither can kill each other and Neutrals don't win together
            if ((player1.Is(LayerEnum.Cryomaniac) && player2.Is(LayerEnum.Cryomaniac) && nosolo && nobuttons && neitherknighted) || NoOneWins() || rivals || (cantkill && nobuttons))
                PerformStalemate();
        }
        else if (players.Count == 0)
            PerformStalemate();
    }

    public static void PerformStalemate()
    {
        CallRpc(CustomRPC.WinLose, WinLose.NobodyWins);
        WinState = WinLose.NobodyWins;
    }

    private static bool TasksDone()
    {
        if (IsCustomHnS())
        {
            var allCrew = new List<PlayerControl>();
            var crewWithNoTasks = new List<PlayerControl>();

            foreach (var player in AllPlayers())
            {
                if (player.Is(LayerEnum.Hunted))
                {
                    allCrew.Add(player);

                    if (player.GetRole().TasksDone)
                        crewWithNoTasks.Add(player);
                }
            }

            return allCrew.Count == crewWithNoTasks.Count;
        }
        else
        {
            var crew = Role.GetRoles(Faction.Crew);

            if (crew.All(x => x.Dead && !CrewSettings.GhostTasksCountToWin) || !crew.Any(x => x.Player.CanDoTasks()))
                return false;

            var allCrew = new List<PlayerControl>();
            var crewWithNoTasks = new List<PlayerControl>();

            foreach (var role in crew)
            {
                var player = role.Player;

                if (player.CanDoTasks() && player.Is(Faction.Crew) && (!player.Data.IsDead || (player.Data.IsDead && CrewSettings.GhostTasksCountToWin)))
                {
                    allCrew.Add(player);

                    if (role.TasksDone)
                        crewWithNoTasks.Add(player);
                }
            }

            return allCrew.Count == crewWithNoTasks.Count;
        }
    }

    private static bool Sabotaged()
    {
        var systems = Ship().Systems;

        if (systems.TryGetValue(SystemTypes.LifeSupp, out var life))
        {
            var lifeSuppSystemType = life.TryCast<LifeSuppSystemType>();

            if (lifeSuppSystemType.Countdown <= 0f)
                return true;
        }
        else if (systems.TryGetValue(SystemTypes.Laboratory, out var lab))
        {
            var reactorSystemType = lab.TryCast<ReactorSystemType>();

            if (reactorSystemType.Countdown <= 0f)
                return true;
        }
        else if (systems.TryGetValue(SystemTypes.Reactor, out var reactor))
        {
            var reactorSystemType = reactor.TryCast<ICriticalSabotage>();

            if (reactorSystemType.Countdown <= 0f)
                return true;
        }
        else if (systems.TryGetValue(SystemTypes.HeliSabotage, out var heli))
        {
            var reactorSystemType = heli.TryCast<HeliSabotageSystem>();

            if (reactorSystemType.Countdown <= 0f)
                return true;
        }

        return false;
    }

    [HarmonyPatch(nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    public static bool Prefix(ref bool __result) => __result = false;

    [HarmonyPatch(nameof(LogicGameFlowNormal.EndGameForSabotage)), HarmonyPrefix]
    public static bool EndGameForSabotagePrefix() => false;
}

[HarmonyPatch(typeof(GameManager))]
public static class OverrideTaskEndGame1
{
    [HarmonyPatch(nameof(GameManager.CheckEndGameViaTasks)), HarmonyPrefix]
    public static bool Prefix1(ref bool __result)
    {
        GameData.Instance.RecomputeTaskCounts();
        return __result = IsHnS();
    }

    [HarmonyPatch(nameof(GameManager.CheckTaskCompletion)), HarmonyPrefix]
    public static bool Prefix2(ref bool __result)
    {
        if (TutorialManager.InstanceExists)
        {
            if (CustomPlayer.Local.myTasks.All(t => t.IsComplete))
            {
                HUD().ShowPopUp(TranslationController.Instance.GetString(StringNames.GameOverTaskWin));
                Ship().Begin();
            }
        }
        else
            GameData.Instance.RecomputeTaskCounts();

        return __result = IsHnS();
    }
}