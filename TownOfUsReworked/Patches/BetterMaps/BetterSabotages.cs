namespace TownOfUsReworked.Patches.BetterMaps;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update)), HeaderOption(MultiMenu.Main), Sorted(1)]
public static class BetterSabotages
{
    [ToggleOption]
    public static bool CamouflagedComms = true;

    [ToggleOption]
    public static bool CamouflagedMeetings = false;

    [ToggleOption]
    public static bool NightVision = false;

    [ToggleOption]
    public static bool EvilsIgnoreNv = false;

    [ToggleOption]
    public static bool OxySlow = true;

    [NumberOption(0, 80, 5, Format.Percent)]
    public static Number LowestOxySpeed = 25;

    [NumberOption(0, 100, 5, Format.Percent)]
    private static Number ReactorShake = 30;

    [ToggleOption]
    public static bool CamoHideSize = false;

    [ToggleOption]
    public static bool CamoHideSpeed = false;

    public static void Postfix(HudManager __instance)
    {
        if (!IsInGame() || MapPatches.CurrentMap is 4 or 6)
            return;

        var ship = Ship();

        if (!ship || !ship.Systems.TryGetValue(MapPatches.CurrentMap == 2 ? SystemTypes.Laboratory : SystemTypes.Reactor, out var sab) || !sab.TryCast<ReactorSystemType>(out var system))
            return;

        if (system.IsActive && ReactorShake != 0f)
            __instance.PlayerCam.ShakeScreen(500, ReactorShake * (system.ReactorDuration - system.Countdown) / (100f * system.ReactorDuration));
        else
            __instance.PlayerCam.ShakeScreen(0, 0);
    }
}