namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Anarchist : SyndicateRole
    {
        public Anarchist(PlayerControl player) : base(player)
        {
            Name = "Anarchist";
            RoleType = RoleEnum.Anarchist;
            StartText = () => "Wreck Everyone With A Passion";
            RoleAlignment = RoleAlignment.SyndicateUtil;
            Type = LayerEnum.Anarchist;
            InspectorResults = InspectorResults.IsBasic;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}