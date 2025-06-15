namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(Layer.Tiebreaker)]
public sealed class Tiebreaker : Ability
{
    [ToggleOption]
    private static bool TiebreakerKnows = true;

    protected override UColor MainColor => CustomColorManager.Tiebreaker;
    public override Layer Type => Layer.Tiebreaker;
    public override string Description => "- Your votes break ties";
    public override bool Hidden => !TiebreakerKnows && !Dead;
}