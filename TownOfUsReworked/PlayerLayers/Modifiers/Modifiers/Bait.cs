namespace TownOfUsReworked.PlayerLayers.Modifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Bait : Modifier
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BaitKnows { get; set; } = true;

    [NumberOption(MultiMenu.LayerSubOptions, 0f, 15f, 0.5f, Format.Time)]
    public static Number BaitMinDelay { get; set; } = new(0);

    [NumberOption(MultiMenu.LayerSubOptions, 0f, 15f, 0.5f, Format.Time)]
    public static Number BaitMaxDelay { get; set; } = new(1);

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Bait : CustomColorManager.Modifier;
    public override string Name => "Bait";
    public override LayerEnum Type => LayerEnum.Bait;
    public override Func<string> Description => () => "- Killing you causes the killer to report your body, albeit with a slight delay";
    public override bool Hidden => !BaitKnows && !Dead;
}