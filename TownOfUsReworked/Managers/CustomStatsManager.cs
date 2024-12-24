namespace TownOfUsReworked.Managers;

public static class CustomStatsManager
{
    public static bool MigratedFromVanillaStats;

    public static StringNames Buffer;

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

    public static float FastestGame;
    public static float SlowestGame;

    public static Dictionary<StringNames, string> CustomStatTranslations;
    public static Dictionary<StringNames, StringNames> VanillaToCustomStatsMap;
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

        // These are mapped to custom stats
        StringNames.StatsImpostorKills,
        StringNames.StatsGamesImpostor,
        StringNames.StatsGamesCrewmate,
    ];


    public static readonly Dictionary<MapNames, uint> MapWins = [];
    public static readonly Dictionary<LayerEnum, uint> LayerWins = [];
    public static readonly Dictionary<StringNames, uint> CustomStats = [];

    public static void Setup()
    {
        Buffer = TranslationManager.GetNextName();

        StatsGamesWon = TranslationManager.GetNextName();
        StatsGamesLost = TranslationManager.GetNextName();
        StatsGamesDrawn = TranslationManager.GetNextName();
        StatsRoleblocked = TranslationManager.GetNextName();
        StatsKilled = TranslationManager.GetNextName();
        StatsHitImmune = TranslationManager.GetNextName();
        StatsGamesCrew = TranslationManager.GetNextName();
        StatsGamesIntruder = TranslationManager.GetNextName();
        StatsGamesSyndicate = TranslationManager.GetNextName();
        StatsGamesNeutral = TranslationManager.GetNextName();

        CustomStatTranslations = new()
        {
            { StatsGamesWon, "Stats.GamesWon" },
            { StatsGamesLost, "Stats.GamesLost" },
            { StatsGamesDrawn, "Stats.GamesDrawn" },
            { StatsRoleblocked, "Stats.Roleblocked" },
            { StatsKilled, "Stats.Killed" },
            { StatsHitImmune, "Stats.HitImmune" },
            { StatsGamesCrew, "Stats.CrewGames" },
            { StatsGamesIntruder, "Stats.IntruderGames" },
            { StatsGamesSyndicate, "Stats.SyndicateGames" },
            { StatsGamesNeutral, "Stats.NeutralGames" }
        };

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
            StringNames.StatsTimesPettedPet
        ];

        VanillaToCustomStatsMap = new()
        {
            { StringNames.StatsImpostorKills, StatsKilled },
            { StringNames.StatsGamesImpostor, StatsGamesIntruder },
            { StringNames.StatsGamesCrewmate, StatsGamesCrew }
        };
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

        if (stats.stats.mapWins.TryGetValue((MapNames)6, out var value))
            MapWins[(MapNames)6] = value;

        if (stats.stats.mapWins.TryGetValue((MapNames)7, out value))
            MapWins[(MapNames)7] = value;

        foreach (var map in Enum.GetValues<MapNames>())
        {
            if (stats.stats.mapWins.TryGetValue(map, out value))
                MapWins[map] = value;
        }

        foreach (var stat in SupportVanillaStats)
        {
            if (stats.stats.gameplayStats.TryGetValue(stat, out value))
            {
                if (VanillaToCustomStatsMap.TryGetValue(stat, out var customStat))
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

    public static void IncrementStat(MapNames map)
    {
        if (!MapWins.ContainsKey(map))
            MapWins[map] = 0;

        MapWins[map]++;
        StatsManager.Instance.SaveStats();
    }

    public static void IncrementStat(StringNames stat)
    {
        if (!SupportVanillaStats.Contains(stat))
        {
            Message($"Tried to increment an unsupported stat: {stat}");
            return;
        }

        if (VanillaToCustomStatsMap.TryGetValue(stat, out var customStat))
            stat = customStat;

        if (!CustomStats.ContainsKey(stat))
            CustomStats[stat] = 0;

        CustomStats[stat]++;
        StatsManager.Instance.SaveStats();
    }

    public static uint GetStat(StringNames stat) => CustomStats.TryGetValue(stat, out var value) ? value : 0;

    public static uint GetMapWins(MapNames map) => MapWins.TryGetValue(map, out var value) ? value : 0;

    public static uint GetLayerWins(LayerEnum layer) => LayerWins.TryGetValue(layer, out var value) ? value : 0;

    public static void AddWin(byte map, IEnumerable<PlayerLayer> layers)
    {
        IncrementStat(StatsGamesWon);
        IncrementStat((MapNames)map);

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
        writer.Write(System.Convert.ToUInt32(Buffer)); // To use to compare with the next time we deserialize and the game added more translations since the last time the stats were saved
        WriteEnumDict(writer, MapWins);
        WriteEnumDict(writer, LayerWins);
        WriteEnumDict(writer, CustomStats);
    }

    public static void DeserializeCustomStats(BinaryReader reader)
    {
        MigratedFromVanillaStats = reader.ReadBoolean();
        Buffer = Enum.Parse<StringNames>($"{reader.ReadUInt32()}");
        ReadEnumDict(reader, MapWins);
        ReadEnumDict(reader, LayerWins);
        ReadEnumDict(reader, CustomStats);

        var diff = StatsGamesWon - Buffer - 1; // The extra 1 is to account for the buffer itself

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
        writer.Write((byte)dict.Count);

        foreach (var (key, value) in dict)
        {
            writer.Write(System.Convert.ToUInt32(key));
            writer.Write(value);
        }
    }

    public static void ReadEnumDict<T>(BinaryReader reader, Dictionary<T, uint> dict) where T : struct, Enum
    {
        var num = reader.ReadByte();

        while (num-- > 0)
            dict[Enum.Parse<T>($"{reader.ReadUInt32()}")] = reader.ReadUInt32();
    }
}