namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(Layer.Insider)]
public sealed class Insider : Ability
{
    [ToggleOption]
    private static bool InsiderKnows = true;

    protected override UColor MainColor => CustomColorManager.Insider;
    public override Layer Type => Layer.Insider;
    public override string Description => "- You can finish your tasks to see the votes of others";
    public override bool Hidden => !InsiderKnows && !TasksDone && !Dead;
}