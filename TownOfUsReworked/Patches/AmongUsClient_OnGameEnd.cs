using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class AmongUsClient_OnGameEnd
    {
        public static void Postfix(AmongUsClient __instance)
        {
            Utils.potentialWinners.Clear();

            foreach (var player in PlayerControl.AllPlayerControls)
                Utils.potentialWinners.Add(new WinningPlayerData(player.Data));
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class ShipStatus_SetEverythingUp
    {
        public static void Prefix()
        {
            var toRemoveColorIds = Role.AllRoles.Where(o => o.LostByRPC).Select(o => o.Player.Data.DefaultOutfit.ColorId).ToArray();
            var toRemoveWinners = TempData.winners.ToArray().Where(o => toRemoveColorIds.Contains(o.ColorId)).ToArray();

            for (int i = 0; i < toRemoveWinners.Count(); i++)
                TempData.winners.Remove(toRemoveWinners[i]);
            
            TempData.winners = new List<WinningPlayerData>();
            TempData.winners.Clear();

            foreach (var role in Role.AllRoles)
            {
                var type = role.RoleType;
                var faction = role.Faction;
                var subfaction = role.SubFaction;
                var winners = new List<WinningPlayerData>();
                
                if (Role.NobodyWins)
                {
                    TempData.winners = new List<WinningPlayerData>();
                    TempData.winners.Clear();
                    return;
                }
                else if (Role.NeutralsWin)
                {
                    foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                    {
                        if (surv.Alive)
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                    }

                    foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                    {
                        if (ga.TargetAlive)
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                    }

                    foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                    {
                        if (jest.VotedOut)
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                    }

                    foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                    {
                        if (exe.TargetVotedOut)
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                    }

                    TempData.winners = new List<WinningPlayerData>();

                    foreach (var win in winners)
                        TempData.winners.Add(win);

                    return;
                }
                else if (faction == Faction.Crew)
                {
                    if (Role.CrewWin)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(Faction.Crew))
                        {
                            if (!role2.Player.Data.Disconnected && !role2.IsRecruit)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (faction == Faction.Intruder)
                {
                    if (Role.IntruderWin)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(Faction.Intruder))
                        {
                            if (!role2.Player.Data.Disconnected && !role2.IsRecruit)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (faction == Faction.Syndicate)
                {
                    if (Role.SyndicateWin)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(Faction.Syndicate))
                        {
                            if (!role2.Player.Data.Disconnected && !role2.IsRecruit)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (subfaction == SubFaction.Undead)
                {
                    if (Role.UndeadWin)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(SubFaction.Undead))
                        {
                            if (!role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (subfaction == SubFaction.Cabal)
                {
                    if (Role.CabalWin)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(SubFaction.Cabal))
                        {
                            if (!role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (type == RoleEnum.Glitch)
                {
                    var glitch = (Glitch)role;

                    if (glitch.GlitchWins)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Glitch))
                        {
                            if (!role2.IsRecruit && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
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
                            if (!role2.IsRecruit && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                        
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
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
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Arsonist))
                        {
                            if (!role2.IsRecruit && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
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
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.Plaguebearer))
                        {
                            if (!role2.IsRecruit && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.Pestilence))
                        {
                            if (!role2.IsRecruit && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
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
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.Plaguebearer))
                        {
                            if (!role2.IsRecruit && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.Pestilence))
                        {
                            if (!role2.IsRecruit && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
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
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.SerialKiller))
                        {
                            if (!role2.IsRecruit && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
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
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Murderer))
                        {
                            if (!role2.IsRecruit && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
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
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Werewolf))
                        {
                            if (!role2.IsRecruit && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);
                            
                        return;
                    }
                }
                else if (type == RoleEnum.Cannibal)
                {
                    var cannibal = (Cannibal)role;

                    if (cannibal.EatNeed == 0)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == cannibal.PlayerName).ToList()[0]);

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
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Cryomaniac))
                        {
                            if (!role2.IsRecruit && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);
                            
                        return;
                    }
                }
                else if (type == RoleEnum.Troll)
                {
                    var troll = (Troll)role;

                    if (troll.Killed)
                    {
                        foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (surv.Alive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }

                        foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
                        {
                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                        {
                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == troll.PlayerName).ToList()[0]);

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

                    if (lover.LoveCoupleWins)
                    {
                        var otherLover = lover.OtherLover;

                        winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == lover.PlayerName).ToList()[0]);
                        winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == otherLover.PlayerName).ToList()[0]);

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);

                        return;
                    }
                }
                else if (type == ObjectifierEnum.Phantom)
                {
                    var phantom = (Phantom)objectifier;

                    if (phantom.CompletedTasks)
                    {
                        winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == phantom.PlayerName).ToList()[0]);

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);
                        
                        return;
                    }
                }
                else if (type == ObjectifierEnum.Taskmaster)
                {
                    var taskmaster = (Taskmaster)objectifier;

                    if (taskmaster.WinTasksDone)
                    {
                        winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == taskmaster.PlayerName).ToList()[0]);

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