namespace TownOfUsReworked.Enums
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
    
    public enum RoleFactionReports
    {
        Role,
        Faction,
        Neither
    }
    
    public enum AirshipSpawnType
    {
        Normal,
        Fixed,
        RandomSynchronized
    }
    
    public enum MoveAdmin
    {
        DontMove,
        Cockpit,
        MainHall
    }
    
    public enum MoveElectrical
    {
        DontMove,
        Vault,
        Electrical
    }

    public enum GameMode
    {
        Classic,
        AllAny,
        KillingOnly,
        Custom
    }

    public enum Map
    {
        Skeld,
        MiraHQ,
        Polus,
        Airship,
        Submerged
    }

    public enum TaskBarMode
    {
        Normal,
        MeetingOnly,
        Invisible
    }

    public enum WhoCanSeeFirstKillShield
	{
		Everyone,
		PlayerOnly,
		NoOne
	}

    public enum NKsKnow
	{
		Never,
		SameRole,
		AllNKs
	}
}