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
            Base = true;
            InspectorResults = InspectorResults.IsBasic;
            Type = LayerEnum.Crewmate;
        }
    }
}