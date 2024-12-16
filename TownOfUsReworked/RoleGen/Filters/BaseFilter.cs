namespace TownOfUsReworked.RoleGen2;

public abstract class BaseFilter
{
    public abstract void Filter(ref List<RoleOptionData> spawnList, int count);
}