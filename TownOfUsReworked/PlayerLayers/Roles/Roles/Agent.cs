using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;
using TMPro;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Agent : Role
    {
        public System.Collections.Generic.Dictionary<byte, TMP_Text> PlayerNumbers = new System.Collections.Generic.Dictionary<byte, TMP_Text>();

        public Agent(PlayerControl player) : base(player)
        {
            Name = "Agent";
            StartText = "Snoop Around And Find Stuff Out";
            AbilitiesText = "Gain extra information from the Admin table!";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Agent : Colors.Crew;
            RoleType = RoleEnum.Agent;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            CoronerDeadReport = "The technology this one is carrying indicates that this body is an Agent!";
            CoronerKillerReport = "";
            Results = InspResults.DisgMorphCamoAgent;
            SubFaction = SubFaction.None;
            IntroSound = TownOfUsReworked.AgentIntro;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            CrewWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Data.IsImpostor() |
                x.Is(RoleAlignment.NeutralKill) | x.Is(RoleAlignment.NeutralNeo) | x.Is(Faction.Syndicate) | x.Is(RoleAlignment.NeutralPros))) ==
                0) | (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.Disconnected && x.Is(Faction.Crew) && !x.Is(ObjectifierEnum.Lovers)
                && !x.Data.TasksDone()) == 0))
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