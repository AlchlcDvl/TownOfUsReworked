namespace TownOfUsReworked.PlayerLayers.Roles;

public class Anarchist : Syndicate
{
    public override string Name => "Anarchist";
    public override LayerEnum Type => LayerEnum.Anarchist;
    public override Func<string> StartText => () => "Wreck Everyone With A Passion";
    public override Func<string> Description => () => CommonAbilities;

    public Anarchist(PlayerControl player) : base(player) => Alignment = Alignment.SyndicateUtil;
}