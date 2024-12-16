namespace TownOfUsReworked.RoleGen2;

public class AllAnyFilter : BaseFilter
{
    public override void Filter(ref List<RoleOptionData> spawnList, int count)
    {
        var newList = new List<RoleOptionData>();
        spawnList.Shuffle();

        if (count != AllPlayers().Count)
            count = AllPlayers().Count;

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