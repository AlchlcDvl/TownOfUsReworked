namespace TownOfUsReworked.Modules;

/// <summary>
/// A struct that signifies a pair of colors. Used to simplify some code regarding player material colors.
/// </summary>
/// <param name="color1">The first color.</param>
/// <param name="color2">The second color.</param>
public readonly struct ColorPair(UColor color1, UColor color2)
{
    public UColor Color1 { get; } = color1;
    public UColor Color2 { get; } = color2;

    // public UColor Lerp(float t) => UColor.Lerp(Color1, Color2, t);

    public static ColorPair Lerp(ColorPair a, ColorPair b, float t) => new(UColor.Lerp(a.Color1, b.Color1, t), UColor.Lerp(a.Color2, b.Color2, t));
}