using System.Runtime.CompilerServices;
using TownOfUsReworked.RPCs.Messages.Misc;

namespace TownOfUsReworked.Managers;

public static class CustomStatsManager
{
    private static StringNames StatsGamesWon;
    private static StringNames StatsGamesLost;
    private static StringNames StatsGamesDrawn;
    public static StringNames StatsRoleblocked;
    public static StringNames StatsKilled;
    public static StringNames StatsHitImmune;
    public static StringNames StatsConvertedFanatics;
    private static StringNames StatsLayerWins;
    private static StringNames StatsMapWins;
    private static StringNames StatsFactionGames;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static List<StringNames> OrderedStats;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private static readonly List<StringNames> SupportedVanillaStats =
    [
        StringNames.StatsBodiesReported,
        StringNames.StatsEmergenciesCalled,
        StringNames.StatsTasksCompleted,
        StringNames.StatsAllTasksCompleted,
        StringNames.StatsSabotagesFixed,
        StringNames.StatsTimesMurdered,
        StringNames.StatsTimesEjected,
        StringNames.StatsGamesStarted,
        StringNames.StatsGamesFinished,
        StringNames.StatsTimesPettedPet,

        // These are mapped to custom stats
        StringNames.StatsImpostorKills
    ];

    private static readonly Dictionary<Map, uint> MapWins = [];
    private static readonly Dictionary<Faction, uint> FactionWins = [];
    private static readonly Dictionary<Layer, uint> LayerWins = [];
    private static readonly Dictionary<StringNames, uint> CustomStats = [];

    private static readonly ValueMap<StringNames, Map> MapMap = [];
    private static readonly ValueMap<StringNames, StatID> StatsMap = [];
    private static readonly ValueMap<StringNames, Faction> FactionMap = [];
    private static readonly ValueMap<StringNames, Layer> LayerMap = [];

    private static readonly EnumInjector<StatID> Injector = new();

    public static void Setup()
    {
        var path = Path.Combine(Application.persistentDataPath, "reworkedStats");

        if (File.Exists(path))
        {
            try
            {
                using var reader = new BinaryReader(File.OpenRead(path));
                reader.DeserializeCustomStats();
            } catch {}
        }

        StatsGamesWon = TranslationManager.GetOrAddName("Stats.GamesWon");
        StatsGamesLost = TranslationManager.GetOrAddName("Stats.GamesLost");
        StatsGamesDrawn = TranslationManager.GetOrAddName("Stats.GamesDrawn");
        StatsRoleblocked = TranslationManager.GetOrAddName("Stats.Roleblocked");
        StatsKilled = TranslationManager.GetOrAddName("Stats.Killed", StringNames.StatsImpostorKills);
        StatsHitImmune = TranslationManager.GetOrAddName("Stats.HitImmune");
        StatsConvertedFanatics = TranslationManager.GetOrAddName("Stats.ConvertedFanatics");
        StatsLayerWins = TranslationManager.GetOrAddName("Stats.LayerWins");
        StatsMapWins = TranslationManager.GetOrAddName("Stats.MapWins");
        StatsFactionGames = TranslationManager.GetOrAddName("Stats.Games");

        // Preloading any missing stats
        var factions = new[] { Faction.Crew, Faction.Intruder, Faction.Syndicate, Faction.Apocalypse, Faction.Outcast, Faction.Pandorica, Faction.Compliance, Faction.Illuminati,
            Faction.Cabal, Faction.Cult, Faction.Followers, Faction.Reanimated, Faction.Undead };

        Enum.GetValues<Map>().Do(x => GetMapWins(x));
        LayerDictionary.Keys.Do(x => GetLayerWins(x));
        factions.Do(x => GetFactionWins(x));
        Enum.GetValues<StringNames>().Do(x => GetStat(x));

        foreach (var faction in factions)
        {
            FactionMap[TranslationManager.GetOrAddName($"Stats.Games.{faction}", customName: StatsFactionGames, replacements: [("%faction%", () =>
                TranslationManager.Translate($"Faction.{faction}"))])] = faction;
        }

        foreach (var map in MapWins.Keys)
        {
            MapMap[TranslationManager.GetOrAddName($"Stats.MapWins.{map}", customName: StatsMapWins, replacements: [("%map%", () => TranslationManager.Translate($"Map.{map}"))])] = map;
        }

        foreach (var layer in LayerWins.Keys)
        {
            LayerMap[TranslationManager.GetOrAddName($"Stats.LayerWins.{layer}", customName: StatsLayerWins, replacements: [("%layer%", () => TranslationManager.Translate($"Layer.{layer}"))])] =
                layer;
        }

        OrderedStats =
        [
            .. MapMap.Keys,
            StatsGamesWon,
            StatsGamesLost,
            StatsGamesDrawn,
            StringNames.StatsGamesStarted,
            StringNames.StatsGamesFinished,
            StringNames.StatsBodiesReported,
            StringNames.StatsEmergenciesCalled,
            StringNames.StatsTasksCompleted,
            StringNames.StatsAllTasksCompleted,
            StatsRoleblocked,
            StatsKilled,
            StringNames.StatsTimesMurdered,
            StringNames.StatsTimesEjected,
            StatsHitImmune,
            StatsConvertedFanatics,
            StringNames.StatsTimesPettedPet,
            .. LayerMap.Keys
        ];

        foreach (var pair in StatsPopup.BaseStatsToShow)
        {
            if (OrderedStats.Contains(pair.Value))
                StatsMap[pair.Value] = pair.Key;
        }

        OrderedStats.Where(stat => !StatsMap.ContainsKey(stat)).Do(x => StatsMap.Add(x, Injector.InjectAndReturn(x.ToString())));
    }

