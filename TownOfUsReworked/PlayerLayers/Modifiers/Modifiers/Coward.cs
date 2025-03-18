namespace TownOfUsReworked.PlayerLayers.Modifiers;

public sealed class Coward : Modifier
{
    public override UColor MainColor => CustomColorManager.Coward;
    public override LayerEnum Type => LayerEnum.Coward;
    public override Func<string> Description => () => "- You cannot report bodies";
}