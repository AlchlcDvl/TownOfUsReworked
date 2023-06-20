namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Crewmate : Crew
    {
        public Crewmate(PlayerControl player) : base(player)
        {
            Name = "Crewmate";
            RoleType = RoleEnum.Crewmate;
            StartText = () => "Do Your Tasks";
            RoleAlignment = RoleAlignment.CrewUtil;
            InspectorResults = InspectorResults.IsBasic;
            Type = LayerEnum.Crewmate;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}