using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public class ClassicCustomGen : BaseClassicCustomAllAnyGen
{
    public override void InitCrewList()
    {
        base.InitCrewList();

        if (Crew == 0)
            return;

        int minCrew = CrewSettings.CrewMin;
        int maxCrew = CrewSettings.CrewMax;

        if (minCrew > maxCrew)
            (maxCrew, minCrew) = (minCrew, maxCrew);

        while (maxCrew > Crew)
            maxCrew--;

        while (minCrew > Crew)
            minCrew--;

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            int maxCI = CrewInvestigativeSettings.MaxCI;
            int maxCS = CrewSupportSettings.MaxCS;
            int maxCA = CrewAuditorSettings.MaxCA;
            int maxCK = CrewKillingSettings.MaxCK;
            int maxCrP = CrewProtectiveSettings.MaxCrP;
            int maxCSv = CrewSovereignSettings.MaxCSv;

            if (maxCI > RoleGenManager.CrewInvestigativeRoles.Count)
                maxCI = RoleGenManager.CrewInvestigativeRoles.Count;

            if (maxCS > RoleGenManager.CrewSupportRoles.Count)
                maxCS = RoleGenManager.CrewSupportRoles.Count;

            if (maxCA > RoleGenManager.CrewAuditorRoles.Count)
                maxCA = RoleGenManager.CrewAuditorRoles.Count;

            if (maxCK > RoleGenManager.CrewKillingRoles.Count)
                maxCK = RoleGenManager.CrewKillingRoles.Count;

            if (maxCrP > RoleGenManager.CrewProtectiveRoles.Count)
                maxCrP = RoleGenManager.CrewProtectiveRoles.Count;

            if (maxCSv > RoleGenManager.CrewSovereignRoles.Count)
                maxCSv = RoleGenManager.CrewSovereignRoles.Count;

            var maxCrewSum = maxCA + maxCI + maxCK + maxCrP + maxCS + maxCSv;

            while (maxCrewSum > maxCrew)
            {
                switch (URandom.RandomRangeInt(0, 6))
                {
                    case 0:
                    {
                        if (maxCA > 0)
                            maxCA--;

                        break;
                    }
                    case 1:
                    {
                        if (maxCI > 0)
                            maxCI--;

                        break;
                    }
                    case 2:
                    {
                        if (maxCK > 0)
                            maxCK--;

                        break;
                    }
                    case 3:
                    {
                        if (maxCS > 0)
                            maxCS--;

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

                maxCrewSum = maxCA + maxCI + maxCK + maxCrP + maxCS + maxCSv;
            }

            filter.Filter(RoleGenManager.CrewAuditorRoles, maxCA);
            filter.Filter(RoleGenManager.CrewInvestigativeRoles, maxCI);
            filter.Filter(RoleGenManager.CrewKillingRoles, maxCK);
            filter.Filter(RoleGenManager.CrewProtectiveRoles, maxCrP);
            filter.Filter(RoleGenManager.CrewSupportRoles, maxCS);
            filter.Filter(RoleGenManager.CrewSovereignRoles, maxCSv);
        }

        CrewRoles.AddRanges(RoleGenManager.CrewAuditorRoles, RoleGenManager.CrewInvestigativeRoles, RoleGenManager.CrewKillingRoles, RoleGenManager.CrewSupportRoles,
            RoleGenManager.CrewProtectiveRoles, RoleGenManager.CrewSovereignRoles);

        filter.Filter(CrewRoles, GameModeSettings.IgnoreFactionCaps ? Crew : URandom.RandomRangeInt(minCrew, maxCrew + 1));

        while (CrewRoles.Count < Crew)
            CrewRoles.Add(GetSpawnItem(LayerEnum.Crewmate));

        RoleGenManager.CrewAuditorRoles.Clear();
        RoleGenManager.CrewInvestigativeRoles.Clear();
        RoleGenManager.CrewSupportRoles.Clear();
        RoleGenManager.CrewProtectiveRoles.Clear();
        RoleGenManager.CrewSovereignRoles.Clear();
        RoleGenManager.CrewKillingRoles.Clear();
    }

    public override void InitIntList()
    {
        base.InitIntList();

        if (SyndicateSettings.AltImps || Intruders == 0)
            return;

        int minInt = IntruderSettings.IntruderMin;
        int maxInt = IntruderSettings.IntruderMax;

        if (minInt > maxInt)
            (maxInt, minInt) = (minInt, maxInt);

        while (maxInt > Intruders)
            maxInt--;

        while (minInt > Intruders)
            minInt--;

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            int maxIC = IntruderConcealingSettings.MaxIC;
            int maxID = IntruderDeceptionSettings.MaxID;
            int maxIK = IntruderKillingSettings.MaxIK;
            int maxIS = IntruderSupportSettings.MaxIS;
            int maxIH = IntruderHeadSettings.MaxIH;

            if (maxIC > RoleGenManager.IntruderConcealingRoles.Count)
                maxIC = RoleGenManager.IntruderConcealingRoles.Count;

            if (maxID > RoleGenManager.IntruderDeceptionRoles.Count)
                maxID = RoleGenManager.IntruderDeceptionRoles.Count;

            if (maxIK > RoleGenManager.IntruderKillingRoles.Count)
                maxIK = RoleGenManager.IntruderKillingRoles.Count;

            if (maxIS > RoleGenManager.IntruderSupportRoles.Count)
                maxIS = RoleGenManager.IntruderSupportRoles.Count;

            if (maxIH > RoleGenManager.IntruderHeadRoles.Count)
                maxIH = RoleGenManager.IntruderHeadRoles.Count;

            var maxIntSum = maxIC + maxID + maxIK + maxIS + maxIH;

            while (maxIntSum > maxInt)
            {
                switch (URandom.RandomRangeInt(0, 5))
                {
                    case 0:
                    {
                        if (maxIC > 0)
                            maxIC--;

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
                        if (maxIS > 0)
                            maxIS--;

                        break;
                    }
                    case 4:
                    {
                        if (maxIH > 0)
                            maxIH--;

                        break;
                    }
                }

                maxIntSum = maxIC + maxID + maxIK + maxIS + maxIH;
            }

            filter.Filter(RoleGenManager.IntruderConcealingRoles, maxIC);
            filter.Filter(RoleGenManager.IntruderDeceptionRoles, maxID);
            filter.Filter(RoleGenManager.IntruderKillingRoles, maxIK);
            filter.Filter(RoleGenManager.IntruderSupportRoles, maxIS);
            filter.Filter(RoleGenManager.IntruderHeadRoles, maxIH);
        }

        IntruderRoles.AddRanges(RoleGenManager.IntruderConcealingRoles, RoleGenManager.IntruderDeceptionRoles, RoleGenManager.IntruderKillingRoles, RoleGenManager.IntruderSupportRoles,
            RoleGenManager.IntruderHeadRoles);

        filter.Filter(IntruderRoles, GameModeSettings.IgnoreFactionCaps ? Intruders : URandom.RandomRangeInt(minInt, maxInt + 1));

        while (IntruderRoles.Count < Intruders)
            IntruderRoles.Add(GetSpawnItem(LayerEnum.Impostor));

        RoleGenManager.IntruderConcealingRoles.Clear();
        RoleGenManager.IntruderDeceptionRoles.Clear();
        RoleGenManager.IntruderKillingRoles.Clear();
        RoleGenManager.IntruderSupportRoles.Clear();
        RoleGenManager.IntruderHeadRoles.Clear();
    }

    public override void InitNeutList()
    {
        base.InitNeutList();

        if (Neutrals == 0)
            return;

        int minNeut = NeutralSettings.NeutralMin;
        int maxNeut = NeutralSettings.NeutralMax;

        if (minNeut > maxNeut)
            (maxNeut, minNeut) = (minNeut, maxNeut);

        while (maxNeut > Neutrals)
            maxNeut--;

        while (minNeut > Neutrals)
            minNeut--;

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            int maxNE = NeutralEvilSettings.MaxNE;
            int maxNB = NeutralBenignSettings.MaxNB;
            int maxNK = NeutralKillingSettings.MaxNK;
            int maxNN = NeutralNeophyteSettings.MaxNN;
            int maxNH = NeutralHarbingerSettings.MaxNH;

            if (maxNE > RoleGenManager.NeutralEvilRoles.Count)
                maxNE = RoleGenManager.NeutralEvilRoles.Count;

            if (maxNB > RoleGenManager.NeutralBenignRoles.Count)
                maxNB = RoleGenManager.NeutralBenignRoles.Count;

            if (maxNK > RoleGenManager.NeutralKillingRoles.Count)
                maxNK = RoleGenManager.NeutralKillingRoles.Count;

            if (maxNN > RoleGenManager.NeutralNeophyteRoles.Count)
                maxNN = RoleGenManager.NeutralNeophyteRoles.Count;

            if (maxNH > RoleGenManager.NeutralHarbingerRoles.Count)
                maxNH = RoleGenManager.NeutralHarbingerRoles.Count;

            var maxNeutSum = maxNE + maxNB + maxNK + maxNN + maxNH;

            while (maxNeutSum > maxNeut)
            {
                switch (URandom.RandomRangeInt(0, 5))
                {
                    case 0:
                    {
                        if (maxNE > 0)
                            maxNE--;

                        break;
                    }
                    case 1:
                    {
                        if (maxNB > 0)
                            maxNB--;

                        break;
                    }
                    case 2:
                    {
                        if (maxNK > 0)
                            maxNK--;

                        break;
                    }
                    case 3:
                    {
                        if (maxNN > 0)
                            maxNN--;

                        break;
                    }
                    case 4:
                    {
                        if (maxNH > 0)
                            maxNH--;

                        break;
                    }
                }

                maxNeutSum = maxNE + maxNB + maxNK + maxNN + maxNH;
            }

            filter.Filter(RoleGenManager.NeutralBenignRoles, maxNB);
            filter.Filter(RoleGenManager.NeutralEvilRoles, maxNE);
            filter.Filter(RoleGenManager.NeutralKillingRoles, maxNK);
            filter.Filter(RoleGenManager.NeutralNeophyteRoles, maxNN);
            filter.Filter(RoleGenManager.NeutralHarbingerRoles, maxNH);
        }

        NeutralRoles.AddRanges(RoleGenManager.NeutralBenignRoles, RoleGenManager.NeutralEvilRoles, RoleGenManager.NeutralKillingRoles, RoleGenManager.NeutralNeophyteRoles,
            RoleGenManager.NeutralHarbingerRoles);

        filter.Filter(NeutralRoles, GameModeSettings.IgnoreFactionCaps ? Neutrals : URandom.RandomRangeInt(minNeut, maxNeut + 1));

        while (NeutralRoles.Count < Neutrals)
            NeutralRoles.Add(GetSpawnItem(LayerEnum.Amnesiac));

        RoleGenManager.NeutralBenignRoles.Clear();
        RoleGenManager.NeutralEvilRoles.Clear();
        RoleGenManager.NeutralKillingRoles.Clear();
        RoleGenManager.NeutralNeophyteRoles.Clear();
        RoleGenManager.NeutralHarbingerRoles.Clear();
    }

    public override void InitSynList()
    {
        base.InitSynList();

        if (Syndicate == 0)
            return;

        int minSyn = SyndicateSettings.SyndicateMin;
        int maxSyn = SyndicateSettings.SyndicateMax;

        if (minSyn > maxSyn)
            (maxSyn, minSyn) = (minSyn, maxSyn);

        while (maxSyn > Syndicate)
            maxSyn--;

        while (minSyn > Syndicate)
            minSyn--;

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            int maxSSu = SyndicateSupportSettings.MaxSSu;
            int maxSD = SyndicateDisruptionSettings.MaxSD;
            int maxSyK = SyndicateKillingSettings.MaxSyK;
            int maxSP = SyndicatePowerSettings.MaxSP;

            if (maxSSu > RoleGenManager.SyndicateSupportRoles.Count)
                maxSSu = RoleGenManager.SyndicateSupportRoles.Count;

            if (maxSD > RoleGenManager.SyndicateDisruptionRoles.Count)
                maxSD = RoleGenManager.SyndicateDisruptionRoles.Count;

            if (maxSyK > RoleGenManager.SyndicateKillingRoles.Count)
                maxSyK = RoleGenManager.SyndicateKillingRoles.Count;

            if (maxSP > RoleGenManager.SyndicatePowerRoles.Count)
                maxSP = RoleGenManager.SyndicatePowerRoles.Count;

            var maxSynSum = maxSSu + maxSD + maxSyK + maxSP;

            while (maxSynSum > maxSyn)
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
                        if (maxSP > 0)
                            maxSP--;

                        break;
                    }
                }

                maxSynSum = maxSSu + maxSD + maxSyK + maxSP;
            }

            filter.Filter(RoleGenManager.SyndicateSupportRoles, maxSSu);
            filter.Filter(RoleGenManager.SyndicateDisruptionRoles, maxSD);
            filter.Filter(RoleGenManager.SyndicateKillingRoles, maxSyK);
            filter.Filter(RoleGenManager.SyndicatePowerRoles, maxSP);
        }

        SyndicateRoles.AddRanges(RoleGenManager.SyndicateSupportRoles, RoleGenManager.SyndicateKillingRoles, RoleGenManager.SyndicatePowerRoles, RoleGenManager.SyndicateDisruptionRoles);

        filter.Filter(SyndicateRoles, GameModeSettings.IgnoreFactionCaps ? Syndicate : URandom.RandomRangeInt(minSyn, maxSyn + 1));

        while (SyndicateRoles.Count < Syndicate)
            SyndicateRoles.Add(GetSpawnItem(LayerEnum.Anarchist));

        RoleGenManager.SyndicateSupportRoles.Clear();
        RoleGenManager.SyndicateKillingRoles.Clear();
        RoleGenManager.SyndicatePowerRoles.Clear();
        RoleGenManager.SyndicateDisruptionRoles.Clear();
    }
}