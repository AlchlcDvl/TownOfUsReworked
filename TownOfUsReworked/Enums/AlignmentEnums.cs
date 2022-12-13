namespace TownOfUsReworked.Enums
{
    public enum Faction
    {
        Crew = 0,
        Intruders,
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
        CrewCheck,

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
        
        None
    }
}
