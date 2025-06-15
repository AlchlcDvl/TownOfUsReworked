using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class ClassicGen : BaseClassicAllAnyGen
{
    public override void InitCrewList()
    {
        base.InitCrewList();

        if (Crew == 0)
            return;

        var minCrew = Mathf.Clamp(CrewSettings.CrewMin, 0, Crew);
        var maxCrew = Mathf.Clamp(CrewSettings.CrewMax, 0, Crew);

        if (minCrew > maxCrew)
            (maxCrew, minCrew) = (minCrew, maxCrew);

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            var maxCi = Mathf.Clamp(CrewInvestigativeSettings.MaxCi.Value, 0, RoleGenManager.CrewInvestigativeRoles.Count);
            var maxCs = Mathf.Clamp(CrewSupportSettings.MaxCs.Value, 0, RoleGenManager.CrewSupportRoles.Count);
            var maxCk = Mathf.Clamp(CrewKillingSettings.MaxCk.Value, 0, RoleGenManager.CrewKillingRoles.Count);
            var maxCrP = Mathf.Clamp(CrewProtectiveSettings.MaxCrP.Value, 0, RoleGenManager.CrewProtectiveRoles.Count);
            var maxCSv = Mathf.Clamp(CrewSovereignSettings.MaxCSv.Value, 0, RoleGenManager.CrewSovereignRoles.Count);
            var maxCrewSum = maxCi + maxCk + maxCrP + maxCs + maxCSv;

            while (maxCrewSum > maxCrew && maxCrewSum > 0)
            {
                switch (URandom.RandomRangeInt(1, 6))
                {
                    case 1:
                    {
                        if (maxCi > 0)
                            maxCi--;

                        break;
                    }
                    case 2:
                    {
                        if (maxCk > 0)
                            maxCk--;

                        break;
                    }
                    case 3:
                    {
                        if (maxCs > 0)
                            maxCs--;

                        break;
                    }
                    case 4:
                    {
                        if (maxCSv > 0)
                            maxCSv--;

                        break;
                    }
                    case 5:
                    {
                        if (maxCrP > 0)
                            maxCrP--;

                        break;
                    }
                }

                maxCrewSum = maxCi + maxCk + maxCrP + maxCs + maxCSv;
            }

            filter.Filter(RoleGenManager.CrewInvestigativeRoles, maxCi);
            filter.Filter(RoleGenManager.CrewKillingRoles, maxCk);
            filter.Filter(RoleGenManager.CrewProtectiveRoles, maxCrP);
            filter.Filter(RoleGenManager.CrewSupportRoles, maxCs);
            filter.Filter(RoleGenManager.CrewSovereignRoles, maxCSv);
        }

        CrewRoles.AddRanges(RoleGenManager.CrewInvestigativeRoles, RoleGenManager.CrewKillingRoles, RoleGenManager.CrewSupportRoles, RoleGenManager.CrewProtectiveRoles,
            RoleGenManager.CrewSovereignRoles);

        filter.Filter(CrewRoles, GameModeSettings.IgnoreFactionCaps ? Crew : URandom.RandomRangeInt(minCrew, maxCrew + 1));

        while (CrewRoles.Count < Crew)
            CrewRoles.Add(GetSpawnItem(LayerEnum.Crewmate));
    }

    public override void InitIntList()
    {
        base.InitIntList();

        if (Intruders == 0)
            return;

        var minInt = Mathf.Clamp(IntruderSettings.IntruderMin, 0, Intruders);
        var maxInt = Mathf.Clamp(IntruderSettings.IntruderMax, 0, Intruders);

        if (minInt > maxInt)
            (maxInt, minInt) = (minInt, maxInt);

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            var maxIc = Mathf.Clamp(IntruderConcealingSettings.MaxIc.Value, 0, RoleGenManager.IntruderConcealingRoles.Count);
            var maxID = Mathf.Clamp(IntruderDeceptionSettings.MaxID.Value, 0, RoleGenManager.IntruderDeceptionRoles.Count);
            var maxIK = Mathf.Clamp(IntruderKillingSettings.MaxIK.Value, 0, RoleGenManager.IntruderKillingRoles.Count);
            var maxIs = Mathf.Clamp(IntruderSupportSettings.MaxIs.Value, 0, RoleGenManager.IntruderSupportRoles.Count);
            var maxIh = Mathf.Clamp(IntruderHeadSettings.MaxIh.Value, 0, RoleGenManager.IntruderHeadRoles.Count);
            var maxIntSum = maxIc + maxID + maxIK + maxIs + maxIh;

            while (maxIntSum > maxInt && maxIntSum > 0)
            {
                switch (URandom.RandomRangeInt(0, 5))
                {
                    case 0:
                    {
                        if (maxIc > 0)
                            maxIc--;

                        break;
                    }
                    case 1:
                    {
                        if (maxID > 0)
                            maxID--;

                        break;
                    }
                    case 2:
                    {
                        if (maxIK > 0)
                            maxIK--;

                        break;
                    }
                    case 3:
                    {
                        if (maxIs > 0)
                            maxIs--;

                        break;
                    }
                    case 4:
                    {
                        if (maxIh > 0)
                            maxIh--;

                        break;
                    }
                }

                maxIntSum = maxIc + maxID + maxIK + maxIs + maxIh;
            }

            filter.Filter(RoleGenManager.IntruderConcealingRoles, maxIc);
            filter.Filter(RoleGenManager.IntruderDeceptionRoles, maxID);
            filter.Filter(RoleGenManager.IntruderKillingRoles, maxIK);
            filter.Filter(RoleGenManager.IntruderSupportRoles, maxIs);
            filter.Filter(RoleGenManager.IntruderHeadRoles, maxIh);
        }

        IntruderRoles.AddRanges(RoleGenManager.IntruderConcealingRoles, RoleGenManager.IntruderDeceptionRoles, RoleGenManager.IntruderKillingRoles, RoleGenManager.IntruderSupportRoles,
            RoleGenManager.IntruderHeadRoles);

        filter.Filter(IntruderRoles, GameModeSettings.IgnoreFactionCaps ? Intruders : URandom.RandomRangeInt(minInt, maxInt + 1));

        while (IntruderRoles.Count < Intruders)
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Impostor));
    }

    public override void InitNeutList()
    {
        base.InitNeutList();

        if (Outcasts == 0)
            return;

        var minNeut = Mathf.Clamp(OutcastSettings.OutcastMin, 0, Outcasts);
        var maxNeut = Mathf.Clamp(OutcastSettings.OutcastMax, 0, Outcasts);

        if (minNeut > maxNeut)
            (maxNeut, minNeut) = (minNeut, maxNeut);

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            var maxNe = Mathf.Clamp(OutcastEvilSettings.MaxNe.Value, 0, RoleGenManager.OutcastEvilRoles.Count);
            var maxNb = Mathf.Clamp(OutcastBenignSettings.MaxNb.Value, 0, RoleGenManager.OutcastBenignRoles.Count);
            var maxNk = Mathf.Clamp(OutcastKillingSettings.MaxNk.Value, 0, RoleGenManager.OutcastKillingRoles.Count);
            var maxNn = Mathf.Clamp(OutcastNeophyteSettings.MaxNn.Value, 0, RoleGenManager.OutcastNeophyteRoles.Count);
            var maxNeutSum = maxNe + maxNb + maxNk + maxNn;

            while (maxNeutSum > maxNeut && maxNeutSum > 0)
            {
                switch (URandom.RandomRangeInt(0, 4))
                {
                    case 0:
                    {
                        if (maxNe > 0)
                            maxNe--;

                        break;
                    }
                    case 1:
                    {
                        if (maxNb > 0)
                            maxNb--;

                        break;
                    }
                    case 2:
                    {
                        if (maxNk > 0)
                            maxNk--;

                        break;
                    }
                    case 3:
                    {
                        if (maxNn > 0)
                            maxNn--;

                        break;
                    }
                }

                maxNeutSum = maxNe + maxNb + maxNk + maxNn;
            }

            filter.Filter(RoleGenManager.OutcastBenignRoles, maxNb);
            filter.Filter(RoleGenManager.OutcastEvilRoles, maxNe);
            filter.Filter(RoleGenManager.OutcastKillingRoles, maxNk);
            filter.Filter(RoleGenManager.OutcastNeophyteRoles, maxNn);
        }

        OutcastRoles.AddRanges(RoleGenManager.OutcastBenignRoles, RoleGenManager.OutcastEvilRoles);

        if (BadGuysSettings.OrderOfCompliance)
        {
            var type = BadGuysSettings.ComplianceMembers;

            if (type != ComplianceType.Killers)
                OutcastRoles.AddRange(RoleGenManager.OutcastKillingRoles);

            if (type != ComplianceType.Neophytes)
                OutcastRoles.AddRange(RoleGenManager.OutcastNeophyteRoles);
        }

        filter.Filter(OutcastRoles, GameModeSettings.IgnoreFactionCaps ? Outcasts : URandom.RandomRangeInt(minNeut, maxNeut + 1));

        while (OutcastRoles.Count < Outcasts)
            OutcastRoles.Add(GetSpawnItem(LayerEnum.Amnesiac));
    }

    public override void InitApocList()
    {
        base.InitApocList();

        if (Apocalypse == 0)
            return;

        var minApoc = Mathf.Clamp(ApocalypseSettings.ApocalypseMin, 0, Apocalypse);
        var maxApoc = Mathf.Clamp(ApocalypseSettings.ApocalypseMax, 0, Apocalypse);

        if (minApoc > maxApoc)
            (maxApoc, minApoc) = (minApoc, maxApoc);

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            var maxAh = Mathf.Clamp(ApocalypseHarbingerSettings.MaxAh.Value, 0, RoleGenManager.ApocalypseHarbingerRoles.Count);
            var maxApocSum = maxAh;

            while (maxApocSum > maxApoc && maxApocSum > 0)
            {
                if (maxAh > 0)
                    maxAh--;

                maxApocSum = maxAh;
            }

            filter.Filter(RoleGenManager.ApocalypseHarbingerRoles, maxAh);
        }

        ApocalypseRoles.AddRanges(RoleGenManager.ApocalypseHarbingerRoles);

        filter.Filter(ApocalypseRoles, GameModeSettings.IgnoreFactionCaps ? Apocalypse : URandom.RandomRangeInt(minApoc, maxApoc + 1));

        while (ApocalypseRoles.Count < Apocalypse)
            ApocalypseRoles.Add(GetSpawnItem(LayerEnum.Cultist));
    }

    public override void InitSynList()
    {
        base.InitSynList();

        if (Syndicate == 0)
            return;

        var minSyn = Mathf.Clamp(SyndicateSettings.SyndicateMin, 0, Syndicate);
        var maxSyn = Mathf.Clamp(SyndicateSettings.SyndicateMax, 0, Syndicate);

        if (minSyn > maxSyn)
            (maxSyn, minSyn) = (minSyn, maxSyn);

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            var maxSSu = Mathf.Clamp(SyndicateSupportSettings.MaxSSu.Value, 0, RoleGenManager.SyndicateSupportRoles.Count);
            var maxSD = Mathf.Clamp(SyndicateDisruptionSettings.MaxSD.Value, 0, RoleGenManager.SyndicateDisruptionRoles.Count);
            var maxSyK = Mathf.Clamp(SyndicateKillingSettings.MaxSyK.Value, 0, RoleGenManager.SyndicateKillingRoles.Count);
            var maxSp = Mathf.Clamp(SyndicateHeadSettings.MaxSh.Value, 0, RoleGenManager.SyndicateHeadRoles.Count);
            var maxSynSum = maxSSu + maxSD + maxSyK + maxSp;

            while (maxSynSum > maxSyn && maxSynSum > 0)
            {
                switch (URandom.RandomRangeInt(0, 4))
                {
                    case 0:
                    {
                        if (maxSSu > 0)
                            maxSSu--;

                        break;
                    }
                    case 1:
                    {
                        if (maxSD > 0)
                            maxSD--;

                        break;
                    }
                    case 2:
                    {
                        if (maxSyK > 0)
                            maxSyK--;

                        break;
                    }
                    case 3:
                    {
                        if (maxSp > 0)
                            maxSp--;

                        break;
                    }
                }

                maxSynSum = maxSSu + maxSD + maxSyK + maxSp;
            }

            filter.Filter(RoleGenManager.SyndicateSupportRoles, maxSSu);
            filter.Filter(RoleGenManager.SyndicateDisruptionRoles, maxSD);
            filter.Filter(RoleGenManager.SyndicateKillingRoles, maxSyK);
            filter.Filter(RoleGenManager.SyndicateHeadRoles, maxSp);
        }

        SyndicateRoles.AddRanges(RoleGenManager.SyndicateSupportRoles, RoleGenManager.SyndicateKillingRoles, RoleGenManager.SyndicateHeadRoles, RoleGenManager.SyndicateDisruptionRoles);

        filter.Filter(SyndicateRoles, GameModeSettings.IgnoreFactionCaps ? Syndicate : URandom.RandomRangeInt(minSyn, maxSyn + 1));

        while (SyndicateRoles.Count < Syndicate)
            SyndicateRoles.Add(GetSpawnItem(LayerEnum.Anarchist));
    }

    public override void PreFilter()
    {
        var filter = ModeFilters[GameModeSettings.GameMode];

        if (BadGuysSettings.IlluminatiUnleashed)
        {
            var type = BadGuysSettings.IlluminatiMembers;

            if (type == IlluminatiType.Syndicate)
                IlluminatiRoles.AddRange(SyndicateRoles);

            if (type == IlluminatiType.Intruders)
                IlluminatiRoles.AddRange(IntruderRoles);

            if (type == IlluminatiType.Apocalypse)
                IlluminatiRoles.AddRange(ApocalypseRoles);

            if (type == IlluminatiType.Killers)
                IlluminatiRoles.AddRange(RoleGenManager.OutcastKillingRoles);

            if (type == IlluminatiType.Neophytes)
                IlluminatiRoles.AddRange(RoleGenManager.OutcastNeophyteRoles);

            var minIll = Mathf.Clamp(IlluminatiSettings.IlluminatiMin, 0, IlluminatiSettings.IlluminatiCount);
            var maxIll = Mathf.Clamp(IlluminatiSettings.IlluminatiMax, 0, IlluminatiSettings.IlluminatiCount);

            if (minIll > maxIll)
                (maxIll, minIll) = (minIll, maxIll);

            filter.Filter(IlluminatiRoles, GameModeSettings.IgnoreFactionCaps ? IlluminatiSettings.IlluminatiCount : URandom.RandomRangeInt(minIll, maxIll + 1));

            while (IlluminatiRoles.Count < IlluminatiSettings.IlluminatiCount)
                IlluminatiRoles.Add(GetSpawnItem(GetRandomBaseEvil(Faction.Illuminati)));

            return;
        }

        if (BadGuysSettings.OrderOfCompliance)
        {
            var type = BadGuysSettings.ComplianceMembers;

            if (type == ComplianceType.Killers)
                ComplianceRoles.AddRange(RoleGenManager.OutcastKillingRoles);

            if (type == ComplianceType.Neophytes)
                ComplianceRoles.AddRange(RoleGenManager.OutcastNeophyteRoles);

            var minComp = Mathf.Clamp(ComplianceSettings.ComplianceMin, 0, ComplianceSettings.ComplianceCount);
            var maxComp = Mathf.Clamp(ComplianceSettings.ComplianceMax, 0, ComplianceSettings.ComplianceCount);

            if (minComp > maxComp)
                (maxComp, minComp) = (minComp, maxComp);

            filter.Filter(ComplianceRoles, GameModeSettings.IgnoreFactionCaps ? ComplianceSettings.ComplianceCount : URandom.RandomRangeInt(minComp, maxComp + 1));

            while (ComplianceRoles.Count < ComplianceSettings.ComplianceCount)
                ComplianceRoles.Add(GetSpawnItem(GetRandomBaseEvil(Faction.Compliance)));
        }

        if (BadGuysSettings.PandoricaOpens)
        {
            var type = BadGuysSettings.PandoricaMembers;

            if (type == PandoricaType.Syndicate)
                PandoricaRoles.AddRange(SyndicateRoles);

            if (type == PandoricaType.Intruders)
                PandoricaRoles.AddRange(IntruderRoles);

            if (type == PandoricaType.Apocalypse)
                PandoricaRoles.AddRange(ApocalypseRoles);

            var minPand = Mathf.Clamp(PandoricaSettings.PandoricaMin, 0, PandoricaSettings.PandoricaCount);
            var maxPand = Mathf.Clamp(PandoricaSettings.PandoricaMax, 0, PandoricaSettings.PandoricaCount);

            if (minPand > maxPand)
                (maxPand, minPand) = (minPand, maxPand);

            filter.Filter(PandoricaRoles, GameModeSettings.IgnoreFactionCaps ? PandoricaSettings.PandoricaCount : URandom.RandomRangeInt(minPand, maxPand + 1));

            while (PandoricaRoles.Count < PandoricaSettings.PandoricaCount)
                PandoricaRoles.Add(GetSpawnItem(GetRandomBaseEvil(Faction.Pandorica)));
        }
    }
}