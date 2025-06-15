namespace TownOfUsReworked.PlayerLayers.Abilities;

public sealed class Ninja : Ability
{
    protected override UColor MainColor => CustomColorManager.Ninja;
    public override Layer Type => Layer.Ninja;
    public override string Description => "- You do not lunge when killing";
}