namespace TownOfUsReworked.PlayerLayers.Modifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Giant : Modifier
{
    [NumberOption(0.5f, 1f, 0.05f, Format.Multiplier)]
    public static Number GiantSpeed = 0.75f;

    [NumberOption(1f, 3f, 0.025f, Format.Multiplier)]
    public static Number GiantScale = 1.5f;

    private static bool Chonk => GiantScale > 1;
    private static bool Snail => GiantSpeed < 1;
    private static bool Useless => !Chonk && !Snail;
    private static string Text => Chonk && Snail ? "big and slow" : (Chonk ? "big" : (Snail ? "slow" : ""));

    public override UColor Color => !ClientOptions.CustomModColors || Useless ? CustomColorManager.Modifier : CustomColorManager.Giant;
    public override LayerEnum Type => LayerEnum.Giant;
    public override Func<string> Description => () => Useless ? "- Why" : $"- You are {Text}";

    public override void Init() => Name = TranslationManager.Translate($"Layer.{(Useless ? "Useless" : (Chonk ? (Snail ? "Chonker" : "Giant") : "Sloth"))}");
}