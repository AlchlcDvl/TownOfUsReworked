using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Crewmate : CrewRole
    {
        public Crewmate(PlayerControl player) : base(player)
        {
            Name = "Crewmate";
            Type = RoleEnum.Crewmate;
            StartText = "Do Your Tasks";
            RoleAlignment = RoleAlignment.CrewUtil;
            AlignmentName = CU;
            Base = true;
            InspectorResults = InspectorResults.IsBasic;
        }
    }
}