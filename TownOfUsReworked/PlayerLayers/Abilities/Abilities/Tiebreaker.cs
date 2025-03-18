namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(LayerEnum.Tiebreaker)]
public sealed class Tiebreaker : Ability
{
    [ToggleOption]
    private static bool TiebreakerKnows = true;

    public override UColor MainColor => CustomColorManager.Tiebreaker;
    public override LayerEnum Type => LayerEnum.Tiebreaker;
    public override Func<string> Description => () => "- Your votes break ties";
    public override bool Hidden => !TiebreakerKnows && !Dead;
}