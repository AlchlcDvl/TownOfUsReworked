namespace TownOfUsReworked.Managers;

public static class CustomStatsManager
{
    public static bool MigratedFromVanillaStats;

    private static StringNames StatsGamesWon;
    private static StringNames StatsGamesLost;
    private static StringNames StatsGamesDrawn;
    public static StringNames StatsRoleblocked;
    public static StringNames StatsKilled;
    public static StringNames StatsHitImmune;
    public static StringNames StatsGamesCrew;
    public static StringNames StatsGamesIntruder;
    public static StringNames StatsGamesSyndicate;
    public static StringNames StatsGamesNeutral;
    public static StringNames StatsGamesIlluminati;
    public static StringNames StatsGamesCompliance;
    public static StringNames StatsGamesPandorica;
    public static StringNames StatsConvertedFanatics;
    private static StringNames StatsLayerWins;
    private static StringNames StatsMapWins;

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

    private static readonly Dictionary<MapEnum, uint> MapWins = [];
    private static readonly Dictionary<LayerEnum, uint> LayerWins = [];
    private static readonly Dictionary<StringNames, uint> CustomStats = [];

    private static readonly ValueMap<StringNames, MapEnum> MapMap = [];
    private static readonly ValueMap<StringNames, LayerEnum> LayerMap = [];

    public static void Setup()
    {
        StatsGamesWon = TranslationManager.GetOrAddName("Stats.GamesWon");
        StatsGamesLost = TranslationManager.GetOrAddName("Stats.GamesLost");
        StatsGamesDrawn = TranslationManager.GetOrAddName("Stats.GamesDrawn");
        StatsRoleblocked = TranslationManager.GetOrAddName("Stats.Roleblocked");
        StatsKilled = TranslationManager.GetOrAddName("Stats.Killed", StringNames.StatsImpostorKills);
        StatsHitImmune = TranslationManager.GetOrAddName("Stats.HitImmune");
        StatsConvertedFanatics = TranslationManager.GetOrAddName("Stats.ConvertedFanatics");
        StatsGamesCrew = TranslationManager.GetOrAddName("Stats.CrewGames");
        StatsGamesIntruder = TranslationManager.GetOrAddName("Stats.IntruderGames");
        StatsGamesSyndicate = TranslationManager.GetOrAddName("Stats.SyndicateGames");
        StatsGamesNeutral = TranslationManager.GetOrAddName("Stats.NeutralGames");
        StatsGamesIlluminati = TranslationManager.GetOrAddName("Stats.IlluminatiGames");
        StatsGamesCompliance = TranslationManager.GetOrAddName("Stats.ComplianceGames");
        StatsGamesPandorica = TranslationManager.GetOrAddName("Stats.PandoricaGames");
        StatsLayerWins = TranslationManager.GetOrAddName("Stats.LayerWins");
        StatsMapWins = TranslationManager.GetOrAddName("Stats.MapWins");

        OrderedStats =
        [
            StatsGamesCrew,
            StatsGamesIntruder,
            StatsGamesSyndicate,
            StatsGamesNeutral,
            StatsGamesIlluminati,
            StatsGamesCompliance,
            StatsGamesPandorica,
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
            StringNames.StatsHidenSeekGamesCrewmateSurvived,
            StringNames.StatsHidenSeekTimesVented,
            StringNames.StatsImpostorKills_HideAndSeek,
            StringNames.StatsTimesPettedPet,
            StringNames.StatsFastestCrewmateWin_HideAndSeek,
            StringNames.StatsFastestImpostorWin_HideAndSeek,
            StringNames.StatsHideAndSeekCrewmateVictory,
            StringNames.StatsHideAndSeekImpostorVictory
        ];

        var path = Path.Combine(Application.persistentDataPath, "reworkedStats");

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
            if (!stats.gameplayStats.TryGetValue(stat, out var value))
                continue;

            if (TranslationManager.VanillaToCustomMap.TryGetValue(stat, out var customStat))
                CustomStats[customStat] = value;
            else
                CustomStats[stat] = value;
        }

        MigratedFromVanillaStats = true;
    }

    private static void IncrementStat(LayerEnum layer)
    {
        if (!LayerDictionary.ContainsKey(layer))
            return;

        LayerWins[layer] = LayerWins.GetValueOrDefault(layer) + 1;
        StatsManager.Instance.SaveStats();
    }

    private static void IncrementStat(MapEnum map)
    {
        MapWins[map] = MapWins.GetValueOrDefault(map) + 1;
        StatsManager.Instance.SaveStats();
    }

    public static void IncrementStat(StringNames stat, out bool success)
    {
        success = !(IsFreePlay() || TownOfUsReworked.MciActive) && (SupportVanillaStats.Contains(stat) || stat > ReworkedStart);

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

        CustomStats[stat] = CustomStats.GetValueOrDefault(stat) + 1;
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

        switch (stat)
        {
            case StringNames.StatsFastestCrewmateWin_HideAndSeek:
                return StatsPopup.GetFloatStatStr(StatsManager.Instance.GetFastestHideAndSeekCrewmateWin());
            case StringNames.StatsFastestImpostorWin_HideAndSeek:
                return StatsPopup.GetFloatStatStr(StatsManager.Instance.GetFastestHideAndSeekImpostorWin());
            case StringNames.StatsHideAndSeekCrewmateVictory:
                return StatsManager.Instance.GetWinReason(GameOverReason.HideAndSeek_ByTimer);
            case StringNames.StatsHideAndSeekImpostorVictory:
                return StatsManager.Instance.GetWinReason(GameOverReason.HideAndSeek_ByKills);
        }

        if (TranslationManager.VanillaToCustomMap.TryGetValue(stat, out var customStat))
            stat = customStat;

        if (!CustomStats.TryGetValue(stat, out var val))
            return CustomStats[stat] = 0;

        return val;
    }

    private static uint GetMapWins(MapEnum map)
    {
        if (map == MapEnum.Random)
            return 0;

        if (!MapWins.TryGetValue(map, out var val))
            return MapWins[map] = 0;

        return val;
    }

    private static uint GetLayerWins(LayerEnum layer)
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

            if (layer is not Role role)
                continue;

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

    private static void DeserializeCustomStats(this BinaryReader reader)
    {
        MigratedFromVanillaStats = reader.ReadBoolean();
        reader.ReadEnumDict(CustomStats);
        reader.ReadEnumDict(LayerWins);
        reader.ReadEnumDict(MapWins);

        var diff = TranslationManager.LastID - TranslationManager.PreviousLastID; // Accounting for any changes to the translations in the base game

        if (diff == 0)
            return;

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

    private static void ReadEnumDict<T>(this BinaryReader reader, Dictionary<T, uint> dict) where T : struct, Enum
    {
        var num = reader.ReadUInt32();

        while (num-- > 0)
            dict[reader.Read<T>()] = reader.ReadUInt32();
    }

    private static T Read<T>(this BinaryReader reader) where T : struct, Enum
    {
        if (typeof(T).GetEnumUnderlyingType() == typeof(byte))
            return (T)(object)reader.ReadByte();

        return (T)(object)reader.ReadInt32();
    }

    private static void Write(this BinaryWriter writer, Enum enumVal)
    {
        var enumType = enumVal.GetType();

        if (enumType.GetEnumUnderlyingType() == typeof(byte))
            writer.Write(Convert.ToByte(enumVal));
        else
            writer.Write(Convert.ToInt32(enumVal));
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