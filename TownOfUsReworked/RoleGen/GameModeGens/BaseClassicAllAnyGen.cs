using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public abstract class BaseClassicAllAnyGen : BaseRoleGen
{
    public override void InitList() => GetAdjustedFactions();

    public override void InitCrewList()
    {
        if (Crew == 0)
            return;

        foreach (var layer in GetValuesFromTo(Layer.Altruist, Layer.Vigilante))
        {
            var spawn = GetSpawnItem(layer);

            if (layer == Layer.Revealer)
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
                else if (layer == Layer.Crewmate)
                    CrewRoles.AddMany(spawn.Clone, spawn.Count);
            }
        }
    }

    public override void InitIntList()
    {
        if (Intruders == 0)
            return;

        foreach (var layer in GetValuesFromTo(Layer.Ambusher, Layer.Wraith, x => x is not Layer.Mafioso))
        {
            var spawn = GetSpawnItem(layer);

            if (layer == Layer.Ghoul)
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
                else if (layer == Layer.Impostor)
                    IntruderRoles.AddMany(spawn.Clone, spawn.Count);
            }
        }
    }

    public override void InitNeutList()
    {
        if (Outcasts == 0)
            return;

        foreach (var layer in GetValuesFromTo(Layer.Actor, Layer.Whisperer, x => x != Layer.Betrayer))
        {
            var spawn = GetSpawnItem(layer);

            if (layer == Layer.Phantom)
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (Check(spawn.Chance))
                        SetPostmortals.Phantoms++;
                }
            }
            else if (spawn.IsActive())
            {
                if (NB.Contains(layer))
                    RoleGenManager.OutcastBenignRoles.AddMany(spawn.Clone, spawn.Count);
                else if (NE.Contains(layer))
                    RoleGenManager.OutcastEvilRoles.AddMany(spawn.Clone, spawn.Count);
                else if (NK.Contains(layer))
                    RoleGenManager.OutcastKillingRoles.AddMany(spawn.Clone, spawn.Count);
                else if (NN.Contains(layer))
                    RoleGenManager.OutcastNeophyteRoles.AddMany(spawn.Clone, spawn.Count);
            }
        }
    }

    public override void InitApocList()
    {
        if (Apocalypse == 0)
            return;

        // TODO: Implement a ghost role for apoc
        foreach (var layer in GetValuesFromTo(Layer.Cannibal, Layer.Void))
        {
            var spawn = GetSpawnItem(layer);

            /*if (layer == LayerEnum.Phantom)
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (Check(spawn.Chance))
                        SetPostmortals.Phantoms++;
                }
            }
            else */if (spawn.IsActive())
            {
                if (AH.Contains(layer) || AD.Contains(layer))
                    RoleGenManager.ApocalypseHarbingerRoles.AddMany(spawn.Clone, spawn.Count);
            }
        }
    }

    public override void InitSynList()
    {
        if (Syndicate == 0)
            return;

        foreach (var layer in GetValuesFromTo(Layer.Anarchist, Layer.Warper, x => x is not Layer.Sidekick))
        {
            var spawn = GetSpawnItem(layer);

            if (layer == Layer.Banshee)
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (Check(spawn.Chance))
                        SetPostmortals.Banshees++;
                }
            }
            else if (spawn.IsActive(Syndicate))
            {
                if (SH.Contains(layer))
                    RoleGenManager.SyndicateHeadRoles.AddMany(spawn.Clone, spawn.Count);
                else if (SD.Contains(layer))
                    RoleGenManager.SyndicateDisruptionRoles.AddMany(spawn.Clone, spawn.Count);
                else if (SyK.Contains(layer))
                    RoleGenManager.SyndicateKillingRoles.AddMany(spawn.Clone, spawn.Count);
                else if (SSu.Contains(layer))
                    RoleGenManager.SyndicateSupportRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer == Layer.Anarchist)
                    SyndicateRoles.AddMany(spawn.Clone, spawn.Count);
            }
        }
    }

    public override void Filter()
    {
        AllRoles.AddRanges(OutcastRoles, CrewRoles);

        if (BadGuysSettings.OnlyMainBadGuys)
        {
            AllRoles.AddRange(BadGuysSettings.MainBadGuys switch
            {
                Faction.Intruder => IntruderRoles,
                Faction.Syndicate => SyndicateRoles,
                Faction.Apocalypse => ApocalypseRoles,
                Faction.Pandorica => PandoricaRoles,
                Faction.Compliance => ComplianceRoles,
                _ => IlluminatiRoles
            });
        }
        else
            AllRoles.AddRanges(IntruderRoles, SyndicateRoles, ApocalypseRoles);

        if (!AllRoles.Any(x => x.ID is Layer.Dracula or Layer.Jackal or Layer.Necromancer or Layer.Whisperer))
            AllRoles.AddMany(GetSpawnItem(Layer.Seer).Clone, AllRoles.RemoveAll(x => x.ID == Layer.Mystic));

        if (AllRoles.Any(x => x.ID == Layer.Cannibal) && AllRoles.Any(x => x.ID == Layer.Janitor) && GameModifiers.JaniCanMutuallyExclusive)
        {
            var chance = URandom.RandomRangeInt(0, 2);
            var value = chance == 0 ? OutcastRoles.Random(x => x.ID != Layer.Cannibal, GetSpawnItem(Layer.Amnesiac)) : IntruderRoles.Random(x => x.ID != Layer.Janitor,
                GetSpawnItem(Layer.Impostor));
            AllRoles.AddMany(value.Clone, AllRoles.RemoveAll(x => x.ID == (chance == 0 ? Layer.Cannibal : Layer.Janitor)));
        }

        if (GameData.Instance.PlayerCount <= 4 && AllRoles.Any(x => x.ID == Layer.Amnesiac))
            AllRoles.AddMany(GetSpawnItem(Layer.Thief).Clone, AllRoles.RemoveAll(x => x.ID == Layer.Amnesiac));

        OutcastRoles.Clear();
        CrewRoles.Clear();
        SyndicateRoles.Clear();
        IntruderRoles.Clear();
        ApocalypseRoles.Clear();
        ComplianceRoles.Clear();
        IlluminatiRoles.Clear();

        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(GetSpawnItem(Layer.Crewmate));
    }

    public override void PostAssignment()
    {
        var allPlayers = AllPlayers();

        if (!allPlayers.Any(x => x.GetRole() is Amnesiac or Thief or Godfather or Shifter or Guesser or Rebel or Executioner or GuardianAngel or BountyHunter or Mystic or Actor ||
            x.GetDisposition() is Traitor or Fanatic))
        {
            allPlayers.Where(x => x.Is<Seer>()).Do(x => Gen(x, Layer.Sheriff, PlayerLayerEnum.Role));
        }
    }
}