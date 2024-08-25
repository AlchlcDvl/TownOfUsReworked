namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Ruthless : Ability
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RuthlessKnows { get; set; } = true;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Ruthless : CustomColorManager.Ability;
    public override string Name => "Ruthless";
    public override LayerEnum Type => LayerEnum.Ruthless;
    public override Func<string> Description => () => "- Your attacks cannot be stopped";
    public override bool Hidden => !RuthlessKnows && !Dead;
}