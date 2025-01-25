namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Insider : Ability
{
    [ToggleOption]
    public static bool InsiderKnows = true;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Insider : CustomColorManager.Ability;
    public override LayerEnum Type => LayerEnum.Insider;
    public override Func<string> Description => () => "- You can finish your tasks to see the votes of others";
    public override bool Hidden => !InsiderKnows && !TasksDone && !Dead;
}