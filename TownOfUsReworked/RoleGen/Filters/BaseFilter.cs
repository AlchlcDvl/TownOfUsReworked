namespace TownOfUsReworked.RoleGen;

public abstract class BaseFilter
{
    public abstract void Filter(List<RoleOptionData> spawnList, int count, bool tryUsePlayerCount = false);
}