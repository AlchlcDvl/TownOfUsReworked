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

        var hex = PlayerLayer.GetILayers<IHexer>().Find(x => !x.Dead && !x.Disconnected && x.Spelled.Count == AllPlayers().Count(y => !y.HasDied()));

        if (TasksDone())
        {
            WinState = IsCustomHnS() ? WinLose.HuntedWin : WinLose.CrewWins;
            CallRpc(CustomRPC.WinLose, WinState);
        }
        else if (Sabotaged() && IntruderSettings.IntrudersCanSabotage)
        {
            WinState = SyndicateSettings.AltImps ? WinLose.SyndicateWins : WinLose.IntrudersWin;
            CallRpc(CustomRPC.WinLose, WinState);
        }
        else if (hex != null)
        {
            WinState = hex.Faction switch
            {
                Faction.Crew => WinLose.CrewWins,
                Faction.Intruder => WinLose.IntrudersWin,
                Faction.Neutral => NeutralSettings.NoSolo switch
                {
                    NoSolo.AllNeutrals => WinLose.AllNeutralsWin,
                    NoSolo.AllNKs => WinLose.AllNKsWin,
                    _ => hex.LinkedDisposition switch
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
        else
        {
            DetectStalemate();
            PlayerLayer.GetLayers<Role>().ForEach(x => x.GameEnd());
            PlayerLayer.GetLayers<Disposition>().ForEach(x => x.GameEnd());
        }

        return false;
    }

    // Stalemate detector for unwinnable situations
    private static void DetectStalemate()
    {
        var players = AllPlayers().Where(x => !x.HasDied());

        if (players.Count() == 2)
        {
            var player1 = players.First();
            var player2 = players.Last();
            var nosolo = NeutralSettings.NoSolo == NoSolo.Never;
            var nobuttons1 = player1.RemainingEmergencies == 0;
            var nobuttons2 = player2.RemainingEmergencies == 0;
            var nobuttons = nobuttons1 && nobuttons2;
            var onehasbutton = !nobuttons1 || !nobuttons2;
            var knighted1 = player1.IsKnighted() || player1.Is<Tiebreaker>();
            var knighted2 = player2.IsKnighted() || player2.Is<Tiebreaker>();
            var neitherknighted = (knighted1 && knighted2) || (!knighted1 && !knighted2);
            var onisknighted = !knighted1 || !knighted2;
            var pol1 = player1.Is<Politician>();
            var pol2 = player2.Is<Politician>();
            var cankill1 = player1.CanKill();
            var cankill2 = player2.CanKill();
            var cantkill = !cankill1 && !cankill2;
            var rival1 = player1.Is<Rivals>();
            var rival2 = player2.Is<Rivals>();
            var rivals = rival1 && rival2;

            // NK vs NK when neither can kill each other and Neutrals don't win together
            if ((player1.Is<Cryomaniac>() && player2.Is<Cryomaniac>() && nosolo && nobuttons && neitherknighted) || rivals || (cantkill && nobuttons))
                PerformStalemate();
        }
        else if (!players.Any())
            PerformStalemate();
    }

    public static void PerformStalemate()
    {
        CallRpc(CustomRPC.WinLose, WinLose.NobodyWins);
        WinState = WinLose.NobodyWins;
    }

    private static bool TasksDone()
    {
        if (TaskSettings.LongTasks + (int)TaskSettings.CommonTasks + TaskSettings.ShortTasks == 0)
            return IsCustomHnS();

        if (IsCustomHnS())
        {
            var allCrew = new List<PlayerControl>();
            var crewWithNoTasks = new List<PlayerControl>();

            foreach (var role in PlayerLayer.GetLayers<Hunted>())
            {
                var player = role.Player;
                allCrew.Add(player);

                if (role.TasksDone)
                    crewWithNoTasks.Add(player);
            }

            return allCrew.Count == crewWithNoTasks.Count;
        }
        else
        {
            var allCrew = new List<PlayerControl>();
            var crewWithNoTasks = new List<PlayerControl>();

            foreach (var role in Role.GetRoles(Faction.Crew).Where(x => x.Player.CanDoTasks()))
            {
                if (role.Dead && !CrewSettings.GhostTasksCountToWin)
                    continue;

                var player = role.Player;
                allCrew.Add(player);

                if (role.TasksDone)
                    crewWithNoTasks.Add(player);
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
            if (CustomPlayer.Local.AllTasksCompleted())
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