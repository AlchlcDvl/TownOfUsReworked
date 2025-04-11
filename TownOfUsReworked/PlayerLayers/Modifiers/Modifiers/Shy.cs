namespace TownOfUsReworked.PlayerLayers.Modifiers;

public sealed class Shy : Modifier
{
    protected override UColor MainColor => CustomColorManager.Shy;
    public override LayerEnum Type { get; } = LayerEnum.Shy;
    public override Func<string> Description => () => "- You cannot call meetings";
}