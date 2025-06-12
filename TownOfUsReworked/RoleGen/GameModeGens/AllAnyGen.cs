using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class AllAnyGen : BaseClassicAllAnyGen
{
    public override void PreFilter()
    {
        CrewRoles.AddRanges(RoleGenManager.CrewInvestigativeRoles, RoleGenManager.CrewKillingRoles, RoleGenManager.CrewSupportRoles, RoleGenManager.CrewProtectiveRoles,
            RoleGenManager.CrewSovereignRoles);
        OutcastRoles.AddRanges(RoleGenManager.OutcastBenignRoles, RoleGenManager.OutcastEvilRoles);

        if (BadGuysSettings.IlluminatiUnleashed)
        {
            var type = BadGuysSettings.IlluminatiMembers;
            (type == IlluminatiType.Killers ? IlluminatiRoles : OutcastRoles).AddRange(RoleGenManager.OutcastKillingRoles);
            (type == IlluminatiType.Neophytes ? IlluminatiRoles : OutcastRoles).AddRange(RoleGenManager.OutcastNeophyteRoles);
            (type == IlluminatiType.Apocalypse ? IlluminatiRoles : ApocalypseRoles).AddRanges(RoleGenManager.ApocalypseHarbingerRoles);
            (type == IlluminatiType.Syndicate ? IlluminatiRoles : SyndicateRoles).AddRanges(RoleGenManager.SyndicateSupportRoles, RoleGenManager.SyndicateKillingRoles,
                RoleGenManager.SyndicateHeadRoles, RoleGenManager.SyndicateDisruptionRoles);
            (type == IlluminatiType.Intruders ? IlluminatiRoles : IntruderRoles).AddRanges(RoleGenManager.IntruderConcealingRoles, RoleGenManager.IntruderDeceptionRoles,
                RoleGenManager.IntruderKillingRoles, RoleGenManager.IntruderSupportRoles, RoleGenManager.IntruderHeadRoles);
            return;
        }

        if (BadGuysSettings.OrderOfCompliance)
        {
            var type = BadGuysSettings.ComplianceMembers;
            (type == ComplianceType.Killers ? ComplianceRoles : OutcastRoles).AddRange(RoleGenManager.OutcastKillingRoles);
            (type == ComplianceType.Neophytes ? ComplianceRoles : OutcastRoles).AddRange(RoleGenManager.OutcastNeophyteRoles);
        }
        else
            OutcastRoles.AddRanges(RoleGenManager.OutcastKillingRoles, RoleGenManager.OutcastNeophyteRoles);

        if (BadGuysSettings.PandoricaOpens)
        {
            var type = BadGuysSettings.PandoricaMembers;
            (type == PandoricaType.Apocalypse ? PandoricaRoles : ApocalypseRoles).AddRanges(RoleGenManager.ApocalypseHarbingerRoles);
            (type == PandoricaType.Syndicate ? PandoricaRoles : SyndicateRoles).AddRanges(RoleGenManager.SyndicateSupportRoles, RoleGenManager.SyndicateKillingRoles,
                RoleGenManager.SyndicateHeadRoles, RoleGenManager.SyndicateDisruptionRoles);
            (type == PandoricaType.Intruders ? PandoricaRoles : IntruderRoles).AddRanges(RoleGenManager.IntruderConcealingRoles, RoleGenManager.IntruderDeceptionRoles,
                RoleGenManager.IntruderKillingRoles, RoleGenManager.IntruderSupportRoles, RoleGenManager.IntruderHeadRoles);
        }
        else
        {
            IntruderRoles.AddRanges(RoleGenManager.IntruderConcealingRoles, RoleGenManager.IntruderDeceptionRoles, RoleGenManager.IntruderKillingRoles, RoleGenManager.IntruderSupportRoles,
                RoleGenManager.IntruderHeadRoles);
            SyndicateRoles.AddRanges(RoleGenManager.SyndicateSupportRoles, RoleGenManager.SyndicateKillingRoles, RoleGenManager.SyndicateHeadRoles, RoleGenManager.SyndicateDisruptionRoles);
            ApocalypseRoles.AddRange(RoleGenManager.ApocalypseHarbingerRoles);
        }
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

        Outcasts = URandom.RandomRangeInt(0, players - Intruders - Syndicate - Apocalypse + 1);
        Crew = players - Intruders - Syndicate - Outcasts;

        while (Crew == 0 && (Intruders > 0 || Syndicate > 0 || Outcasts > 0 || Apocalypse > 0) && players > 1)
        {
            _ = URandom.RandomRangeInt(0, 4) switch
            {
                0 when Intruders > 0 => Intruders--,
                1 when Apocalypse > 0 => Apocalypse--,
                2 when Syndicate > 0 => Syndicate--,
                3 when Outcasts > 0 => Outcasts--,
                _ => 0
            };

            Crew++;
        }

        if (TownOfUsReworked.MciActive)
            Info($"Crew = {Crew}, Int = {Intruders}, Syn = {Syndicate}, Neut = {Outcasts}, Apoc = {Apocalypse}");
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