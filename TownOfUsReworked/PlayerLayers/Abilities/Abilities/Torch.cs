namespace TownOfUsReworked.PlayerLayers.Abilities;

public sealed class Torch : Ability
{
    protected override UColor MainColor => CustomColorManager.Torch;
    public override LayerEnum Type => LayerEnum.Torch;
    public override string Description => "- You see more than the others";
}