namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
[HeaderOption(MultiMenu2.Main)]
public static class BetterSabotages
{
    [ToggleOption(MultiMenu2.Main)]
    public static bool CamouflagedComms { get; set; } = true;

    [ToggleOption(MultiMenu2.Main)]
    public static bool CamouflagedMeetings { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool NightVision { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool EvilsIgnoreNV { get; set; } = false;

    [ToggleOption(MultiMenu2.Main)]
    public static bool OxySlow { get; set; } = true;

    [NumberOption(MultiMenu2.Main, 0, 100, 5, Format.Percent)]
    public static int ReactorShake { get; set; } = 30;

    public static void Postfix(HudManager __instance)
    {
        if (!IsInGame || !Ship)
            return;

        if (Ship.Systems.TryGetValue(SystemTypes.Laboratory, out var lab) && MapPatches.CurrentMap == 2)
        {
            var system = lab.Cast<ReactorSystemType>();

            if (system.IsActive && ReactorShake != 0f)
                __instance.PlayerCam.ShakeScreen(500, ReactorShake * (system.ReactorDuration - system.Countdown) / (100f * system.ReactorDuration));
            else
                __instance.PlayerCam.ShakeScreen(0, 0);
        }
        else if (Ship.Systems.TryGetValue(SystemTypes.Reactor, out var reactor) && MapPatches.CurrentMap is 0 or 1 or 3 or 5 or 7)
        {
            var system = reactor.Cast<ReactorSystemType>();

            if (system.IsActive && ReactorShake != 0f)
                __instance.PlayerCam.ShakeScreen(500, ReactorShake * (system.ReactorDuration - system.Countdown) / (100f * system.ReactorDuration));
            else
                __instance.PlayerCam.ShakeScreen(0, 0);
        }
    }
}