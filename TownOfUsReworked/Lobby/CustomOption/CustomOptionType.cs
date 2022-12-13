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
        Custom
        //Mafia
    }

    public enum NoSolo
    {
        Never,
        SameRoles,
        AllNKs,
        AllNeutrals
    }

    public enum Map
    {
        Skeld = 0,
        MiraHQ = 1,
        Polus = 2,
        Airship = 4,
        Submerged = 5
    }

    public enum TaskBarMode
    {
        Normal = 0,
        MeetingOnly = 1,
        Invisible = 2
    }
}