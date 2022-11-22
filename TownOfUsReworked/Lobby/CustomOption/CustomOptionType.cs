namespace TownOfUsReworked.Lobby.CustomOption
{
    public enum CustomOptionType
    {
        Header,
        Toggle,
        Number,
        String,
        Button,
        Menu,
        Tab
    }
    
    public enum MultiMenu
    {
        main,
        crew,
        neutral,
        intruder,
        syndicate,
        modifier,
        objectifier,
        ability,
        external
    }
    
    public enum WhoCanVentOptions
    {
        Default,
        Everyone,
        Noone
    }
    
    public enum DisableSkipButtonMeetings
    {
        No,
        Emergency,
        Always
    }

    public enum GameMode
    {
        Classic,
        AllAny,
        KillingOnly,
        Mafia,
        Custom
    }

    public enum NoSolo
    {
        Never,
        SameRoles,
        AllNKs
    }
}