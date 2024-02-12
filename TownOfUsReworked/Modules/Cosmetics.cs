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

//Idk why i did it, but ig i just really wanted it for consistency's sake
public abstract class CosmeticsJSON {}

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

    [JsonPropertyName("behind")]
    public bool Behind { get; set; }
}

public class HatExtension : CosmeticExtension
{
    public Sprite FlipImage { get; set; }
    public Sprite BackFlipImage { get; set; }
}

public class HatsJSON : CosmeticsJSON
{
    [JsonPropertyName("hats")]
    public List<CustomHat> Hats { get; set; }
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

public class VisorExtension : CosmeticExtension {}

public class VisorsJSON : CosmeticsJSON
{
    [JsonPropertyName("visors")]
    public List<CustomVisor> Visors { get; set; }
}

public class CustomNameplate : CustomCosmetic {}

public class NameplateExtension : CosmeticExtension {}

public class NameplatesJSON : CosmeticsJSON
{
    [JsonPropertyName("nameplates")]
    public List<CustomNameplate> Nameplates { get; set; }
}

public class CustomColor : CustomCosmetic
{
    [JsonPropertyName("stringid")]
    public int StringID { get; set; }

    [JsonPropertyName("rgbmain")]
    public string RGBMain { get; set; }

    [JsonPropertyName("rgbshadow")]
    public string RGBShadow { get; set; }

    [JsonPropertyName("hsbmain")]
    public string HSBMain { get; set; }

    [JsonPropertyName("hsbshadow")]
    public string HSBShadow { get; set; }

    [JsonPropertyName("default")]
    public bool Default { get; set; }

    [JsonPropertyName("contrasting")]
    public bool Contrasting { get; set; }

    [JsonPropertyName("lighter")]
    public bool Lighter { get; set; }

    [JsonPropertyName("noshadow")]
    public bool NoShadow { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonIgnore]
    public int ColorID { get; set; }

    [JsonIgnore]
    public bool Changing => HSBMain != null;

    [JsonIgnore]
    public bool ChangingShadow => HSBShadow != null;

    [JsonIgnore]
    public bool Shadow => RGBShadow != null;

    [JsonIgnore]
    public bool OverrideShadow => ChangingShadow || Shadow;

    [JsonIgnore]
    public Color32 MainColor => Changing ? HSBColor.Parse(HSBMain).ToColor() : CustomColorManager.ParseToColor(RGBMain);

    [JsonIgnore]
    public Color32 ShadowColor
    {
        get
        {
            if (NoShadow)
                return MainColor;
            else if (OverrideShadow)
                return ChangingShadow ? HSBColor.Parse(HSBShadow).ToColor() : CustomColorManager.ParseToColor(RGBShadow);
            else
                return MainColor.Shadow();
        }
    }
}

//Idk why i did it, but ig i just really wanted it for consistency's sake part 2
public class ColorExtention : CosmeticExtension {}

public class ColorsJSON : CosmeticsJSON
{
    [JsonPropertyName("colors")]
    public List<CustomColor> Colors { get; set; }
}