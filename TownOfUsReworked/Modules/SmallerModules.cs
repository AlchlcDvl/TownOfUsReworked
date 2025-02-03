namespace TownOfUsReworked.Modules;

public record class SummaryInfo(string PlayerName, string History, string CachedHistory);

public readonly struct PointInTime(Vector3 position)
{
    public readonly Vector3 Position { get; } = position;
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

public class DownloadableAsset : Asset
{
    [JsonPropertyName("hash")]
    public string Hash { get; set; }
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
        var parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new(int.Parse(parts[0]), int.Parse(parts[1]), bool.Parse(parts[2]), bool.Parse(parts[3]), Enum.Parse<LayerEnum>(parts[4]));
    }

    public readonly RoleOptionData Clone() => new(Chance, Count, Unique, Active, ID);

    public readonly bool IsActive(int? relatedCount = null) => ((Chance > 0 && IsClassic()) || (Active && IsAllAny()) || (IsRoleList() && ListEntryAttribute.IsAdded(ID))) &&
        ID.IsValid(relatedCount);
}

public record class LayerDictionaryEntry(Type LayerType, UColor Color, LayerEnum Layer)
{
    public string Name => TranslationManager.Translate($"Layer.{Layer}");
}

public readonly struct Number(float num) : IComparable, IConvertible, ISpanFormattable, IEquatable<Number>, IComparable<Number>
{
    public float Value { get; } = num;

    public static implicit operator float(Number number) => number.Value;

    public static implicit operator int(Number number) => (int)number.Value;

    public static implicit operator byte(Number number) => (byte)number.Value;

    public static implicit operator Number(float num) => new(num);

    public static implicit operator Number(int num) => new(num);

    public static bool operator ==(Number a, Number b) => a.Value == b.Value;

    public static bool operator !=(Number a, Number b) => a.Value != b.Value;

    public static bool operator >(Number a, Number b) => a.Value > b.Value;

    public static bool operator >=(Number a, Number b) => a.Value >= b.Value;

    public static bool operator <(Number a, Number b) => a.Value < b.Value;

    public static bool operator <=(Number a, Number b) => a.Value <= b.Value;

    public static Number operator ^(Number a, Number b) => Mathf.Pow(a.Value, b.Value);

    public override readonly string ToString() => Value.ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);

    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) =>
        Value.TryFormat(destination, out charsWritten, format, provider);

    public override readonly bool Equals(object obj) => obj is Number number && Equals(number);

    public readonly bool Equals(Number other) => Value == other.Value;

    public override readonly int GetHashCode() => Value.GetHashCode();

    public static Number Parse(string value) => new(float.Parse(value));

    public TypeCode GetTypeCode() => TypeCode.Single;

    public bool ToBoolean(IFormatProvider? provider) => Convert.ToBoolean(Value, provider);

    public byte ToByte(IFormatProvider? provider) => Convert.ToByte(Value, provider);

    public char ToChar(IFormatProvider? provider) => Convert.ToChar(Value, provider);

    public DateTime ToDateTime(IFormatProvider? provider) => Convert.ToDateTime(Value, provider);

    public decimal ToDecimal(IFormatProvider? provider) => Convert.ToDecimal(Value, provider);

    public double ToDouble(IFormatProvider? provider) => Convert.ToDouble(Value, provider);

    public short ToInt16(IFormatProvider? provider) => Convert.ToInt16(Value, provider);

    public int ToInt32(IFormatProvider? provider) => Convert.ToInt32(Value, provider);

    public long ToInt64(IFormatProvider? provider) => Convert.ToInt64(Value, provider);

    public sbyte ToSByte(IFormatProvider? provider) => Convert.ToSByte(Value, provider);

    public float ToSingle(IFormatProvider? provider) => Convert.ToSingle(Value, provider);

    public string ToString(IFormatProvider? provider) => Convert.ToString(Value, provider);

    public object ToType(Type conversionType, IFormatProvider? provider) => Convert.ChangeType(Value, conversionType, provider);

    public ushort ToUInt16(IFormatProvider? provider) => Convert.ToUInt16(Value, provider);

    public uint ToUInt32(IFormatProvider? provider) => Convert.ToUInt32(Value, provider);

    public ulong ToUInt64(IFormatProvider? provider) => Convert.ToUInt64(Value, provider);

    public int CompareTo(object? obj)
    {
        if (obj is Number other)
            return Value.CompareTo(other.Value);

        throw new ArgumentException("Object is not a Number");
    }

    public int CompareTo(Number other) => Value.CompareTo(other.Value);
}

