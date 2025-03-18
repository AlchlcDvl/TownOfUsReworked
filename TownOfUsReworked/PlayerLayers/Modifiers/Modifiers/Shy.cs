namespace TownOfUsReworked.PlayerLayers.Modifiers;

public sealed class Shy : Modifier
{
    public override UColor MainColor => CustomColorManager.Shy;
    public override LayerEnum Type => LayerEnum.Shy;
    public override Func<string> Description => () => "- You cannot call meetings";
}