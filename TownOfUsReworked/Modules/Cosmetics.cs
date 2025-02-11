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

public abstract class CustomCosmetic<TView, TData> : CustomCosmetic where TView : ScriptableObject where TData : CosmeticData
{
    [JsonPropertyName("artist")]
    public string Artist { get; set; }

    [JsonIgnore]
    public TView ViewData { get; set; }

    [JsonIgnore]
    public TData CosmeticData { get; set; }

    [JsonPropertyName("mainhash")]
    public string MainHash { get; set; }
}

public class CustomHat : CustomCosmetic<HatViewData, HatData>
{
    [JsonPropertyName("flipid")]
    public string FlipID { get; set; }

    [JsonPropertyName("backid")]
    public string BackID { get; set; }

    [JsonPropertyName("backflipid")]
    public string BackFlipID { get; set; }

    [JsonPropertyName("climbid")]
    public string ClimbID { get; set; }

    [JsonPropertyName("climbflipid")]
    public string ClimbFlipID { get; set; }

    [JsonPropertyName("floorid")]
    public string FloorID { get; set; }

    [JsonPropertyName("floorflipid")]
    public string FloorFlipID { get; set; }

    [JsonPropertyName("fliphash")]
    public string FlipHash { get; set; }

    [JsonPropertyName("backhash")]
    public string BackHash { get; set; }

    [JsonPropertyName("backfliphash")]
    public string BackFlipHash { get; set; }

    [JsonPropertyName("climbhash")]
    public string ClimbHash { get; set; }

    [JsonPropertyName("climbfliphash")]
    public string ClimbFlipHash { get; set; }

    [JsonPropertyName("floorhash")]
    public string FloorHash { get; set; }

    [JsonPropertyName("floorfliphash")]
    public string FloorFlipHash { get; set; }

    [JsonPropertyName("nobounce")]
    public bool NoBounce { get; set; }

    [JsonPropertyName("adaptive")]
    public bool Adaptive { get; set; }
}

public class CustomVisor : CustomCosmetic<VisorViewData, VisorData>
{
    [JsonPropertyName("flipid")]
    public string FlipID { get; set; }

    [JsonPropertyName("floorid")]
    public string FloorID { get; set; }

    [JsonPropertyName("climbid")]
    public string ClimbID { get; set; }

    [JsonPropertyName("fliphash")]
    public string FlipHash { get; set; }

    [JsonPropertyName("climbhash")]
    public string ClimbHash { get; set; }

    [JsonPropertyName("floorhash")]
    public string FloorHash { get; set; }

    [JsonPropertyName("adaptive")]
    public bool Adaptive { get; set; }

    [JsonPropertyName("infront")]
    public bool InFront { get; set; }
}

public class CustomNameplate : CustomCosmetic<NamePlateViewData, NamePlateData>;

public class CustomColor : CustomCosmetic
{
    [JsonPropertyName("stringid")]
    public StringNames StringID { get; set; }

    [JsonPropertyName("default")]
    public bool Default { get; set; }

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

        return MainColor = LerpColors(TimeSpeed, MainColors, MainColor);
    }

    public UColor GetShadowColor()
    {
        if (ShadowColorValues == null || ShadowColorValues.Length == 0)
            return ShadowColor = MainColor.Shadow();

        return ShadowColor = LerpColors(TimeSpeed, ShadowColors, ShadowColor);
    }

    private static UColor LerpColors(float mul, UColor[] colors, UColor prevColor)
    {
        if (colors.Length == 1)
            return colors[0];

        try
        {
            // Math nerd rambling
            // Mapping these next 4 lines onto desmos gives a nice little zigzag graph, make sure your x is Time and that mul and colors.Length are control variables for the function
            var dx = mul * Time.time;
            var f = Mathf.FloorToInt(dx);
            var m = f % 2;
            var point = (colors.Length - 1) * (((dx - f) * ((2 * m) - 1)) + 1 - m);

            var index = Mathf.FloorToInt(point);

            return UColor.Lerp(colors[index], colors[index + 1], point - index);
        }
        catch
        {
            return prevColor;
        }
    }
}