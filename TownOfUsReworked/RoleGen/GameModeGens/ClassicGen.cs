using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class ClassicGen : BaseClassicAllAnyGen
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
            int maxCi = CrewInvestigativeSettings.MaxCi;
            int maxCs = CrewSupportSettings.MaxCs;
            int maxCk = CrewKillingSettings.MaxCk;
            int maxCrP = CrewProtectiveSettings.MaxCrP;
            int maxCSv = CrewSovereignSettings.MaxCSv;

            if (maxCi > RoleGenManager.CrewInvestigativeRoles.Count)
                maxCi = RoleGenManager.CrewInvestigativeRoles.Count;

            if (maxCs > RoleGenManager.CrewSupportRoles.Count)
                maxCs = RoleGenManager.CrewSupportRoles.Count;

            if (maxCk > RoleGenManager.CrewKillingRoles.Count)
                maxCk = RoleGenManager.CrewKillingRoles.Count;

            if (maxCrP > RoleGenManager.CrewProtectiveRoles.Count)
                maxCrP = RoleGenManager.CrewProtectiveRoles.Count;

            if (maxCSv > RoleGenManager.CrewSovereignRoles.Count)
                maxCSv = RoleGenManager.CrewSovereignRoles.Count;

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

        RoleGenManager.CrewInvestigativeRoles.Clear();
        RoleGenManager.CrewSupportRoles.Clear();
        RoleGenManager.CrewProtectiveRoles.Clear();
        RoleGenManager.CrewSovereignRoles.Clear();
        RoleGenManager.CrewKillingRoles.Clear();
    }

    public override void InitIntList()
    {
        base.InitIntList();

        if ((BadGuysSettings.OnlyMainBadGuys && BadGuysSettings.MainBadGuys is not (Faction.Intruder or Faction.Pandorica or Faction.Illuminati)) || Intruders == 0)
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
            int maxIc = IntruderConcealingSettings.MaxIc;
            int maxID = IntruderDeceptionSettings.MaxID;
            int maxIK = IntruderKillingSettings.MaxIK;
            int maxIs = IntruderSupportSettings.MaxIs;
            int maxIh = IntruderHeadSettings.MaxIh;

            if (maxIc > RoleGenManager.IntruderConcealingRoles.Count)
                maxIc = RoleGenManager.IntruderConcealingRoles.Count;

            if (maxID > RoleGenManager.IntruderDeceptionRoles.Count)
                maxID = RoleGenManager.IntruderDeceptionRoles.Count;

            if (maxIK > RoleGenManager.IntruderKillingRoles.Count)
                maxIK = RoleGenManager.IntruderKillingRoles.Count;

            if (maxIs > RoleGenManager.IntruderSupportRoles.Count)
                maxIs = RoleGenManager.IntruderSupportRoles.Count;

            if (maxIh > RoleGenManager.IntruderHeadRoles.Count)
                maxIh = RoleGenManager.IntruderHeadRoles.Count;

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
            int maxNe = NeutralEvilSettings.MaxNe;
            int maxNb = NeutralBenignSettings.MaxNb;
            int maxNk = NeutralKillingSettings.MaxNk;
            int maxNn = NeutralNeophyteSettings.MaxNn;

            if (maxNe > RoleGenManager.NeutralEvilRoles.Count)
                maxNe = RoleGenManager.NeutralEvilRoles.Count;

            if (maxNb > RoleGenManager.NeutralBenignRoles.Count)
                maxNb = RoleGenManager.NeutralBenignRoles.Count;

            if (maxNk > RoleGenManager.NeutralKillingRoles.Count)
                maxNk = RoleGenManager.NeutralKillingRoles.Count;

            if (maxNn > RoleGenManager.NeutralNeophyteRoles.Count)
                maxNn = RoleGenManager.NeutralNeophyteRoles.Count;

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

            filter.Filter(RoleGenManager.NeutralBenignRoles, maxNb);
            filter.Filter(RoleGenManager.NeutralEvilRoles, maxNe);
            filter.Filter(RoleGenManager.NeutralKillingRoles, maxNk);
            filter.Filter(RoleGenManager.NeutralNeophyteRoles, maxNn);
        }

        NeutralRoles.AddRanges(RoleGenManager.NeutralBenignRoles, RoleGenManager.NeutralEvilRoles, RoleGenManager.NeutralKillingRoles, RoleGenManager.NeutralNeophyteRoles);

        filter.Filter(NeutralRoles, GameModeSettings.IgnoreFactionCaps ? Neutrals : URandom.RandomRangeInt(minNeut, maxNeut + 1));

        while (NeutralRoles.Count < Neutrals)
            NeutralRoles.Add(GetSpawnItem(LayerEnum.Amnesiac));

        RoleGenManager.NeutralBenignRoles.Clear();
        RoleGenManager.NeutralEvilRoles.Clear();
        RoleGenManager.NeutralKillingRoles.Clear();
        RoleGenManager.NeutralNeophyteRoles.Clear();
    }

    public override void InitApocList()
    {
        base.InitApocList();

        if ((BadGuysSettings.OnlyMainBadGuys && BadGuysSettings.MainBadGuys is not (Faction.Apocalypse or Faction.Pandorica or Faction.Illuminati)) || Apocalypse == 0)
            return;

        int minApoc = ApocalypseSettings.ApocalypseMin;
        int maxApoc = ApocalypseSettings.ApocalypseMax;

        if (minApoc > maxApoc)
            (maxApoc, minApoc) = (minApoc, maxApoc);

        while (maxApoc > Apocalypse)
            maxApoc--;

        while (minApoc > Apocalypse)
            minApoc--;

        var filter = ModeFilters[GameModeSettings.GameMode];

        if (!GameModeSettings.IgnoreAlignmentCaps)
        {
            int maxAh = ApocalypseHarbingerSettings.MaxAh;

            if (maxAh > RoleGenManager.ApocalypseHarbingerRoles.Count)
                maxAh = RoleGenManager.ApocalypseHarbingerRoles.Count;

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

        RoleGenManager.ApocalypseHarbingerRoles.Clear();
    }

    public override void InitSynList()
    {
        base.InitSynList();

        if ((BadGuysSettings.OnlyMainBadGuys && BadGuysSettings.MainBadGuys is not (Faction.Syndicate or Faction.Pandorica or Faction.Illuminati)) || Syndicate == 0)
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
            int maxSp = SyndicatePowerSettings.MaxSp;

            if (maxSSu > RoleGenManager.SyndicateSupportRoles.Count)
                maxSSu = RoleGenManager.SyndicateSupportRoles.Count;

            if (maxSD > RoleGenManager.SyndicateDisruptionRoles.Count)
                maxSD = RoleGenManager.SyndicateDisruptionRoles.Count;

            if (maxSyK > RoleGenManager.SyndicateKillingRoles.Count)
                maxSyK = RoleGenManager.SyndicateKillingRoles.Count;

            if (maxSp > RoleGenManager.SyndicatePowerRoles.Count)
                maxSp = RoleGenManager.SyndicatePowerRoles.Count;

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
            filter.Filter(RoleGenManager.SyndicatePowerRoles, maxSp);
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