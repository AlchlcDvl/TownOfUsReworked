// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace TownOfUsReworked.Modules;

// Json stuff for deserializing cosmetics
// The ones marked with [JsonIgnore] are serialised elsewhere

public abstract class CustomCosmetic : Asset
{
    [JsonPropertyName("name")]
    public string Name;

    [JsonPropertyName("stream")]
    public bool StreamOnly;

    [JsonPropertyName("test")]
    public bool TestOnly;
}

public abstract class CustomCosmetic<TView, TData> : CustomCosmetic
    where TView : ScriptableObject
    where TData : CosmeticData
{
    [JsonPropertyName("artist")]
    public string Artist;

    [JsonPropertyName("mainHash")]
    public string MainHash;

    [JsonIgnore]
    public TView ViewData;

    [JsonIgnore]
    public TData CosmeticData;

    [JsonIgnore]
    public string MainID => ID; // For reflection purposes
}

public sealed class CustomNameplate : CustomCosmetic<NamePlateViewData, NamePlateData>; // Simplifying the definition

public abstract class AdaptiveCosmetic<TView, TData> : CustomCosmetic<TView, TData>
    where TView : ScriptableObject
    where TData : CosmeticData
{
    [JsonPropertyName("adaptive")]
    public bool Adaptive;

    [JsonPropertyName("chipOffset"), JsonConverter(typeof(Vector2JsonConverter))]
    public Vector2 ChipOffset;

    [JsonPropertyName("noLongMode")]
    public bool NoLongMode;
}

// public sealed class CustomSkin : AdaptiveCosmetic<SkinViewData, SkinData>
// {
//     [JsonPropertyName("previewHash")]
//     public string PreviewHash;

//     [JsonPropertyName("baseSkin")]
//     public string BaseSkin;

//     [JsonIgnore]
//     public bool AnimsInitialised;
// }

public abstract class TopCosmetic<TView, TData> : AdaptiveCosmetic<TView, TData>
    where TView : ScriptableObject
    where TData : CosmeticData
{
    [JsonPropertyName("flipId")]
    public string FlipID;

    [JsonPropertyName("climbId")]
    public string ClimbID;

    [JsonPropertyName("floorId")]
    public string FloorID;

    [JsonPropertyName("flipHash")]
    public string FlipHash;

    [JsonPropertyName("climbHash")]
    public string ClimbHash;

    [JsonPropertyName("floorHash")]
    public string FloorHash;

    [JsonPropertyName("behind")]
    public bool Behind;
}

public sealed class CustomHat : TopCosmetic<HatViewData, HatData>
{
    [JsonPropertyName("backId")]
    public string BackID;

    [JsonPropertyName("backFlipId")]
    public string BackFlipID;

    [JsonPropertyName("climbFlipId")]
    public string ClimbFlipID;

    [JsonPropertyName("floorFlipId")]
    public string FloorFlipID;

    [JsonPropertyName("backHash")]
    public string BackHash;

    [JsonPropertyName("backFlipHash")]
    public string BackFlipHash;

    [JsonPropertyName("climbFlipHash")]
    public string ClimbFlipHash;

    [JsonPropertyName("floorFlipHash")]
    public string FloorFlipHash;

    [JsonPropertyName("noBounce")]
    public bool NoBounce = true;

    [JsonPropertyName("blocksVisors")]
    public bool BlocksVisors;
}

public sealed class CustomVisor : TopCosmetic<VisorViewData, VisorData>; // Simplifying the definition again

public sealed class CustomColor : CustomCosmetic // There's no view or regular data for this, so we don't need to specify them
{
    [JsonPropertyName("stringId")]
    public StringNames StringID;

    [JsonPropertyName("default")]
    public bool Default;

    // Reserved for future use; do not remove
    // [JsonPropertyName("contrasting")]
    // public bool Contrasting;

    [JsonPropertyName("lighter")]
    public bool Lighter;

    [JsonPropertyName("cyclic")]
    public bool Cyclic;

    [JsonPropertyName("mainColorValues")]
    public string[] MainColorValues;

    [JsonPropertyName("shadowColorValues")]
    public string[] ShadowColorValues;

    [JsonPropertyName("timeSpeed")]
    private float TimeSpeed = 1f;

    [JsonIgnore]
    public bool Changing => MainColorValues?.Length is > 1 || ShadowColorValues?.Length is > 1;

    [JsonIgnore]
    public int ColorID;

    [JsonIgnore]
    public UColor[] MainColors;

    [JsonIgnore]
    public UColor[] ShadowColors;

    [JsonIgnore]
    private UColor MainColor;

    [JsonIgnore]
    private UColor ShadowColor;

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

        return index == colors.Length - 1 ? UColor.Lerp(colors[^1], colors[0], t) : UColor.Lerp(colors[index], colors[index + 1], t);
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