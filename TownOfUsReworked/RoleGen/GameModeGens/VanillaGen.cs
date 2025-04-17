using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class VanillaGen : BaseRoleGen
{
    public override bool AllowNonRoles => false;
    public override bool HasTargets => false;

    public override void InitList()
    {
        var mainBG = BadGuysSettings.MainBadGuys switch
        {
            Faction.Intruder => LayerEnum.Impostor,
            Faction.Syndicate => LayerEnum.Anarchist,
            _ => LayerEnum.Cultist,
        };

        if (BadGuysSettings.MainBadGuys is Faction.Pandorica or Faction.Illuminati or Faction.Compliance)
        {
            mainBG = URandom.RandomRangeInt(0, 4) switch
            {
                0 => LayerEnum.Impostor,
                1 => LayerEnum.Anarchist,
                2 => LayerEnum.Murderer,
                _ => LayerEnum.Cultist
            };
        }

        while (AllRoles.Count < BadGuysSettings.BadGuyCount)
            AllRoles.Add(GetSpawnItem(mainBG));

        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(GetSpawnItem(LayerEnum.Crewmate));
    }
}