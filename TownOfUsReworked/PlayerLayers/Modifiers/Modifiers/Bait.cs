namespace TownOfUsReworked.PlayerLayers.Modifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Bait : Modifier
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BaitKnows { get; set; } = true;

    [NumberOption(MultiMenu.LayerSubOptions, 0f, 15f, 0.5f, Format.Time)]
    public static float BaitMinDelay { get; set; } = 0f;

    [NumberOption(MultiMenu.LayerSubOptions, 0f, 15f, 0.5f, Format.Time)]
    public static float BaitMaxDelay { get; set; } = 1f;

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Bait : CustomColorManager.Modifier;
    public override string Name => "Bait";
    public override LayerEnum Type => LayerEnum.Bait;
    public override Func<string> Description => () => "- Killing you causes the killer to report your body, albeit with a slight delay";
    public override bool Hidden => !BaitKnows && !Dead;
}