namespace TownOfUsReworked.RoleGen;

public sealed class CommonFilter : BaseFilter
{
    public override void Filter(List<RoleOptionData> spawnList, int count, bool tryUsePlayerCount = false)
    {
        if (count <= 0 || spawnList.Count == 0)
        {
            spawnList.Clear();
            return;
        }

        if (spawnList.Count < count)
            count = spawnList.Count;

        var filtered = spawnList.Where(x => RoleGenManager.Check(x.Chance)).OrderByDescending(x => x.Chance);
        var newList = new List<RoleOptionData>();

        var guaranteed = filtered.Where(x => x.Chance == 100).ToList();
        guaranteed.Shuffle();
        newList.AddRange(guaranteed);

        var optionals = filtered.Where(x => x.Chance < 100).ToList();
        optionals.Shuffle();
        newList.AddRange(optionals);

        spawnList.Clear();
        spawnList.AddRange(newList.GetRange(0, count));
        spawnList.Shuffle();
    }
}