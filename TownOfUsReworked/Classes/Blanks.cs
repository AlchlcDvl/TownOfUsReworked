namespace TownOfUsReworked.Classes;

public static class Blanks
{
    public static void BlankVoid() {}

    public static void BlankVoid(object _) {}

    public static void BlankVoid(OptionBehaviour _) {}

    public static bool BlankTrue() => true;

    public static bool BlankFalse() => false;

    public static bool BlankFalse(Vent _) => false;

    public static bool BlankFalse(Console _) => false;

    public static bool BlankFalse(PlayerControl _) => false;

    public static bool BlankFalse(PlayerVoteArea _) => false;

    public static float BlankOne() => 1f;

    public static float BlankZero() => 0f;

    public static string BlankButtonLabel() => "ABILITY";
}