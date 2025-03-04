// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace TownOfUsReworked.Modules;

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

    [JsonIgnore]
    public TView ViewData { get; set; }

    [JsonIgnore]
    public TData CosmeticData { get; set; }

    [JsonPropertyName("mainhash")]
    public string MainHash { get; set; }

    [JsonIgnore]
    public string MainID => ID; // For reflection purposes
}

public abstract class TopCosmetic<TView, TData> : CustomCosmetic<TView, TData>
    where TView : ScriptableObject
    where TData : CosmeticData
{
    [JsonPropertyName("flipid")]
    public string FlipID { get; set; }

    [JsonPropertyName("climbid")]
    public string ClimbID { get; set; }

    [JsonPropertyName("floorid")]
    public string FloorID { get; set; }

    [JsonPropertyName("fliphash")]
    public string FlipHash { get; set; }

    [JsonPropertyName("climbhash")]
    public string ClimbHash { get; set; }

    [JsonPropertyName("floorhash")]
    public string FloorHash { get; set; }

    [JsonPropertyName("adaptive")]
    public bool Adaptive { get; set; }
}

public sealed class CustomHat : TopCosmetic<HatViewData, HatData>
{
    [JsonPropertyName("backid")]
    public string BackID { get; set; }

    [JsonPropertyName("backflipid")]
    public string BackFlipID { get; set; }

    [JsonPropertyName("climbflipid")]
    public string ClimbFlipID { get; set; }

    [JsonPropertyName("floorflipid")]
    public string FloorFlipID { get; set; }

    [JsonPropertyName("backhash")]
    public string BackHash { get; set; }

    [JsonPropertyName("backfliphash")]
    public string BackFlipHash { get; set; }

    [JsonPropertyName("climbfliphash")]
    public string ClimbFlipHash { get; set; }

    [JsonPropertyName("floorfliphash")]
    public string FloorFlipHash { get; set; }

    [JsonPropertyName("nobounce")]
    public bool NoBounce { get; set; }
}

public sealed class CustomVisor : TopCosmetic<VisorViewData, VisorData>
{
    [JsonPropertyName("infront")]
    public bool InFront { get; set; }
}

public sealed class CustomNameplate : CustomCosmetic<NamePlateViewData, NamePlateData>; // Simplifying the definition

public sealed class CustomColor : CustomCosmetic // There's no view or data for this, so we don't need to specify them
{
    [JsonPropertyName("stringid")]
    public StringNames StringID { get; set; }

    [JsonPropertyName("default")]
    public bool Default { get; set; }

    // Reserved for future use; do not remove.
    // [JsonPropertyName("contrasting")]
    // public bool Contrasting { get; set; }

    [JsonPropertyName("lighter")]
    public bool Lighter { get; set; }

    [JsonPropertyName("mainColorValues")]
    public string[] MainColorValues { get; set; }

    [JsonPropertyName("shadowColorValues")]
    public string[] ShadowColorValues { get; set; }

    [JsonPropertyName("timeSpeed")]
    public float TimeSpeed { get; set; }

    // The ones marked with [JsonIgnore] are serialised elsewhere

    [JsonIgnore]
    public bool Changing => MainColorValues?.Length > 1 || ShadowColorValues?.Length > 1;

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

    public UColor GetColor()
    {
        if (MainColorValues == null || MainColorValues.Length == 0)
            return MainColor = ShadowColor.Light();

        return MainColor = LerpColors(TimeSpeed, MainColors);
    }

    public UColor GetShadowColor()
    {
        if (ShadowColorValues == null || ShadowColorValues.Length == 0)
            return ShadowColor = MainColor.Shadow();

        return ShadowColor = LerpColors(TimeSpeed, ShadowColors);
    }

    private static UColor LerpColors(float mul, UColor[] colors)
    {
        if (colors.Length == 1)
            return colors[0];

        // Math nerd rambling
        // Mapping these next 4 lines onto desmos gives a nice little zig-zag graph, make sure your x is Time and that mul and colors.Length are control variables for the function
        var dx = mul * Time.time;
        var floor = Mathf.FloorToInt(dx);
        var phase = floor % 2; // 0 = forward, 1 = backward
        var point = Mathf.Clamp((colors.Length - 1) * (((dx - floor) * ((2 * phase) - 1)) + 1 - phase), 0f, colors.Length - 1);

        var index = Mathf.FloorToInt(point);

        return UColor.Lerp(colors[index], colors[index + 1], point - index);
        // Index out of range usually never happens because how the function is set up, and if does happen it's only for a fraction of a second and not perceptible
    }
}