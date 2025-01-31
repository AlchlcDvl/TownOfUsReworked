namespace TownOfUsReworked.Managers;

public static class CustomStatsManager
{
    public static bool MigratedFromVanillaStats;

    public static StringNames StatsGamesWon;
    public static StringNames StatsGamesLost;
    public static StringNames StatsGamesDrawn;
    public static StringNames StatsRoleblocked;
    public static StringNames StatsKilled;
    public static StringNames StatsHitImmune;
    public static StringNames StatsGamesCrew;
    public static StringNames StatsGamesIntruder;
    public static StringNames StatsGamesSyndicate;
    public static StringNames StatsGamesNeutral;
    public static StringNames StatsLayerWins;
    public static StringNames StatsMapWins;

    public static List<StringNames> OrderedStats;
    private static readonly List<StringNames> SupportVanillaStats =
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
        StringNames.StatsHidenSeekGamesCrewmateSurvived,
        StringNames.StatsHidenSeekTimesVented,
        StringNames.StatsTimesPettedPet,
        StringNames.StatsImpostorKills_HideAndSeek,
        StringNames.StatsFastestCrewmateWin_HideAndSeek,
        StringNames.StatsFastestImpostorWin_HideAndSeek,
        StringNames.StatsHideAndSeekCrewmateVictory,
        StringNames.StatsHideAndSeekImpostorVictory,

