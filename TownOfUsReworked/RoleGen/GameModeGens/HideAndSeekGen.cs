using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class HideAndSeekGen : BaseRoleGen
{
    public override bool AllowNonRoles => false;
    public override bool HasTargets => false;

    public override void InitList()
    {
        while (AllRoles.Count < Hunter.HunterCount)
            AllRoles.Add(GetSpawnItem(Layer.Hunter));

        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(GetSpawnItem(Layer.Hunted));
    }
}