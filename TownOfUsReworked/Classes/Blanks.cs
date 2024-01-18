namespace TownOfUsReworked.Classes;

public static class Blanks
{
    public static void BlankVoid() {}

    public static void BlankVoid(object _, object __) {}

    public static bool BlankTrue() => true;

    public static bool BlankFalse() => false;

    public static bool BlankFalse(PlayerControl _) => false;

    public static bool BlankFalse(Vent _) => false;

    public static bool BlankFalse(Console _) => false;

    public static bool BlankFalse(PlayerVoteArea _) => false;
}