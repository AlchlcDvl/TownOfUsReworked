namespace TownOfUsReworked.Modules;

public abstract class CustomCosmetic : Asset
{
    [JsonPropertyName("artist")]
    public string Artist { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("condition")]
    public string Condition { get; set; }

    [JsonPropertyName("stream")]
    public bool StreamOnly { get; set; }

    [JsonPropertyName("test")]
    public bool TestOnly { get; set; }
}

public abstract class CosmeticExtension
{
    public string Artist { get; set; }
    public string Condition { get; set; }
    public bool StreamOnly { get; set; }
    public bool TestOnly { get; set; }
}

public class CustomHat : CustomCosmetic
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

    [JsonPropertyName("nobounce")]
    public bool NoBounce { get; set; }

    [JsonPropertyName("adaptive")]
    public bool Adaptive { get; set; }

    [JsonIgnore]
    public bool Behind { get; set; }
}

public class HatExtension : CosmeticExtension
{
    public Sprite FlipImage { get; set; }
    public Sprite BackFlipImage { get; set; }
}

public class CustomVisor : CustomCosmetic
{
    [JsonPropertyName("flipid")]
    public string FlipID { get; set; }

    [JsonPropertyName("floorid")]
    public string FloorID { get; set; }

    [JsonPropertyName("climbid")]
    public string ClimbID { get; set; }

    [JsonPropertyName("adaptive")]
    public bool Adaptive { get; set; }

    [JsonPropertyName("infront")]
    public bool InFront { get; set; }
}

public class VisorExtension : CosmeticExtension
{
    public Sprite ClimbImage { get; set; }
    public Sprite FloorImage { get; set; }
}

public class CustomNameplate : CustomCosmetic;

public class NameplateExtension : CosmeticExtension;

public class CustomColor : CustomCosmetic
{
    [JsonPropertyName("stringid")]
    public int StringID { get; set; }

    [JsonPropertyName("default")]
    public bool Default { get; set; }

    [JsonPropertyName("contrasting")]
    public bool Contrasting { get; set; }

    [JsonPropertyName("lighter")]
    public bool Lighter { get; set; }

    [JsonPropertyName("mainColorValues")]
    public string[] MainColorValues { get; set; }

    [JsonPropertyName("shadowColorValues")]
    public string[] ShadowColorValues { get; set; }

    [JsonPropertyName("timeSpeed")]
    public float TimeSpeed { get; set; }

    [JsonIgnore]
    public int ColorID { get; set; }

    [JsonIgnore]
    public UColor[] MainColors { get; set; }

    [JsonIgnore]
    public UColor[] ShadowColors { get; set; }

    [JsonIgnore]
    public bool Changing { get; set; }

    [JsonIgnore]
    public UColor MainColor { get; set; }

    [JsonIgnore]
    public UColor ShadowColor { get; set; }

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

    public static UColor LerpColors(float mul, UColor[] colors, UColor? prevColor = null)
    {
        if (colors.Length == 1)
            return colors[0];

        try
        {
            // Math nerd rambling
            // Mapping these next 4 lines onto desmos gives a nice little zig zag graph, make sure your x is Time and that mul and colors.Length are control variables for the function
            var dx = mul * Time.time;
            var f = Mathf.FloorToInt(dx);
            var m = f % 2;
            var point = (colors.Length - 1) * (((dx - f) * ((2 * m) - 1)) + 1 - m);

            var index = Mathf.FloorToInt(point);

            return UColor.Lerp(colors[index], colors[index + 1], point - index);
        }
        catch
        {
            return prevColor ?? colors[0];
        }
    }
}

// Idk why i did it, but ig i just really wanted it for consistency's sake
// public class ColorExtention : CosmeticExtension;