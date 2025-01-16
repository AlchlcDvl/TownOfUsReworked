using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public class RoleListGen : BaseRoleGen
{
    public override bool AllowNonRoles => false;

    public override void InitList()
    {
        var entries = OptionAttribute.GetOptions<RoleListEntryAttribute>().Where(x => x.Name.Contains("Entry"));
        var alignments = entries.Where(x => AlignmentEntries.Contains(x.Get()));
        var randoms = entries.Where(x => RandomEntries.Contains(x.Get()));
        var roles = entries.Where(x => Alignments.Any(y => y.Contains(x.Get())));
        var anies = entries.Where(x => x.Get() == LayerEnum.Any);
        // I have no idea what plural for any is lmao

        SetPostmortals.Phantoms = (byte)(RoleListEntries.EnablePhantom ? 1 : 0);
        SetPostmortals.Revealers = (byte)(RoleListEntries.EnableRevealer ? 1 : 0);
        SetPostmortals.Banshees = (byte)(RoleListEntries.EnableBanshee ? 1 : 0);
        SetPostmortals.Ghouls = (byte)(RoleListEntries.EnableGhoul ? 1 : 0);

        foreach (var entry in roles)
        {
            var ratelimit = 0;
            var id = entry.Get();
            var cachedCount = AllRoles.Count;

            while (cachedCount == AllRoles.Count && ratelimit < 10000)
            {
                if (CannotAdd(id))
                    ratelimit++;
                else
                    AllRoles.Add(GetSpawnItem(id));
            }
        }

        foreach (var entry in alignments)
        {
            var cachedCount = AllRoles.Count;
            var id = entry.Get();
            var ratelimit = 0;

            while (cachedCount == AllRoles.Count && ratelimit < 10000)
            {
                var random = id switch
                {
                    LayerEnum.CrewInvest => CI.Random(),
                    LayerEnum.CrewSov => CSv.Random(),
                    LayerEnum.CrewProt => CrP.Random(),
                    LayerEnum.CrewKill => CK.Random(),
                    LayerEnum.CrewSupport => CS.Random(),
                    LayerEnum.NeutralBen => NB.Random(),
                    LayerEnum.NeutralEvil => NE.Random(),
                    LayerEnum.NeutralNeo => NN.Random(),
                    LayerEnum.NeutralHarb => NH.Random(),
                    LayerEnum.NeutralApoc => NA.Random(),
                    LayerEnum.NeutralKill => NK.Random(),
                    LayerEnum.IntruderConceal => IC.Random(),
                    LayerEnum.IntruderDecep => ID.Random(),
                    LayerEnum.IntruderKill => IK.Random(),
                    LayerEnum.IntruderSupport => IS.Random(),
                    LayerEnum.SyndicateSupport => SSu.Random(),
                    LayerEnum.SyndicatePower => SP.Random(),
                    LayerEnum.SyndicateDisrup => SD.Random(),
                    LayerEnum.SyndicateKill => SyK.Random(),
                    LayerEnum.IntruderHead => IH.Random(),
                    _ => LayerEnum.None
                };

                if (CannotAdd(random))
                    ratelimit++;
                else
                    AllRoles.Add(GetSpawnItem(random));
            }
        }

        foreach (var entry in randoms)
        {
            var cachedCount = AllRoles.Count;
            var id = entry.Get();
            var ratelimit = 0;

            while (cachedCount == AllRoles.Count && ratelimit < 10000)
            {
                var random = id switch
                {
                    LayerEnum.RandomCrew => RoleGenManager.Crew.Random().Random(),
                    LayerEnum.RandomNeutral => RoleGenManager.Neutral.Random().Random(),
                    LayerEnum.RandomIntruder => RoleGenManager.Intruders.Random().Random(),
                    LayerEnum.RandomSyndicate => RoleGenManager.Syndicate.Random().Random(),
                    LayerEnum.RegularCrew => RegCrew.Random().Random(),
                    LayerEnum.RegularIntruder => RegIntruders.Random().Random(),
                    LayerEnum.RegularNeutral => RegNeutral.Random().Random(),
                    LayerEnum.HarmfulNeutral => HarmNeutral.Random().Random(),
                    LayerEnum.RegularSyndicate => RegSyndicate.Random().Random(),
                    LayerEnum.NonIntruder => NonIntruders.Random().Random().Random(),
                    LayerEnum.NonCrew => NonCrew.Random().Random().Random(),
                    LayerEnum.NonNeutral => NonNeutral.Random().Random().Random(),
                    LayerEnum.NonSyndicate => NonSyndicate.Random().Random().Random(),
                    LayerEnum.FactionedEvil => FactionedEvils.Random().Random().Random(),
                    _ => LayerEnum.None
                };

                if (CannotAdd(random))
                    ratelimit++;
                else
                    AllRoles.Add(GetSpawnItem(random));
            }
        }

        foreach (var entry in anies)
        {
            var cachedCount = AllRoles.Count;
            var ratelimit = 0;

            while (cachedCount == AllRoles.Count && ratelimit < 10000)
            {
                var random = Alignments.Random().Random();

                if (CannotAdd(random))
                    ratelimit++;
                else
                    AllRoles.Add(GetSpawnItem(random));
            }
        }

        // Added rate limits to ensure the loops do not go on forever if roles have been set to unique

        // In case if the ratelimits and bans disallow the spawning of roles from the role list, vanilla Crewmate should spawn
        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(GetSpawnItem(LayerEnum.Crewmate));
    }

    private static bool CannotAdd(LayerEnum id) => AllRoles.Any(x => x.ID == id && x.Unique) || RoleListEntryAttribute.IsBanned(id);
}