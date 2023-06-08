namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class OnGameEndPatch
    {
        private readonly static List<WinningPlayerData> PotentialWinners = new();

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
        public static class AmongUsClient_OnGameEnd
        {
            public static void Postfix()
            {
                PotentialWinners.Clear();

                foreach (var player in PlayerControl.AllPlayerControls)
                    PotentialWinners.Add(new(player.Data));
            }
        }

        [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
        public static class ShipStatus_SetEverythingUp
        {
            public static void Prefix()
            {
                var winners = new List<WinningPlayerData>();

                if (Role.AllNeutralsWin)
                {
                    foreach (var role2 in Role.GetRoles(Faction.Neutral))
                    {
                        if (!role2.Disconnected && role2.Faithful)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.NKWins)
                {
                    foreach (var role2 in Role.GetRoles(RoleAlignment.NeutralKill))
                    {
                        if (!role2.Disconnected && role2.Faithful)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.CrewWin)
                {
                    foreach (var role2 in Role.GetRoles(Faction.Crew))
                    {
                        if (!role2.Disconnected && role2.Faithful)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }

                    foreach (var ally in Objectifier.GetObjectifiers<Allied>(ObjectifierEnum.Allied))
                    {
                        if (!ally.Disconnected && ally.Side == Faction.Crew)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == ally.PlayerName));
                    }

                    foreach (var defect in Objectifier.GetObjectifiers<Defector>(ObjectifierEnum.Defector))
                    {
                        if (!defect.Disconnected && defect.Side == Faction.Crew)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == defect.PlayerName));
                    }
                }
                else if (Role.IntruderWin)
                {
                    foreach (var role2 in Role.GetRoles(Faction.Intruder))
                    {
                        if (!role2.Disconnected && role2.Faithful)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }

                    foreach (var ally in Objectifier.GetObjectifiers<Allied>(ObjectifierEnum.Allied))
                    {
                        if (!ally.Disconnected && ally.Side == Faction.Intruder)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == ally.PlayerName));
                    }

                    foreach (var traitor in Objectifier.GetObjectifiers<Traitor>(ObjectifierEnum.Traitor))
                    {
                        if (!traitor.Disconnected && traitor.Side == Faction.Intruder)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == traitor.PlayerName));
                    }

                    foreach (var fanatic in Objectifier.GetObjectifiers<Fanatic>(ObjectifierEnum.Fanatic))
                    {
                        if (!fanatic.Disconnected && fanatic.Side == Faction.Intruder)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == fanatic.PlayerName));
                    }

                    foreach (var defect in Objectifier.GetObjectifiers<Defector>(ObjectifierEnum.Defector))
                    {
                        if (!defect.Disconnected && defect.Side == Faction.Intruder)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == defect.PlayerName));
                    }
                }
                else if (Role.SyndicateWin)
                {
                    foreach (var role2 in Role.GetRoles(Faction.Syndicate))
                    {
                        if (!role2.Disconnected && role2.Faithful)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }

                    foreach (var ally in Objectifier.GetObjectifiers<Allied>(ObjectifierEnum.Allied))
                    {
                        if (!ally.Disconnected && ally.Side == Faction.Syndicate)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == ally.PlayerName));
                    }

                    foreach (var traitor in Objectifier.GetObjectifiers<Traitor>(ObjectifierEnum.Traitor))
                    {
                        if (!traitor.Disconnected && traitor.Side == Faction.Syndicate)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == traitor.PlayerName));
                    }

                    foreach (var fanatic in Objectifier.GetObjectifiers<Fanatic>(ObjectifierEnum.Fanatic))
                    {
                        if (!fanatic.Disconnected && fanatic.Side == Faction.Syndicate)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == fanatic.PlayerName));
                    }

                    foreach (var defect in Objectifier.GetObjectifiers<Defector>(ObjectifierEnum.Defector))
                    {
                        if (!defect.Disconnected && defect.Side == Faction.Syndicate)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == defect.PlayerName));
                    }
                }
                else if (Role.UndeadWin)
                {
                    foreach (var role2 in Role.GetRoles(SubFaction.Undead))
                    {
                        if (!role2.Disconnected)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.CabalWin)
                {
                    foreach (var role2 in Role.GetRoles(SubFaction.Cabal))
                    {
                        if (!role2.Disconnected)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.SectWin)
                {
                    foreach (var role2 in Role.GetRoles(SubFaction.Sect))
                    {
                        if (!role2.Disconnected)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.ReanimatedWin)
                {
                    foreach (var role2 in Role.GetRoles(SubFaction.Reanimated))
                    {
                        if (!role2.Disconnected)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.InfectorsWin)
                {
                    foreach (var role2 in Role.GetRoles<Plaguebearer>(RoleEnum.Plaguebearer))
                    {
                        if (!role2.Disconnected && role2.Faithful)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }

                    foreach (var role2 in Role.GetRoles<Pestilence>(RoleEnum.Pestilence))
                    {
                        if (!role2.Disconnected && role2.Faithful)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.GlitchWins)
                {
                    foreach (var role2 in Role.GetRoles<Glitch>(RoleEnum.Glitch))
                    {
                        if (!role2.Disconnected && role2.Faithful && role2.Winner)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.JuggernautWins)
                {
                    foreach (var role2 in Role.GetRoles<Juggernaut>(RoleEnum.Juggernaut))
                    {
                        if (!role2.Disconnected && role2.Faithful && role2.Winner)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.ArsonistWins)
                {
                    foreach (var role2 in Role.GetRoles<Arsonist>(RoleEnum.Arsonist))
                    {
                        if (!role2.Disconnected && role2.Faithful)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.SerialKillerWins)
                {
                    foreach (var role2 in Role.GetRoles<SerialKiller>(RoleEnum.SerialKiller))
                    {
                        if (!role2.Disconnected && role2.Faithful && role2.Winner)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.MurdererWins)
                {
                    foreach (var role2 in Role.GetRoles<Murderer>(RoleEnum.Murderer))
                    {
                        if (!role2.Disconnected && role2.Faithful)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.WerewolfWins)
                {
                    foreach (var role2 in Role.GetRoles<Werewolf>(RoleEnum.Werewolf))
                    {
                        if (!role2.Disconnected && role2.Faithful && role2.Winner)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.CryomaniacWins)
                {
                    foreach (var role2 in Role.GetRoles<Cryomaniac>(RoleEnum.Cryomaniac))
                    {
                        if (!role2.Disconnected && role2.Faithful && role2.Winner)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Role.PhantomWins)
                {
                    foreach (Phantom role2 in Role.GetRoles<Phantom>(RoleEnum.Phantom))
                    {
                        if (!role2.Disconnected && role2.Faithful && role2.CompletedTasks)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == role2.PlayerName));
                    }
                }
                else if (Objectifier.LoveWins)
                {
                    foreach (var lover in Objectifier.GetObjectifiers(ObjectifierEnum.Lovers))
                    {
                        if (!lover.Disconnected && lover.Winner)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == lover.PlayerName));
                    }
                }
                else if (Objectifier.RivalWins)
                {
                    foreach (var rival in Objectifier.GetObjectifiers(ObjectifierEnum.Rivals))
                    {
                        if (!rival.Disconnected && rival.Winner)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == rival.PlayerName));
                    }
                }
                else if (Objectifier.TaskmasterWins)
                {
                    foreach (var tm in Objectifier.GetObjectifiers(ObjectifierEnum.Taskmaster))
                    {
                        if (!tm.Disconnected && tm.Winner)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == tm.PlayerName));
                    }
                }
                else if (Objectifier.OverlordWins)
                {
                    foreach (var ov in Objectifier.GetObjectifiers(ObjectifierEnum.Overlord))
                    {
                        if (!ov.Disconnected && ov.Winner)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == ov.PlayerName));
                    }
                }
                else if (Objectifier.CorruptedWins)
                {
                    foreach (var corr in Objectifier.GetObjectifiers(ObjectifierEnum.Corrupted))
                    {
                        if (!corr.Disconnected && corr.Winner)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == corr.PlayerName));
                    }
                }
                else if (Objectifier.MafiaWins)
                {
                    foreach (var maf in Objectifier.GetObjectifiers(ObjectifierEnum.Mafia))
                    {
                        if (!maf.Disconnected)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == maf.PlayerName));
                    }
                }

                if (!Objectifier.ObjectifierWins)
                {
                    if (!(Role.ActorWins || Role.BountyHunterWins || Role.CannibalWins || Role.ExecutionerWins || Role.GuesserWins || Role.JesterWins || Role.TrollWins) ||
                        !CustomGameOptions.NeutralEvilsEndGame)
                    {
                        foreach (var surv in Role.GetRoles<Survivor>(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(PotentialWinners.Find(x => x.PlayerName == surv.PlayerName));
                        }

                        foreach (var ga in Role.GetRoles<GuardianAngel>(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(PotentialWinners.Find(x => x.PlayerName == ga.PlayerName));
                        }
                    }

                    foreach (var jest in Role.GetRoles<Jester>(RoleEnum.Jester))
                    {
                        if (jest.VotedOut && !jest.Disconnected)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == jest.PlayerName));
                    }

                    foreach (var exe in Role.GetRoles<Executioner>(RoleEnum.Executioner))
                    {
                        if (exe.TargetVotedOut && !exe.Disconnected)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == exe.PlayerName));
                    }

                    foreach (var bh in Role.GetRoles<BountyHunter>(RoleEnum.BountyHunter))
                    {
                        if (bh.TargetKilled && !bh.Disconnected)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == bh.PlayerName));
                    }

                    foreach (var act in Role.GetRoles<Actor>(RoleEnum.Actor))
                    {
                        if (act.Guessed && !act.Disconnected)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == act.PlayerName));
                    }

                    foreach (var cann in Role.GetRoles<Cannibal>(RoleEnum.Cannibal))
                    {
                        if (cann.Eaten && !cann.Disconnected)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == cann.PlayerName));
                    }

                    foreach (var guess in Role.GetRoles<Guesser>(RoleEnum.Guesser))
                    {
                        if (guess.TargetGuessed && !guess.Disconnected)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == guess.PlayerName));
                    }

                    foreach (var troll in Role.GetRoles<Troll>(RoleEnum.Troll))
                    {
                        if (troll.Killed && !troll.Disconnected)
                            winners.Add(PotentialWinners.Find(x => x.PlayerName == troll.PlayerName));
                    }
                }

                TempData.winners.Clear();
                TempData.winners = new();

                foreach (var win in winners)
                    TempData.winners.Add(win);
            }
        }
    }
}