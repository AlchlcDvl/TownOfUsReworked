using static TownOfUsReworked.Languages.Language;
namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Crewmate : Crew
    {
        public Crewmate(PlayerControl player) : base(player)
        {
            Name = GetString("Crewmate");
            RoleType = RoleEnum.Crewmate;
            StartText = () => GetString("CrewmateStartText");
            RoleAlignment = RoleAlignment.CrewUtil;
            InspectorResults = InspectorResults.IsBasic;
            Type = LayerEnum.Crewmate;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}