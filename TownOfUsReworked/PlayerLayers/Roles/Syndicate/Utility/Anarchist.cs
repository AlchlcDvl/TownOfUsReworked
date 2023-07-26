namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Anarchist : Syndicate
    {
        public override string Name => "Anarchist";
        public override LayerEnum Type => LayerEnum.Anarchist;
        public override RoleEnum RoleType => RoleEnum.Anarchist;
        public override Func<string> StartText => () => "Wreck Everyone With A Passion";
        public override Func<string> AbilitiesText => () => CommonAbilities;
        public override InspectorResults InspectorResults => InspectorResults.IsBasic;

        public Anarchist(PlayerControl player) : base(player) => RoleAlignment = RoleAlignment.SyndicateUtil;
    }
}