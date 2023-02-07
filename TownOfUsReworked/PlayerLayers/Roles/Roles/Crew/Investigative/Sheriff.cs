using System;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Sheriff : Role
    {
        public List<byte> Interrogated = new List<byte>();
        public PlayerControl ClosestPlayer;
        private KillButton _interrogateButton;
        public DateTime LastInterrogated { get; set; }

        public Sheriff(PlayerControl player) : base(player)
        {
            Name = "Sheriff";
            StartText = "Reveal The Alignment Of Other Players";
            AbilitiesText = "Reveal alignments of other players to find the <color=#FF0000FF>Intruders</color>";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Sheriff : Colors.Crew;
            LastInterrogated = DateTime.UtcNow;
            RoleType = RoleEnum.Sheriff;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewKill;
            AlignmentName = "Crew (Investigative)";
            Results = InspResults.SherConsigInspBm;
            FactionDescription = CrewFactionDescription;
            AlignmentDescription = CIDescription;
            Objectives = CrewWinCon;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        public KillButton InterrogateButton
        {
            get => _interrogateButton;
            set
            {
                _interrogateButton = value;
                AddToAbilityButtons(value, this);
            }
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

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
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

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CabalWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsIntTraitor || IsIntFanatic)
            {
                if (Utils.IntrudersWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.IntruderWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsSynTraitor || IsSynFanatic)
            {
                if (Utils.SyndicateWins())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SyndicateWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.CrewWins())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.CrewWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}