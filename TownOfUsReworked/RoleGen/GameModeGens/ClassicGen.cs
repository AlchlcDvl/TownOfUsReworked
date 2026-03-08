using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class ClassicGen : BaseClassicAllAnyGen
{
    public override void InitCrewList()
    {
        base.InitCrewList();
        ProcessFaction(Crew, CrewSettings.CrewMin, CrewSettings.CrewMax, CrewRoles, Layer.Crewmate,
            [RoleGenManager.CrewInvestigativeRoles, RoleGenManager.CrewKillingRoles, RoleGenManager.CrewSupportRoles, RoleGenManager.CrewProtectiveRoles, RoleGenManager.CrewSovereignRoles],
            [CrewInvestigativeSettings.MaxCi.Value, CrewKillingSettings.MaxCk.Value, CrewSupportSettings.MaxCs.Value, CrewProtectiveSettings.MaxCrP.Value, CrewSovereignSettings.MaxCSv.Value]
        );
    }

    public override void InitIntList()
    {
        base.InitIntList();
        ProcessFaction(Intruders, IntruderSettings.IntruderMin, IntruderSettings.IntruderMax, IntruderRoles, Layer.Impostor,
            [RoleGenManager.IntruderConcealingRoles, RoleGenManager.IntruderDeceptionRoles, RoleGenManager.IntruderKillingRoles, RoleGenManager.IntruderSupportRoles, RoleGenManager.IntruderHeadRoles],
            [IntruderConcealingSettings.MaxIc.Value, IntruderDeceptionSettings.MaxID.Value, IntruderKillingSettings.MaxIK.Value, IntruderSupportSettings.MaxIs.Value, IntruderHeadSettings.MaxIh.Value]
        );
    }

    public override void InitNeutList()
    {
        base.InitNeutList();

        var includeKilling = !BadGuysSettings.OrderOfCompliance || BadGuysSettings.ComplianceMembers != ComplianceType.Killers;
        var includeNeophyte = !BadGuysSettings.OrderOfCompliance || BadGuysSettings.ComplianceMembers != ComplianceType.Neophytes;

        ProcessFaction(Outcasts, OutcastSettings.OutcastMin, OutcastSettings.OutcastMax, OutcastRoles, Layer.Amnesiac,
            [RoleGenManager.OutcastBenignRoles, RoleGenManager.OutcastEvilRoles, RoleGenManager.OutcastKillingRoles, RoleGenManager.OutcastNeophyteRoles],
            [OutcastBenignSettings.MaxNb.Value, OutcastEvilSettings.MaxNe.Value, OutcastKillingSettings.MaxNk.Value, OutcastNeophyteSettings.MaxNn.Value],
            [true, true, includeKilling, includeNeophyte]
        );
    }

    public override void InitApocList()
    {
        base.InitApocList();
        ProcessFaction(Apocalypse, ApocalypseSettings.ApocalypseMin, ApocalypseSettings.ApocalypseMax, ApocalypseRoles, Layer.Cultist,
            [RoleGenManager.ApocalypseHarbingerRoles], [ApocalypseHarbingerSettings.MaxAh.Value]
        );
    }

    public override void InitSynList()
    {
        base.InitSynList();
        ProcessFaction(Syndicate, SyndicateSettings.SyndicateMin, SyndicateSettings.SyndicateMax, SyndicateRoles, Layer.Anarchist,
            [RoleGenManager.SyndicateSupportRoles, RoleGenManager.SyndicateDisruptionRoles, RoleGenManager.SyndicateKillingRoles, RoleGenManager.SyndicateHeadRoles],
            [SyndicateSupportSettings.MaxSSu.Value, SyndicateDisruptionSettings.MaxSD.Value, SyndicateKillingSettings.MaxSyK.Value, SyndicateHeadSettings.MaxSh.Value]
        );
    }

    public override void PreFilter()
    {
        if (BadGuysSettings.IlluminatiUnleashed)
        {
            var type = BadGuysSettings.IlluminatiMembers;

            if (type == IlluminatiType.Syndicate)
            {
                IlluminatiRoles.AddRange(SyndicateRoles);
                SyndicateRoles.Clear();
            }

            if (type == IlluminatiType.Intruders)
            {
                IlluminatiRoles.AddRange(IntruderRoles);
                IntruderRoles.Clear();
            }

            if (type == IlluminatiType.Apocalypse)
            {
                IlluminatiRoles.AddRange(ApocalypseRoles);
                ApocalypseRoles.Clear();
            }

            if (type == IlluminatiType.Killers)
            {
                IlluminatiRoles.AddRange(RoleGenManager.OutcastKillingRoles);
                RoleGenManager.OutcastKillingRoles.Clear();
            }

            if (type == IlluminatiType.Neophytes)
            {
                IlluminatiRoles.AddRange(RoleGenManager.OutcastNeophyteRoles);
                RoleGenManager.OutcastNeophyteRoles.Clear();
            }

            FinalizeBadGuysFaction(IlluminatiRoles, IlluminatiSettings.IlluminatiCount, IlluminatiSettings.IlluminatiMin, IlluminatiSettings.IlluminatiMax, Faction.Illuminati);
            return;
        }

        if (BadGuysSettings.OrderOfCompliance)
        {
            var type = BadGuysSettings.ComplianceMembers;

            if (type == ComplianceType.Killers)
            {
                ComplianceRoles.AddRange(RoleGenManager.OutcastKillingRoles);
                RoleGenManager.OutcastKillingRoles.Clear();
            }

            if (type == ComplianceType.Neophytes)
            {
                ComplianceRoles.AddRange(RoleGenManager.OutcastNeophyteRoles);
                RoleGenManager.OutcastNeophyteRoles.Clear();
            }

            FinalizeBadGuysFaction(ComplianceRoles, ComplianceSettings.ComplianceCount, ComplianceSettings.ComplianceMin, ComplianceSettings.ComplianceMax, Faction.Compliance);
        }

        if (BadGuysSettings.PandoricaOpens)
        {
            var type = BadGuysSettings.PandoricaMembers;

            if (type == PandoricaType.Syndicate)
            {
                PandoricaRoles.AddRange(SyndicateRoles);
                SyndicateRoles.Clear();
            }

            if (type == PandoricaType.Intruders)
            {
                PandoricaRoles.AddRange(IntruderRoles);
                IntruderRoles.Clear();
            }

            if (type == PandoricaType.Apocalypse)
            {
                PandoricaRoles.AddRange(ApocalypseRoles);
                ApocalypseRoles.Clear();
            }

            FinalizeBadGuysFaction(PandoricaRoles, PandoricaSettings.PandoricaCount, PandoricaSettings.PandoricaMin, PandoricaSettings.PandoricaMax, Faction.Pandorica);
        }
    }

    private static void ProcessFaction(int totalFactionCount, int minSetting, int maxSetting, List<RoleOptionData> finalFactionRoles, Layer defaultLayer, List<RoleOptionData>[] subFactionLists, Number[] subFactionMaxes,
        bool[]? mergeFlags = null)
    {
        if (totalFactionCount == 0)
            return;

        var minAllowed = Mathf.Clamp(minSetting, 0, totalFactionCount);
        var maxAllowed = Mathf.Clamp(maxSetting, 0, totalFactionCount);

        if (minAllowed > maxAllowed)
            (maxAllowed, minAllowed) = (minAllowed, maxAllowed);

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            for (var i = 0; i < subFactionMaxes.Length; i++)
                subFactionMaxes[i] = Mathf.Clamp(subFactionMaxes[i], 0, subFactionLists[i].Count);

            var maxSum = subFactionMaxes.Sum(x => x);
            var validIndices = new List<int>(subFactionMaxes.Length);

            while (maxSum > maxAllowed && maxSum > 0)
            {
                validIndices.Clear();

                for (var i = 0; i < subFactionMaxes.Length; i++)
                {
                    if (subFactionMaxes[i] > 0)
                        validIndices.Add(i);
                }

                if (validIndices.Count == 0)
                    break;

                subFactionMaxes[validIndices.Random()]--;
                maxSum--;
            }

            for (var i = 0; i < subFactionLists.Length; i++)
                filter.Filter(subFactionLists[i], subFactionMaxes[i]);
        }

        for (var i = 0; i < subFactionLists.Length; i++)
        {
            if (mergeFlags?[i] != false)
                finalFactionRoles.AddRange(subFactionLists[i]);
        }

        filter.Filter(finalFactionRoles, GameModeSettings.IgnoreFactionCaps ? totalFactionCount : URandom.RandomRangeInt(minAllowed, maxAllowed + 1));

        while (finalFactionRoles.Count < totalFactionCount)
            finalFactionRoles.Add(GetSpawnItem(defaultLayer));
    }

    private static void FinalizeBadGuysFaction(List<RoleOptionData> targetList, int totalCount, int minSetting, int maxSetting, Faction faction)
    {
        var minAllowed = Mathf.Clamp(minSetting, 0, totalCount);
        var maxAllowed = Mathf.Clamp(maxSetting, 0, totalCount);

        if (minAllowed > maxAllowed)
            (maxAllowed, minAllowed) = (minAllowed, maxAllowed);

        ModeFilters[GameModeSettings.GameMode].Filter(targetList, GameModeSettings.IgnoreFactionCaps ? totalCount : URandom.RandomRangeInt(minAllowed, maxAllowed + 1));

        while (targetList.Count < totalCount)
            targetList.Add(GetSpawnItem(GetRandomBaseEvil(faction)));
    }
}