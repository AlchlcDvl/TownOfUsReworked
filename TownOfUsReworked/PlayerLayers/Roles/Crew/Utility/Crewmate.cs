namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Crewmate : Crew
    {
        public override string Name => "Crewmate";
        public override LayerEnum Type => LayerEnum.Crewmate;
        public override RoleEnum RoleType => RoleEnum.Crewmate;
        public override Func<string> StartText => () => "Do Your Tasks";
        public override InspectorResults InspectorResults => InspectorResults.IsBasic;

        public Crewmate(PlayerControl player) : base(player) => RoleAlignment = RoleAlignment.CrewUtil;
    }
}