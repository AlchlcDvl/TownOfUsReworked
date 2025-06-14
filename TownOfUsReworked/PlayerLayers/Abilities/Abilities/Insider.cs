namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(LayerEnum.Insider)]
public sealed class Insider : Ability
{
    [ToggleOption]
    private static bool InsiderKnows = true;

    protected override UColor MainColor => CustomColorManager.Insider;
    public override LayerEnum Type => LayerEnum.Insider;
    public override string Description => "- You can finish your tasks to see the votes of others";
    public override bool Hidden => !InsiderKnows && !TasksDone && !Dead;
}