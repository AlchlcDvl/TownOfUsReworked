namespace TownOfUsReworked.PlayerLayers.Modifiers;

public abstract class Modifier : PlayerLayer
{
    protected override UColor MainColor => CustomColorManager.Modifier;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Modifier;
    public override LayerEnum Type => LayerEnum.NoneModifier;
    protected override UColor LayerColor => CustomColorManager.Modifier;
    protected override bool UseMainColor => ClientOptions.CustomModColors;
}