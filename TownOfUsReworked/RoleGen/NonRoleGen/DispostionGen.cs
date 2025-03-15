using Cpp2IL.Core.Extensions;
using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class DispositionGen : BaseGen
{
    private static readonly LayerEnum[] LoverRival = [ LayerEnum.Lovers, LayerEnum.Rivals ];
    private static readonly LayerEnum[] CrewDisp = [ LayerEnum.Corrupted, LayerEnum.Fanatic, LayerEnum.Traitor ];
    private static readonly LayerEnum[] NeutralDisp = [ LayerEnum.Taskmaster, LayerEnum.Overlord, LayerEnum.Linked ];

    public override void Clear() => AllDispositions.Clear();

    public override void InitList()
    {
        if (IsRoleList())
            InitRlList();
        else
            InitRegList();
    }

    private static void InitRlList()
    {
        var abilities = GetValuesFromTo(LayerEnum.Allied, LayerEnum.Traitor);

        foreach (var entry in OptionAttribute.GetOptions<ListEntryAttribute>().Where(x => !x.IsBan && x.EntryType == PlayerLayerEnum.Disposition))
        {
            foreach (var id in entry.Value)
            {
                if (id == RoleListSlot.None)
                    break;

                var rateLimit = 0;
                var cachedCount = AllDispositions.Count;

                while (rateLimit < 10000 && AllDispositions.Count == cachedCount)
                {
                    if (!id.TryCastToLayer(out var layer))
                        layer = abilities.Random();

                    if (RoleListGen.CannotAdd(layer, AllDispositions))
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

        int maxDisp = DispositionsSettings.MaxDispositions;
        int minDisp = DispositionsSettings.MinDispositions;
        var playerCount = AllPlayers().Count(x => x != Pure);

        while (maxDisp > playerCount)
            maxDisp--;

        while (minDisp > playerCount)
            minDisp--;

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
            var id = AllDispositions.RemoveAndReturn(0).ID;
            var assigned = id switch
            {
                LayerEnum.Mafia when playerList.Count > 1 => playerList.FirstOrDefault(),
                LayerEnum.Defector => playerList.FirstOrDefault(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate)),
                LayerEnum.Allied => playerList.FirstOrDefault(x => x.Is(Alignment.Killing) && x.Is(Faction.Neutral)),
                _ when LoverRival.Contains(id) && playerList.Count > 1 => playerList.FirstOrDefault(x => x.GetRole() is not (Altruist or Troll or Actor or Jester or Shifter)),
                _ when CrewDisp.Contains(id) => playerList.FirstOrDefault(x => x.Is(Faction.Crew)),
                _ when NeutralDisp.Contains(id) => playerList.FirstOrDefault(x => x.Is(Faction.Neutral)),
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