using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class DispositionGen : BaseGen
{
    private static readonly LayerEnum[] LoverRival = [ LayerEnum.Lovers, LayerEnum.Rivals ];
    private static readonly LayerEnum[] CrewDisp = [ LayerEnum.Corrupted, LayerEnum.Fanatic, LayerEnum.Traitor ];
    private static readonly LayerEnum[] OutcastDisp = [ LayerEnum.Taskmaster, LayerEnum.Overlord, LayerEnum.Linked ];

    public override void InitList()
    {
        if (IsList())
            InitRlList();
        else
            InitRegList();
    }

    private static void InitRlList()
    {
        var dispositions = GetValuesFromTo(LayerEnum.Allied, LayerEnum.Traitor);

        foreach (var entry in Option.GetOptions<ListEntryOption>().Where(x => !x.IsBan && x.EntryType == PlayerLayerEnum.Disposition && x.Num <= GameData.Instance.PlayerCount))
        {
            foreach (var id in entry.Value)
            {
                if (id == ListSlot.None)
                    break;

                var rateLimit = 0;
                var cachedCount = AllDispositions.Count;

                if (!id.TryCastToLayer(out var layer))
                    layer = dispositions.Random();

                while (rateLimit < 10000 && AllDispositions.Count == cachedCount)
                {
                    if (ListGen.CannotAdd(layer, AllDispositions))
                        rateLimit++;
                    else
                        AllDispositions.Add(GetSpawnItem(layer));
                }
            }
        }
    }

    private static void InitRegList()
    {
        foreach (var spawn in GetValuesFromToAndMorph(LayerEnum.Allied, LayerEnum.Traitor, GetSpawnItem))
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
        var invalid = new List<LayerEnum>();

        if (TownOfUsReworked.MciActive && AllDispositions.Any())
            Message("Dispositions in the game: " + Join(" ", AllDispositions.Select(ab => ab.ID)));

        while (playerList.Any() && AllDispositions.Any())
        {
            var id = AllDispositions.TakeFirst().ID;
            var assigned = id switch
            {
                LayerEnum.Mafia when playerList.Count > 1 => playerList.FirstOrDefault(),
                LayerEnum.Defector => playerList.FirstOrDefault(x => x.GetFaction() is not (Faction.Crew or Faction.Outcast)),
                LayerEnum.Allied => playerList.FirstOrDefault(x => x.Is(Alignment.Killing) && x.Is(Faction.Outcast)),
                _ when LoverRival.Contains(id) && playerList.Count > 1 => playerList.FirstOrDefault(x => x.GetRole() is not (Altruist or Troll or Actor or Jester or Shifter)),
                _ when CrewDisp.Contains(id) => playerList.FirstOrDefault(x => x.Is(Faction.Crew)),
                _ when OutcastDisp.Contains(id) => playerList.FirstOrDefault(x => x.Is(Faction.Outcast)),
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