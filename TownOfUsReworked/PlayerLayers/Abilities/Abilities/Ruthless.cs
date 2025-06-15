namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(Layer.Ruthless)]
public sealed class Ruthless : Ability
{
    [ToggleOption]
    private static bool RuthlessKnows = true;

    protected override UColor MainColor => CustomColorManager.Ruthless;
    public override Layer Type => Layer.Ruthless;
    public override string Description => "- Your attacks cannot be stopped";
    public override bool Hidden => !RuthlessKnows && !Dead;
}