using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class AmongUsClient_OnGameEnd
    {
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] EndGameResult endGameResult)
        {
            Utils.potentialWinners.Clear();
            foreach (var player in PlayerControl.AllPlayerControls)
                Utils.potentialWinners.Add(new WinningPlayerData(player.Data));
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class EndGameManager_SetEverythingUp
    {
        public static void Prefix()
        {
            var toRemoveColorIds = Role.AllRoles.Where(o => o.LostByRPC).Select(o => o.Player.Data.DefaultOutfit.ColorId).ToArray();
            var toRemoveWinners = TempData.winners.ToArray().Where(o => toRemoveColorIds.Contains(o.ColorId)).ToArray();
            for (int i = 0; i < toRemoveWinners.Count(); i++)
            {
                TempData.winners.Remove(toRemoveWinners[i]);
            }

            if (Role.NobodyWins)
            {
                TempData.winners = new List<WinningPlayerData>();
                return;
            }

            if (Role.SurvOnlyWins)
            {
                var winners = new List<WinningPlayerData>();
                foreach (var role in Role.GetRoles(RoleEnum.Survivor))
                {
                    var surv = (Survivor)role;
                    if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                    {
                        winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                    }
                }

                TempData.winners = new List<WinningPlayerData>();
                foreach (var win in winners) TempData.winners.Add(win);
                return;
            }

            foreach (var role in Role.AllRoles)
            {
                var type = role.RoleType;

                if (type == RoleEnum.Glitch)
                {
                    var glitch = (Glitch)role;
                    if (glitch.GlitchWins)
                    {
                        var winners = Utils.potentialWinners.Where(x => x.PlayerName == glitch.PlayerName).ToList();

                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;
                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                            }
                        }
                        
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;
                            if (!ga.target.Data.IsDead && !ga.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                            }
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Glitch))
                        {                            
                            var glitch2 = (Glitch)role2;

                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == glitch2.PlayerName).ToList()[0]);

                            var glitch3 = glitch2;
                        }

                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var win in winners) TempData.winners.Add(win);
                        return;
                    }
                }
                else if (type == RoleEnum.Juggernaut)
                {
                    var juggernaut = (Juggernaut)role;
                    if (juggernaut.JuggernautWins)
                    {
                        var winners = Utils.potentialWinners.Where(x => x.PlayerName == juggernaut.PlayerName).ToList();
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;
                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                            }
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Juggernaut))
                        {
                            var jugg = (Juggernaut)role2;
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == jugg.PlayerName).ToList()[0]);
                        }

                        winners.Remove(Utils.potentialWinners.Where(x => x.PlayerName == juggernaut.PlayerName).ToList()[0]);
                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var win in winners) TempData.winners.Add(win);
                        return;
                    }
                }
                else if (type == RoleEnum.Arsonist)
                {
                    var arsonist = (Arsonist)role;
                    if (arsonist.ArsonistWins)
                    {
                        var winners = Utils.potentialWinners.Where(x => x.PlayerName == arsonist.PlayerName).ToList();
                        var sub = Utils.potentialWinners.Where(x => x.PlayerName == arsonist.PlayerName).ToList();
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;
                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                            }
                        }

                        foreach (var role2 in Role.GetRoles(RoleEnum.Arsonist))
                        {
                            var arso = (Arsonist)role2;
                            winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == arso.PlayerName).ToList()[0]);
                        }

                        winners.Remove(Utils.potentialWinners.Where(x => x.PlayerName == arsonist.PlayerName).ToList()[0]);
                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var win in winners) TempData.winners.Add(win);
                        return;
                    }
                }
                else if (type == RoleEnum.Plaguebearer)
                {
                    var plaguebearer = (Plaguebearer)role;
                    if (plaguebearer.PlaguebearerWins)
                    {
                        var winners = Utils.potentialWinners.Where(x => x.PlayerName == plaguebearer.PlayerName).ToList();
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;
                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                            }
                        }
                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var win in winners) TempData.winners.Add(win);
                        return;
                    }
                }
                else if (type == RoleEnum.Pestilence)
                {
                    var pestilence = (Pestilence)role;
                    if (pestilence.PestilenceWins)
                    {
                        var winners = Utils.potentialWinners.Where(x => x.PlayerName == pestilence.PlayerName).ToList();
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;
                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                            }
                        }
                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var win in winners) TempData.winners.Add(win);
                        return;
                    }
                }
                else if (type == RoleEnum.Werewolf)
                {
                    var werewolf = (Werewolf)role;
                    if (werewolf.WerewolfWins)
                    {
                        var winners = Utils.potentialWinners.Where(x => x.PlayerName == werewolf.PlayerName).ToList();
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;
                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                            }
                        }
                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var win in winners) TempData.winners.Add(win);
                        return;
                    }
                }
                else if (type == RoleEnum.Phantom)
                {
                    var phantom = (Phantom)role;
                    if (phantom.CompletedTasks)
                    {
                        var winners = Utils.potentialWinners.Where(x => x.PlayerName == phantom.PlayerName).ToList();
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;
                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                            }
                        }
                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var win in winners) TempData.winners.Add(win);
                    }
                }
                else if (type == RoleEnum.Jester)
                {
                    var jester = (Jester)role;
                    if (jester.VotedOut)
                    {
                        var winners = Utils.potentialWinners.Where(x => x.PlayerName == jester.PlayerName).ToList();
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;
                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                            }
                        }
                        
                        foreach (var role2 in Role.GetRoles(RoleEnum.GuardianAngel))
                        {
                            var ga = (GuardianAngel)role2;
                            if (!ga.target.Data.IsDead && !ga.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == ga.PlayerName).ToList()[0]);
                            }
                        }
                        
                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var win in winners)
                        {
                            win.IsDead = false;
                            TempData.winners.Add(win);
                        }
                        return;
                    }
                }
                else if (type == RoleEnum.Executioner)
                {
                    var executioner = (Executioner)role;
                    if (executioner.TargetVotedOut)
                    {
                        var winners = Utils.potentialWinners.Where(x => x.PlayerName == executioner.PlayerName).ToList();
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;
                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                            }
                        }

                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var win in winners) TempData.winners.Add(win);
                        return;
                    }
                }
                else if (type == RoleEnum.Cannibal)
                {
                    var cannibal = (Cannibal)role;
                    if (cannibal.CannibalWin)
                    {
                        var winners = Utils.potentialWinners.Where(x => x.PlayerName == cannibal.PlayerName).ToList();
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;
                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                            }
                        }

                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var win in winners) TempData.winners.Add(win);
                        return;
                    }
                }
                else if (type == RoleEnum.Taskmaster)
                {
                    var taskmaster = (Taskmaster)role;
                    if (taskmaster.WinTasksDone)
                    {
                        var winners = Utils.potentialWinners.Where(x => x.PlayerName == taskmaster.PlayerName).ToList();
                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;
                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                            }
                        }

                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var win in winners) TempData.winners.Add(win);
                        return;
                    }
                }
                /*else if (type == RoleEnum.Dracula || type == RoleEnum.Vampire)
                {
                    var drac = (Dracula)role;
                    var vamp = (Vampire)role;
                    if (drac.HasMaj)
                    {
                        var winners = Utils.potentialWinners.Where(x => x.PlayerName == drac.PlayerName).ToList();
                        winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == vamp.PlayerName).ToList()[0]);

                        foreach (var role2 in Role.GetRoles(RoleEnum.Survivor))
                        {
                            var surv = (Survivor)role2;
                            if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                            {
                                winners.Add(Utils.potentialWinners.Where(x => x.PlayerName == surv.PlayerName).ToList()[0]);
                            }
                        }

                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var win in winners) TempData.winners.Add(win);
                        return;
                    }
                }*/
            }

            foreach (var modifier in Modifier.AllModifiers)
            {
                foreach (var role in Role.AllRoles)
                {
                    var type = modifier.ModifierType;

                    if (type == ModifierEnum.Lover)
                    {
                        var lover = (Lover)modifier;
                        if (lover.LoveCoupleWins)
                        {
                            var otherLover = lover.OtherLover;
                            List<WinningPlayerData> winners = new List<WinningPlayerData>();
                            foreach (var player in Utils.potentialWinners)
                            {
                                if (player.PlayerName == lover.PlayerName || player.PlayerName == otherLover.PlayerName) winners.Add(player);
                            }
                            TempData.winners = new List<WinningPlayerData>();
                            foreach (var win in winners) TempData.winners.Add(win);
                            return;
                        }
                    }
                }
            }
        }
    }
}
