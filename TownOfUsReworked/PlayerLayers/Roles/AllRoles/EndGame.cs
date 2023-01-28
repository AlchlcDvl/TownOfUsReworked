using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(GameManager __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            foreach (var role in Role.GetRoles(Faction.Syndicate))
            {
                if (!Role.SyndicateWin)
                {
                    role.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SyndicateLose);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Arsonist arso in Role.GetRoles(RoleEnum.Arsonist))
            {
                if (!Role.AllNeutralsWin && !arso.ArsonistWins && !Role.NKWins)
                {
                    arso.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.ArsonistLose);
                    writer.Write(arso.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (var role in Role.GetRoles(Faction.Crew))
            {
                if (!Role.CrewWin)
                {
                    role.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CrewLose);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Amnesiac amne in Role.GetRoles(RoleEnum.Amnesiac))
            {
                if (!Role.AllNeutralsWin)
                {
                    amne.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.AmnesiacLose);
                    writer.Write(amne.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (var role in Role.GetRoles(SubFaction.Cabal))
            {
                if (!Role.CabalWin)
                {
                    role.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CabalLose);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
            {
                if (!Role.AllNeutralsWin && cann.EatNeed > 0)
                {
                    cann.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CannibalLose);
                    writer.Write(cann.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Cryomaniac cryo in Role.GetRoles(RoleEnum.Cryomaniac))
            {
                if (!Role.AllNeutralsWin && !Role.NKWins && !cryo.CryoWins)
                {
                    cryo.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CryomaniacLose);
                    writer.Write(cryo.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Glitch gli in Role.GetRoles(RoleEnum.Glitch))
            {
                if (!Role.AllNeutralsWin && !gli.GlitchWins && !Role.NKWins)
                {
                    gli.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.GlitchLose);
                    writer.Write(gli.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
            {
                if (!Role.AllNeutralsWin && !guess.GuesserWins)
                {
                    guess.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.GuesserLose);
                    writer.Write(guess.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Jester jest in Role.GetRoles(RoleEnum.Jester))
            {
                if (!jest.VotedOut && !Role.AllNeutralsWin)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.JesterLose);
                    writer.Write(jest.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    jest.Loses();
                }
            }

            foreach (Juggernaut jugg in Role.GetRoles(RoleEnum.Juggernaut))
            {
                if (!Role.AllNeutralsWin && !Role.NKWins && !jugg.JuggernautWins)
                {
                    jugg.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.JuggernautLose);
                    writer.Write(jugg.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Murderer murd in Role.GetRoles(RoleEnum.Murderer))
            {
                if (!Role.AllNeutralsWin && !Role.NKWins && !murd.MurdWins)
                {
                    murd.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.MurdererLose);
                    writer.Write(murd.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Pestilence pest in Role.GetRoles(RoleEnum.Pestilence))
            {
                if (!Role.AllNeutralsWin && !Role.NKWins && !pest.PestilenceWins)
                {
                    pest.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.PestilenceLose);
                    writer.Write(pest.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Phantom phan in Role.GetRoles(RoleEnum.Phantom))
            {
                if (!Role.AllNeutralsWin && !phan.CompletedTasks)
                {
                    phan.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.PhantomLose);
                    writer.Write(phan.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Plaguebearer pb in Role.GetRoles(RoleEnum.Plaguebearer))
            {
                if (!Role.AllNeutralsWin && !Role.NKWins && !pb.PlaguebearerWins)
                {
                    pb.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.PlaguebearerLose);
                    writer.Write(pb.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (var role in Role.GetRoles(SubFaction.Reanimated))
            {
                if (!Role.ReanimatedWin)
                {
                    role.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.ReanimatedLose);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (var role in Role.GetRoles(SubFaction.Sect))
            {
                if (!Role.SectWin)
                {
                    role.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SectLose);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (SerialKiller sk in Role.GetRoles(RoleEnum.SerialKiller))
            {
                if (!Role.AllNeutralsWin && !Role.NKWins && !sk.SerialKillerWins)
                {
                    sk.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SerialKillerLose);
                    writer.Write(sk.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                if (ga.TargetPlayer == null)
                    continue;
                    
                if (ga.TargetAlive || Role.AllNeutralsWin)
                {
                    ga.Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.GuardianAngelWin);
                    writer.Write(ga.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else
                {
                    ga.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.GuardianAngelLose);
                    writer.Write(ga.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
            {
                if (!exe.TargetVotedOut && !Role.AllNeutralsWin)
                {
                    exe.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.ExecutionerLose);
                    writer.Write(exe.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Survivor surv in Role.GetRoles(RoleEnum.Survivor))
            {
                if (surv.Alive || Role.AllNeutralsWin)
                {
                    surv.Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SurvivorWin);
                    writer.Write(surv.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else
                {
                    surv.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SurvivorLose);
                    writer.Write(surv.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Thief thief in Role.GetRoles(RoleEnum.Thief))
            {
                if (!Role.AllNeutralsWin)
                {
                    thief.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.ThiefLose);
                    writer.Write(thief.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
            {
                if (!troll.Killed && !Role.AllNeutralsWin)
                {
                    troll.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.TrollLose);
                    writer.Write(troll.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (var role in Role.GetRoles(SubFaction.Undead))
            {
                if (!Role.UndeadWin)
                {
                    role.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.UndeadLose);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (Werewolf ww in Role.GetRoles(RoleEnum.Werewolf))
            {
                if (!Role.AllNeutralsWin && !Role.NKWins && !ww.WWWins)
                {
                    ww.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.WerewolfLose);
                    writer.Write(ww.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            foreach (var role in Role.GetRoles(Faction.Intruder))
            {
                if (!Role.IntruderWin)
                {
                    role.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.IntruderLose);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            return true;
        }
    }
}