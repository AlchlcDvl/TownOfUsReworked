using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen2;

public class KillingOnlyGen : BaseRoleGen
{
    public override void InitList() => GetAdjustedFactions();

    public override void InitIntList()
    {
        if (Intruders == 0 || SyndicateSettings.AltImps)
            return;

        IntruderRoles.Add(GetSpawnItem(LayerEnum.Enforcer), GetSpawnItem(LayerEnum.Morphling), GetSpawnItem(LayerEnum.Blackmailer), GetSpawnItem(LayerEnum.Miner),
            GetSpawnItem(LayerEnum.Teleporter), GetSpawnItem(LayerEnum.Wraith), GetSpawnItem(LayerEnum.Consort), GetSpawnItem(LayerEnum.Janitor), GetSpawnItem(LayerEnum.Camouflager),
            GetSpawnItem(LayerEnum.Grenadier), GetSpawnItem(LayerEnum.Impostor), GetSpawnItem(LayerEnum.Consigliere), GetSpawnItem(LayerEnum.Disguiser), GetSpawnItem(LayerEnum.Ambusher));

        if (Intruders >= 3)
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Godfather));

        ModeFilters[GameModeSettings.GameMode].Filter(IntruderRoles, Intruders);
    }

    public override void InitSynList()
    {
        if (Syndicate == 0)
            return;

        SyndicateRoles.Add(GetSpawnItem(LayerEnum.Anarchist), GetSpawnItem(LayerEnum.Bomber), GetSpawnItem(LayerEnum.Poisoner), GetSpawnItem(LayerEnum.Crusader),
            GetSpawnItem(LayerEnum.Collider));

        if (Syndicate >= 3)
            SyndicateRoles.Add(GetSpawnItem(LayerEnum.Rebel));

        ModeFilters[GameModeSettings.GameMode].Filter(SyndicateRoles, Syndicate);
    }

    public override void InitNeutList()
    {
        if (Neutrals == 0)
            return;

        NeutralRoles.Add(GetSpawnItem(LayerEnum.Glitch), GetSpawnItem(LayerEnum.Werewolf), GetSpawnItem(LayerEnum.SerialKiller), GetSpawnItem(LayerEnum.Juggernaut),
            GetSpawnItem(LayerEnum.Murderer), GetSpawnItem(LayerEnum.Thief));

        if (GameModeSettings.AddArsonist)
            NeutralRoles.Add(GetSpawnItem(LayerEnum.Arsonist));

        if (GameModeSettings.AddCryomaniac)
            NeutralRoles.Add(GetSpawnItem(LayerEnum.Cryomaniac));

        if (GameModeSettings.AddPlaguebearer)
            NeutralRoles.Add(GetSpawnItem(NeutralApocalypseSettings.DirectSpawn ? LayerEnum.Pestilence : LayerEnum.Plaguebearer));

        ModeFilters[GameModeSettings.GameMode].Filter(NeutralRoles, Neutrals);
    }

    public override void InitCrewList()
    {
        if (Crew == 0)
            return;

        var vigis = Crew / 3;
        var vets = Crew / 3;
        var basts = Crew / 3;

        while (vigis > 0 || vets > 0 || basts > 0)
        {
            if (vigis > 0)
            {
                CrewRoles.Add(GetSpawnItem(LayerEnum.Vigilante));
                vigis--;
            }

            if (vets > 0)
            {
                CrewRoles.Add(GetSpawnItem(LayerEnum.Veteran));
                vets--;
            }

            if (basts > 0)
            {
                CrewRoles.Add(GetSpawnItem(LayerEnum.Bastion));
                basts--;
            }
        }

        ModeFilters[GameModeSettings.GameMode].Filter(CrewRoles, Crew);
    }

    public override void Filter()
    {
        AllRoles.AddRanges(NeutralRoles, CrewRoles, SyndicateRoles);

        if (!SyndicateSettings.AltImps)
            AllRoles.AddRange(IntruderRoles);

        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(GetSpawnItem(LayerEnum.Vigilante));
    }

    public override void PostAssignment() => AssignChaosDrive();
}