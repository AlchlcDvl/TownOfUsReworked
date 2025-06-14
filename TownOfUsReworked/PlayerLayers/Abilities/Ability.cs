namespace TownOfUsReworked.PlayerLayers.Abilities;

public abstract class Ability : PlayerLayer
{
    protected override UColor MainColor => CustomColorManager.Ability;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Ability;
    public override LayerEnum Type => LayerEnum.NoneAbility;
    protected override UColor LayerColor => CustomColorManager.Ability;
    protected override bool UseMainColor => ClientOptions.CustomAbColors;
}