using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public class RoleListGen : BaseRoleGen
{
    public override bool AllowNonRoles => false;

    public override void InitList()
    {
        SetPostmortals.Phantoms = GameModeSettings.PhantomCount;
        SetPostmortals.Revealers = GameModeSettings.RevealerCount;
        SetPostmortals.Banshees = GameModeSettings.BansheeCount;
        SetPostmortals.Ghouls = GameModeSettings.GhoulCount;

        foreach (var entry in OptionAttribute.GetOptions<ListEntryAttribute>().Where(x => !x.IsBan && x.EntryType == PlayerLayerEnum.Role))
        {
            var entries = new List<LayerEnum>();

            foreach (var id in entry.Get())
            {
                var rateLimit = 0;
                var cachedCount = AllRoles.Count;

                while (rateLimit < 10000 && AllRoles.Count == cachedCount)
                {
                    if (id is not LayerEnum layer)
                    {
                        layer = (LayerEnum)(id switch
                        {
                            RoleListSlot.CrewInvest => CI.Random(),
                            RoleListSlot.CrewSupport => CS.Random(),
                            RoleListSlot.CrewProt => CrP.Random(),
                            RoleListSlot.CrewKill => CK.Random(),
                            RoleListSlot.CrewSov => CSv.Random(),
                            RoleListSlot.CrewUtil => CU.Random(),
                            RoleListSlot.RegularCrew => RegCrew.Random().Random(),
                            RoleListSlot.PowerCrew => PowerCrew.Random().Random(),
                            RoleListSlot.RandomCrew => RoleGenManager.Crew.Random().Random(),
                            RoleListSlot.NonCrew => NonCrew.Random().Random().Random(),
                            RoleListSlot.NeutralKill or RoleListSlot.ComplianceKill => NK.Random(),
                            RoleListSlot.NeutralHarb or RoleListSlot.ComplianceHarb => NH.Random(),
                            RoleListSlot.NeutralNeo or RoleListSlot.ComplianceNeo => NN.Random(),
                            RoleListSlot.NeutralBen => NB.Random(),
                            RoleListSlot.NeutralEvil => NE.Random(),
                            RoleListSlot.RandomNeutral => RoleGenManager.Neutral.Random().Random(),
                            RoleListSlot.RegularNeutral or RoleListSlot.NonCompNeutral => RegNeutral.Random().Random(),
                            RoleListSlot.HarmfulNeutral or RoleListSlot.RandomCompliance => HarmNeutral.Random().Random(),
                            RoleListSlot.NonNeutral => NonNeutral.Random().Random().Random(),
                            RoleListSlot.IntruderSupport => IS.Random(),
                            RoleListSlot.IntruderConceal => IC.Random(),
                            RoleListSlot.IntruderDecep => ID.Random(),
                            RoleListSlot.IntruderKill => IK.Random(),
                            RoleListSlot.IntruderUtil => IU.Random(),
                            RoleListSlot.IntruderHead => IH.Random(),
                            RoleListSlot.RandomIntruder => RoleGenManager.Intruders.Random().Random(),
                            RoleListSlot.RegularIntruder => RegNeutral.Random().Random(),
                            RoleListSlot.PowerIntruder => PowerIntruders.Random().Random(),
                            RoleListSlot.NonIntruder => NonIntruders.Random().Random().Random(),
                            RoleListSlot.SyndicateKill => SyK.Random(),
                            RoleListSlot.SyndicateSupport => SSu.Random(),
                            RoleListSlot.SyndicateDisrup => SD.Random(),
                            RoleListSlot.SyndicatePower => SP.Random(),
                            RoleListSlot.SyndicateUtil => SU.Random(),
                            RoleListSlot.RandomSyndicate => RoleGenManager.Syndicate.Random().Random(),
                            RoleListSlot.RegularSyndicate => RegSyndicate.Random().Random(),
                            RoleListSlot.PowerSyndicate => PowerSyndicate.Random().Random(),
                            RoleListSlot.NonSyndicate => NonSyndicate.Random().Random().Random(),
                            RoleListSlot.PandoraKill => PK.Random(),
                            RoleListSlot.PandoraConceal => PC.Random(),
                            RoleListSlot.PandoraDecep => PDe.Random(),
                            RoleListSlot.PandoraDisrup => PDi.Random(),
                            RoleListSlot.PandoraPower => PP.Random(),
                            RoleListSlot.PandoraSupport => PS.Random(),
                            RoleListSlot.PandoraHead => PH.Random(),
                            RoleListSlot.PandoraUtil => PU.Random(),
                            RoleListSlot.RandomPandora => Pandorica.Random().Random(),
                            RoleListSlot.RegularPandora => RegPandorica.Random().Random(),
                            RoleListSlot.PowerPandora => PowerPandorica.Random().Random(),
                            RoleListSlot.NonPandora => NonPandorica.Random().Random().Random(),
                            RoleListSlot.NonCompliance => NonCompliance.Random().Random().Random(),
                            RoleListSlot.IlluminatiKill => IlK.Random(),
                            RoleListSlot.IlluminatiConceal => IlC.Random(),
                            RoleListSlot.IlluminatiDecep => IlDe.Random(),
                            RoleListSlot.IlluminatiDisrup => IlDi.Random(),
                            RoleListSlot.IlluminatiPower => IP.Random(),
                            RoleListSlot.IlluminatiSupport => IlS.Random(),
                            RoleListSlot.IlluminatiHead => IlHe.Random(),
                            RoleListSlot.IlluminatiUtil => IlU.Random(),
                            RoleListSlot.IlluminatiHarb => IlHa.Random(),
                            RoleListSlot.IlluminatiNeo => IN.Random(),
                            RoleListSlot.RandomIlluminati => Illuminati.Random().Random(),
                            RoleListSlot.RegularIlluminati => RegIlluminati.Random().Random(),
                            RoleListSlot.PowerIlluminati => PowerIlluminati.Random().Random(),
                            RoleListSlot.NonIlluminati => NonIlluminati.Random().Random().Random(),
                            _ => Alignments.Random().Random(),
                        });
                    }

                    if (CannotAdd(layer))
                        rateLimit++;
                    else
                        AllRoles.Add(GetSpawnItem(layer));
                }
            }
        }

        // Added rate limits to ensure the loops do not go on forever if roles have been set to unique

        while (AllRoles.Count > GameData.Instance.PlayerCount)
            AllRoles.TakeLast();

        // In case if the ratelimits and bans disallow the spawning of roles from the role list, vanilla Crewmate should spawn
        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(GetSpawnItem(LayerEnum.Crewmate));
    }

    private static bool CannotAdd(LayerEnum id) => AllRoles.Any(x => x.ID == id && x.Unique) || ListEntryAttribute.IsBanned(id);
}