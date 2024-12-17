using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen2;

public class VanillaGen : BaseRoleGen
{
    public override bool AllowNonRoles => false;
    public override bool HasTargets => false;

    public override void InitList()
    {
        while (AllRoles.Count < (SyndicateSettings.AltImps ? SyndicateSettings.SyndicateCount : IntruderSettings.IntruderCount))
            AllRoles.Add(GetSpawnItem(SyndicateSettings.AltImps ? LayerEnum.Anarchist : LayerEnum.Impostor));

        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(GetSpawnItem(LayerEnum.Crewmate));
    }
}