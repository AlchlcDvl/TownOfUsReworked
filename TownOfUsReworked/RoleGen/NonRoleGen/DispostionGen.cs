using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class DispositionGen : BaseGen
{
    private static readonly Layer[] LoverRival = [ Layer.Lovers, Layer.Rivals ];
    private static readonly Layer[] CrewDisp = [ Layer.Corrupted, Layer.Fanatic, Layer.Traitor ];
    private static readonly Layer[] OutcastDisp = [ Layer.Taskmaster, Layer.Overlord, Layer.Linked ];

    public override void InitList()
    {
        if (IsList())
            InitRlList();
        else
            InitRegList();
    }

    private static void InitRlList()
    {
        var dispositions = GetValuesFromTo(Layer.Allied, Layer.Traitor);

        foreach (var entry in Option.GetOptions<ListEntryOption>().Where(x => !x.IsBan && x.EntryType == PlayerLayerEnum.Disposition && x.Num <= GameData.Instance.PlayerCount))
        {
            var rateLimit = 0;
            var cachedCount = AllDispositions.Count;
            var bucket = entry.Value.Select(x => x.TryCastToLayer(out var layer) ? layer : dispositions.Random());

            while (rateLimit < 10000 && AllDispositions.Count == cachedCount)
            {
                var layer2 = bucket.Random();

                if (ListGen.CannotAdd(layer2, AllDispositions))
                    rateLimit++;
                else
                    AllDispositions.Add(GetSpawnItem(layer2));
            }
        }
    }

    private static void InitRegList()
    {
        foreach (var spawn in GetValuesFromToAndMorph(Layer.Allied, Layer.Traitor, GetSpawnItem))
        {
            if (spawn.IsActive())
                AllDispositions.AddMany(spawn.Clone, spawn.Count);
        }

        var playerCount = AllPlayers().Count(x => x != Pure);
        var maxDisp = Mathf.Clamp(DispositionsSettings.MaxDispositions.Value, 0, playerCount);
        var minDisp = Mathf.Clamp(DispositionsSettings.MinDispositions.Value, 0, playerCount);
        ModeFilters[GameModeSettings.GameMode].Filter(AllDispositions, GameModeSettings.IgnoreLayerCaps ? playerCount : URandom.RandomRangeInt(minDisp, maxDisp + 1), true);
    }

    public override void Assign()
    {
        var playerList = AllPlayers().ToList();
        playerList.Shuffle();
        AllDispositions.Shuffle();
        var invalid = new List<Layer>();

        if (TownOfUsReworked.MciActive && AllDispositions.Any())
            Message("Dispositions in the game: " + Join(" ", AllDispositions.Select(ab => ab.ID)));

        while (playerList.Any() && AllDispositions.Any())
        {
            var id = AllDispositions.TakeFirst().ID;
            var assigned = id switch
            {
                Layer.Mafia when playerList.Count > 1 => playerList.FirstOrDefault(),
                Layer.Defector => playerList.FirstOrDefault(x => x.GetFaction().IsFactionedEvil()),
                Layer.Allied => playerList.FirstOrDefault(x => x.Is(Alignment.Killing) && x.GetFaction().IsOutcast()),
                _ when LoverRival.Contains(id) && playerList.Count > 1 => playerList.FirstOrDefault(x => x.GetRole() is not (Troll or Actor or Jester)),
                _ when CrewDisp.Contains(id) => playerList.FirstOrDefault(x => x.Is(Faction.Crew)),
                _ when OutcastDisp.Contains(id) => playerList.FirstOrDefault(x => x.GetFaction().IsOutcast()),
                _ => null
            };

            if (!assigned)
                continue;

            playerList.Remove(assigned);
            playerList.Shuffle();
            AllDispositions.Shuffle();

            if (!assigned.GetDisposition())
                Gen(assigned, id, PlayerLayerEnum.Disposition);
            else
                invalid.Add(id);
        }

        if (TownOfUsReworked.MciActive && invalid.Any())
            Message("Invalid Dispositions in the game: " + Join(" ", invalid));

        AllDispositions.Clear();
    }
}