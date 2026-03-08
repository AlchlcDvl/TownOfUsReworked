using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public abstract class BaseClassicAllAnyGen : BaseRoleGen
{
    public override void InitList() => GetAdjustedFactions();

    public override void InitCrewList() => ProcessLayerSpawns(GetValuesFromTo(Layer.Altruist, Layer.Vigilante), Crew, Layer.Revealer, ref SetPostmortals.Revealers,
        (CSv, RoleGenManager.CrewSovereignRoles),
        (CI, RoleGenManager.CrewInvestigativeRoles),
        (CK, RoleGenManager.CrewKillingRoles),
        (CrP, RoleGenManager.CrewProtectiveRoles),
        (CS, RoleGenManager.CrewSupportRoles),
        ([Layer.Crewmate], CrewRoles)
    );

    public override void InitIntList() => ProcessLayerSpawns(GetValuesFromTo(Layer.Ambusher, Layer.Wraith, x => x is not Layer.Mafioso), Intruders, Layer.Ghoul, ref SetPostmortals.Ghouls,
        (IH, RoleGenManager.IntruderHeadRoles),
        (IC, RoleGenManager.IntruderConcealingRoles),
        (ID, RoleGenManager.IntruderDeceptionRoles),
        (IK, RoleGenManager.IntruderKillingRoles),
        (IS, RoleGenManager.IntruderSupportRoles),
        ([Layer.Impostor], IntruderRoles)
    );

    public override void InitNeutList() => ProcessLayerSpawns(GetValuesFromTo(Layer.Actor, Layer.Whisperer, x => x != Layer.Betrayer), Outcasts, Layer.Phantom, ref SetPostmortals.Phantoms,
        (NB, RoleGenManager.OutcastBenignRoles),
        (NE, RoleGenManager.OutcastEvilRoles),
        (NK, RoleGenManager.OutcastKillingRoles),
        (NN, RoleGenManager.OutcastNeophyteRoles)
    );

    public override void InitApocList()
    {
        byte dump = 0;
        ProcessLayerSpawns(GetValuesFromTo(Layer.Cannibal, Layer.Void), Apocalypse, null, ref dump, // TODO: Add an apoc postmortal role
            (AH, RoleGenManager.ApocalypseHarbingerRoles),
            (AD, RoleGenManager.ApocalypseHarbingerRoles)
        );
    }

    public override void InitSynList() => ProcessLayerSpawns(GetValuesFromTo(Layer.Anarchist, Layer.Warper, x => x is not Layer.Sidekick), Syndicate, Layer.Banshee, ref SetPostmortals.Banshees,
        (SH, RoleGenManager.SyndicateHeadRoles),
        (SD, RoleGenManager.SyndicateDisruptionRoles),
        (SyK, RoleGenManager.SyndicateKillingRoles),
        (SSu, RoleGenManager.SyndicateSupportRoles),
        ([Layer.Anarchist], SyndicateRoles)
    );

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
            AllRoles.AddRanges(IntruderRoles, SyndicateRoles, ApocalypseRoles, PandoricaRoles, ComplianceRoles, IlluminatiRoles);

        if (!AllRoles.Any(x => x.ID is Layer.Dracula or Layer.Jackal or Layer.Necromancer or Layer.Whisperer))
            AllRoles.AddMany(GetSpawnItem(Layer.Seer).Clone, AllRoles.RemoveAll(x => x.ID == Layer.Mystic));

        if (AllRoles.Any(x => x.ID == Layer.Cannibal) && AllRoles.Any(x => x.ID == Layer.Janitor) && GameModifiers.JaniCanMutuallyExclusive)
        {
            var chance = URandom.RandomRangeInt(0, 2);
            var value = chance == 0 ? OutcastRoles.Random(x => x.ID != Layer.Cannibal, GetSpawnItem(Layer.Amnesiac)) : IntruderRoles.Random(x => x.ID != Layer.Janitor, GetSpawnItem(Layer.Impostor));
            AllRoles.AddMany(value.Clone, AllRoles.RemoveAll(x => x.ID == (chance == 0 ? Layer.Cannibal : Layer.Janitor)));
        }

        if (GameData.Instance.PlayerCount <= 4 && AllRoles.Any(x => x.ID == Layer.Amnesiac))
            AllRoles.AddMany(GetSpawnItem(Layer.Thief).Clone, AllRoles.RemoveAll(x => x.ID == Layer.Amnesiac));

        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(GetSpawnItem(Layer.Crewmate));

        OutcastRoles.Clear();
        CrewRoles.Clear();
        SyndicateRoles.Clear();
        IntruderRoles.Clear();
        ApocalypseRoles.Clear();
        ComplianceRoles.Clear();
        IlluminatiRoles.Clear();
    }

    public override void PostAssignment()
    {
        var allPlayers = AllPlayers();

        if (!allPlayers.Any(x => x.GetRole() is Amnesiac or Thief or Godfather or Rebel or Shifter or ITargeter or Actor || x.GetDisposition() is Traitor or Fanatic))
            allPlayers.Where(x => x.Is<Seer>()).Do(x => Gen(x, Layer.Sheriff, PlayerLayerEnum.Role));
    }

    private static void ProcessLayerSpawns(IEnumerable<Layer> layers, int factionCount, Layer? postmortalLayer, ref byte postmortalCount, params (IEnumerable<Layer> collection, List<RoleOptionData> destination)[] routings)
    {
        if (factionCount == 0)
            return;

        foreach (var layer in layers)
        {
            var spawn = GetSpawnItem(layer);

            if (postmortalLayer.HasValue && layer == postmortalLayer)
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (Check(spawn.Chance))
                        postmortalCount++;
                }
            }
            else if (spawn.IsActive(factionCount))
            {
                foreach (var (collection, destination) in routings)
                {
                    if (!collection.Contains(layer))
                        continue;

                    destination.AddMany(spawn.Clone, spawn.Count);
                    break;
                }
            }
        }
    }
}