namespace TownOfUsReworked.Enums
{
    public enum Faction
    {
        Crew = 0,
        Intruder,
        Neutral,
        Syndicate,
        
        None
    }

    public enum RoleAlignment
    {
        CrewSupport = 0,
        CrewInvest,
        CrewProt,
        CrewKill,
        CrewUtil,
        CrewSov,
        CrewAudit,

        IntruderSupport,
        IntruderConceal,
        IntruderDecep,
        IntruderKill,
        IntruderUtil,

        NeutralKill,
        NeutralNeo,
        NeutralEvil,
        NeutralBen,
        NeutralPros,

        SyndicateKill,
        SyndicateSupport,
        SyndicateChaos,
        SyndicatePower,
        SyndicateUtil,
        
        Other,
        
        None
    }

    public enum SubFaction
    {
        Undead = 0,
        Cabal,
        
        None
    }
}
