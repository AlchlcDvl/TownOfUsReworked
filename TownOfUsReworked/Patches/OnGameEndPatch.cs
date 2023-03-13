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
    public class AmongUsClient_OnGameEnd
    {
        public static void Postfix(AmongUsClient __instance)
        {
            Utils.PotentialWinners.Clear();

            foreach (var player in PlayerControl.AllPlayerControls)
                Utils.PotentialWinners.Add(new WinningPlayerData(player.Data));
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class ShipStatus_SetEverythingUp
    {
        public static void Prefix()
        {
            TempData.winners = new List<WinningPlayerData>();
            TempData.winners.Clear();

            foreach (var role in Role.AllRoles)
            {
                var type = role.RoleType;
                var faction = role.Faction;
                var subfaction = role.SubFaction;
                var winners = new List<WinningPlayerData>();

                if (Role.NobodyWins)
                    return;

                if (Role.NKWins)
                {
                    foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                    {
                        if (surv.Alive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                    }

                    foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                    {
                        if (ga.TargetAlive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                    }

                    foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                    {
                        if (jest.VotedOut)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                    }

                    foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                    {
                        if (exe.TargetVotedOut)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                    }

                    foreach (var role2 in Role.GetRoles(RoleAlignment.NeutralKill))
                    {
                        if (!role2.Player.Data.Disconnected && role2.NotDefective)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                    {
                        if (bh.TargetKilled)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                    }

                    foreach (Pestilence pest in Role.GetRoles(RoleEnum.Pestilence))
                    {
                        if (!pest.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == pest.PlayerName).ToList()[0]);
                    }

                    foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                    {
                        if (act.Guessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                    }

                    foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                    {
                        if (cann.EatWin)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                    }

                    foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                    {
                        if (guess.TargetGuessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                    }

                    foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                    {
                        if (troll.Killed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                    }

                    TempData.winners = new List<WinningPlayerData>();

                    foreach (var win in winners)
                        TempData.winners.Add(win);

                    return;
                }
                else if (Role.AllNeutralsWin)
                {
                    foreach (var role2 in Role.GetRoles(Faction.Neutral))
                    {
                        if (!role2.Player.Data.Disconnected && role2.NotDefective)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    TempData.winners = new List<WinningPlayerData>();

                    foreach (var win in winners)
                        TempData.winners.Add(win);

                    return;
                }
                else if (Role.CrewWin)
                {
                    foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                    {
                        if (surv.Alive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                    }

                    foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                    {
                        if (ga.TargetAlive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                    }

                    foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                    {
                        if (jest.VotedOut && !jest.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                    }

                    foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                    {
                        if (exe.TargetVotedOut && !exe.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                    }

                    foreach (var role2 in Role.GetRoles(Faction.Crew))
                    {
                        if (!role2.Player.Data.Disconnected && role2.NotDefective)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    foreach (Allied ally in Objectifier.GetObjectifiers(ObjectifierEnum.Allied))
                    {
                        if (!ally.Player.Data.Disconnected && ally.Side2 == Faction.Crew)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ally.PlayerName).ToList()[0]);
                    }

                    foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                    {
                        if (bh.TargetKilled)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                    }

                    foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                    {
                        if (act.Guessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                    }

                    foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                    {
                        if (cann.EatWin)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                    }

                    foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                    {
                        if (guess.TargetGuessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                    }

                    foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                    {
                        if (troll.Killed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                    }

                    TempData.winners = new List<WinningPlayerData>();

                    foreach (var win in winners)
                        TempData.winners.Add(win);

                    return;
                }
                else if (Role.IntruderWin)
                {
                    foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                    {
                        if (surv.Alive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                    }

                    foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                    {
                        if (ga.TargetAlive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                    }

                    foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                    {
                        if (jest.VotedOut && !jest.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                    }

                    foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                    {
                        if (exe.TargetVotedOut)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                    }

                    foreach (var role2 in Role.GetRoles(Faction.Intruder))
                    {
                        if (!role2.Player.Data.Disconnected && role2.NotDefective)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    foreach (Allied ally in Objectifier.GetObjectifiers(ObjectifierEnum.Allied))
                    {
                        if (!ally.Player.Data.Disconnected && ally.Side2 == Faction.Intruder)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ally.PlayerName).ToList()[0]);
                    }

                    foreach (Traitor traitor in Objectifier.GetObjectifiers(ObjectifierEnum.Traitor))
                    {
                        if (!traitor.Player.Data.Disconnected && traitor.Side2 == Faction.Intruder)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == traitor.PlayerName).ToList()[0]);
                    }

                    foreach (Fanatic fanatic in Objectifier.GetObjectifiers(ObjectifierEnum.Fanatic))
                    {
                        if (!fanatic.Player.Data.Disconnected && fanatic.Side2 == Faction.Intruder)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == fanatic.PlayerName).ToList()[0]);
                    }

                    foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                    {
                        if (bh.TargetKilled)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                    }

                    foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                    {
                        if (act.Guessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                    }

                    foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                    {
                        if (cann.EatWin)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                    }

                    foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                    {
                        if (guess.TargetGuessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                    }

                    foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                    {
                        if (troll.Killed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                    }

                    TempData.winners = new List<WinningPlayerData>();

                    foreach (var win in winners)
                        TempData.winners.Add(win);

                    return;
                }
                else if (Role.SyndicateWin)
                {
                    foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                    {
                        if (surv.Alive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                    }

                    foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                    {
                        if (ga.TargetAlive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                    }

                    foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                    {
                        if (jest.VotedOut && !jest.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                    }

                    foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                    {
                        if (exe.TargetVotedOut)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                    }

                    foreach (var role2 in Role.GetRoles(Faction.Syndicate))
                    {
                        if (!role2.Player.Data.Disconnected && role2.NotDefective)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    foreach (Allied ally in Objectifier.GetObjectifiers(ObjectifierEnum.Allied))
                    {
                        if (!ally.Player.Data.Disconnected && ally.Side2 == Faction.Syndicate)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ally.PlayerName).ToList()[0]);
                    }

                    foreach (Traitor traitor in Objectifier.GetObjectifiers(ObjectifierEnum.Traitor))
                    {
                        if (!traitor.Player.Data.Disconnected && traitor.Side2 == Faction.Syndicate)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == traitor.PlayerName).ToList()[0]);
                    }

                    foreach (Fanatic fanatic in Objectifier.GetObjectifiers(ObjectifierEnum.Fanatic))
                    {
                        if (!fanatic.Player.Data.Disconnected && fanatic.Side2 == Faction.Syndicate)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == fanatic.PlayerName).ToList()[0]);
                    }

                    foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                    {
                        if (bh.TargetKilled)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                    }

                    foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                    {
                        if (act.Guessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                    }

                    foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                    {
                        if (cann.EatWin)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                    }

                    foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                    {
                        if (guess.TargetGuessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                    }

                    foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                    {
                        if (troll.Killed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                    }

                    TempData.winners = new List<WinningPlayerData>();

                    foreach (var win in winners)
                        TempData.winners.Add(win);

                    return;
                }

                if (Role.UndeadWin)
                {
                    foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                    {
                        if (surv.Alive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                    }

                    foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                    {
                        if (ga.TargetAlive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                    }

                    foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                    {
                        if (jest.VotedOut && !jest.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                    }

                    foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                    {
                        if (exe.TargetVotedOut)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                    }

                    foreach (var role2 in Role.GetRoles(SubFaction.Undead))
                    {
                        if (!role2.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                    {
                        if (bh.TargetKilled)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                    }

                    foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                    {
                        if (act.Guessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                    }

                    foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                    {
                        if (cann.EatWin)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                    }

                    foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                    {
                        if (guess.TargetGuessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                    }

                    foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                    {
                        if (troll.Killed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                    }

                    TempData.winners = new List<WinningPlayerData>();

                    foreach (var win in winners)
                        TempData.winners.Add(win);

                    return;
                }
                else if (Role.CabalWin)
                {
                    foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                    {
                        if (surv.Alive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                    }

                    foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                    {
                        if (ga.TargetAlive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                    }

                    foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                    {
                        if (jest.VotedOut && !jest.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                    }

                    foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                    {
                        if (exe.TargetVotedOut)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                    }

                    foreach (var role2 in Role.GetRoles(SubFaction.Cabal))
                    {
                        if (!role2.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                    {
                        if (bh.TargetKilled)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                    }

                    foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                    {
                        if (act.Guessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                    }

                    foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                    {
                        if (cann.EatWin)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                    }

                    foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                    {
                        if (guess.TargetGuessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                    }

                    foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                    {
                        if (troll.Killed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                    }

                    TempData.winners = new List<WinningPlayerData>();

                    foreach (var win in winners)
                        TempData.winners.Add(win);

                    return;
                }
                else if (Role.SectWin)
                {
                    foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                    {
                        if (surv.Alive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                    }

                    foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                    {
                        if (ga.TargetAlive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                    }

                    foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                    {
                        if (jest.VotedOut && !jest.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                    }

                    foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                    {
                        if (exe.TargetVotedOut)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                    }

                    foreach (var role2 in Role.GetRoles(SubFaction.Sect))
                    {
                        if (!role2.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                    {
                        if (bh.TargetKilled)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                    }

                    foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                    {
                        if (act.Guessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                    }

                    foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                    {
                        if (cann.EatWin)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                    }

                    foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                    {
                        if (guess.TargetGuessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                    }

                    foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                    {
                        if (troll.Killed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                    }

                    TempData.winners = new List<WinningPlayerData>();

                    foreach (var win in winners)
                        TempData.winners.Add(win);

                    return;
                }
                else if (Role.ReanimatedWin)
                {
                    foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                    {
                        if (surv.Alive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                    }

                    foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                    {
                        if (ga.TargetAlive)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                    }

                    foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                    {
                        if (jest.VotedOut && !jest.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                    }

                    foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                    {
                        if (exe.TargetVotedOut)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                    }

                    foreach (var role2 in Role.GetRoles(SubFaction.Reanimated))
                    {
                        if (!role2.Player.Data.Disconnected)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                    }

                    foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                    {
                        if (bh.TargetKilled)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                    }

                    foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                    {
                        if (act.Guessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                    }

                    foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                    {
                        if (cann.EatWin)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                    }

                    foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                    {
                        if (guess.TargetGuessed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                    }

                    foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                    {
                        if (troll.Killed)
                            winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                    }

                    TempData.winners = new List<WinningPlayerData>();

                    foreach (var win in winners)
                        TempData.winners.Add(win);

                    return;
                }

                if (type == RoleEnum.Glitch)
                {
                    var glitch = (Glitch)role;

                    if (glitch.GlitchWins)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut && !jest.Player.Data.Disconnected)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Glitch))
                        {
                            if (!role2.Player.Data.Disconnected && role2.NotDefective)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                        {
                            if (bh.TargetKilled)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                        }

                        foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                        {
                            if (act.Guessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                        }

                        foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                        {
                            if (cann.EatWin)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                        }

                        foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                        {
                            if (guess.TargetGuessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                        }

                        foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                        {
                            if (troll.Killed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (type == RoleEnum.Juggernaut)
                {
                    var juggernaut = (Juggernaut)role;

                    if (juggernaut.JuggernautWins)
                    {
                        foreach (var role2 in Role.GetRoles(RoleEnum.Juggernaut))
                        {
                            if (!role2.Player.Data.Disconnected && role2.NotDefective)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut && !jest.Player.Data.Disconnected)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                        {
                            if (bh.TargetKilled)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                        }

                        foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                        {
                            if (act.Guessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                        }

                        foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                        {
                            if (cann.EatWin)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                        }

                        foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                        {
                            if (guess.TargetGuessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                        }

                        foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                        {
                            if (troll.Killed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (type == RoleEnum.Arsonist)
                {
                    var arsonist = (Arsonist)role;

                    if (arsonist.ArsonistWins)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut && !jest.Player.Data.Disconnected)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Arsonist))
                        {
                            if (!role2.Player.Data.Disconnected && role2.NotDefective)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                        {
                            if (bh.TargetKilled)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                        }

                        foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                        {
                            if (act.Guessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                        }

                        foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                        {
                            if (cann.EatWin)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                        }

                        foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                        {
                            if (guess.TargetGuessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                        }

                        foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                        {
                            if (troll.Killed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (type == RoleEnum.Plaguebearer)
                {
                    var plaguebearer = (Plaguebearer)role;

                    if (plaguebearer.PlaguebearerWins)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut && !jest.Player.Data.Disconnected)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

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

                        foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                        {
                            if (bh.TargetKilled)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                        }

                        foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                        {
                            if (act.Guessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                        }

                        foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                        {
                            if (cann.EatWin)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                        }

                        foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                        {
                            if (guess.TargetGuessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                        }

                        foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                        {
                            if (troll.Killed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (type == RoleEnum.Pestilence)
                {
                    var pestilence = (Pestilence)role;

                    if (pestilence.PestilenceWins)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut && !jest.Player.Data.Disconnected)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

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

                        foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                        {
                            if (bh.TargetKilled)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                        }

                        foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                        {
                            if (act.Guessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                        }

                        foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                        {
                            if (cann.EatWin)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                        }

                        foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                        {
                            if (guess.TargetGuessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                        }

                        foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                        {
                            if (troll.Killed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (type == RoleEnum.SerialKiller)
                {
                    var serialkiller = (SerialKiller)role;

                    if (serialkiller.SerialKillerWins)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut && !jest.Player.Data.Disconnected)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.SerialKiller))
                        {
                            if (!role2.Player.Data.Disconnected && role2.NotDefective)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                        {
                            if (bh.TargetKilled)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                        }

                        foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                        {
                            if (act.Guessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                        }

                        foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                        {
                            if (cann.EatWin)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                        }

                        foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                        {
                            if (guess.TargetGuessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                        }

                        foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                        {
                            if (troll.Killed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (type == RoleEnum.Murderer)
                {
                    var murderer = (Murderer)role;

                    if (murderer.MurdWins)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut && !jest.Player.Data.Disconnected)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Murderer))
                        {
                            if (!role2.Player.Data.Disconnected && role2.NotDefective)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                        {
                            if (bh.TargetKilled)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                        }

                        foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                        {
                            if (act.Guessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                        }

                        foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                        {
                            if (cann.EatWin)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                        }

                        foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                        {
                            if (guess.TargetGuessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                        }

                        foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                        {
                            if (troll.Killed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);
                            
                        return;
                    }
                }
                else if (type == RoleEnum.Werewolf)
                {
                    var ww = (Werewolf)role;

                    if (ww.WWWins)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut && !jest.Player.Data.Disconnected)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Werewolf))
                        {
                            if (!role2.Player.Data.Disconnected && role2.NotDefective)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                        {
                            if (bh.TargetKilled)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                        }

                        foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                        {
                            if (act.Guessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                        }

                        foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                        {
                            if (cann.EatWin)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                        }

                        foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                        {
                            if (guess.TargetGuessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                        }

                        foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                        {
                            if (troll.Killed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (type == RoleEnum.Cryomaniac)
                {
                    var cryo = (Cryomaniac)role;

                    if (cryo.CryoWins)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut && !jest.Player.Data.Disconnected)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Cryomaniac))
                        {
                            if (!role2.Player.Data.Disconnected && role2.NotDefective)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                        {
                            if (bh.TargetKilled)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                        }

                        foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                        {
                            if (act.Guessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                        }

                        foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                        {
                            if (cann.EatWin)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                        }

                        foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                        {
                            if (guess.TargetGuessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                        }

                        foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                        {
                            if (troll.Killed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (type == RoleEnum.Phantom)
                {
                    var phantom = (Phantom)role;

                    if (phantom.PhantomWin)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut && !jest.Player.Data.Disconnected)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
                        {
                            if (bh.TargetKilled)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == bh.PlayerName).ToList()[0]);
                        }

                        foreach (Actor act in Role.GetRoles(RoleEnum.Actor))
                        {
                            if (act.Guessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == act.PlayerName).ToList()[0]);
                        }

                        foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
                        {
                            if (cann.EatWin)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == cann.PlayerName).ToList()[0]);
                        }

                        foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                        {
                            if (guess.TargetGuessed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == guess.PlayerName).ToList()[0]);
                        }

                        foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
                        {
                            if (troll.Killed)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);
                        }

                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == phantom.PlayerName).ToList()[0]);

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }

                winners.Clear();
            }

            foreach (var objectifier in Objectifier.AllObjectifiers)
            {
                var type = objectifier.ObjectifierType;
                var winners = new List<WinningPlayerData>();

                if (type == ObjectifierEnum.Lovers)
                {
                    var lover = (Lovers)objectifier;

                    if (lover.LoveWins)
                    {
                        var other = lover.OtherLover;
                        var otherLover = Objectifier.GetObjectifier<Lovers>(other);

                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == lover.PlayerName).ToList()[0]);
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == otherLover.PlayerName).ToList()[0]);

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (type == ObjectifierEnum.Rivals)
                {
                    var rival = (Rivals)objectifier;

                    if (rival.RivalWins)
                    {
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == rival.PlayerName).ToList()[0]);

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (type == ObjectifierEnum.Taskmaster)
                {
                    var taskmaster = (Taskmaster)objectifier;

                    if (taskmaster.TaskmasterWins)
                    {
                        winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == taskmaster.PlayerName).ToList()[0]);

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);
                            
                        return;
                    }
                }
                else if (type == ObjectifierEnum.Overlord)
                {
                    var overlord = (Overlord)objectifier;

                    if (overlord.OverlordWins)
                    {
                        foreach (Overlord ov in Objectifier.GetObjectifiers(ObjectifierEnum.Overlord))
                        {
                            if (ov.IsAlive)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == ov.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (type == ObjectifierEnum.Corrupted)
                {
                    var corrupted = (Corrupted)objectifier;

                    if (corrupted.CorruptedWin)
                    {
                        foreach (Corrupted corr in Objectifier.GetObjectifiers(ObjectifierEnum.Corrupted))
                        {
                            if (!corr.Player.Data.Disconnected)
                                winners.Add(Utils.PotentialWinners.Where(x => x.PlayerName == corr.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }

                winners.Clear();
            }
        }
    }
}