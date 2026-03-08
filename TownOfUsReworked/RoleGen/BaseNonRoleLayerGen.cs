using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public abstract class BaseNonRoleLayerGen : BaseGen
{
    protected abstract Layer MinLayer { get; }
    protected abstract Layer MaxLayer { get; }
    protected abstract int MinSetting { get; }
    protected abstract int MaxSetting { get; }
    protected abstract PlayerLayerEnum LayerType { get; }
    protected abstract List<RoleOptionData> TargetList { get; }

    protected virtual string PluralName => LayerType + "s";

    protected abstract PlayerControl? GetAssignee(Layer id, List<PlayerControl> playerList);

    protected abstract bool HasLayer(PlayerControl player);

    protected virtual int GetPlayerCountCap() => GameData.Instance.PlayerCount;

    public override sealed void InitList()
    {
        if (IsList())
            InitRlList();
        else
            InitRegList();
    }

    private void InitRlList()
    {
        var elements = GetValuesFromTo(MinLayer, MaxLayer);

        foreach (var entry in Option.GetOptions<ListEntryOption>().Where(x => !x.IsBan && x.EntryType == LayerType && x.Num <= GameData.Instance.PlayerCount))
        {
            var rateLimit = 0;
            var cachedCount = TargetList.Count;
            var bucket = entry.Value.Select(x => x.TryCastToLayer(out var layer) ? layer : elements.Random());

            while (rateLimit < 10000 && TargetList.Count == cachedCount)
            {
                var layer2 = bucket.Random();

                if (ListGen.CannotAdd(layer2, TargetList))
                    rateLimit++;
                else
                    TargetList.Add(GetSpawnItem(layer2));
            }
        }
    }

    private void InitRegList()
    {
        foreach (var spawn in GetValuesFromToAndMorph(MinLayer, MaxLayer, GetSpawnItem))
        {
            if (spawn.IsActive())
                TargetList.AddMany(spawn.Clone, spawn.Count);
        }

        var players = GetPlayerCountCap();
        var maxElem = Mathf.Clamp(MaxSetting, 0, players);
        var minElem = Mathf.Clamp(MinSetting, 0, players);

        ModeFilters[GameModeSettings.GameMode].Filter(TargetList, GameModeSettings.IgnoreLayerCaps ? players : URandom.RandomRangeInt(minElem, maxElem + 1), true);
    }

    public override sealed void Assign()
    {
        var playerList = AllPlayers();
        playerList.Shuffle();
        TargetList.Shuffle();

        var invalid = new List<Layer>();

        if (TownOfUsReworked.MciActive && TargetList.Any())
            Message($"{PluralName} in the game: " + Join(" ", TargetList.Select(ab => ab.ID)));

        while (playerList.Any() && TargetList.Any())
        {
            var id = TargetList.TakeFirst().ID;
            var assigned = GetAssignee(id, playerList);

            if (!assigned)
                continue;

            playerList.Remove(assigned!);
            playerList.Shuffle();
            TargetList.Shuffle();

            if (!HasLayer(assigned!))
                Gen(assigned!, id, LayerType);
            else
                invalid.Add(id);
        }

        if (TownOfUsReworked.MciActive && invalid.Any())
            Message($"Invalid {PluralName} in the game: " + Join(" ", invalid));
    }

    public sealed override void Clear() => TargetList.Clear();
}