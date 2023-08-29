namespace TownOfUsReworked.Patches;

[HarmonyPatch]
public static class OnGameEndPatch
{
    private static readonly List<WinningPlayerData> PotentialWinners = new();

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public static class AmongUsClient_OnGameEnd
    {
        public static void Postfix()
        {
            PotentialWinners.Clear();
            CustomPlayer.AllPlayers.ForEach(x => PotentialWinners.Add(new(x.Data)));
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
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.NKWins)
            {
                foreach (var role2 in Role.GetRoles(RoleAlignment.NeutralKill))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.CrewWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Crew))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }

                foreach (var ally in Objectifier.GetObjectifiers<Allied>(LayerEnum.Allied))
                {
                    if (!ally.Disconnected && ally.Side == Faction.Crew)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == ally.PlayerName));
                }

                foreach (var defect in Objectifier.GetObjectifiers<Defector>(LayerEnum.Defector))
                {
                    if (!defect.Disconnected && defect.Side == Faction.Crew && Role.GetRole(defect.Player).BaseFaction != defect.Side)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == defect.PlayerName));
                }
            }
            else if (Role.IntruderWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Intruder))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }

                foreach (var ally in Objectifier.GetObjectifiers<Allied>(LayerEnum.Allied))
                {
                    if (!ally.Disconnected && ally.Side == Faction.Intruder)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == ally.PlayerName));
                }

                foreach (var traitor in Objectifier.GetObjectifiers<Traitor>(LayerEnum.Traitor))
                {
                    if (!traitor.Disconnected && traitor.Side == Faction.Intruder)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == traitor.PlayerName));
                }

                foreach (var fanatic in Objectifier.GetObjectifiers<Fanatic>(LayerEnum.Fanatic))
                {
                    if (!fanatic.Disconnected && fanatic.Side == Faction.Intruder)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == fanatic.PlayerName));
                }

                foreach (var defect in Objectifier.GetObjectifiers<Defector>(LayerEnum.Defector))
                {
                    if (!defect.Disconnected && defect.Side == Faction.Intruder && Role.GetRole(defect.Player).BaseFaction != defect.Side)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == defect.PlayerName));
                }
            }
            else if (Role.SyndicateWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Syndicate))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }

                foreach (var ally in Objectifier.GetObjectifiers<Allied>(LayerEnum.Allied))
                {
                    if (!ally.Disconnected && ally.Side == Faction.Syndicate)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == ally.PlayerName));
                }

                foreach (var traitor in Objectifier.GetObjectifiers<Traitor>(LayerEnum.Traitor))
                {
                    if (!traitor.Disconnected && traitor.Side == Faction.Syndicate)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == traitor.PlayerName));
                }

                foreach (var fanatic in Objectifier.GetObjectifiers<Fanatic>(LayerEnum.Fanatic))
                {
                    if (!fanatic.Disconnected && fanatic.Side == Faction.Syndicate)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == fanatic.PlayerName));
                }

                foreach (var defect in Objectifier.GetObjectifiers<Defector>(LayerEnum.Defector))
                {
                    if (!defect.Disconnected && defect.Side == Faction.Syndicate && Role.GetRole(defect.Player).BaseFaction != defect.Side)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == defect.PlayerName));
                }
            }
            else if (Role.UndeadWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Undead))
                {
                    if (!role2.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.CabalWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Cabal))
                {
                    if (!role2.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.SectWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Sect))
                {
                    if (!role2.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.ReanimatedWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Reanimated))
                {
                    if (!role2.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.ApocalypseWins)
            {
                foreach (var role2 in Role.GetRoles(RoleAlignment.NeutralApoc))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }

                foreach (var role2 in Role.GetRoles(RoleAlignment.NeutralHarb))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.GlitchWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Glitch))
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.JuggernautWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Juggernaut))
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.ArsonistWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Arsonist))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.SerialKillerWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.SerialKiller))
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.MurdererWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Murderer))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.WerewolfWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Werewolf))
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.CryomaniacWins)
            {
                foreach (var role2 in Role.GetRoles(LayerEnum.Cryomaniac))
                {
                    if (!role2.Disconnected && role2.Faithful && role2.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Role.PhantomWins)
            {
                foreach (var role2 in Role.GetRoles<Phantom>(LayerEnum.Phantom))
                {
                    if (!role2.Disconnected && role2.Faithful && role2.CompletedTasks)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == role2.PlayerName));
                }
            }
            else if (Objectifier.LoveWins)
            {
                foreach (var lover in Objectifier.GetObjectifiers(LayerEnum.Lovers))
                {
                    if (!lover.Disconnected && lover.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == lover.PlayerName));
                }
            }
            else if (Objectifier.RivalWins)
            {
                foreach (var rival in Objectifier.GetObjectifiers(LayerEnum.Rivals))
                {
                    if (!rival.Disconnected && rival.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == rival.PlayerName));
                }
            }
            else if (Objectifier.TaskmasterWins)
            {
                foreach (var tm in Objectifier.GetObjectifiers(LayerEnum.Taskmaster))
                {
                    if (!tm.Disconnected && tm.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == tm.PlayerName));
                }
            }
            else if (Objectifier.OverlordWins)
            {
                foreach (var ov in Objectifier.GetObjectifiers(LayerEnum.Overlord))
                {
                    if (ov.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == ov.PlayerName));
                }
            }
            else if (Objectifier.CorruptedWins)
            {
                foreach (var corr in Objectifier.GetObjectifiers(LayerEnum.Corrupted))
                {
                    if (!corr.Disconnected && corr.Winner)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == corr.PlayerName));
                }
            }
            else if (Objectifier.MafiaWins)
            {
                foreach (var maf in Objectifier.GetObjectifiers(LayerEnum.Mafia))
                {
                    if (!maf.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == maf.PlayerName));
                }
            }

            if (!Objectifier.ObjectifierWins)
            {
                if (!(Role.ActorWins || Role.BountyHunterWins || Role.CannibalWins || Role.ExecutionerWins || Role.GuesserWins || Role.JesterWins || Role.TrollWins) ||
                    !CustomGameOptions.NeutralEvilsEndGame)
                {
                    foreach (var surv in Role.GetRoles<Survivor>(LayerEnum.Survivor))
                    {
                        if (surv.Alive)
                            winners.Add(PotentialWinners.First(x => x.PlayerName == surv.PlayerName));
                    }

                    foreach (var ga in Role.GetRoles<GuardianAngel>(LayerEnum.GuardianAngel))
                    {
                        if (!ga.Failed && ga.TargetPlayer != null && ga.TargetAlive)
                            winners.Add(PotentialWinners.First(x => x.PlayerName == ga.PlayerName));
                    }
                }

                foreach (var jest in Role.GetRoles<Jester>(LayerEnum.Jester))
                {
                    if (jest.VotedOut && !jest.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == jest.PlayerName));
                }

                foreach (var exe in Role.GetRoles<Executioner>(LayerEnum.Executioner))
                {
                    if (exe.TargetVotedOut && !exe.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == exe.PlayerName));
                }

                foreach (var bh in Role.GetRoles<BountyHunter>(LayerEnum.BountyHunter))
                {
                    if (bh.TargetKilled && !bh.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == bh.PlayerName));
                }

                foreach (var act in Role.GetRoles<Actor>(LayerEnum.Actor))
                {
                    if (act.Guessed && !act.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == act.PlayerName));
                }

                foreach (var cann in Role.GetRoles<Cannibal>(LayerEnum.Cannibal))
                {
                    if (cann.Eaten && !cann.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == cann.PlayerName));
                }

                foreach (var guess in Role.GetRoles<Guesser>(LayerEnum.Guesser))
                {
                    if (guess.TargetGuessed && !guess.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == guess.PlayerName));
                }

                foreach (var troll in Role.GetRoles<Troll>(LayerEnum.Troll))
                {
                    if (troll.Killed && !troll.Disconnected)
                        winners.Add(PotentialWinners.First(x => x.PlayerName == troll.PlayerName));
                }

                foreach (var link in Objectifier.GetObjectifiers<Linked>(LayerEnum.Linked))
                {
                    if (winners.Any(x => x.PlayerName == link.PlayerName) && !winners.Any(x => x.PlayerName == link.OtherLink.Data.PlayerName))
                        winners.Add(PotentialWinners.First(x => x.PlayerName == link.OtherLink.Data.PlayerName));
                }
            }

            winners = winners.Distinct().ToList();
            TempData.winners.Clear();
            TempData.winners = winners.SystemToIl2Cpp();
        }
    }
}