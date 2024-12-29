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
        StringNames.StatsImpostorKills,
    ];

    public static readonly Dictionary<MapEnum, uint> MapWins = [];
    public static readonly Dictionary<LayerEnum, uint> LayerWins = [];
    public static readonly Dictionary<StringNames, uint> CustomStats = [];

    private static readonly Dictionary<StringNames, MapEnum> MapMap = [];
    private static readonly Dictionary<StringNames, LayerEnum> LayerMap = [];

    public static void Setup()
    {
        StatsGamesWon = TranslationManager.GetNextName("Stats.GamesWon");
        StatsGamesLost = TranslationManager.GetNextName("Stats.GamesLost");
        StatsGamesDrawn = TranslationManager.GetNextName("Stats.GamesDrawn");
        StatsRoleblocked = TranslationManager.GetNextName("Stats.Roleblocked");
        StatsKilled = TranslationManager.GetNextName("Stats.Killed", StringNames.StatsImpostorKills);
        StatsHitImmune = TranslationManager.GetNextName("Stats.HitImmune");
        StatsGamesCrew = TranslationManager.GetNextName("Stats.CrewGames");
        StatsGamesIntruder = TranslationManager.GetNextName("Stats.IntruderGames");
        StatsGamesSyndicate = TranslationManager.GetNextName("Stats.SyndicateGames");
        StatsGamesNeutral = TranslationManager.GetNextName("Stats.NeutralGames");

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

        var path = Path.Combine(PlatformPaths.persistentDataPath, "reworkedStats");

        if (File.Exists(path))
        {
            try
            {
                using var reader = new BinaryReader(File.OpenRead(path));
                DeserializeCustomStats(reader);
            } catch {}
        }

        StatsManager.Instance.LoadStats(); // Forcing stat loading to ensure proper stats are loaded

        // Preloading missing stats
        Enum.GetValues<MapEnum>().ForEach(x => GetMapWins(x));
        LayerDictionary.Keys.ForEach(x => GetLayerWins(x));
        Enum.GetValues<StringNames>().ForEach(x => GetStat(x));

        foreach (var map in MapWins.Keys)
        {
            var val = TranslationManager.GetNextName($"MapWins.{map}");
            OrderedStats.Add(val);
            MapMap[val] = map;
        }

        foreach (var layer in LayerWins.Keys)
        {
            var val = TranslationManager.GetNextName($"LayerWins.{layer}");
            OrderedStats.Add(val);
            LayerMap[val] = layer;
        }

        StatsManager.Instance.SaveStats();
    }

    public static void Reset()
    {
        MapWins.Clear();
        LayerWins.Clear();
        CustomStats.Clear();
    }

    public static void MigrateFromVanillaStats(StatsManager stats)
    {
        if (MigratedFromVanillaStats)
            return;

        foreach (var map in Enum.GetValues<MapEnum>())
        {
            if (stats.stats.mapWins.TryGetValue((MapNames)(byte)map, out var value))
                MapWins[map] = value;
        }

        foreach (var stat in SupportVanillaStats)
        {
            if (stats.stats.gameplayStats.TryGetValue(stat, out var value))
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
        success = SupportVanillaStats.Contains(stat) || stat > ReworkedStart;

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

        if (!CustomStats.ContainsKey(stat))
            CustomStats[stat] = 0;

        return CustomStats[stat];
    }

    public static uint GetMapWins(MapEnum map)
    {
        if (map == MapEnum.Random)
            return 0;

        if (!MapWins.ContainsKey(map))
            MapWins[map] = 0;

        return MapWins[map];
    }

    public static uint GetLayerWins(LayerEnum layer)
    {
        if (!LayerDictionary.ContainsKey(layer))
            return 0;

        if (!LayerWins.ContainsKey(layer))
            LayerWins[layer] = 0;

        return LayerWins[layer];
    }

    public static void AddWin(byte map, IEnumerable<PlayerLayer> layers)
    {
        IncrementStat(StatsGamesWon);
        IncrementStat((MapEnum)map);

        foreach (var layer in layers)
        {
            IncrementStat(layer.Type);

            if (layer is Role role)
            {
                IncrementStat(role.SubFaction switch
                {
                    SubFaction.Undead => LayerEnum.Undead,
                    SubFaction.Cabal => LayerEnum.Cabal,
                    SubFaction.Sect => LayerEnum.Sect,
                    SubFaction.Reanimated => LayerEnum.Reanimated,
                    _ => LayerEnum.None
                });
            }
        }

        IncrementStat(StringNames.StatsGamesFinished);
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

    public static void SerializeCustomStats(BinaryWriter writer)
    {
        writer.Write(MigratedFromVanillaStats);
        writer.Write(System.Convert.ToUInt32(ReworkedStart)); // Used to mark the start of custom translations, this value changes as more translations are added in the base game
        writer.Write(TranslationManager.LastID);
        WriteEnumDict(writer, MapWins);
        WriteEnumDict(writer, LayerWins);
        WriteEnumDict(writer, CustomStats);
    }

    public static void DeserializeCustomStats(BinaryReader reader)
    {
        MigratedFromVanillaStats = reader.ReadBoolean();
        ReworkedStart = (StringNames)reader.ReadUInt32();
        TranslationManager.PreviousLastID = reader.ReadInt32();
        ReadEnumDict(reader, MapWins);
        ReadEnumDict(reader, LayerWins);
        ReadEnumDict(reader, CustomStats);

        var diff = TranslationManager.LastID - TranslationManager.PreviousLastID;

        if (diff > 0)
        {
            var keys = CustomStats.Keys.ToList();
            var values = CustomStats.Values.ToList();
            var count = CustomStats.Count;

            CustomStats.Clear();

            // Remapping stats to ids that have been pushed further down the enum
            for (var i = 0; i < count; i++)
                CustomStats[keys[i] + diff] = values[i];
        }
    }

    public static void WriteEnumDict<T>(BinaryWriter writer, Dictionary<T, uint> dict) where T : struct, Enum
    {
        writer.Write((uint)dict.Count);

        foreach (var (key, value) in dict)
        {
            writer.Write(System.Convert.ToUInt32(key));
            writer.Write(value);
        }
    }

    public static void ReadEnumDict<T>(BinaryReader reader, Dictionary<T, uint> dict) where T : struct, Enum
    {
        var num = reader.ReadUInt32();

        while (num-- > 0)
            dict[(T)(object)reader.ReadUInt32()] = reader.ReadUInt32();
    }
}