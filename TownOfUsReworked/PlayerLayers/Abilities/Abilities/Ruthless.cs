namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(LayerEnum.Ruthless)]
public sealed class Ruthless : Ability
{
    [ToggleOption]
    private static bool RuthlessKnows = true;

    protected override UColor MainColor => CustomColorManager.Ruthless;
    public override LayerEnum Type => LayerEnum.Ruthless;
    public override Func<string> Description => () => "- Your attacks cannot be stopped";
    public override bool Hidden => !RuthlessKnows && !Dead;
}