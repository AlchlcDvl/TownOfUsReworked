namespace TownOfUsReworked.Data.Enums;

public enum CustomTypeCode : byte
{
    // Base game types
    PlayerControl,
    DeadBody,
    Vent,
    PlayerVoteArea,
    Vector2,

    // Base C# types
    Byte,
    SByte,
    Short,
    UShort,
    Int,
    UInt,
    Long,
    ULong,
    Float,
    Double,
    Decimal,
    Boolean,
    Char,
    String,
    Enum,
    Type,
    Half,

    // Custom classes
    NetData,
    Button,
    MultiSelectValue,
    Number,
    RoleOptionData,
    PlayerLayer,

    // Collections
    IEnumerable,
    Array
}