using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class VanillaGen : BaseRoleGen
{
    public override bool AllowNonRoles => false;
    public override bool HasTargets => false;

    public override void InitList()
    {
        var baseBad = GetSpawnItem(GetRandomBaseEvil(BadGuysSettings.MainBadGuys));
        var safeBadGuyCount = Mathf.Clamp(BadGuysSettings.BadGuyCount, 1, GameData.Instance.PlayerCount - 1);

        while (AllRoles.Count < safeBadGuyCount)
            AllRoles.Add(baseBad);

        var crew = GetSpawnItem(Layer.Crewmate);

        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(crew);
    }
}