using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class ListGen : BaseRoleGen
{
    public override void InitList()
    {
        SetPostmortals.Phantoms = GameModeSettings.PhantomCount;
        SetPostmortals.Revealers = GameModeSettings.RevealerCount;
        SetPostmortals.Banshees = GameModeSettings.BansheeCount;
        SetPostmortals.Ghouls = GameModeSettings.GhoulCount;

        foreach (var entry in Option.GetOptions<ListEntryOption>().Where(x => !x.IsBan && x.EntryType == PlayerLayerEnum.Role))
        {
            foreach (var id in entry.Value)
            {
                if (id == ListSlot.None)
                    break;

                var rateLimit = 0;
                var cachedCount = AllRoles.Count;

                while (rateLimit < 10000 && AllRoles.Count == cachedCount)
                {
                    if (!id.TryCastToLayer(out var layer))
                    {
                        layer = (LayerEnum)(id switch
                        {
                            ListSlot.CrewInvest => CI.Random(),
                            ListSlot.CrewSupport => CS.Random(),
                            ListSlot.CrewProt => CrP.Random(),
                            ListSlot.CrewKill => CK.Random(),
                            ListSlot.CrewSov => CSv.Random(),
                            ListSlot.CrewUtil => CU.Random(),
                            ListSlot.RegularCrew => RegCrew.Random().Random(),
                            ListSlot.PowerCrew => PowerCrew.Random().Random(),
                            ListSlot.RandomCrew => RoleGenManager.Crew.Random().Random(),
                            ListSlot.NonCrew => NonCrew.Random().Random().Random(),
                            ListSlot.NeutralKill or ListSlot.ComplianceKill => NK.Random(),
                            ListSlot.NeutralHarb or ListSlot.ComplianceHarb => NH.Random(),
                            ListSlot.NeutralNeo or ListSlot.ComplianceNeo => NN.Random(),
                            ListSlot.NeutralBen => NB.Random(),
                            ListSlot.NeutralEvil => NE.Random(),
                            ListSlot.RandomNeutral => RoleGenManager.Neutral.Random().Random(),
                            ListSlot.RegularNeutral or ListSlot.NonCompNeutral => RegNeutral.Random().Random(),
                            ListSlot.HarmfulNeutral or ListSlot.RandomCompliance => HarmNeutral.Random().Random(),
                            ListSlot.NonNeutral => NonNeutral.Random().Random().Random(),
                            ListSlot.IntruderSupport => IS.Random(),
                            ListSlot.IntruderConceal => IC.Random(),
                            ListSlot.IntruderDecep => ID.Random(),
                            ListSlot.IntruderKill => IK.Random(),
                            ListSlot.IntruderUtil => IU.Random(),
                            ListSlot.IntruderHead => IH.Random(),
                            ListSlot.RandomIntruder => RoleGenManager.Intruders.Random().Random(),
                            ListSlot.RegularIntruder => RegIntruders.Random().Random(),
                            ListSlot.PowerIntruder => PowerIntruders.Random().Random(),
                            ListSlot.NonIntruder => NonIntruders.Random().Random().Random(),
                            ListSlot.SyndicateKill => SyK.Random(),
                            ListSlot.SyndicateSupport => SSu.Random(),
                            ListSlot.SyndicateDisrup => SD.Random(),
                            ListSlot.SyndicatePower => SP.Random(),
                            ListSlot.SyndicateUtil => SU.Random(),
                            ListSlot.RandomSyndicate => RoleGenManager.Syndicate.Random().Random(),
                            ListSlot.RegularSyndicate => RegSyndicate.Random().Random(),
                            ListSlot.PowerSyndicate => PowerSyndicate.Random().Random(),
                            ListSlot.NonSyndicate => NonSyndicate.Random().Random().Random(),
                            ListSlot.PandoraKill => PK.Random(),
                            ListSlot.PandoraConceal => PC.Random(),
                            ListSlot.PandoraDecep => PDe.Random(),
                            ListSlot.PandoraDisrup => PDi.Random(),
                            ListSlot.PandoraPower => PP.Random(),
                            ListSlot.PandoraSupport => PS.Random(),
                            ListSlot.PandoraHead => PH.Random(),
                            ListSlot.PandoraUtil => PU.Random(),
                            ListSlot.RandomPandora => Pandorica.Random().Random(),
                            ListSlot.RegularPandora => RegPandorica.Random().Random(),
                            ListSlot.PowerPandora => PowerPandorica.Random().Random(),
                            ListSlot.NonPandora => NonPandorica.Random().Random().Random(),
                            ListSlot.NonCompliance => NonCompliance.Random().Random().Random(),
                            ListSlot.IlluminatiKill => IlK.Random(),
                            ListSlot.IlluminatiConceal => IlC.Random(),
                            ListSlot.IlluminatiDecep => IlDe.Random(),
                            ListSlot.IlluminatiDisrup => IlDi.Random(),
                            ListSlot.IlluminatiPower => IP.Random(),
                            ListSlot.IlluminatiSupport => IlS.Random(),
                            ListSlot.IlluminatiHead => IlHe.Random(),
                            ListSlot.IlluminatiUtil => IlU.Random(),
                            ListSlot.IlluminatiHarb => IlHa.Random(),
                            ListSlot.IlluminatiNeo => IN.Random(),
                            ListSlot.RandomIlluminati => Illuminati.Random().Random(),
                            ListSlot.RegularIlluminati => RegIlluminati.Random().Random(),
                            ListSlot.PowerIlluminati => PowerIlluminati.Random().Random(),
                            ListSlot.NonIlluminati => NonIlluminati.Random().Random().Random(),
                            _ => Alignments.Random().Random(),
                        });
                    }

                    if (CannotAdd(layer, AllRoles))
                        rateLimit++;
                    else
                        AllRoles.Add(GetSpawnItem(layer));
                }
            }
        }

        // Added rate limits to ensure the loops do not go on forever if roles have been set to unique

        while (AllRoles.Count > GameData.Instance.PlayerCount)
            AllRoles.TakeLast();

        // In case if the rate limits and bans disallow the spawning of roles from the role list, vanilla Crewmate should spawn
        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(GetSpawnItem(LayerEnum.Crewmate));
    }

    public static bool CannotAdd(LayerEnum id, List<RoleOptionData> list) => list.Any(x => x.ID == id && x.Unique) || ListEntryOption.IsBanned(id.CastToSlot());
}