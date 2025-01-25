namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
[HeaderOption(MultiMenu.Main, priority: 0)]
public static class BetterSabotages
{
    [ToggleOption]
    public static bool CamouflagedComms = true;

    [ToggleOption]
    public static bool CamouflagedMeetings = false;

    [ToggleOption]
    public static bool NightVision = false;

    [ToggleOption]
    public static bool EvilsIgnoreNV = false;

    [ToggleOption]
    public static bool OxySlow = true;

    [NumberOption(0, 100, 5, Format.Percent)]
    public static Number ReactorShake = 30;

    [ToggleOption]
    public static bool CamoHideSize = false;

    [ToggleOption]
    public static bool CamoHideSpeed = false;

    public static void Postfix(HudManager __instance)
    {
        if (!IsInGame() || !Ship())
            return;

        if (Ship().Systems.TryGetValue(SystemTypes.Laboratory, out var lab) && MapPatches.CurrentMap == 2)
        {
            var system = lab.Cast<ReactorSystemType>();

            if (system.IsActive && ReactorShake != 0f)
                __instance.PlayerCam.ShakeScreen(500, ReactorShake * (system.ReactorDuration - system.Countdown) / (100f * system.ReactorDuration));
            else
                __instance.PlayerCam.ShakeScreen(0, 0);
        }
        else if (Ship().Systems.TryGetValue(SystemTypes.Reactor, out var reactor) && MapPatches.CurrentMap is 0 or 1 or 3 or 5 or 7)
        {
            var system = reactor.Cast<ReactorSystemType>();

            if (system.IsActive && ReactorShake != 0f)
                __instance.PlayerCam.ShakeScreen(500, ReactorShake * (system.ReactorDuration - system.Countdown) / (100f * system.ReactorDuration));
            else
                __instance.PlayerCam.ShakeScreen(0, 0);
        }
    }
}