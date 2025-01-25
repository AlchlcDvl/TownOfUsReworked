namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Multitasker : Ability
{
    [NumberOption(10f, 80f, 5f, Format.Percent)]
    public static Number Transparancy = 50;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Multitasker : CustomColorManager.Ability;
    public override LayerEnum Type => LayerEnum.Multitasker;
    public override Func<string> Description => () => "- Your task windows are transparent";
}