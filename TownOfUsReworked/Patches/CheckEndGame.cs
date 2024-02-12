namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
public static class CheckEndGame
{
    public static bool Prefix()
    {
        if (IsFreePlay || IsHnS || !AmongUsClient.Instance.AmHost)
            return false;

        var spell = PlayerLayer.GetLayers<Spellslinger>().Find(x => !x.IsDead && !x.Disconnected && x.Spelled.Count == CustomPlayer.AllPlayers.Count(y => !y.HasDied()));
        var reb = PlayerLayer.GetLayers<PromotedRebel>().Find(x => !x.IsDead && !x.Disconnected && x.Spelled.Count == CustomPlayer.AllPlayers.Count(y => !y.HasDied()));

        if (TasksDone())
        {
            var winLose = WinLoseRPC.NobodyWins;

            if (IsCustomHnS)
            {
                winLose = WinLoseRPC.HuntedWins;
                Role.HuntedWins = true;
            }
            else
            {
                winLose = WinLoseRPC.CrewWin;
                Role.CrewWin = true;
            }

            CallRpc(CustomRPC.WinLose, winLose);
            EndGame();
        }
        else if (spell)
        {
            var winLose = WinLoseRPC.NobodyWins;

            if (spell.Faction == Faction.Syndicate)
            {
                winLose = WinLoseRPC.SyndicateWin;
                Role.SyndicateWin = true;
            }
            else if (spell.Faction == Faction.Intruder)
            {
                winLose = WinLoseRPC.IntruderWin;
                Role.IntruderWin = true;
            }
            else if (spell.Faction == Faction.Crew)
            {
                winLose = WinLoseRPC.CrewWin;
                Role.CrewWin = true;
            }
            else if (spell.Faction == Faction.Neutral)
            {
                winLose = WinLoseRPC.AllNeutralsWin;
                Role.AllNeutralsWin = true;
            }

            CallRpc(CustomRPC.WinLose, winLose);
            EndGame();
        }
        else if (reb)
        {
            var winLose = WinLoseRPC.NobodyWins;

            if (reb.Faction == Faction.Syndicate)
            {
                winLose = WinLoseRPC.SyndicateWin;
                Role.SyndicateWin = true;
            }
            else if (reb.Faction == Faction.Intruder)
            {
                winLose = WinLoseRPC.IntruderWin;
                Role.IntruderWin = true;
            }
            else if (reb.Faction == Faction.Crew)
            {
                winLose = WinLoseRPC.CrewWin;
                Role.CrewWin = true;
            }
            else if (reb.Faction == Faction.Neutral)
            {
                winLose = WinLoseRPC.AllNeutralsWin;
                Role.AllNeutralsWin = true;
            }

            CallRpc(CustomRPC.WinLose, winLose);
            EndGame();
        }
        else if (Sabotaged() && CustomGameOptions.IntrudersCanSabotage)
        {
            var winLose = WinLoseRPC.NobodyWins;

            if (CustomGameOptions.AltImps)
            {
                winLose = WinLoseRPC.SyndicateWin;
                Role.SyndicateWin = true;
            }
            else
            {
                winLose = WinLoseRPC.IntruderWin;
                Role.IntruderWin = true;
            }

            EndGame();
            CallRpc(CustomRPC.WinLose, winLose);
        }
        else
        {
            Objectifier.AllObjectifiers.ForEach(x => x.GameEnd());
            Role.AllRoles.ForEach(x => x.GameEnd());
            DetectStalemate();
        }

        return false;
    }

    //Stalemate detector for unwinnable situations
    private static void DetectStalemate()
    {
        var players = CustomPlayer.AllPlayers.Where(x => !x.HasDied()).ToList();

        if (players.Count == 2)
        {
            var player1 = players[0];
            var player2 = players[1];
            var nosolo = CustomGameOptions.NoSolo == NoSolo.Never;
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

            //NK vs NK when neither can kill each other and Neutrals don't win together
            if ((player1.Is(LayerEnum.Cryomaniac) && player2.Is(LayerEnum.Cryomaniac) && nosolo && nobuttons && neitherknighted) || NoOneWins || rivals || (cantkill && nobuttons))
                PerformStalemate();
        }
        else if (players.Count == 0)
            PerformStalemate();
    }

    public static void PerformStalemate()
    {
        CallRpc(CustomRPC.WinLose, WinLoseRPC.NobodyWins);
        PlayerLayer.NobodyWins = true;
        EndGame();
    }

    private static bool TasksDone()
    {
        try
        {
            if (IsCustomHnS)
            {
                var allCrew = new List<PlayerControl>();
                var crewWithNoTasks = new List<PlayerControl>();

                foreach (var player in CustomPlayer.AllPlayers)
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
                if (Role.GetRoles(Faction.Crew).All(x => x.IsDead && !CustomGameOptions.GhostTasksCountToWin) || !Role.GetRoles(Faction.Crew).Any(x => x.Player.CanDoTasks()))
                    return false;

                var allCrew = new List<PlayerControl>();
                var crewWithNoTasks = new List<PlayerControl>();

                foreach (var player in CustomPlayer.AllPlayers)
                {
                    if (player.CanDoTasks() && player.Is(Faction.Crew) && (!player.Data.IsDead || (player.Data.IsDead && CustomGameOptions.GhostTasksCountToWin)))
                    {
                        allCrew.Add(player);

                        if (player.GetRole().TasksDone)
                            crewWithNoTasks.Add(player);
                    }
                }

                return allCrew.Count == crewWithNoTasks.Count;
            }
        } catch {}

        return false;
    }

    private static bool Sabotaged()
    {
        try
        {
            if (Ship.Systems.ContainsKey(SystemTypes.LifeSupp))
            {
                var lifeSuppSystemType = Ship.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                if (lifeSuppSystemType.Countdown <= 0f)
                    return true;
            }
            else if (Ship.Systems.ContainsKey(SystemTypes.Laboratory))
            {
                var reactorSystemType = Ship.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                if (reactorSystemType.Countdown <= 0f)
                    return true;
            }
            else if (Ship.Systems.ContainsKey(SystemTypes.Reactor))
            {
                var reactorSystemType = Ship.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();

                if (reactorSystemType.Countdown <= 0f)
                    return true;
            }
            else if (Ship.Systems.ContainsKey(SystemTypes.HeliSabotage))
            {
                var reactorSystemType = Ship.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();

                if (reactorSystemType.Countdown <= 0f)
                    return true;
            }
        } catch {}

        return false;
    }
}

[HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
public static class OverrideKillEndGame
{
    public static bool Prefix(ref bool __result) => __result = false;
}

[HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.EndGameForSabotage))]
public static class OverrideSabEndGame
{
    public static bool Prefix() => false;
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.CheckEndGameViaTasks))]
public static class OverrideTaskEndGame1
{
    public static bool Prefix(ref bool __result)
    {
        GameData.Instance.RecomputeTaskCounts();
        return __result = IsHnS;
    }
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.CheckTaskCompletion))]
public static class OverrideTaskEndGame2
{
    public static bool Prefix(ref bool __result)
    {
        if (TutorialManager.InstanceExists)
        {
            if (CustomPlayer.Local.myTasks.All(t => t.IsComplete))
            {
                HUD.ShowPopUp(TranslationController.Instance.GetString(StringNames.GameOverTaskWin));
                Ship.Begin();
            }
        }
        else
            GameData.Instance.RecomputeTaskCounts();

        return __result = IsHnS;
    }
}