    // public static void Reset()
    // {
    //     MapWins.Clear();
    //     LayerWins.Clear();
    //     CustomStats.Clear();
    // }

    private static void IncrementStat(Layer layer)
    {
        if (LayerDictionary.ContainsKey(layer))
            LayerWins[layer] = LayerWins.GetValueOrDefault(layer) + 1;
    }

    private static void IncrementStat(Map map) => MapWins[map] = MapWins.GetValueOrDefault(map) + 1;

    public static void IncrementStat(Faction faction)
    {
        if (FactionDictionary.ContainsKey(faction))
            FactionWins[faction] = FactionWins.GetValueOrDefault(faction) + 1;
    }

    public static void IncrementStat(StatID stat) => IncrementStat(StatsMap[stat]);

    public static void IncrementStat(StringNames stat)
    {
        var success = !(IsFreePlay() || TownOfUsReworked.MciActive) && (SupportedVanillaStats.Contains(stat) || stat > 0);

        if (!success)
            return;

        if (LayerMap.TryGetValue(stat, out var layer))
            IncrementStat(layer);
        else if (MapMap.TryGetValue(stat, out var map))
            IncrementStat(map);
        else
        {
            if (TranslationManager.VanillaToCustomMap.TryGetValue(stat, out var customStat))
                stat = customStat;

            CustomStats[stat] = CustomStats.GetValueOrDefault(stat) + 1;
        }

        ReworkedDataManager.Save();
    }

    public static void RpcIncrementStat(PlayerControl player, StringNames stat)
    {
        if (player.AmOwner || TownOfUsReworked.MciActive)
            IncrementStat(stat);
        else
            CallTargetedRpc(new StatMessage(stat), player.OwnerId);
    }

    public static IObject GetStat(StringNames stat)
    {
        if (!SupportedVanillaStats.Contains(stat) && stat >= 0)
            return null!;

        if (LayerMap.TryGetValue(stat, out var layer))
            return GetLayerWins(layer);

        if (MapMap.TryGetValue(stat, out var map))
            return GetMapWins(map);

        if (FactionMap.TryGetValue(stat, out var faction))
            return GetFactionWins(faction);

        if (!CustomStats.TryGetValue(stat, out var val))
            CustomStats[stat] = val = 0;

        return val;
    }

    private static uint GetMapWins(Map map)
    {
        if (map == Map.Random)
            return 0;

        if (!MapWins.TryGetValue(map, out var val))
            return MapWins[map] = 0;

        return val;
    }

    private static uint GetLayerWins(Layer layer)
    {
        if (!LayerDictionary.ContainsKey(layer))
            return 0;

        if (!LayerWins.TryGetValue(layer, out var val))
            return LayerWins[layer] = 0;

        return val;
    }

    private static uint GetFactionWins(Faction faction)
    {
        if (!FactionDictionary.ContainsKey(faction))
            return 0;

        if (!FactionWins.TryGetValue(faction, out var val))
            return FactionWins[faction] = 0;

        return val;
    }

