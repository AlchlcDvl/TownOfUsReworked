using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public class ClassicCustomGen : BaseClassicCustomAllAnyGen
{
    public override void InitCrewList()
    {
        base.InitCrewList();

        if (Crew == 0)
            return;

        int maxCI = CrewInvestigativeSettings.MaxCI;
        int maxCS = CrewSupportSettings.MaxCS;
        int maxCA = CrewAuditorSettings.MaxCA;
        int maxCK = CrewKillingSettings.MaxCK;
        int maxCrP = CrewProtectiveSettings.MaxCrP;
        int maxCSv = CrewSovereignSettings.MaxCSv;
        int minCrew = CrewSettings.CrewMin;
        int maxCrew = CrewSettings.CrewMax;

        if (minCrew > maxCrew)
            (maxCrew, minCrew) = (minCrew, maxCrew);

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

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            filter.Filter(RoleGenManager.CrewAuditorRoles, maxCA);
            filter.Filter(RoleGenManager.CrewInvestigativeRoles, maxCI);
            filter.Filter(RoleGenManager.CrewKillingRoles, maxCK);
            filter.Filter(RoleGenManager.CrewProtectiveRoles, maxCrP);
            filter.Filter(RoleGenManager.CrewSupportRoles, maxCS);
            filter.Filter(RoleGenManager.CrewSovereignRoles, maxCSv);
        }

        CrewRoles.AddRanges(RoleGenManager.CrewAuditorRoles, RoleGenManager.CrewInvestigativeRoles, RoleGenManager.CrewKillingRoles, RoleGenManager.CrewSupportRoles,
            RoleGenManager.CrewProtectiveRoles, RoleGenManager.CrewSovereignRoles);

        while (maxCrew > Crew)
            maxCrew--;

        while (minCrew > Crew)
            minCrew--;

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

        int maxIC = IntruderConcealingSettings.MaxIC;
        int maxID = IntruderDeceptionSettings.MaxID;
        int maxIK = IntruderKillingSettings.MaxIK;
        int maxIS = IntruderSupportSettings.MaxIS;
        int maxIH = IntruderHeadSettings.MaxIH;
        int minInt = IntruderSettings.IntruderMin;
        int maxInt = IntruderSettings.IntruderMax;

        if (minInt > maxInt)
            (maxInt, minInt) = (minInt, maxInt);

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

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            filter.Filter(RoleGenManager.IntruderConcealingRoles, maxIC);
            filter.Filter(RoleGenManager.IntruderDeceptionRoles, maxID);
            filter.Filter(RoleGenManager.IntruderKillingRoles, maxIK);
            filter.Filter(RoleGenManager.IntruderSupportRoles, maxIS);
            filter.Filter(RoleGenManager.IntruderHeadRoles, maxIH);
        }

        IntruderRoles.AddRanges(RoleGenManager.IntruderConcealingRoles, RoleGenManager.IntruderDeceptionRoles, RoleGenManager.IntruderKillingRoles, RoleGenManager.IntruderSupportRoles,
            RoleGenManager.IntruderHeadRoles);

        while (maxInt > Intruders)
            maxInt--;

        while (minInt > Intruders)
            minInt--;

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

        int maxNE = NeutralEvilSettings.MaxNE;
        int maxNB = NeutralBenignSettings.MaxNB;
        int maxNK = NeutralKillingSettings.MaxNK;
        int maxNN = NeutralNeophyteSettings.MaxNN;
        int maxNH = NeutralHarbingerSettings.MaxNH;
        int minNeut = NeutralSettings.NeutralMin;
        int maxNeut = NeutralSettings.NeutralMax;

        if (minNeut > maxNeut)
            (maxNeut, minNeut) = (minNeut, maxNeut);

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

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            filter.Filter(RoleGenManager.NeutralBenignRoles, maxNB);
            filter.Filter(RoleGenManager.NeutralEvilRoles, maxNE);
            filter.Filter(RoleGenManager.NeutralKillingRoles, maxNK);
            filter.Filter(RoleGenManager.NeutralNeophyteRoles, maxNN);
            filter.Filter(RoleGenManager.NeutralHarbingerRoles, maxNH);
        }

        NeutralRoles.AddRanges(RoleGenManager.NeutralBenignRoles, RoleGenManager.NeutralEvilRoles, RoleGenManager.NeutralKillingRoles, RoleGenManager.NeutralNeophyteRoles,
            RoleGenManager.NeutralHarbingerRoles);

        while (maxNeut > Neutrals)
            maxNeut--;

        while (minNeut > Neutrals)
            minNeut--;

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

        int maxSSu = SyndicateSupportSettings.MaxSSu;
        int maxSD = SyndicateDisruptionSettings.MaxSD;
        int maxSyK = SyndicateKillingSettings.MaxSyK;
        int maxSP = SyndicatePowerSettings.MaxSP;
        int minSyn = SyndicateSettings.SyndicateMin;
        int maxSyn = SyndicateSettings.SyndicateMax;

        if (minSyn > maxSyn)
            (maxSyn, minSyn) = (minSyn, maxSyn);

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

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            filter.Filter(RoleGenManager.SyndicateSupportRoles, maxSSu);
            filter.Filter(RoleGenManager.SyndicateDisruptionRoles, maxSD);
            filter.Filter(RoleGenManager.SyndicateKillingRoles, maxSyK);
            filter.Filter(RoleGenManager.SyndicatePowerRoles, maxSP);
        }

        SyndicateRoles.AddRanges(RoleGenManager.SyndicateSupportRoles, RoleGenManager.SyndicateKillingRoles, RoleGenManager.SyndicatePowerRoles, RoleGenManager.SyndicateDisruptionRoles);

        while (maxSyn > Syndicate)
            maxSyn--;

        while (minSyn > Syndicate)
            minSyn--;

        filter.Filter(SyndicateRoles, GameModeSettings.IgnoreFactionCaps ? Syndicate : URandom.RandomRangeInt(minSyn, maxSyn + 1));

        while (SyndicateRoles.Count < Syndicate)
            SyndicateRoles.Add(GetSpawnItem(LayerEnum.Anarchist));

        RoleGenManager.SyndicateSupportRoles.Clear();
        RoleGenManager.SyndicateKillingRoles.Clear();
        RoleGenManager.SyndicatePowerRoles.Clear();
        RoleGenManager.SyndicateDisruptionRoles.Clear();
    }
}