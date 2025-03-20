namespace TownOfUsReworked.Data;

// TODO: Move the rest of the enums to their own file.

public enum DouseActionsRPC : byte
{
    Douse,
    UnDouse
}

public enum DictActionsRPC : byte
{
    Tribunal,
    SelectToEject,
}

public enum GlitchActionsRPC : byte
{
    Mimic,
    Hack
}

public enum PoliticianActionsRPC : byte
{
    Add,
    Remove
}

public enum ThiefActionsRPC : byte
{
    Steal,
    Guess
}

[Flags]
public enum AbilityTypes : byte
{
    None = 0,
    Targetless = 1 << 0,
    Player = 1 << 1,
    Body = 1 << 2,
    Vent = 1 << 3,
    Console = 1 << 4
}

public enum MeetingTypes : byte
{
    Toggle,
    Click
}

public enum CustomOptionType : byte
{
    Header,
    Toggle,
    Number,
    String,
    Layer,
    Entry,
    MultiSelect,
    Alignment,
    LayerHeader,
    ListHolder
}

public enum MultiMenu : byte
{
    Main,
    Layer,
    Client,
    LayerSubOptions,
    AlignmentSubOptions
}

public enum WhoCanVentOptions : byte
{
    Everyone,
    Default,
    NoOne
}

public enum DisableSkipButtonMeetings : byte
{
    Never,
    Emergency,
    Always
}

public enum RoleFactionReports : byte
{
    Neither,
    Role,
    Faction,
    Both
}

public enum AirshipSpawnType : byte
{
    Normal,
    Fixed,
    RandomSynchronized,
    Random,
    Meeting
}

public enum AirshipSpawnLocation : byte
{
    Brig,
    Engine,
    MainHall,
    Kitchen,
    Records,
    CargoBay,
    VaultRoom,
    Cockpit,
    Medical
}

public enum MoveAdmin : byte
{
    DontMove,
    Cockpit,
    MainHall
}

public enum MoveElectrical : byte
{
    DontMove,
    Vault,
    Electrical
}

public enum GameMode : byte
{
    Classic,
    AllAny,
    List,
    HideAndSeek,
    TaskRace,
    Vanilla,
    None
}

public enum HnSMode : byte
{
    Classic,
    Infection
}

public enum PlayerNames : byte
{
    Obstructed,
    Visible,
    NotVisible
}

public enum WhoCanSeeFirstKillShield : byte
{
    Everyone,
    PlayerOnly,
    NoOne
}

public enum TestRPC : byte
{
    Argless,
    Args
}

public enum VanillaRPC : byte
{
    SnapTo,
    SetColor
}

public enum VigiOptions : byte
{
    Immediate,
    PreMeeting,
    PostMeeting
}

public enum VigiNotif : byte
{
    Never,
    Message,
    Flash
}

public enum AdminDeadPlayers : byte
{
    Nobody,
    Operative,
    EveryoneButOperative,
    Everyone
}

public enum ShieldOptions : byte
{
    Nobody,
    Shielded,
    Medic,
    Everyone
}

public enum BecomeEnum : byte
{
    Shifter,
    Crewmate
}

public enum RetActionsRPC : byte
{
    Shield,
    Roleblock,
    Transport,
    Mediate,
    Revive,
    Bomb,
    Place,
    Trigger,
    AltRevive
}

public enum JanitorOptions : byte
{
    Never,
    Body,
    Bodyless,
    Always
}

public enum DisguiserTargets : byte
{
    Everyone,
    Intruders,
    NonIntruders
}

public enum ConsigInfo : byte
{
    Role,
    Faction
}

public enum RevealerCanBeClickedBy : byte
{
    Everyone,
    NonCrew,
    EvilsOnly
}

public enum AlliedFaction : byte
{
    Random,
    Crew,
    Intruder,
    Syndicate,
    Pandorica
}

public enum SyndicateVentOptions : byte
{
    Always,
    ChaosDrive,
    Never
}

public enum SkVentOptions : byte
{
    Always,
    BloodLust,
    NoLust,
    Never
}