    public static void AddWin(byte map, IEnumerable<PlayerLayer> layers)
    {
        IncrementStat(StatsGamesWon);
        IncrementStat(StatID.GamesFinished);
        IncrementStat((Map)map);

        foreach (var layer in layers)
        {
            IncrementStat(layer.Type);

            if (GetLayerWins(layer.Type) == 5)
                CustomAchievementManager.UnlockAchievement($"LayerWins.{layer.Type}");

            if (layer is not Role role)
                continue;

            foreach (var (role2, _) in role.Handler.History)
            {
                IncrementStat(role2);

                if (GetLayerWins(role2) == 5)
                    CustomAchievementManager.UnlockAchievement($"LayerWins.{role2}");
            }

            if (role is not IPromoter promoter)
                continue;

            if (promoter.IsUnderling || promoter.IsPromoted)
            {
                IncrementStat(promoter.UnderlingType);

                if (GetLayerWins(promoter.UnderlingType) == 5)
                    CustomAchievementManager.UnlockAchievement($"LayerWins.{promoter.UnderlingType}");
            }

            if (!promoter.IsPromoted)
                continue;

            IncrementStat(promoter.PromoterType);

            if (GetLayerWins(promoter.PromoterType) == 5)
                CustomAchievementManager.UnlockAchievement($"LayerWins.{promoter.PromoterType}");
        }

        ReworkedDataManager.Save();
    }

    public static void AddLoss()
    {
        IncrementStat(StatsGamesLost);
        IncrementStat(StringNames.StatsGamesFinished);
        ReworkedDataManager.Save();
    }

    public static void AddDraw()
    {
        IncrementStat(StatsGamesDrawn);
        IncrementStat(StringNames.StatsGamesFinished);
        ReworkedDataManager.Save();
    }

    public static void SerializeCustomStats(this BinaryWriter writer)
    {
        writer.Write(CustomStats);
        writer.Write(LayerWins);
        writer.Write(MapWins);
    }

    private static void DeserializeCustomStats(this BinaryReader reader)
    {
        reader.ReadEnumDict(CustomStats);
        reader.ReadEnumDict(LayerWins);
        reader.ReadEnumDict(MapWins);
    }

    private static void ReadEnumDict<T>(this BinaryReader reader, Dictionary<T, uint> dict) where T : struct, Enum
    {
        var num = reader.ReadUInt32();

        while (num-- > 0)
            dict[reader.ReadEnum<T>()] = reader.ReadUInt32();
    }

    private static T ReadEnum<T>(this BinaryReader reader) where T : struct, Enum
    {
        var size = Unsafe.SizeOf<T>();

        switch (size)
        {
            case 1:
            {
                var v = reader.ReadByte();
                return Unsafe.As<byte, T>(ref v);
            }
            case 2:
            {
                var v = reader.ReadUInt16();
                return Unsafe.As<ushort, T>(ref v);
            }
            case 4:
            {
                var v = reader.ReadUInt32();
                return Unsafe.As<uint, T>(ref v);
            }
            case 8:
            {
                var v = reader.ReadUInt64();
                return Unsafe.As<ulong, T>(ref v);
            }
            default:
                throw new ArgumentException($"Enum size {size} not supported");
        }
    }

    private static void Write<T>(this BinaryWriter writer, T value) where T : struct, Enum
    {
        var size = Unsafe.SizeOf<T>();

        switch (size)
        {
            case 1:
            {
                writer.Write(Unsafe.As<T, byte>(ref value));
                break;
            }
            case 2:
            {
                writer.Write(Unsafe.As<T, ushort>(ref value));
                break;
            }
            case 4:
            {
                writer.Write(Unsafe.As<T, uint>(ref value));
                break;
            }
            case 8:
            {
                writer.Write(Unsafe.As<T, ulong>(ref value));
                break;
            }
            default:
                throw new ArgumentException($"Enum size {size} not supported");
        }
    }

    private static void Write<T>(this BinaryWriter writer, Dictionary<T, uint> dict) where T : struct, Enum
    {
        writer.Write((uint)dict.Count);

        foreach (var (key, stat) in dict)
        {
            writer.Write(key);
            writer.Write(stat);
        }
    }
}