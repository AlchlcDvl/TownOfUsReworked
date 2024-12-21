namespace TownOfUsReworked.RoleGen;

public class CommonFilter : BaseFilter
{
    public override void Filter(List<RoleOptionData> spawnList, int count, bool tryUsePlayerCount = false)
    {
        if (count == 0 || spawnList.Count == 0)
        {
            spawnList.Clear();
            return;
        }

        if (spawnList.Count < count)
            count = spawnList.Count;

        var newList = new List<RoleOptionData>();
        newList.AddRange(spawnList.Where(x => x.Chance == 100));
        var optionals = spawnList.Where(x => x.Chance < 100).ToList();
        optionals.Shuffle();

        foreach (var spawn in optionals)
        {
            if (newList.Count >= count)
                break;

            if (RoleGenManager.Check(spawn))
                newList.Add(spawn);
        }

        spawnList.Clear();
        spawnList.AddRange(newList);
        spawnList.Shuffle();
    }
}