public class UnsupportedLanguageException(string message) : Exception($"Selected language is unsupported {message}");

public class Achievement(string name, bool unlocked = false, bool hidden = false, bool eog = false, string icon = "Placeholder")
{
    public string Name { get; } = name;
    public bool Hidden { get; } = hidden;
    public bool EndOfGame { get; } = eog;
    public string Icon { get; } = icon;
    public bool Unlocked { get; set; } = unlocked;
}

// public struct PlayerRecord()
// {
//     public readonly List<LayerEnum> Layers = [];
//     public readonly List<LayerEnum> History = [];
//     public readonly List<string> Titles = [];

//     public string PlayerName { get; set; }
//     public uint ColorId { get; set; }
//     public string SkinId { get; set; }
//     public string HatId { get; set; }
//     public string VisorId { get; set; }

//     public bool CanDoTasks { get; set; }
//     public uint TasksDone { get; set; }
//     public uint TasksLeft { get; set; }

//     public bool IsExeTarget { get; set; }
//     public bool IsGATarget { get; set; }
//     public bool IsGuessTarget { get; set; }
//     public bool IsBHTarget { get; set; }

//     public bool DriveHolder { get; set; }

//     public SubFaction SubFaction { get; set; }
//     public DeathReasonEnum DeathReason { get; set; }

//     public static PlayerRecord Generate(PlayerControl player)
//     {
//         var info = player.GetLayers();
//         var role = (Role)info.First();
//         role.RoleHistory.Reverse();
//         var outfit = player.Data.DefaultOutfit;

//         var record = new PlayerRecord()
//         {
//             PlayerName = outfit.PlayerName,
//             ColorId = (uint)outfit.ColorId,
//             SkinId = outfit.SkinId,
//             HatId = outfit.HatId,
//             VisorId = outfit.VisorId,
//             CanDoTasks = player.CanDoTasks(),
//             TasksLeft = (uint)role.TasksLeft,
//             TasksDone = (uint)(role.TotalTasks - role.TasksLeft),
//             IsExeTarget = player.IsExeTarget(),
//             IsGATarget = player.IsGATarget(),
//             IsBHTarget = player.IsBHTarget(),
//             IsGuessTarget = player.IsGuessTarget(),
//             DriveHolder = Syndicate.DriveHolder == player,
//             DeathReason = role.DeathReason,
//             SubFaction = role.SubFaction
//         };

//         record.Layers.AddRange(info.Select(x => x.Type));
//         record.History.AddRange(role.RoleHistory);

//         if (player.CanKill())
//         {
//             if (!KillCounts.TryGetValue(player.PlayerId, out var count) || count == 0)
//                 record.Titles.Add("Pacifist");
//             else if (count >= KillCounts.Values.Max())
//                 record.Titles.Add("Bloodthirsty");
//         }

//         return record;
//     }

//     public static bool operator ==(PlayerRecord a, PlayerRecord b) => a.PlayerName == b.PlayerName;

//     public static bool operator !=(PlayerRecord a, PlayerRecord b) => a.PlayerName != b.PlayerName;

//     public override readonly bool Equals(object obj)
//     {
//         if (obj is PlayerRecord other)
//             return PlayerName == other.PlayerName;

//         return false;
//     }

//     public override readonly int GetHashCode() => HashCode.Combine(PlayerName, ColorId, SkinId, HatId, VisorId);
// }