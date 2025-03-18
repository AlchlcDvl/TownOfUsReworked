namespace TownOfUsReworked.PlayerLayers.Modifiers;

public abstract class Modifier : PlayerLayer
{
    public override UColor MainColor => CustomColorManager.Modifier;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Modifier;
    public override LayerEnum Type => LayerEnum.NoneModifier;
    public override UColor LayerColor => CustomColorManager.Modifier;
    public override bool UseMainColor => ClientOptions.CustomModColors;
}