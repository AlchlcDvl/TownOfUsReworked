using static TownOfUsReworked.RoleGen2.RoleGenManager;

namespace TownOfUsReworked.RoleGen2;

public class KillingOnlyGen : BaseRoleGen
{
    public override void InitIntList()
    {
        if (Intruders > 0)
        {
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Enforcer), GetSpawnItem(LayerEnum.Morphling), GetSpawnItem(LayerEnum.Blackmailer), GetSpawnItem(LayerEnum.Miner),
                GetSpawnItem(LayerEnum.Teleporter), GetSpawnItem(LayerEnum.Wraith), GetSpawnItem(LayerEnum.Consort), GetSpawnItem(LayerEnum.Janitor), GetSpawnItem(LayerEnum.Camouflager),
                GetSpawnItem(LayerEnum.Grenadier), GetSpawnItem(LayerEnum.Impostor), GetSpawnItem(LayerEnum.Consigliere), GetSpawnItem(LayerEnum.Disguiser), GetSpawnItem(LayerEnum.Ambusher));

            if (Intruders >= 3)
                IntruderRoles.Add(GetSpawnItem(LayerEnum.Godfather));
        }
    }

    public override void InitSynList()
    {
        if (Syndicate > 0)
        {
            SyndicateRoles.Add(GetSpawnItem(LayerEnum.Anarchist), GetSpawnItem(LayerEnum.Bomber), GetSpawnItem(LayerEnum.Poisoner), GetSpawnItem(LayerEnum.Crusader),
                GetSpawnItem(LayerEnum.Collider));

            if (Syndicate >= 3)
                SyndicateRoles.Add(GetSpawnItem(LayerEnum.Rebel));
        }
    }

    public override void InitNeutList()
    {
        if (Neutrals > 0)
        {
            NeutralRoles.Add(GetSpawnItem(LayerEnum.Glitch), GetSpawnItem(LayerEnum.Werewolf), GetSpawnItem(LayerEnum.SerialKiller), GetSpawnItem(LayerEnum.Juggernaut),
                GetSpawnItem(LayerEnum.Murderer), GetSpawnItem(LayerEnum.Thief));

            if (GameModeSettings.AddArsonist)
                NeutralRoles.Add(GetSpawnItem(LayerEnum.Arsonist));

            if (GameModeSettings.AddCryomaniac)
                NeutralRoles.Add(GetSpawnItem(LayerEnum.Cryomaniac));

            if (GameModeSettings.AddPlaguebearer)
                NeutralRoles.Add(GetSpawnItem(NeutralApocalypseSettings.DirectSpawn ? LayerEnum.Pestilence : LayerEnum.Plaguebearer));
        }
    }

    public override void InitCrewList()
    {
        if (Crew > 0)
        {
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
        }
    }

    public override void BeginFiltering()
    {
        var filer = ModeFilters[GameModeSettings.GameMode];
        filer.Filter(ref IntruderRoles, Intruders);
        filer.Filter(ref SyndicateRoles, Syndicate);
        filer.Filter(ref NeutralRoles, Neutrals);
        filer.Filter(ref CrewRoles, Crew);
        AllRoles.AddRanges(NeutralRoles, CrewRoles, SyndicateRoles);

        if (!SyndicateSettings.AltImps)
            AllRoles.AddRange(IntruderRoles);
    }

    public override void EndFiltering()
    {
        while (AllRoles.Count < AllPlayers().Count)
            AllRoles.Add(GetSpawnItem(LayerEnum.Vigilante));
    }

    public override void PostAssignment() => AssignChaosDrive();
}