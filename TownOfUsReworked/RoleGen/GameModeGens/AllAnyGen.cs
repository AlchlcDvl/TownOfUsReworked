using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public class AllAnyGen : BaseClassicCustomAllAnyGen
{
    public override void Filter()
    {
        CrewRoles.AddRanges(RoleGenManager.CrewAuditorRoles, RoleGenManager.CrewInvestigativeRoles, RoleGenManager.CrewKillingRoles, RoleGenManager.CrewSupportRoles,
            RoleGenManager.CrewProtectiveRoles, RoleGenManager.CrewSovereignRoles);
        IntruderRoles.AddRanges(RoleGenManager.IntruderConcealingRoles, RoleGenManager.IntruderDeceptionRoles, RoleGenManager.IntruderKillingRoles, RoleGenManager.IntruderSupportRoles,
            RoleGenManager.IntruderHeadRoles);
        SyndicateRoles.AddRanges(RoleGenManager.SyndicateSupportRoles, RoleGenManager.SyndicateKillingRoles, RoleGenManager.SyndicatePowerRoles, RoleGenManager.SyndicateDisruptionRoles);
        NeutralRoles.AddRanges(RoleGenManager.NeutralBenignRoles, RoleGenManager.NeutralEvilRoles, RoleGenManager.NeutralKillingRoles, RoleGenManager.NeutralNeophyteRoles,
            RoleGenManager.NeutralHarbingerRoles);

        RoleGenManager.CrewAuditorRoles.Clear();
        RoleGenManager.CrewInvestigativeRoles.Clear();
        RoleGenManager.CrewSupportRoles.Clear();
        RoleGenManager.CrewProtectiveRoles.Clear();
        RoleGenManager.CrewSovereignRoles.Clear();
        RoleGenManager.CrewKillingRoles.Clear();

        RoleGenManager.IntruderConcealingRoles.Clear();
        RoleGenManager.IntruderDeceptionRoles.Clear();
        RoleGenManager.IntruderKillingRoles.Clear();
        RoleGenManager.IntruderSupportRoles.Clear();
        RoleGenManager.IntruderHeadRoles.Clear();

        RoleGenManager.SyndicateSupportRoles.Clear();
        RoleGenManager.SyndicateKillingRoles.Clear();
        RoleGenManager.SyndicatePowerRoles.Clear();
        RoleGenManager.SyndicateDisruptionRoles.Clear();

        RoleGenManager.NeutralBenignRoles.Clear();
        RoleGenManager.NeutralEvilRoles.Clear();
        RoleGenManager.NeutralKillingRoles.Clear();
        RoleGenManager.NeutralNeophyteRoles.Clear();
        RoleGenManager.NeutralHarbingerRoles.Clear();

        base.Filter();
    }

    public override void GetAdjustedFactions()
    {
        var players = GameData.Instance.PlayerCount;
        Intruders = GetRandomCount();
        Syndicate = GetRandomCount();

        if (Intruders == 0 && Syndicate == 0)
        {
            var random = URandom.RandomRangeInt(0, 2);

            if (random == 0)
                Intruders++;
            else if (random == 1)
                Syndicate++;
        }

        Neutrals = URandom.RandomRangeInt(0, players - Intruders - Syndicate);
        Crew = players - Intruders - Syndicate - Neutrals;

        while (Crew == 0 && (Intruders > 0 || Syndicate > 0 || Neutrals > 0) && players > 1)
        {
            var random2 = URandom.RandomRangeInt(0, 3);

            if (random2 == 0 && Intruders > 0)
            {
                Intruders--;
                Crew++;
            }
            else if (random2 == 1 && Syndicate > 0)
            {
                Syndicate--;
                Crew++;
            }
            else if (random2 == 2 && Neutrals > 0)
            {
                Neutrals--;
                Crew++;
            }
        }

        if (TownOfUsReworked.IsTest)
            Info($"Crew = {Crew}, Int = {Intruders}, Syn = {Syndicate}, Neut = {Neutrals}");
    }
}