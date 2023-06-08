namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Impostor : IntruderRole
    {
        public Impostor(PlayerControl player) : base(player)
        {
            Name = "Impostor";
            RoleType = RoleEnum.Impostor;
            StartText = () => "Sabotage And Kill Everyone";
            AbilitiesText = () => CommonAbilities;
            RoleAlignment = RoleAlignment.IntruderUtil;
            Type = LayerEnum.Impostor;
            InspectorResults = InspectorResults.IsBasic;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}