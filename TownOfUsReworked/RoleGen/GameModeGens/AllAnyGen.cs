using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class AllAnyGen : BaseClassicAllAnyGen
{
    public override void Filter()
    {
        CrewRoles.AddRanges(RoleGenManager.CrewInvestigativeRoles, RoleGenManager.CrewKillingRoles, RoleGenManager.CrewSupportRoles, RoleGenManager.CrewProtectiveRoles,
            RoleGenManager.CrewSovereignRoles);
        IntruderRoles.AddRanges(RoleGenManager.IntruderConcealingRoles, RoleGenManager.IntruderDeceptionRoles, RoleGenManager.IntruderKillingRoles, RoleGenManager.IntruderSupportRoles,
            RoleGenManager.IntruderHeadRoles);
        SyndicateRoles.AddRanges(RoleGenManager.SyndicateSupportRoles, RoleGenManager.SyndicateKillingRoles, RoleGenManager.SyndicatePowerRoles, RoleGenManager.SyndicateDisruptionRoles);
        NeutralRoles.AddRanges(RoleGenManager.NeutralBenignRoles, RoleGenManager.NeutralEvilRoles, RoleGenManager.NeutralKillingRoles, RoleGenManager.NeutralNeophyteRoles,
            RoleGenManager.ApocalypseHarbingerRoles);
        ApocalypseRoles.AddRange(RoleGenManager.ApocalypseHarbingerRoles);

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

        RoleGenManager.ApocalypseHarbingerRoles.Clear();

        base.Filter();
    }

    protected override void GetAdjustedFactions()
    {
        var players = GameData.Instance.PlayerCount;
        Intruders = GetRandomCount();
        Syndicate = GetRandomCount();
        Apocalypse = GetRandomCount();

        if (Intruders == 0 && Syndicate == 0 && Apocalypse == 0)
        {
            _ = URandom.RandomRangeInt(0, 3) switch
            {
                0 => Intruders++,
                1 => Apocalypse++,
                _ => Syndicate++
            };
        }

        Neutrals = URandom.RandomRangeInt(0, players - Intruders - Syndicate - Apocalypse + 1);
        Crew = players - Intruders - Syndicate - Neutrals;

        while (Crew == 0 && (Intruders > 0 || Syndicate > 0 || Neutrals > 0 || Apocalypse > 0) && players > 1)
        {
            _ = URandom.RandomRangeInt(0, 4) switch
            {
                0 when Intruders > 0 => Intruders--,
                1 when Apocalypse > 0 => Apocalypse--,
                2 when Syndicate > 0 => Syndicate--,
                3 when Neutrals > 0 => Neutrals--,
                _ => 0
            };

            Crew++;
        }

        if (TownOfUsReworked.MciActive)
            Info($"Crew = {Crew}, Int = {Intruders}, Syn = {Syndicate}, Neut = {Neutrals}, Apoc = {Apocalypse}");
    }

    private static int GetRandomCount()
    {
        var random = URandom.RandomRangeInt(0, 100);
        return GameData.Instance.PlayerCount switch
        {
            <= 6 => random <= 5 ? 0 : 1,
            7 => random switch
            {
                < 5 => 0,
                < 20 => 2,
                _ => 1
            },
            8 => random switch
            {
                < 5 => 0,
                < 40 => 2,
                _ => 1
            },
            9 => random switch
            {
                < 5 => 0,
                < 50 => 2,
                _ => 1
            },
            10 => random switch
            {
                < 5 => 0,
                < 10 => 3,
                < 60 => 2,
                _ => 1
            },
            11 => random switch
            {
                < 5 => 0,
                < 40 => 3,
                < 70 => 2,
                _ => 1
            },
            12 => random switch
            {
                < 5 => 0,
                < 60 => 3,
                < 80 => 2,
                _ => 1
            },
            13 => random switch
            {
                < 5 => 0,
                < 60 => 3,
                < 90 => 2,
                _ => 1
            },
            14 => random switch
            {
                < 5 => 0,
                < 25 => 1,
                < 60 => 3,
                _ => 2
            },
            _ => random switch
            {
                < 5 => 0,
                < 20 => 1,
                < 60 => 3,
                < 90 => 2,
                _ => 4
            }
        };
    }
}