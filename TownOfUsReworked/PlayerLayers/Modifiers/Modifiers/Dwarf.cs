namespace TownOfUsReworked.PlayerLayers.Modifiers;

[LayerHeaderOption(LayerEnum.Dwarf)]
public sealed class Dwarf : Modifier
{
    [NumberOption(1f, 2f, 0.05f, Format.Multiplier)]
    public static Number DwarfSpeed = 1.5f;

    [NumberOption(0.3f, 1f, 0.025f, Format.Multiplier)]
    public static Number DwarfScale = 0.5f;

    private static bool Smol => DwarfScale < 1;
    private static bool Sped => DwarfSpeed > 1;
    private static bool Useless => !Smol && !Sped;
    private static string Text => Smol && Sped ? "tiny and speedy" : (Smol ? "tiny" : (Sped ? "speedy" : ""));

    protected override UColor MainColor => Useless ? CustomColorManager.Modifier : CustomColorManager.Dwarf;
    public override LayerEnum Type => LayerEnum.Dwarf;
    public override string Description => Useless ? "- Why" : $"- You are {Text}";

    public override void Init() => Name = TranslationManager.Translate($"Layer.{(Useless ? "Useless" : (Smol ? (Sped ? "Gremlin" : "Dwarf") : "Flash"))}");
}