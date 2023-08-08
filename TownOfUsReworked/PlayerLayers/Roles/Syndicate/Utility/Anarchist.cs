namespace TownOfUsReworked.PlayerLayers.Roles;

public class Anarchist : Syndicate
{
    public override string Name => "Anarchist";
    public override LayerEnum Type => LayerEnum.Anarchist;
    public override Func<string> StartText => () => "Wreck Everyone With A Passion";
    public override Func<string> Description => () => CommonAbilities;
    public override InspectorResults InspectorResults => InspectorResults.IsBasic;

    public Anarchist(PlayerControl player) : base(player) => RoleAlignment = RoleAlignment.SyndicateUtil;
}