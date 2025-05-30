namespace TownOfUsReworked.Data.Enums;

public static class CustomPlayerOutfitType
{
    private static readonly EnumInjector<PlayerOutfitType> Injector = new();

    public const PlayerOutfitType Default = PlayerOutfitType.Default;
    public const PlayerOutfitType Shapeshifted = PlayerOutfitType.Shapeshifted;
    public const PlayerOutfitType HorseWrangler = PlayerOutfitType.HorseWrangler;
    public const PlayerOutfitType MushroomMixup = PlayerOutfitType.MushroomMixup;

    public static readonly PlayerOutfitType Morph = Injector.InjectAndReturn("Morph"); // 4
    public static readonly PlayerOutfitType Camouflage = Injector.InjectAndReturn("Camouflage"); // 5
    public static readonly PlayerOutfitType Invis = Injector.InjectAndReturn("Invis"); // 6
    public static readonly PlayerOutfitType Ghostly = Injector.InjectAndReturn("Ghostly"); // 7
    public static readonly PlayerOutfitType Colorblind = Injector.InjectAndReturn("Colorblind"); // 8
    public static readonly PlayerOutfitType NightVision = Injector.InjectAndReturn("NightVision"); // 9
    public static readonly PlayerOutfitType Custom = Injector.InjectAndReturn("Custom"); // 10
    public static readonly PlayerOutfitType GameDefault = Injector.InjectAndReturn("GameDefault"); // 11
}