using System;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Sheriff : Role
    {
        public List<byte> Interrogated = new List<byte>();
        public bool UsedThisRound { get; set; } = false;
        public PlayerControl ClosestPlayer;
        public DateTime LastInterrogated { get; set; }

        public Sheriff(PlayerControl player) : base(player)
        {
            Name = "Sheriff";
            StartText = "Reveal The Alignment Of Other Players";
            AbilitiesText = "Reveal alignments of other players to find the <color=#FF0000FF>Intruders</color>";
            Color = IsRecruit ? Colors.Cabal : (IsIntTraitor ? Colors.Intruder : (IsSynTraitor ? Colors.Syndicate : (CustomGameOptions.CustomCrewColors ? Colors.Sheriff : Colors.Crew)));
            LastInterrogated = DateTime.UtcNow;
            RoleType = RoleEnum.Sheriff;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewKill;
            AlignmentName = "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.SherConsigInspBm;
            AddToRoleHistory(RoleType);
        }

        public float InterrogateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInterrogated;
            var num = CustomGameOptions.InterrogateCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.EvilRecruit);
            }

            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else if (IsIntTraitor)
                IntruderWin = true;
            else if (IsSynTraitor)
                SyndicateWin = true;
            else
                CrewWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CabalWin,
                        SendOption.Reliable, -1);
                    writer.Write(Player.PlayerId);
                    Wins();
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsIntTraitor)
            {
                if (Utils.IntrudersWin())
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.IntruderWin,
                        SendOption.Reliable, -1);
                    writer.Write(Player.PlayerId);
                    Wins();
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsSynTraitor)
            {
                if (Utils.SyndicateWins())
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyndicateWin,
                        SendOption.Reliable, -1);
                    writer.Write(Player.PlayerId);
                    Wins();
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.CrewWins())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CrewWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}