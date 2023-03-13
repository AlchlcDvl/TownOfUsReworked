using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Crewmate : CrewRole
    {
        public Crewmate(PlayerControl player) : base(player)
        {
            Name = "Crewmate";
            RoleType = RoleEnum.Crewmate;
            StartText = "Do Your Tasks";
            RoleAlignment = RoleAlignment.CrewUtil;
            AlignmentName = CU;
            //IntroSound = TownOfUsReworked.CrewmateIntro;
            Base = true;
            RoleDescription = "You are a Crewmate! Your role is the base role for the Crew faction. You have no special abilities and should probably just do your tasks.";
            InspectorResults = InspectorResults.IsBasic;
        }
    }
}