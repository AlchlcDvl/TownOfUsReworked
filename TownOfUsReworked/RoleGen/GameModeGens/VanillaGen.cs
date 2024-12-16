using static TownOfUsReworked.RoleGen2.RoleGenManager;

namespace TownOfUsReworked.RoleGen2;

public class VanillaGen : BaseRoleGen
{
    public override bool AllowNonRoles => false;
    public override bool HasTargets => false;

    public override void InitList()
    {
        while (AllRoles.Count < (SyndicateSettings.AltImps ? SyndicateSettings.SyndicateCount : IntruderSettings.IntruderCount))
            AllRoles.Add(GetSpawnItem(SyndicateSettings.AltImps ? LayerEnum.Anarchist : LayerEnum.Impostor));

        while (AllRoles.Count < AllPlayers().Count)
            AllRoles.Add(GetSpawnItem(LayerEnum.Crewmate));
    }
}