        // These are mapped to custom stats
        StringNames.StatsImpostorKills
    ];

    public static readonly Dictionary<MapEnum, uint> MapWins = [];
    public static readonly Dictionary<LayerEnum, uint> LayerWins = [];
    public static readonly Dictionary<StringNames, uint> CustomStats = [];

    public static readonly ValueMap<StringNames, MapEnum> MapMap = [];
    public static readonly ValueMap<StringNames, LayerEnum> LayerMap = [];

    public static void Setup()
    {
        StatsGamesWon = TranslationManager.GetOrAddName("Stats.GamesWon");
        StatsGamesLost = TranslationManager.GetOrAddName("Stats.GamesLost");
        StatsGamesDrawn = TranslationManager.GetOrAddName("Stats.GamesDrawn");
        StatsRoleblocked = TranslationManager.GetOrAddName("Stats.Roleblocked");
        StatsKilled = TranslationManager.GetOrAddName("Stats.Killed", StringNames.StatsImpostorKills);
        StatsHitImmune = TranslationManager.GetOrAddName("Stats.HitImmune");
        StatsGamesCrew = TranslationManager.GetOrAddName("Stats.CrewGames");
        StatsGamesIntruder = TranslationManager.GetOrAddName("Stats.IntruderGames");
        StatsGamesSyndicate = TranslationManager.GetOrAddName("Stats.SyndicateGames");
        StatsGamesNeutral = TranslationManager.GetOrAddName("Stats.NeutralGames");
        StatsLayerWins = TranslationManager.GetOrAddName("Stats.LayerWins");
        StatsMapWins = TranslationManager.GetOrAddName("Stats.MapWins");

        OrderedStats =
        [
            StatsGamesCrew,
            StatsGamesIntruder,
            StatsGamesSyndicate,
            StatsGamesNeutral,
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
            StringNames.StatsHidenSeekGamesCrewmateSurvived,
            StringNames.StatsHidenSeekTimesVented,
            StringNames.StatsImpostorKills_HideAndSeek,
            StringNames.StatsTimesPettedPet,
            StringNames.StatsFastestCrewmateWin_HideAndSeek,
            StringNames.StatsFastestImpostorWin_HideAndSeek,
            StringNames.StatsHideAndSeekCrewmateVictory,
            StringNames.StatsHideAndSeekImpostorVictory
        ];

        foreach (var map in MapWins.Keys)
        {
            var val = TranslationManager.GetOrAddName($"Stats.MapWins.{map}", customName: StatsMapWins, replacements: [ ("%map%", () => TranslationManager.Translate($"Map.{map}")) ]);
            OrderedStats.Add(val);
            MapMap[val] = map;
        }

        foreach (var layer in LayerWins.Keys)
        {
            var val = TranslationManager.GetOrAddName($"Stats.LayerWins.{layer}", customName: StatsLayerWins, replacements: [ ("%layer%", () => TranslationManager.Translate($"Layer.{layer}")) ]);
            OrderedStats.Add(val);
            LayerMap[val] = layer;
        }

        var path = Path.Combine(PlatformPaths.persistentDataPath, "reworkedStats");

        if (File.Exists(path))
        {
            try
            {
                using var reader = new BinaryReader(File.OpenRead(path));
                reader.DeserializeCustomStats();
            } catch {}
        }

        // Preloading any missing stats
        Enum.GetValues<MapEnum>().ForEach(x => GetMapWins(x));
        LayerDictionary.Keys.ForEach(x => GetLayerWins(x));
        Enum.GetValues<StringNames>().ForEach(x => GetStat(x));
    }

    public static void Reset()
    {
        MapWins.Clear();
        LayerWins.Clear();
        CustomStats.Clear();
    }

    public static void MigrateFromVanillaStats(this StatsManager.Stats stats)
    {
        if (MigratedFromVanillaStats)
            return;

        foreach (var map in Enum.GetValues<MapEnum>())
        {
            if (stats.mapWins.TryGetValue((MapNames)(byte)map, out var value))
                MapWins[map] = value;
        }

        foreach (var stat in SupportVanillaStats)
        {
            if (stats.gameplayStats.TryGetValue(stat, out var value))
            {
                if (TranslationManager.VanillaToCustomMap.TryGetValue(stat, out var customStat))
                    CustomStats[customStat] = value;
                else
                    CustomStats[stat] = value;
            }
        }

        MigratedFromVanillaStats = true;
    }

    public static void IncrementStat(LayerEnum layer)
    {
        if (!LayerDictionary.ContainsKey(layer))
            return;

        if (!LayerWins.ContainsKey(layer))
            LayerWins[layer] = 0;

        LayerWins[layer]++;
        StatsManager.Instance.SaveStats();
    }

    public static void IncrementStat(MapEnum map)
    {
        if (!MapWins.ContainsKey(map))
            MapWins[map] = 0;

        MapWins[map]++;
        StatsManager.Instance.SaveStats();
    }

    public static void IncrementStat(StringNames stat, out bool success)
    {
        success = !(IsFreePlay() || TownOfUsReworked.MCIActive) && (SupportVanillaStats.Contains(stat) || stat > ReworkedStart);

        if (!success)
            return;

        if (LayerMap.TryGetValue(stat, out var layer))
        {
            IncrementStat(layer);
            return;
        }

        if (MapMap.TryGetValue(stat, out var map))
        {
            IncrementStat(map);
            return;
        }

        if (TranslationManager.VanillaToCustomMap.TryGetValue(stat, out var customStat))
            stat = customStat;

        if (!CustomStats.ContainsKey(stat))
            CustomStats[stat] = 0;

        CustomStats[stat]++;
        StatsManager.Instance.SaveStats();
    }

    public static void IncrementStat(StringNames stat) => IncrementStat(stat, out _);

    public static Il2CppSystem.Object GetStat(StringNames stat)
    {
        if (!SupportVanillaStats.Contains(stat) && stat < ReworkedStart)
            return null;

        if (LayerMap.TryGetValue(stat, out var layer))
            return GetLayerWins(layer);

        if (MapMap.TryGetValue(stat, out var map))
            return GetMapWins(map);

        if (stat == StringNames.StatsFastestCrewmateWin_HideAndSeek)
            return StatsPopup.GetFloatStatStr(StatsManager.Instance.GetFastestHideAndSeekCrewmateWin());

        if (stat == StringNames.StatsFastestImpostorWin_HideAndSeek)
            return StatsPopup.GetFloatStatStr(StatsManager.Instance.GetFastestHideAndSeekImpostorWin());

        if (stat == StringNames.StatsHideAndSeekCrewmateVictory)
            return StatsManager.Instance.GetWinReason(GameOverReason.HideAndSeek_ByTimer);

        if (stat == StringNames.StatsHideAndSeekImpostorVictory)
            return StatsManager.Instance.GetWinReason(GameOverReason.HideAndSeek_ByKills);

        if (TranslationManager.VanillaToCustomMap.TryGetValue(stat, out var customStat))
            stat = customStat;

        if (!CustomStats.TryGetValue(stat, out var val))
            return CustomStats[stat] = 0;

        return val;
    }

    public static uint GetMapWins(MapEnum map)
    {
        if (map == MapEnum.Random)
            return 0;

        if (!MapWins.TryGetValue(map, out var val))
            return MapWins[map] = 0;

        return val;
    }

    public static uint GetLayerWins(LayerEnum layer)
    {
        if (!LayerDictionary.ContainsKey(layer))
            return 0;

        if (!LayerWins.TryGetValue(layer, out var val))
            return LayerWins[layer] = 0;

        return val;
    }

    public static void AddWin(byte map, IEnumerable<PlayerLayer> layers)
    {
        IncrementStat(StatsGamesWon);
        IncrementStat(StringNames.StatsGamesFinished);
        IncrementStat((MapEnum)map);

        foreach (var layer in layers)
        {
            IncrementStat(layer.Type);

            if (GetLayerWins(layer.Type) == 5)
                CustomAchievementManager.UnlockAchievement($"LayerWins.{layer.Type}");

            if (layer is Role role)
            {
                var type = role.SubFaction switch
                {
                    SubFaction.Undead => LayerEnum.Undead,
                    SubFaction.Cabal => LayerEnum.Cabal,
                    SubFaction.Cult => LayerEnum.Cult,
                    SubFaction.Reanimated => LayerEnum.Reanimated,
                    _ => LayerEnum.None
                };
                IncrementStat(type);

                if (GetLayerWins(type) == 5)
                    CustomAchievementManager.UnlockAchievement($"LayerWins.{type}");

                foreach (var role2 in role.RoleHistory)
                {
                    IncrementStat(role2);

                    if (GetLayerWins(role2) == 5)
                        CustomAchievementManager.UnlockAchievement($"LayerWins.{role2}");
                }
            }
        }
    }

    public static void AddLoss()
    {
        IncrementStat(StatsGamesLost);
        IncrementStat(StringNames.StatsGamesFinished);
    }

    public static void AddDraw()
    {
        IncrementStat(StatsGamesDrawn);
        IncrementStat(StringNames.StatsGamesFinished);
    }

    public static void SerializeCustomStats(this BinaryWriter writer)
    {
        writer.Write(MigratedFromVanillaStats);
        writer.Write(CustomStats);
        writer.Write(LayerWins);
        writer.Write(MapWins);
    }

    public static void DeserializeCustomStats(this BinaryReader reader)
    {
        MigratedFromVanillaStats = reader.ReadBoolean();
        reader.ReadEnumDict(CustomStats);
        reader.ReadEnumDict(LayerWins);
        reader.ReadEnumDict(MapWins);

        var diff = TranslationManager.LastID - TranslationManager.PreviousLastID; // Accounting for any changes to the translations in the base game

        if (diff != 0)
        {
            var keys = CustomStats.Keys.ToList();
            var values = CustomStats.Values.ToList();
            var count = CustomStats.Count;

            CustomStats.Clear();

            // Remapping stats to ids that have been pushed further up or down the enum
            for (var i = 0; i < count; i++)
            {
                if (keys[i] < ReworkedStart)
                    CustomStats[keys[i]] = values[i];
                else
                    CustomStats[keys[i] + diff] = values[i];
            }
        }
    }

    public static void ReadEnumDict<T>(this BinaryReader reader, Dictionary<T, uint> dict) where T : struct, Enum
    {
        var num = reader.ReadUInt32();

        while (num-- > 0)
            dict[reader.ReadEnum<T>()] = reader.ReadUInt32();
    }

    public static T ReadEnum<T>(this BinaryReader reader) where T : struct, Enum
    {
        if (typeof(T).GetEnumUnderlyingType() == typeof(byte))
            return (T)(object)reader.ReadByte();
        else
            return (T)(object)reader.ReadInt32();
    }

    public static void Write(this BinaryWriter writer, Enum enumVal)
    {
        var enumType = enumVal.GetType();

        if (enumType.GetEnumUnderlyingType() == typeof(byte))
            writer.Write(Convert.ToByte(enumVal));
        else
            writer.Write(Convert.ToInt32(enumVal));
    }

    public static void Write<T>(this BinaryWriter writer, Dictionary<T, uint> dict) where T : struct, Enum
    {
        writer.Write((uint)dict.Count);

        foreach (var (key, stat) in dict)
        {
            writer.Write(key);
            writer.Write(stat);
        }
    }
}