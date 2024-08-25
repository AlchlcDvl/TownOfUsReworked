namespace TownOfUsReworked.PlayerLayers.Modifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Dwarf : Modifier
{
    [NumberOption(MultiMenu.LayerSubOptions, 1f, 2f, 0.05f, Format.Multiplier)]
    public static float DwarfSpeed { get; set; } = 1.5f;

    [NumberOption(MultiMenu.LayerSubOptions, 0.3f, 1f, 0.025f, Format.Multiplier)]
    public static float DwarfScale { get; set; } = 0.5f;

    private static bool Smol => DwarfScale != 1;
    private static bool Sped => DwarfSpeed != 1;
    private static bool Useless => !Smol && !Sped;
    private static string Text => Smol && Sped ? "tiny and speedy" : (Smol ? "tiny" : (Sped ? "speedy" : ""));

    public override UColor Color => ClientOptions.CustomModColors ? (Useless ? CustomColorManager.Modifier : CustomColorManager.Dwarf) : CustomColorManager.Modifier;
    public override string Name => Useless ? "Useless" : (!Smol ? "Flash" : (Sped ? "Gremlin" : "Dwarf"));
    public override LayerEnum Type => LayerEnum.Dwarf;
    public override Func<string> Description => () => Useless ? "- Why" : $"- You are {Text}";
}