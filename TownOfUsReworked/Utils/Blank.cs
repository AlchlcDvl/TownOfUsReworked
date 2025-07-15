namespace TownOfUsReworked.Utils;

public static class BlankUtils
{
    public static void BlankVoid() {}

    public static void BlankVoid(object _) {}

    public static bool BlankTrue() => true;

    public static bool BlankFalse() => false;

    public static bool BlankFalse(object _) => false;

    public static bool BlankFalse(PlayerControl _, out bool __) => __ = false;

    public static float BlankOne() => 1f;

    public static float BlankZero() => 0f;

    public static string BlankButtonLabel() => "ABILITY";

    public static string BlankButtonSprite() => "Placeholder";
}