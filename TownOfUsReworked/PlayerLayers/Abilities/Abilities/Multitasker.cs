namespace TownOfUsReworked.PlayerLayers.Abilities;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Multitasker : Ability
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 80f, 5f, Format.Percent)]
    public static float Transparancy { get; set; } = 50f;

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Multitasker : CustomColorManager.Ability;
    public override string Name => "Multitasker";
    public override LayerEnum Type => LayerEnum.Multitasker;
    public override Func<string> Description => () => "- Your task windows are transparent";
}