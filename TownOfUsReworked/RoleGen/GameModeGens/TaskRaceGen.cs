using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class TaskRaceGen : BaseRoleGen
{
    public override bool AllowNonRoles => false;
    public override bool HasTargets => false;

    public override void InitList()
    {
        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(GetSpawnItem(Layer.Runner));
    }
}