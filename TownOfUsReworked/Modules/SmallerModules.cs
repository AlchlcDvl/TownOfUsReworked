namespace TownOfUsReworked.Modules;

public record class SummaryInfo(string PlayerName, string History, string CachedHistory);

public readonly struct PointInTime(Vector3 position)
{
    public readonly Vector3 Position { get; } = position;
}

public record class PlayerVersion(Guid Guid, string VersionFinal, Version Version)
{
    public int Diff => TownOfUsReworked.Version.CompareTo(Version);
    public bool VersionMatches => Diff == 0;
    public bool GuidMatches => TownOfUsReworked.Core.ManifestModule.ModuleVersionId.Equals(Guid);
    public bool VersionStringMatches => VersionFinal.Replace("_test", "") == TownOfUsReworked.VersionFinal.Replace("_test", "");
    public bool EverythingMatches => GuidMatches && VersionStringMatches && VersionMatches;
}

public class GitHubApiObject
{
    [JsonPropertyName("tag_name")]
    public string Tag { get; set; }

    [JsonPropertyName("body")]
    public string Description { get; set; }

    [JsonPropertyName("assets")]
    public GitHubApiAsset[] Assets { get; set; }
}

public class GitHubApiAsset
{
    [JsonPropertyName("browser_download_url")]
    public string URL { get; set; }
}

public class Asset
{
    [JsonPropertyName("id")]
    public string ID { get; set; }
}

public struct RoleOptionData(int chance, int count, bool unique, bool active, LayerEnum layer)
{
    public int Chance { get; set; } = chance;
    public int Count { get; set; } = count;
    public bool Unique { get; set; } = unique;
    public bool Active { get; set; } = active;
    public LayerEnum ID { get; set; } = layer;

    public override readonly string ToString() => $"{Chance},{Count},{Unique},{Active},{ID}";

    public static RoleOptionData Parse(string input)
    {
        var parts = input.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]), bool.Parse(parts[2]), bool.Parse(parts[3]), Enum.Parse<LayerEnum>(parts[4]));
    }

    public readonly RoleOptionData Clone() => new(Chance, Count, Unique, Active, ID);

    public readonly bool IsActive(int? relatedCount = null) => ((Chance > 0 && (IsClassic() || IsCustom())) || (Active && IsAA()) || (IsRoleList() && RoleListEntryAttribute.IsAdded(ID))) &&
        ID.IsValid(relatedCount);
}

public record class LayerDictionaryEntry(Type LayerType, UColor Color, string Name);

public readonly struct Number(float num)
{
    public float Value { get; } = num;

    public static implicit operator float(Number number) => number.Value;

    public static implicit operator int(Number number) => (int)number.Value;

    public override readonly string ToString() => Value.ToString();

    public static Number Parse(string value) => new(float.Parse(value));
}

public class UnsupportedLanguageException(string message) : Exception($"Selected language is unsupported {message}");

public class Achievement(string name, bool unlocked = false, bool hidden = false, bool eog = false, string icon = "Placeholder")
{
    public string Name { get; } = name;
    public bool Hidden { get; } = hidden;
    public bool EndOfGame { get; } = eog;
    public string Icon { get; } = icon;
    public bool Unlocked { get; set; } = unlocked;

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Name);
        writer.Write(Unlocked);
    }

    public static Achievement Deserialize(BinaryReader reader) => new(reader.ReadString(), reader.ReadBoolean());
}