using HarmonyLib;
using TownOfUsReworked.Classes;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public static class AmongUsClient_OnGameEnd
    {
        public static void Postfix()
        {
            Utils.PotentialWinners.Clear();

            foreach (var player in PlayerControl.AllPlayerControls)
                Utils.PotentialWinners.Add(new WinningPlayerData(player.Data));
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

            if (Role.NobodyWins)
            {
                return;
            }
            else if (Objectifier.NobodyWins)
            {
                return;
            }
            else if (Role.AllNeutralsWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Neutral))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
            }
            else if (Role.NKWins)
            {
                foreach (var role2 in Role.GetRoles(RoleAlignment.NeutralKill))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.CrewWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Crew))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                foreach (Allied ally in Objectifier.GetObjectifiers(ObjectifierEnum.Allied).Cast<Allied>())
                {
                    if (!ally.Player.Data.Disconnected && ally.Side == Faction.Crew)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ally.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.IntruderWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Intruder))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                foreach (Allied ally in Objectifier.GetObjectifiers(ObjectifierEnum.Allied).Cast<Allied>())
                {
                    if (!ally.Player.Data.Disconnected && ally.Side == Faction.Intruder)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ally.PlayerName).ToList()[0]);
                }

                foreach (Traitor traitor in Objectifier.GetObjectifiers(ObjectifierEnum.Traitor).Cast<Traitor>())
                {
                    if (!traitor.Player.Data.Disconnected && traitor.Side == Faction.Intruder)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == traitor.PlayerName).ToList()[0]);
                }

                foreach (Fanatic fanatic in Objectifier.GetObjectifiers(ObjectifierEnum.Fanatic).Cast<Fanatic>())
                {
                    if (!fanatic.Player.Data.Disconnected && fanatic.Side == Faction.Intruder)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == fanatic.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.SyndicateWin)
            {
                foreach (var role2 in Role.GetRoles(Faction.Syndicate))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                foreach (Allied ally in Objectifier.GetObjectifiers(ObjectifierEnum.Allied).Cast<Allied>())
                {
                    if (!ally.Player.Data.Disconnected && ally.Side == Faction.Syndicate)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ally.PlayerName).ToList()[0]);
                }

                foreach (Traitor traitor in Objectifier.GetObjectifiers(ObjectifierEnum.Traitor).Cast<Traitor>())
                {
                    if (!traitor.Player.Data.Disconnected && traitor.Side == Faction.Syndicate)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == traitor.PlayerName).ToList()[0]);
                }

                foreach (Fanatic fanatic in Objectifier.GetObjectifiers(ObjectifierEnum.Fanatic).Cast<Fanatic>())
                {
                    if (!fanatic.Player.Data.Disconnected && fanatic.Side == Faction.Syndicate)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == fanatic.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.UndeadWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Undead))
                {
                    if (!role2.Player.Data.Disconnected)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.CabalWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Cabal))
                {
                    if (!role2.Player.Data.Disconnected)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.SectWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Sect))
                {
                    if (!role2.Player.Data.Disconnected)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.ReanimatedWin)
            {
                foreach (var role2 in Role.GetRoles(SubFaction.Reanimated))
                {
                    if (!role2.Player.Data.Disconnected)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.InfectorsWin)
            {
                foreach (var role2 in Role.GetRoles(RoleEnum.Plaguebearer))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                foreach (var role2 in Role.GetRoles(RoleEnum.Pestilence))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.GlitchWins)
            {
                foreach (var role2 in Role.GetRoles(RoleEnum.Glitch))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective && role2.Winner)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.JuggernautWins)
            {
                foreach (var role2 in Role.GetRoles(RoleEnum.Juggernaut))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective && role2.Winner)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.ArsonistWins)
            {
                foreach (var role2 in Role.GetRoles(RoleEnum.Arsonist))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.SerialKillerWins)
            {
                foreach (var role2 in Role.GetRoles(RoleEnum.SerialKiller))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective && role2.Winner)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.MurdererWins)
            {
                foreach (var role2 in Role.GetRoles(RoleEnum.Murderer))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.WerewolfWins)
            {
                foreach (var role2 in Role.GetRoles(RoleEnum.Werewolf))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective && role2.Winner)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.CryomaniacWins)
            {
                foreach (var role2 in Role.GetRoles(RoleEnum.Cryomaniac))
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective && role2.Winner)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Role.PhantomWins)
            {
                foreach (Phantom role2 in Role.GetRoles(RoleEnum.Phantom).Cast<Phantom>())
                {
                    if (!role2.Player.Data.Disconnected && role2.NotDefective && role2.CompletedTasks)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                }

                gameEnded = true;
                includeNEs = true;
            }
            else if (Objectifier.LoveWins)
            {
                foreach (var lover in Objectifier.GetObjectifiers(ObjectifierEnum.Lovers))
                {
                    if (!lover.Player.Data.Disconnected && lover.Winner)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == lover.PlayerName).ToList()[0]);
                }

                gameEnded = true;
            }
            else if (Objectifier.RivalWins)
            {
                foreach (var rival in Objectifier.GetObjectifiers(ObjectifierEnum.Rivals))
                {
                    if (!rival.Player.Data.Disconnected && rival.Winner)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == rival.PlayerName).ToList()[0]);
                }

                gameEnded = true;
            }
            else if (Objectifier.TaskmasterWins)
            {
                foreach (var tm in Objectifier.GetObjectifiers(ObjectifierEnum.Taskmaster))
                {
                    if (!tm.Player.Data.Disconnected && tm.Winner)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == tm.PlayerName).ToList()[0]);
                }

                gameEnded = true;
            }
            else if (Objectifier.OverlordWins)
            {
                foreach (var ov in Objectifier.GetObjectifiers(ObjectifierEnum.Overlord))
                {
                    if (!ov.Player.Data.Disconnected && ov.Winner)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ov.PlayerName).ToList()[0]);
                }

                gameEnded = true;
            }
            else if (Objectifier.CorruptedWins)
            {
                foreach (var corr in Objectifier.GetObjectifiers(ObjectifierEnum.Corrupted))
                {
                    if (!corr.Player.Data.Disconnected && corr.Winner)
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == corr.PlayerName).ToList()[0]);
                }

                gameEnded = true;
            }

            if (gameEnded)
            {
                if (includeNEs)
                {
                    foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor).Cast<Survivor>())
                    {
                        if (surv.Alive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                    }

                    foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel).Cast<GuardianAngel>())
                    {
                        if (ga.TargetAlive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                    }

                    foreach (Jester jest in Role.GetRoles(RoleEnum.Jester).Cast<Jester>())
                    {
                        if (jest.VotedOut && !jest.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                    }

                    foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner).Cast<Executioner>())
                    {
                        if (exe.TargetVotedOut && !exe.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                    }

                    foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter).Cast<BountyHunter>())
                    {
                        if (bh.TargetKilled && !bh.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                    }

                    foreach (Actor act in Role.GetRoles(RoleEnum.Actor).Cast<Actor>())
                    {
                        if (act.Guessed && !act.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                    }

                    foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal).Cast<Cannibal>())
                    {
                        if (cann.EatWin && !cann.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                    }

                    foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser).Cast<Guesser>())
                    {
                        if (guess.TargetGuessed && !guess.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                    }

                    foreach (Troll troll in Role.GetRoles(RoleEnum.Troll).Cast<Troll>())
                    {
                        if (troll.Killed && !troll.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                    }
                }

                TempData.winners = new List<WinningPlayerData>();

                foreach (var win in winners)
                    TempData.winners.Add(win);
            }
        }
    }
}