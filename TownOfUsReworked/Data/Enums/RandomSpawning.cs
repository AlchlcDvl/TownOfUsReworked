namespace TownOfUsReworked.Data.Enums;

// TODO: Consider [Flags]
public enum RandomSpawning : byte
{
    Disabled,
    GameStart,
    PostMeeting,
    Both
}

/*
[Flags]
public enum RandomSpawning : byte
{
    Disabled = 0,

    GameStart = 2,

    PostMeeting = 4,

    Both = GameStart | PostMeeting,
}
*/