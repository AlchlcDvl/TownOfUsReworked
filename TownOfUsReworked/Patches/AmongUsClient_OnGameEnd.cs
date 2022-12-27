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

            if (Role.NobodyWins)
            {
                TempData.winners = new List<WinningPlayerData>();
                TempData.winners.Clear();
                return;
            }

            if (Role.NeutralsWin)
            {
                var winners = new List<WinningPlayerData>();

                foreach (var role in Role.GetRoles(RoleEnum.Survivor))
                {
                    var surv = (Survivor)role;

                    if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                        winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                }

                foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
                {
                    var ga = (GuardianAngel)role;

                    if (ga.TargetAlive)
                        winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                }

                foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                {
                    var jest = (Jester)role2;

                    if (jest.VotedOut)
                        winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                }

                foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                {
                    var exe = (Executioner)role2;

                    if (exe.TargetVotedOut)
                        winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                }

                TempData.winners = new List<WinningPlayerData>();

                foreach (var win in winners)
                    TempData.winners.Add(win);

                return;
            }

            foreach (var role in Role.AllRoles)
            {
                var type = role.RoleType;
                var faction = role.Faction;
                var subfaction = role.SubFaction;
                var winners = new List<WinningPlayerData>();

                if (faction == Faction.Crew)
                {
                    if (Role.CrewWin)
                    {
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;

                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(Faction.Crew))
                        {
                            if (!role2.Player.Data.Disconnected)
                            {
                                if (!role2.Player.IsRecruit())
                                    winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                            }
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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;

                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(Faction.Intruder))
                        {
                            if (!role2.Player.Data.Disconnected)
                            {
                                if (!role2.Player.IsRecruit())
                                    winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                            }
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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;

                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(Faction.Syndicate))
                        {
                            if (!role2.Player.Data.Disconnected)
                            {
                                if (!role2.Player.IsRecruit())
                                    winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                            }
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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;

                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;

                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(SubFaction.Cabal))
                        {
                            var jackal = (Jackal)role2;

                            if (!jackal.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jackal.PlayerName).ToList()[0]);

                            var goodRole = Role.GetRole(jackal.GoodRecruit);
                            var evilRole = Role.GetRole(jackal.EvilRecruit);
                            var backupRole = Role.GetRole(jackal.BackupRecruit);
                            
                            if (!jackal.GoodRecruit.Data.Disconnected && jackal.GoodRecruit != null)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == goodRole.PlayerName).ToList()[0]);
                            
                            if (!jackal.EvilRecruit.Data.Disconnected && jackal.EvilRecruit != null)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == evilRole.PlayerName).ToList()[0]);
                            
                            if (!jackal.BackupRecruit.Data.Disconnected && jackal.BackupRecruit != null)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == backupRole.PlayerName).ToList()[0]);
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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (!role2.Player.Data.IsDead && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Glitch))
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (!role2.Player.Data.IsDead && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Juggernaut))
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (!role2.Player.Data.IsDead && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Arsonist))
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (!role2.Player.Data.IsDead && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.Plaguebearer))
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.Pestilence))
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (!role2.Player.Data.IsDead && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.Plaguebearer))
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.Pestilence))
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (!role2.Player.Data.IsDead && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                                
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.SerialKiller))
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (!role2.Player.Data.IsDead && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Murderer))
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (!role2.Player.Data.IsDead && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Werewolf))
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (!role2.Player.Data.IsDead && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

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
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (!role2.Player.Data.IsDead && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

                            if (exe.TargetVotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == exe.PlayerName).ToList()[0]);
                        }

                        winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == cryo.PlayerName).ToList()[0]);

                        TempData.winners = new List<WinningPlayerData>();

                        foreach (var win in winners)
                            TempData.winners.Add(win);
                            
                        return;
                    }
                }
                else if (type == RoleEnum.Cryomaniac)
                {
                    var troll = (Troll)role;

                    if (troll.Killed)
                    {
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            if (!role2.Player.Data.IsDead && !role2.Player.Data.Disconnected)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == role2.PlayerName).ToList()[0]);
                        }
                            
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;

                            if (ga.TargetAlive)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Jester))
                        {
                            var jest = (Jester)role2;

                            if (jest.VotedOut)
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jest.PlayerName).ToList()[0]);
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Executioner))
                        {
                            var exe = (Executioner)role2;

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
            }
        }
    }
}
