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

        var guaranteed = new List<RoleOptionData>();
        var optionals = new List<RoleOptionData>();

        foreach (var role in spawnList)
        {
            if (RoleGenManager.Check(role.Chance))
            {
                if (role.Chance == 100)
                    guaranteed.Add(role);
                else
                    optionals.Add(role);
            }
        }

        guaranteed.Shuffle();
        optionals.Shuffle();

        var newList = new List<RoleOptionData>(guaranteed.Count + optionals.Count);
        newList.AddRange(guaranteed);
        newList.AddRange(optionals);

        var finalCount = Math.Min(count, newList.Count);

        spawnList.Clear();
        spawnList.AddRange(newList.GetRange(0, finalCount));
        spawnList.Shuffle();
    }
}