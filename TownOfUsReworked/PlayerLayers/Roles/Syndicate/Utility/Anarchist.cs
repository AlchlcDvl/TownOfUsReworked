namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Anarchist : Syndicate
    {
        public Anarchist(PlayerControl player) : base(player)
        {
            Name = "Anarchist";
            RoleType = RoleEnum.Anarchist;
            StartText = () => "Wreck Everyone With A Passion";
            AbilitiesText = () => CommonAbilities;
            RoleAlignment = RoleAlignment.SyndicateUtil;
            Type = LayerEnum.Anarchist;
            InspectorResults = InspectorResults.IsBasic;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}