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

        foreach (var entry in Option.GetOptions<ListEntryOption>().Where(x => !x.IsBan && x.EntryType == PlayerLayerEnum.Role && x.Num <= GameData.Instance.PlayerCount))
        {
            var bucket = entry.Value
                .SelectMany(x => x.TryCastToLayer(out var layer) ? [layer] : GetBucket(x)!)
                .ToList();

            var validLayers = bucket.Where(layer => !CannotAdd(layer, AllRoles)).ToList();

            if (validLayers.Count > 0)
                AllRoles.Add(GetSpawnItem(validLayers.Random()));
        }

        while (AllRoles.Count > GameData.Instance.PlayerCount)
            AllRoles.TakeLast();

        // In case if the rate limits and bans disallow the spawning of roles from the role list, vanilla Crewmate should spawn
        while (AllRoles.Count < GameData.Instance.PlayerCount)
            AllRoles.Add(GetSpawnItem(Layer.Crewmate));
    }

    public static bool CannotAdd(Layer id, List<RoleOptionData> list) => list.Any(x => x.ID == id && x.Unique) || ListEntryOption.IsBanned(id.CastToSlot()) || (id == Layer.Anarchist &&
        GameModeSettings.BanAnarchist) || (id == Layer.Crewmate && GameModeSettings.BanCrewmate) || (id == Layer.Impostor && GameModeSettings.BanImpostor) || (id == Layer.Murderer &&
        GameModeSettings.BanMurderer) || (id == Layer.Cultist && GameModeSettings.BanCultist) || (id == Layer.Zealot && GameModeSettings.BanZealot);

    public static Layer[]? GetBucket(ListSlot slot) => slot switch
    {
        ListSlot.CrewInvest => CI,
        ListSlot.CrewSupport => CS,
        ListSlot.CrewProt => CrP,
        ListSlot.CrewKill => CK,
        ListSlot.CrewSov => CSv,
        ListSlot.CrewUtil => CU,
        ListSlot.RegularCrew => RegCrew,
        ListSlot.PowerCrew => PowerCrew,
        ListSlot.RandomCrew => RoleGenManager.Crew,
        ListSlot.NonCrew => NonCrew,
        ListSlot.OutcastKill or ListSlot.ComplianceKill => NK,
        ListSlot.OutcastNeo or ListSlot.ComplianceNeo => NN,
        ListSlot.OutcastBen => NB,
        ListSlot.OutcastEvil => NE,
        ListSlot.RandomOutcast => RoleGenManager.Outcast,
        ListSlot.RegularOutcast => RegOutcast,
        ListSlot.HarmfulOutcast => HarmOutcast,
        ListSlot.NonOutcast => NonOutcast,
        ListSlot.IntruderSupport => IS,
        ListSlot.IntruderConceal => IC,
        ListSlot.IntruderDecep => ID,
        ListSlot.IntruderKill => IK,
        ListSlot.IntruderUtil => IU,
        ListSlot.IntruderHead => IH,
        ListSlot.RandomIntruder => RoleGenManager.Intruders,
        ListSlot.RegularIntruder => RegIntruders,
        ListSlot.PowerIntruder => PowerIntruders,
        ListSlot.NonIntruder => NonIntruders,
        ListSlot.SyndicateKill => SyK,
        ListSlot.SyndicateSupport => SSu,
        ListSlot.SyndicateDisrup => SD,
        ListSlot.SyndicateHead => SH,
        ListSlot.SyndicateUtil => SU,
        ListSlot.RandomSyndicate => RoleGenManager.Syndicate,
        ListSlot.RegularSyndicate => RegSyndicate,
        ListSlot.PowerSyndicate => PowerSyndicate,
        ListSlot.NonSyndicate => NonSyndicate,
        ListSlot.ApocHarb => AH,
        ListSlot.NonApocalypse => NonApocalypse,
        ListSlot.NonCompOutcast => NonCompOutcast(),
        ListSlot.RandomCompliance => Compliance(),
        ListSlot.PandoraKill => PK(),
        ListSlot.PandoraConceal => PC(),
        ListSlot.PandoraDecep => PDe(),
        ListSlot.PandoraDisrup => PDi(),
        ListSlot.PandoraSupport => PS(),
        ListSlot.PandoraHead => PHe(),
        ListSlot.PandoraHarb => PHa(),
        ListSlot.PandoraUtil => PU(),
        ListSlot.RandomPandora => Pandorica(),
        ListSlot.RegularPandora => RegPandorica(),
        ListSlot.PowerPandora => PowerPandorica(),
        ListSlot.NonPandora => NonPandorica(),
        ListSlot.NonCompliance => NonCompliance(),
        ListSlot.IlluminatiKill => IlK(),
        ListSlot.IlluminatiConceal => IlC(),
        ListSlot.IlluminatiDecep => IlDe(),
        ListSlot.IlluminatiDisrup => IlDi(),
        ListSlot.IlluminatiSupport => IlS(),
        ListSlot.IlluminatiHead => IlHe(),
        ListSlot.IlluminatiUtil => IlU(),
        ListSlot.IlluminatiHarb => IlHa(),
        ListSlot.IlluminatiNeo => IN(),
        ListSlot.RandomIlluminati => Illuminati(),
        ListSlot.RegularIlluminati => RegIlluminati(),
        ListSlot.PowerIlluminati => PowerIlluminati(),
        ListSlot.NonIlluminati => NonIlluminati(),
        ListSlot.NonIllOutcast => NonIlluminati(),
        ListSlot.Any => Alignments,
        _ => null
    };
}