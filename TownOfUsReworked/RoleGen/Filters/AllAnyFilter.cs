namespace TownOfUsReworked.RoleGen;

public sealed class AllAnyFilter : BaseFilter
{
    public override void Filter(List<RoleOptionData> spawnList, int count, bool tryUsePlayerCount = false)
    {
        if (count == 0)
        {
            spawnList.Clear();
            return;
        }

        if (count != GameData.Instance.PlayerCount && tryUsePlayerCount)
            count = GameData.Instance.PlayerCount;

        var newList = new List<RoleOptionData>();
        spawnList.Shuffle();

        while (newList.Count < count && spawnList.Any())
        {
            var randomIndex = URandom.RandomRangeInt(0, spawnList.Count);
            var selected = spawnList[randomIndex];

            if (selected.Unique)
            {
                spawnList.RemoveAt(randomIndex);
                newList.Add(selected);
            }
            else
                newList.Add(selected.Clone());
        }

        spawnList.Clear();
        spawnList.AddRange(newList);
        spawnList.Shuffle();
    }
}