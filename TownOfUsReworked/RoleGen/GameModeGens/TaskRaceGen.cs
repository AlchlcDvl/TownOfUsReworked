using static TownOfUsReworked.RoleGen2.RoleGenManager;

namespace TownOfUsReworked.RoleGen2;

public class TaskRaceGen : BaseRoleGen
{
    public override bool AllowNonRoles => false;
    public override bool HasTargets => false;

    public override void InitList()
    {
        while (AllRoles.Count < AllPlayers().Count)
            AllRoles.Add(GetSpawnItem(LayerEnum.Runner));
    }
}