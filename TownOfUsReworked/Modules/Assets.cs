namespace TownOfUsReworked.Modules;

public class Asset
{
    [JsonPropertyName("id")]
    public string ID { get; set; }
}

public class PresetsJSON
{
    [JsonPropertyName("presets")]
    public List<Asset> Presets { get; set; }
}

public class ImagesJSON
{
    [JsonPropertyName("images")]
    public List<Asset> Images { get; set; }
}

public class PortalJSON
{
    [JsonPropertyName("portal")]
    public List<Asset> Portal { get; set; }
}

public class SoundsJSON
{
    [JsonPropertyName("sounds")]
    public List<Asset> Sounds { get; set; }
}