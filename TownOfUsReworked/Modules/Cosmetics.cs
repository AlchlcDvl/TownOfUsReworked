// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace TownOfUsReworked.Modules;

// Json stuff for deserializing cosmetics
// The ones marked with [JsonIgnore] are serialised elsewhere

public abstract class CustomCosmetic : Asset
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("stream")]
    public bool StreamOnly { get; set; }

    [JsonPropertyName("test")]
    public bool TestOnly { get; set; }
}

public abstract class CustomCosmetic<TView, TData> : CustomCosmetic
    where TView : ScriptableObject
    where TData : CosmeticData
{
    [JsonPropertyName("artist")]
    public string Artist { get; set; }

    [JsonPropertyName("mainHash")]
    public string MainHash { get; set; }

    [JsonIgnore]
    public TView ViewData { get; set; }

    [JsonIgnore]
    public TData CosmeticData { get; set; }

    [JsonIgnore]
    public string MainID => ID; // For reflection purposes
}

[JsonSerializable(typeof(CustomNameplate))]
public sealed class CustomNameplate : CustomCosmetic<NamePlateViewData, NamePlateData>; // Simplifying the definition

public abstract class AdaptiveCosmetic<TView, TData> : CustomCosmetic<TView, TData>
    where TView : ScriptableObject
    where TData : CosmeticData
{
    [JsonPropertyName("adaptive")]
    public bool Adaptive { get; set; }

    [JsonPropertyName("chipOffset"), JsonConverter(typeof(Vector2JsonConverter))]
    public Vector2 ChipOffset { get; set; }

    [JsonPropertyName("noLongMode")]
    public bool NoLongMode { get; set; }
}

// [JsonSerializable(typeof(CustomSkin))]
// public sealed class CustomSkin : AdaptiveCosmetic<SkinViewData, SkinData>
// {
//     [JsonPropertyName("previewHash")]
//     public string PreviewHash { get; set; }

//     [JsonPropertyName("baseSkin")]
//     public string BaseSkin { get; set; }

//    [JsonIgnore]
//    public bool AnimsInitialised { get; set; }
// }

public abstract class TopCosmetic<TView, TData> : AdaptiveCosmetic<TView, TData>
    where TView : ScriptableObject
    where TData : CosmeticData
{
    [JsonPropertyName("flipId")]
    public string FlipID { get; set; }

    [JsonPropertyName("climbId")]
    public string ClimbID { get; set; }

    [JsonPropertyName("floorId")]
    public string FloorID { get; set; }

    [JsonPropertyName("flipHash")]
    public string FlipHash { get; set; }

    [JsonPropertyName("climbHash")]
    public string ClimbHash { get; set; }

    [JsonPropertyName("floorHash")]
    public string FloorHash { get; set; }

    [JsonPropertyName("behind")]
    public bool Behind { get; set; }
}

[JsonSerializable(typeof(CustomHat))]
public sealed class CustomHat : TopCosmetic<HatViewData, HatData>
{
    [JsonPropertyName("backId")]
    public string BackID { get; set; }

    [JsonPropertyName("backFlipId")]
    public string BackFlipID { get; set; }

    [JsonPropertyName("climbFlipId")]
    public string ClimbFlipID { get; set; }

    [JsonPropertyName("floorFlipId")]
    public string FloorFlipID { get; set; }

    [JsonPropertyName("backHash")]
    public string BackHash { get; set; }

    [JsonPropertyName("backFlipHash")]
    public string BackFlipHash { get; set; }

    [JsonPropertyName("climbFlipHash")]
    public string ClimbFlipHash { get; set; }

    [JsonPropertyName("floorFlipHash")]
    public string FloorFlipHash { get; set; }

    [JsonPropertyName("noBounce")]
    public bool NoBounce { get; set; } = true;

    [JsonPropertyName("blocksVisors")]
    public bool BlocksVisors { get; set; }
}

[JsonSerializable(typeof(CustomVisor))]
public sealed class CustomVisor : TopCosmetic<VisorViewData, VisorData>; // Simplifying the definition again

[JsonSerializable(typeof(CustomColor))]
public sealed class CustomColor : CustomCosmetic // There's no view or regular data for this, so we don't need to specify them
{
    [JsonPropertyName("stringId")]
    public StringNames StringID { get; set; }

    [JsonPropertyName("default")]
    public bool Default { get; set; }

    // Reserved for future use; do not remove
    // [JsonPropertyName("contrasting")]
    // public bool Contrasting { get; set; }

    [JsonPropertyName("lighter")]
    public bool Lighter { get; set; }

    [JsonPropertyName("cyclic")]
    public bool Cyclic { get; set; }

    [JsonPropertyName("mainColorValues")]
    public string[] MainColorValues { get; set; }

    [JsonPropertyName("shadowColorValues")]
    public string[] ShadowColorValues { get; set; }

    [JsonPropertyName("timeSpeed")]
    private float TimeSpeed { get; } = 1f;

    [JsonIgnore]
    public bool Changing => MainColorValues?.Length is > 1 || ShadowColorValues?.Length is > 1;

    [JsonIgnore]
    public int ColorID { get; set; }

    [JsonIgnore]
    public UColor[] MainColors { get; set; }

    [JsonIgnore]
    public UColor[] ShadowColors { get; set; }

    [JsonIgnore]
    private UColor MainColor { get; set; }

    [JsonIgnore]
    private UColor ShadowColor { get; set; }

    public UColor GetMainColor() => MainColor = MainColorValues?.Length is null or 0 ? ShadowColor.Light() : LerpColors(TimeSpeed, MainColors, Cyclic);

    public UColor GetShadowColor() => ShadowColor = ShadowColorValues?.Length is null or 0 ? MainColor.Shadow() : LerpColors(TimeSpeed, ShadowColors, Cyclic);

    private static UColor LerpColors(float mul, UColor[] colors, bool cyclic)
    {
        if (colors.Length == 1)
            return colors[0];

        var length = colors.Length - (cyclic ? 0 : 1);
        var point = length * ZigZag(mul);
        var index = Mathf.Clamp(Mathf.FloorToInt(point), 0, length - 1); // Clamping to ensure there's no out of range exception
        var t = point - index;

        if (index == colors.Length - 1)
            return UColor.Lerp(colors[^1], colors[0], t);

        return UColor.Lerp(colors[index], colors[index + 1], t);
        // Index out of range usually never happens because how the function is set up, and if it does happen, it's only for a fraction of a second and not perceptible
    }

    private static float ZigZag(float mul)
    {
        // Math nerd rambling
        // Mapping these next 4 lines onto desmos gives a nice little zigzag graph, make sure your x is Time and that mul is a control variable for the function
        var dx = mul * Time.time;
        var floor = Mathf.FloorToInt(dx);
        var phase = floor % 2; // 0 = forward, 1 = backward
        return Mathf.Clamp01(((dx - floor) * ((2 * phase) - 1)) + 1 - phase);
    }
}