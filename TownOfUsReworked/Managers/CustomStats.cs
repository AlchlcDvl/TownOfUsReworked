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
        StringNames.StatsTimesPettedPet,

        // These are mapped to custom stats
        StringNames.StatsImpostorKills
    ];

    private static readonly Dictionary<MapEnum, uint> MapWins = [];
    private static readonly Dictionary<Faction, uint> FactionWins = [];
    private static readonly Dictionary<LayerEnum, uint> LayerWins = [];
    private static readonly Dictionary<StringNames, uint> CustomStats = [];

    private static readonly ValueMap<StringNames, MapEnum> MapMap = [];
    private static readonly ValueMap<StringNames, StatID> StatsMap = [];
    private static readonly ValueMap<StringNames, Faction> FactionMap = [];
    private static readonly ValueMap<StringNames, LayerEnum> LayerMap = [];

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

        Enum.GetValues<MapEnum>().Do(x => GetMapWins(x));
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
            .. MapMap.Keys,
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

    private static void IncrementStat(LayerEnum layer)
    {
        if (LayerDictionary.ContainsKey(layer))
            LayerWins[layer] = LayerWins.GetValueOrDefault(layer) + 1;
    }

    private static void IncrementStat(MapEnum map) => MapWins[map] = MapWins.GetValueOrDefault(map) + 1;

    public static void IncrementStat(Faction faction)
    {
        if (FactionDictionary.ContainsKey(faction))
            FactionWins[faction] = FactionWins.GetValueOrDefault(faction) + 1;
    }

    public static void IncrementStat(StatID stat) => IncrementStat(StatsMap[stat]);

    public static void IncrementStat(StringNames stat)
    {
        var success = !(IsFreePlay() || TownOfUsReworked.MciActive) && (SupportVanillaStats.Contains(stat) || stat > 0);

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
            CallTargetedRpc(player.OwnerId, CustomRPC.Misc, MiscRPC.Stat, stat);
    }

    public static IObject GetStat(StringNames stat)
    {
        if (!SupportVanillaStats.Contains(stat) && stat >= 0)
            return null;

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
        IncrementStat((MapEnum)map);

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
            dict[reader.Read<T>()] = reader.ReadUInt32();
    }

    private static T Read<T>(this BinaryReader reader) where T : struct, Enum
    {
        if (typeof(T).GetEnumUnderlyingType() == typeof(byte))
            return (T)(object)reader.ReadByte();

        return (T)(object)reader.ReadInt32();
    }

    private static void Write(this BinaryWriter writer, Enum enumVal) => writer.Write([.. RpcWriter.GetBytes(enumVal, false)]);

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