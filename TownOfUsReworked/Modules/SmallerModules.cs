// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace TownOfUsReworked.Modules;

public readonly record struct PointInTime(Vector3 Position);

public sealed class GitHubApiObject
{
    [JsonPropertyName("tag_name")]
    public string Tag;

    [JsonPropertyName("body")]
    public string Description;

    [JsonPropertyName("assets")]
    public GitHubApiAsset[] Assets;
}

public sealed class GitHubApiAsset
{
    [JsonPropertyName("browser_download_url")]
    public string URL;
}

public readonly record struct LayerDictionaryEntry(Type LayerType, UColor Color, Layer Layer, string Symbol = null)
{
    public readonly string Name = TranslationManager.Translate($"Layer.{Layer}");
}

public readonly record struct FactionDictionaryEntry(Faction Faction, UColor Color)
{
    public readonly string Name = TranslationManager.Translate($"Faction.{Faction}");
}

public sealed class Achievement(string name, bool unlocked = false, bool hidden = false, bool eog = false, string icon = "Placeholder")
{
    public readonly string Name = name;
    public readonly string Icon = icon;
    public readonly bool Hidden = hidden;
    public readonly bool EndOfGame = eog;
    public bool Unlocked = unlocked;

    public bool IsId(string id) => id.IsAny($"Achievement.{Name}.Title", $"Achievement.{Name}.Description");
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
public sealed class SortedAttribute(int order) : Attribute
{
    public readonly int Order = order;
}

public delegate bool WhereSelectFilter<in T1, T2>(T1 param, out T2 value);

// /// <summary>
// /// Wrapper to act as an out parameter for coroutines.
// /// </summary>
// /// <typeparam name="T">The type of the value being set.</typeparam>
// public sealed class Out<T>(T value = default)
// {
//     public T Value = value;
//
//     public static implicit operator T(Out<T> outVal) => outVal.Value;
//
//     public static implicit operator Out<T>(T val) => new(val);
// }

/// <summary>
/// A struct that signifies a pair of colors. Used to simplify some code regarding player material colors.
/// </summary>
/// <param name="color1">The first color.</param>
/// <param name="color2">The second color.</param>
public readonly struct ColorPair(UColor color1, UColor color2)
{
    public readonly UColor Color1 = color1;
    public readonly UColor Color2 = color2;

    // public UColor Lerp(float t) => UColor.Lerp(Color1, Color2, t);

    public static ColorPair Lerp(ColorPair a, ColorPair b, float t) => new(UColor.Lerp(a.Color1, b.Color1, t), UColor.Lerp(a.Color2, b.Color2, t));
}

public sealed class VersionData
{
    [JsonPropertyName("gameVers")]
    // ReSharper disable once CollectionNeverUpdated.Global
    public Dictionary<int, string> GameVersions;

    [JsonPropertyName("modVers")]
    public string[] ModVersions;
}