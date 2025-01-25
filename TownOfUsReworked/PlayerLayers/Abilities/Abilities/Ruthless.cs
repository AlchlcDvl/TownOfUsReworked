namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Ruthless : Ability
{
    [ToggleOption]
    public static bool RuthlessKnows = true;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Ruthless : CustomColorManager.Ability;
    public override LayerEnum Type => LayerEnum.Ruthless;
    public override Func<string> Description => () => "- Your attacks cannot be stopped";
    public override bool Hidden => !RuthlessKnows && !Dead;
}