using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NeutralsMod;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class NeutralRole : Role
    {
        protected NeutralRole(PlayerControl player) : base(player)
        {
            Faction = Faction.Neutral;
            FactionColor = Colors.Neutral;
            Color = Colors.Neutral;
            BaseFaction = Faction.Neutral;
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;

            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();
                team.Add(jackal.Player);

                if (Player.Is(RoleAlignment.NeutralKill))
                    team.Add(jackal.GoodRecruit);
                else
                    team.Add(jackal.EvilRecruit);
            }
            else if (Player.Is(RoleEnum.Jackal))
            {
                var jackal = (Jackal)this;
                team.Add(jackal.GoodRecruit);
                team.Add(jackal.EvilRecruit);
            }

            if (IsIntAlly || IsSynAlly)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                        team.Add(player);
                }
            }

            if (this.HasTarget())
                team.Add(Player.GetTarget());

            if (Player.Is(ObjectifierEnum.Lovers))
                team.Add(Player.GetOtherLover());
            else if (Player.Is(ObjectifierEnum.Rivals))
                team.Add(Player.GetOtherRival());

            __instance.teamToShow = team;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if ((IsRecruit || RoleType == RoleEnum.Jackal) && Utils.CabalWin())
            {
                CabalWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CabalWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if ((IsPersuaded || RoleType == RoleEnum.Whisperer) && Utils.SectWin())
            {
                SectWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SectWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if ((IsBitten || RoleType == RoleEnum.Dracula) && Utils.UndeadWin())
            {
                UndeadWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.UndeadWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if ((IsResurrected || RoleType == RoleEnum.Necromancer) && Utils.ReanimatedWin())
            {
                ReanimatedWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.ReanimatedWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.AllNeutralsWin() && NotDefective)
            {
                AllNeutralsWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.AllNeutralsWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.AllNKsWin() && NotDefective && RoleAlignment == RoleAlignment.NeutralKill)
            {
                NKWins = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.AllNKsWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsCrewAlly && Utils.CrewWins())
            {
                CrewWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CrewWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if ((IsIntAlly || (RoleType == RoleEnum.Betrayer && Faction == Faction.Intruder)) && Utils.IntrudersWin())
            {
                IntruderWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if ((IsSynAlly || (RoleType == RoleEnum.Betrayer && Faction == Faction.Syndicate)) && Utils.SyndicateWins())
            {
                SyndicateWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SyndicateWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (NotDefective && RoleAlignment == RoleAlignment.NeutralKill && (Utils.SameNKWins(RoleType) || Utils.SoloNKWins(RoleType, Player)))
            {
                switch (RoleType)
                {
                    case RoleEnum.Glitch:
                        GlitchWins = true;
                        break;

                    case RoleEnum.Arsonist:
                        ArsonistWins = true;
                        break;

                    case RoleEnum.Cryomaniac:
                        CryomaniacWins = true;
                        break;

                    case RoleEnum.Juggernaut:
                        JuggernautWins = true;
                        break;

                    case RoleEnum.Murderer:
                        MurdererWins = true;
                        break;

                    case RoleEnum.Werewolf:
                        WerewolfWins = true;
                        break;

                    case RoleEnum.SerialKiller:
                        SerialKillerWins = true;
                        break;
                }

                if (CustomGameOptions.NoSolo == NoSolo.SameNKs)
                {
                    foreach (var role in GetRoles(RoleType))
                    {
                        if (!role.Player.Data.Disconnected && role.NotDefective)
                            role.Winner = true;
                    }
                }

                Winner = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)(CustomGameOptions.NoSolo == NoSolo.SameNKs ? WinLoseRPC.SameNKWins : WinLoseRPC.SoloNKWins));
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.PestOrPBWins() && NotDefective && (RoleType == RoleEnum.Plaguebearer || RoleType == RoleEnum.Pestilence))
            {
                InfectorsWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.InfectorsWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return RoleAlignment != RoleAlignment.NeutralKill && RoleAlignment != RoleAlignment.NeutralNeo && RoleType != RoleEnum.Pestilence && RoleType != RoleEnum.Betrayer &&
                NotDefective;
        }
    }
}