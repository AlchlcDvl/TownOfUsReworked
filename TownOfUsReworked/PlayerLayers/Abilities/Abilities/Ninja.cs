namespace TownOfUsReworked.PlayerLayers.Abilities;

public sealed class Ninja : Ability
{
    protected override UColor MainColor => CustomColorManager.Ninja;
    public override LayerEnum Type => LayerEnum.Ninja;
    public override string Description => "- You do not lunge when killing";
}