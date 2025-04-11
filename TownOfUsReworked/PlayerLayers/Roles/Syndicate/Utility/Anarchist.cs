namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Anarchist : Syndicate
{
    public override LayerEnum Type { get; } = LayerEnum.Anarchist;
    public override Func<string> StartText { get; } = () => "Wreck Everyone With A Passion";
    public override Func<string> Description => () => CommonAbilities;

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
    }
}