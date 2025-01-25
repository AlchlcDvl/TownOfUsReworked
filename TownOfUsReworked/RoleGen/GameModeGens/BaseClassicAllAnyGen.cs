using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public abstract class BaseClassicAllAnyGen : BaseRoleGen
{
    public override void InitList() => GetAdjustedFactions();

    public override void InitCrewList()
    {
        if (Crew == 0)
            return;

        foreach (var layer in GetValuesFromTo(LayerEnum.Altruist, LayerEnum.Vigilante))
        {
            var spawn = GetSpawnItem(layer);

            if (layer == LayerEnum.Revealer)
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (Check(spawn.Chance))
                        SetPostmortals.Revealers++;
                }
            }
            else if (spawn.IsActive(Crew))
            {
                if (CSv.Contains(layer))
                    RoleGenManager.CrewSovereignRoles.AddMany(spawn.Clone, spawn.Count);
                else if (CI.Contains(layer))
                    RoleGenManager.CrewInvestigativeRoles.AddMany(spawn.Clone, spawn.Count);
                else if (CK.Contains(layer))
                    RoleGenManager.CrewKillingRoles.AddMany(spawn.Clone, spawn.Count);
                else if (CrP.Contains(layer))
                    RoleGenManager.CrewProtectiveRoles.AddMany(spawn.Clone, spawn.Count);
                else if (CS.Contains(layer))
                    RoleGenManager.CrewSupportRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer == LayerEnum.Crewmate)
                    CrewRoles.AddMany(spawn.Clone, spawn.Count);
            }
        }
    }

    public override void InitIntList()
    {
        if (Intruders == 0)
            return;

        foreach (var layer in GetValuesFromTo(LayerEnum.Ambusher, LayerEnum.Wraith, x => x is not (LayerEnum.PromotedGodfather or LayerEnum.Mafioso)))
        {
            var spawn = GetSpawnItem(layer);

            if (layer == LayerEnum.Ghoul)
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (Check(spawn.Chance))
                        SetPostmortals.Ghouls++;
                }
            }
            else if (spawn.IsActive(Intruders))
            {
                if (IH.Contains(layer))
                    RoleGenManager.IntruderHeadRoles.AddMany(spawn.Clone, spawn.Count);
                else if (IC.Contains(layer))
                    RoleGenManager.IntruderConcealingRoles.AddMany(spawn.Clone, spawn.Count);
                else if (ID.Contains(layer))
                    RoleGenManager.IntruderDeceptionRoles.AddMany(spawn.Clone, spawn.Count);
                else if (IK.Contains(layer))
                    RoleGenManager.IntruderKillingRoles.AddMany(spawn.Clone, spawn.Count);
                else if (IS.Contains(layer))
                    RoleGenManager.IntruderSupportRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer == LayerEnum.Impostor)
                    IntruderRoles.AddMany(spawn.Clone, spawn.Count);
            }
        }
    }

    public override void InitNeutList()
    {
        if (Neutrals == 0)
            return;

        foreach (var layer in GetValuesFromTo(LayerEnum.Actor, LayerEnum.Whisperer, x => x != LayerEnum.Betrayer && !(x == LayerEnum.Pestilence && !NeutralApocalypseSettings.DirectSpawn) &&
            !(x == LayerEnum.Plaguebearer && NeutralApocalypseSettings.DirectSpawn)))
        {
            var spawn = GetSpawnItem(layer);

            if (layer == LayerEnum.Phantom)
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (Check(spawn.Chance))
                        SetPostmortals.Phantoms++;
                }
            }
            else if (spawn.IsActive())
            {
                if (NH.Contains(layer) || NA.Contains(layer))
                    RoleGenManager.NeutralHarbingerRoles.AddMany(spawn.Clone, spawn.Count);
                else if (NB.Contains(layer))
                    RoleGenManager.NeutralBenignRoles.AddMany(spawn.Clone, spawn.Count);
                else if (NE.Contains(layer))
                    RoleGenManager.NeutralEvilRoles.AddMany(spawn.Clone, spawn.Count);
                else if (NK.Contains(layer))
                    RoleGenManager.NeutralKillingRoles.AddMany(spawn.Clone, spawn.Count);
                else if (NN.Contains(layer))
                    RoleGenManager.NeutralNeophyteRoles.AddMany(spawn.Clone, spawn.Count);
            }
        }
    }

    public override void InitSynList()
    {
        if (Syndicate == 0)
            return;

        foreach (var layer in GetValuesFromTo(LayerEnum.Anarchist, LayerEnum.Warper, x => x is not (LayerEnum.PromotedRebel or LayerEnum.Sidekick)))
        {
            var spawn = GetSpawnItem(layer);

            if (layer == LayerEnum.Banshee)
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (Check(spawn.Chance))
                        SetPostmortals.Banshees++;
                }
            }
            else if (spawn.IsActive(Syndicate))
            {
                if (SP.Contains(layer))
                    RoleGenManager.SyndicatePowerRoles.AddMany(spawn.Clone, spawn.Count);
                else if (SD.Contains(layer))
                    RoleGenManager.SyndicateDisruptionRoles.AddMany(spawn.Clone, spawn.Count);
                else if (SyK.Contains(layer))
                    RoleGenManager.SyndicateKillingRoles.AddMany(spawn.Clone, spawn.Count);
                else if (SSu.Contains(layer))
                    RoleGenManager.SyndicateSupportRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer == LayerEnum.Anarchist)
                    SyndicateRoles.AddMany(spawn.Clone, spawn.Count);
            }
        }
    }

    public override void Filter()
    {
        AllRoles.AddRanges(NeutralRoles, CrewRoles, SyndicateRoles);

        if (!SyndicateSettings.AltImps)
            AllRoles.AddRange(IntruderRoles);

        if (!AllRoles.Any(x => x.ID is LayerEnum.Dracula or LayerEnum.Jackal or LayerEnum.Necromancer or LayerEnum.Whisperer))
            AllRoles.AddMany(GetSpawnItem(LayerEnum.Seer).Clone, AllRoles.RemoveAll(x => x.ID == LayerEnum.Mystic));

        if (AllRoles.Any(x => x.ID == LayerEnum.Cannibal) && AllRoles.Any(x => x.ID == LayerEnum.Janitor) && GameModifiers.JaniCanMutuallyExclusive)
        {
            var chance = URandom.RandomRangeInt(0, 2);
            var value = chance == 0 ? NeutralRoles.Random(x => x.ID != LayerEnum.Cannibal, GetSpawnItem(LayerEnum.Amnesiac)) : IntruderRoles.Random(x => x.ID != LayerEnum.Janitor,
                GetSpawnItem(LayerEnum.Impostor));
            AllRoles.AddMany(value.Clone, AllRoles.RemoveAll(x => x.ID == (chance == 0 ? LayerEnum.Cannibal : LayerEnum.Janitor)));
        }

        if (GameData.Instance.PlayerCount <= 4 && AllRoles.Any(x => x.ID == LayerEnum.Amnesiac))
            AllRoles.AddMany(GetSpawnItem(LayerEnum.Thief).Clone, AllRoles.RemoveAll(x => x.ID == LayerEnum.Amnesiac));

        NeutralRoles.Clear();
        CrewRoles.Clear();
        SyndicateRoles.Clear();
        IntruderRoles.Clear();
    }

    public override void PostAssignment()
    {
        var allPlayers = AllPlayers();

        if (!allPlayers.Any(x => x.GetRole() is Amnesiac or Thief or Godfather or Shifter or Guesser or Rebel or Executioner or GuardianAngel or BountyHunter or Mystic or Actor ||
            x.GetDisposition() is Traitor or Fanatic))
        {
            allPlayers.Where(x => x.Is<Seer>()).ForEach(x => Gen(x, LayerEnum.Sheriff, PlayerLayerEnum.Role));
        }
    }
}