using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class HideAndSeekGen : BaseRoleGen
{
    public override bool AllowNonRoles => false;
    public override bool HasTargets => false;

    public override void InitList()
    {
        var safeHunterCount = Mathf.Clamp(Hunter.HunterCount, 1, GameData.Instance.PlayerCount - 1);

        while (AllRoles.Count < safeHunterCount)
            AllRoles.Add(GetSpawnItem(Layer.Hunter));

        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(GetSpawnItem(Layer.Hunted));
    }
}