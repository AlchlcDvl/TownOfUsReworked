using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class VanillaGen : BaseRoleGen
{
    public override bool AllowNonRoles => false;
    public override bool HasTargets => false;

    public override void InitList()
    {
        while (AllRoles.Count < BadGuysSettings.BadGuyCount)
            AllRoles.Add(GetSpawnItem(GetRandomBaseEvil(BadGuysSettings.MainBadGuys)));

        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(GetSpawnItem(LayerEnum.Crewmate));
    }
}