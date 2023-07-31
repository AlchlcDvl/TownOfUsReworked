namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    [HarmonyPriority(Priority.First)]
    public static class CheckEndGame
    {
        public static bool Prefix()
        {
            if (IsFreePlay || IsHnS || !AmongUsClient.Instance.AmHost)
                return false;

            var spell = Role.GetRoles<Spellslinger>(RoleEnum.Spellslinger).Find(x => x.Spelled.Count >= CustomPlayer.AllPlayers.Count(y => !y.Data.IsDead && !y.Data.Disconnected &&
                !y.Is(x.Faction)));
            var reb = Role.GetRoles<PromotedRebel>(RoleEnum.PromotedRebel).Find(x => x.Spelled.Count >= CustomPlayer.AllPlayers.Count(y => !y.Data.IsDead && !y.Data.Disconnected &&
                !y.Is(x.Faction)));

            if (TasksDone() && Role.GetRoles(Faction.Crew).Any(x => x.Player.CanDoTasks()))
            {
                CallRpc(CustomRPC.WinLose, WinLoseRPC.CrewWin);
                Role.CrewWin = true;
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
                PlayerLayer.AllLayers.ForEach(x => x?.GameEnd());
                //Stalemate detector for unwinnable situations
                DetectStalemate();
            }

            return false;
        }

        private static void DetectStalemate()
        {
            var players = CustomPlayer.AllPlayers.Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (players.Count == 2)
            {
                var player1 = players[0];
                var player2 = players[1];
                var nosolo = CustomGameOptions.NoSolo == NoSolo.Never;
                var nobuttons1 = player1.RemainingEmergencies == 0;
                var nobuttons2 = player2.RemainingEmergencies == 0;
                var nobuttons = nobuttons1 && nobuttons2;
                var onehasbutton = !nobuttons1 || !nobuttons2;
                var knighted1 = player1.IsKnighted();
                var knighted2 = player2.IsKnighted();
                var neitherknighted = (knighted1 && knighted2) || (!knighted1 && !knighted2);
                var onisknighted = !knighted1 || !knighted2;
                var pol1 = player1.Is(AbilityEnum.Politician);
                var pol2 = player2.Is(AbilityEnum.Politician);
                var cankill1 = player1.CanKill();
                var cankill2 = player2.CanKill();
                var cantkill = !cankill1 && !cankill2;
                var rival1 = player1.Is(ObjectifierEnum.Rivals);
                var rival2 = player2.Is(ObjectifierEnum.Rivals);
                var rivals = rival1 && rival2;

                //NK vs NK when neither can kill each other and Neutrals don't win together
                if ((player1.Is(RoleEnum.Cryomaniac) && player2.Is(RoleEnum.Cryomaniac) && nosolo && nobuttons && neitherknighted) || NoOneWins || (player1.Is(RoleEnum.Pestilence) &&
                    player2.Is(RoleEnum.Pestilence) && nosolo && (nobuttons || neitherknighted)) || rivals || (cantkill && nobuttons))
                {
                    PerformStalemate();
                }
            }
        }

        private static void PerformStalemate()
        {
            CallRpc(CustomRPC.WinLose, WinLoseRPC.NobodyWins);
            PlayerLayer.NobodyWins = true;
            EndGame();
        }

        public static bool TasksDone()
        {
            try
            {
                if (Role.GetRoles(Faction.Crew).All(x => x.IsDead))
                    return false;

                var allCrew = new List<PlayerControl>();
                var crewWithNoTasks = new List<PlayerControl>();

                foreach (var player in CustomPlayer.AllPlayers)
                {
                    if (player.CanDoTasks() && player.Is(Faction.Crew) && (!player.Data.IsDead || (player.Data.IsDead && CustomGameOptions.GhostTasksCountToWin)))
                    {
                        allCrew.Add(player);

                        if (Role.GetRole(player).TasksDone)
                            crewWithNoTasks.Add(player);
                    }
                }

                return allCrew.Count == crewWithNoTasks.Count;
            }
            catch
            {
                return false;
            }
        }

        public static bool Sabotaged()
        {
            if (ShipStatus.Instance.Systems != null)
            {
                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
                {
                    var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (lifeSuppSystemType.Countdown < 0f)
                        return true;
                }
                else if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (reactorSystemType.Countdown < 0f)
                        return true;
                }
                else if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor))
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();

                    if (reactorSystemType.Countdown < 0f)
                        return true;
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    public static class OverrideEndGame
    {
        public static void Postfix(ref bool __result) => __result = false;
    }
}