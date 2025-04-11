namespace TownOfUsReworked.PlayerLayers.Abilities;

public sealed class Ninja : Ability
{
    protected override UColor MainColor => CustomColorManager.Ninja;
    public override LayerEnum Type { get; } = LayerEnum.Ninja;
    public override Func<string> Description => () => "- You do not lunge when killing";
}