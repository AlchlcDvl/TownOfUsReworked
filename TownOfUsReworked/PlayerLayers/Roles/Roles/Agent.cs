using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Agent : Role
    {
        public bool AgentWin;

        public Agent(PlayerControl player) : base(player)
        {
            Name = "Agent";
            ImpostorText = () => "Snoop Around And Find Stuff Out";
            TaskText = () => "Gain extra information from the Admin table!";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Agent : Colors.Crew;
            RoleType = RoleEnum.Agent;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = () => "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            CoronerDeadReport = "The technology this one is carrying indicates that this body is an Agent!";
            CoronerKillerReport = "";
            Results = InspResults.DisgMorphCamoAgent;
            SubFaction = SubFaction.None;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }
    }
}