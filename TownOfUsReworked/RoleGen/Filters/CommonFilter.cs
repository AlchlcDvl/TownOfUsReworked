namespace TownOfUsReworked.RoleGen2;

public class CommonFilter : BaseFilter
{
    public override void Filter(List<RoleOptionData> spawnList, int count, bool tryUsePlayerCount = false)
    {
        if (count == 0)
        {
            spawnList.Clear();
            return;
        }

        if (spawnList.Count < count)
            count = spawnList.Count;

        var newList = new List<RoleOptionData>();
        var guaranteed = spawnList.Where(x => x.Chance == 100).ToList();
        guaranteed.Shuffle();
        var optionals = spawnList.Where(x => x.Chance != 100 && RoleGenManager.Check(x, true)).ToList();
        optionals.Shuffle();
        newList.AddRanges(guaranteed, optionals);

        if (newList.Count < count)
            newList.AddRange(spawnList.GetRandomRange(count - newList.Count, x => x.Chance < 100 && !newList.Contains(x)));

        spawnList.Clear();
        spawnList.AddRange(newList);
        spawnList.Shuffle();
    }
}