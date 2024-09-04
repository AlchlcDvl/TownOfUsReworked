namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Underdog : Ability
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool UnderdogKnows { get; set; } = true;

    [NumberOption(MultiMenu.LayerSubOptions, 2.5f, 30f, 2.5f, Format.Time)]
    public static float UnderdogCdBonus { get; set; } = 5f;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool UnderdogIncreasedCd { get; set; } = true;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Underdog : CustomColorManager.Ability;
    public override string Name => "Underdog";
    public override LayerEnum Type => LayerEnum.Underdog;
    public override Func<string> Description => () => Last(Player) ? "- You have shortened cooldowns" : (UnderdogIncreasedCd ? "- You have long cooldowns while you're not alone" : ("- You " +
        "have short cooldowns when you're alone"));
    public override bool Hidden => !UnderdogKnows && !Last(Player) && !Dead;
}