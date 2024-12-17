namespace TownOfUsReworked.RoleGen2;

public class AllAnyFilter : BaseFilter
{
    public override void Filter(List<RoleOptionData> spawnList, int count)
    {
        if (count == 0)
        {
            spawnList.Clear();
            return;
        }

        var newList = new List<RoleOptionData>();
        spawnList.Shuffle();
        var pc = GameData.Instance.PlayerCount;

        while (newList.Count < count && spawnList.Any())
        {
            spawnList.Shuffle();
            var first = spawnList[0];
            newList.Add(first.Unique ? spawnList.TakeFirst() : first);
        }

        spawnList.Clear();
        spawnList.AddRange(newList);
        spawnList.Shuffle();
    }
}