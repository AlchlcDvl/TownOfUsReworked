namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Anarchist : SUtility
{
    public override LayerEnum Type => LayerEnum.Anarchist;
    public override Func<string> StartText { get; } = () => "Wreck Everyone With A Passion";
    public override Func<string> Description => () => CommonAbilities;
}