using HarmonyLib;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.Data;
using System.Collections.Generic;

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
                    PotentialWinners.Add(new WinningPlayerData(player.Data));
            }
        }

        [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
        public static class ShipStatus_SetEverythingUp
        {
            public static void Prefix()
            {
                TempData.winners = new();
                TempData.winners.Clear();
                var winners = new List<WinningPlayerData>();
                var gameEnded = false;
                var includeNEs = false;

                if (Role.NobodyWins || Objectifier.NobodyWins)
                    return;
                else if (Role.AllNeutralsWin)
                {
                    foreach (var role2 in Role.GetRoles(Faction.Neutral))
                    {
                        if (!role2.Disconnected && role2.NotDefective)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                }
                else if (Role.NKWins)
                {
                    foreach (var role2 in Role.GetRoles(RoleAlignment.NeutralKill))
                    {
                        if (!role2.Disconnected && role2.NotDefective)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.CrewWin)
                {
                    foreach (var role2 in Role.GetRoles(Faction.Crew))
                    {
                        if (!role2.Disconnected && role2.NotDefective)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    foreach (var ally in Objectifier.GetObjectifiers<Allied>(ObjectifierEnum.Allied))
                    {
                        if (!ally.Disconnected && ally.Side == Faction.Crew)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == ally.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.IntruderWin)
                {
                    foreach (var role2 in Role.GetRoles(Faction.Intruder))
                    {
                        if (!role2.Disconnected && role2.NotDefective)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    foreach (var ally in Objectifier.GetObjectifiers<Allied>(ObjectifierEnum.Allied))
                    {
                        if (!ally.Disconnected && ally.Side == Faction.Intruder)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == ally.PlayerName).ToList()[0]);
                    }

                    foreach (var traitor in Objectifier.GetObjectifiers<Traitor>(ObjectifierEnum.Traitor))
                    {
                        if (!traitor.Disconnected && traitor.Side == Faction.Intruder)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == traitor.PlayerName).ToList()[0]);
                    }

                    foreach (var fanatic in Objectifier.GetObjectifiers<Fanatic>(ObjectifierEnum.Fanatic))
                    {
                        if (!fanatic.Disconnected && fanatic.Side == Faction.Intruder)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == fanatic.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.SyndicateWin)
                {
                    foreach (var role2 in Role.GetRoles(Faction.Syndicate))
                    {
                        if (!role2.Disconnected && role2.NotDefective)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    foreach (var ally in Objectifier.GetObjectifiers<Allied>(ObjectifierEnum.Allied))
                    {
                        if (!ally.Disconnected && ally.Side == Faction.Syndicate)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == ally.PlayerName).ToList()[0]);
                    }

                    foreach (var traitor in Objectifier.GetObjectifiers<Traitor>(ObjectifierEnum.Traitor))
                    {
                        if (!traitor.Disconnected && traitor.Side == Faction.Syndicate)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == traitor.PlayerName).ToList()[0]);
                    }

                    foreach (var fanatic in Objectifier.GetObjectifiers<Fanatic>(ObjectifierEnum.Fanatic))
                    {
                        if (!fanatic.Disconnected && fanatic.Side == Faction.Syndicate)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == fanatic.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.UndeadWin)
                {
                    foreach (var role2 in Role.GetRoles(SubFaction.Undead))
                    {
                        if (!role2.Disconnected)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.CabalWin)
                {
                    foreach (var role2 in Role.GetRoles(SubFaction.Cabal))
                    {
                        if (!role2.Disconnected)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.SectWin)
                {
                    foreach (var role2 in Role.GetRoles(SubFaction.Sect))
                    {
                        if (!role2.Disconnected)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.ReanimatedWin)
                {
                    foreach (var role2 in Role.GetRoles(SubFaction.Reanimated))
                    {
                        if (!role2.Disconnected)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.InfectorsWin)
                {
                    foreach (var role2 in Role.GetRoles<Plaguebearer>(RoleEnum.Plaguebearer))
                    {
                        if (!role2.Disconnected && role2.NotDefective)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    foreach (var role2 in Role.GetRoles<Pestilence>(RoleEnum.Pestilence))
                    {
                        if (!role2.Disconnected && role2.NotDefective)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.GlitchWins)
                {
                    foreach (var role2 in Role.GetRoles<Glitch>(RoleEnum.Glitch))
                    {
                        if (!role2.Disconnected && role2.NotDefective && role2.Winner)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.JuggernautWins)
                {
                    foreach (var role2 in Role.GetRoles<Juggernaut>(RoleEnum.Juggernaut))
                    {
                        if (!role2.Disconnected && role2.NotDefective && role2.Winner)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.ArsonistWins)
                {
                    foreach (var role2 in Role.GetRoles<Arsonist>(RoleEnum.Arsonist))
                    {
                        if (!role2.Disconnected && role2.NotDefective)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.SerialKillerWins)
                {
                    foreach (var role2 in Role.GetRoles<SerialKiller>(RoleEnum.SerialKiller))
                    {
                        if (!role2.Disconnected && role2.NotDefective && role2.Winner)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.MurdererWins)
                {
                    foreach (var role2 in Role.GetRoles<Murderer>(RoleEnum.Murderer))
                    {
                        if (!role2.Disconnected && role2.NotDefective)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.WerewolfWins)
                {
                    foreach (var role2 in Role.GetRoles<Werewolf>(RoleEnum.Werewolf))
                    {
                        if (!role2.Disconnected && role2.NotDefective && role2.Winner)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.CryomaniacWins)
                {
                    foreach (var role2 in Role.GetRoles<Cryomaniac>(RoleEnum.Cryomaniac))
                    {
                        if (!role2.Disconnected && role2.NotDefective && role2.Winner)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Role.PhantomWins)
                {
                    foreach (Phantom role2 in Role.GetRoles<Phantom>(RoleEnum.Phantom))
                    {
                        if (!role2.Disconnected && role2.NotDefective && role2.CompletedTasks)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                    includeNEs = true;
                }
                else if (Objectifier.LoveWins)
                {
                    foreach (var lover in Objectifier.GetObjectifiers(ObjectifierEnum.Lovers))
                    {
                        if (!lover.Disconnected && lover.Winner)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == lover.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                }
                else if (Objectifier.RivalWins)
                {
                    foreach (var rival in Objectifier.GetObjectifiers(ObjectifierEnum.Rivals))
                    {
                        if (!rival.Disconnected && rival.Winner)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == rival.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                }
                else if (Objectifier.TaskmasterWins)
                {
                    foreach (var tm in Objectifier.GetObjectifiers(ObjectifierEnum.Taskmaster))
                    {
                        if (!tm.Disconnected && tm.Winner)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == tm.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                }
                else if (Objectifier.OverlordWins)
                {
                    foreach (var ov in Objectifier.GetObjectifiers(ObjectifierEnum.Overlord))
                    {
                        if (!ov.Disconnected && ov.Winner)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == ov.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                }
                else if (Objectifier.CorruptedWins)
                {
                    foreach (var corr in Objectifier.GetObjectifiers(ObjectifierEnum.Corrupted))
                    {
                        if (!corr.Disconnected && corr.Winner)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == corr.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                }
                else if (Objectifier.MafiaWins)
                {
                    foreach (var maf in Objectifier.GetObjectifiers(ObjectifierEnum.Mafia))
                    {
                        if (!maf.Disconnected)
                            winners.Add(PotentialWinners.Where(x => x.PlayerName == maf.PlayerName).ToList()[0]);
                    }

                    gameEnded = true;
                }

                if (gameEnded)
                {
                    if (includeNEs)
                    {
                        foreach (var surv in Role.GetRoles<Survivor>(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (var ga in Role.GetRoles<GuardianAngel>(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var jest in Role.GetRoles<Jester>(RoleEnum.Jester))
                        {
                            if (jest.VotedOut && !jest.Disconnected)
                                winners.Add(PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var exe in Role.GetRoles<Executioner>(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut && !exe.Disconnected)
                                winners.Add(PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (var bh in Role.GetRoles<BountyHunter>(RoleEnum.BountyHunter))
                        {
                            if (bh.TargetKilled && !bh.Disconnected)
                                winners.Add(PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                        }

                        foreach (var act in Role.GetRoles<Actor>(RoleEnum.Actor))
                        {
                            if (act.Guessed && !act.Disconnected)
                                winners.Add(PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                        }

                        foreach (var cann in Role.GetRoles<Cannibal>(RoleEnum.Cannibal))
                        {
                            if (cann.EatWin && !cann.Disconnected)
                                winners.Add(PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                        }

                        foreach (var guess in Role.GetRoles<Guesser>(RoleEnum.Guesser))
                        {
                            if (guess.TargetGuessed && !guess.Disconnected)
                                winners.Add(PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                        }

                        foreach (var troll in Role.GetRoles<Troll>(RoleEnum.Troll))
                        {
                            if (troll.Killed && !troll.Disconnected)
                                winners.Add(PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                        }
                    }

                    TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();

                    foreach (var win in winners)
                        TempData.winners.Add(win);
                }
            }
        }
    }
}