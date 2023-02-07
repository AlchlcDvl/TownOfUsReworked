using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Classes;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Altruist : Role
    {
        public bool CurrentlyReviving = false;
        public DeadBody CurrentTarget = null;
        public bool ReviveUsed = false;
        private KillButton _reviveButton;
        
        public Altruist(PlayerControl player) : base(player)
        {
            Name = "Altruist";
            StartText = "Sacrifice Yourself To Save Another";
            AbilitiesText = "- You can revive a dead body at the cost of your own life.";
            AttributesText = "- Reviving someone takes time.\n- If a meeting is called during your revive, both you and\nyour target will be pronounced dead.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Altruist : Colors.Crew;
            RoleType = RoleEnum.Altruist;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewProt;
            AlignmentName = "Crew (Protective)";
            Results = InspResults.TLAltTMCann;
            FactionDescription = CrewFactionDescription;
            AlignmentDescription = CPDescription;
            RoleDescription = "Your are an Altruist! You can revive a dead person if you find their body. Be careful though, because it takes time" +
                " to revive someone and a meeting being called will kill both you and your target.";
            Objectives = CrewWinCon;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new List<PlayerControl>();

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

        public KillButton ReviveButton
        {
            get => _reviveButton;
            set
            {
                _reviveButton = value;
                AddToAbilityButtons(value, this);
            }
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