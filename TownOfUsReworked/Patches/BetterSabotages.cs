namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class BetterSabotages
{
    public static void Postfix(HudManager __instance)
    {
        if (!IsInGame || Ship == null)
            return;

        if (Ship.Systems.ContainsKey(SystemTypes.Laboratory) && MapPatches.CurrentMap == 2)
        {
            var system = Ship.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

            if (system.IsActive && CustomGameOptions.ReactorShake != 0f)
                __instance.PlayerCam.ShakeScreen(500, CustomGameOptions.ReactorShake * (system.ReactorDuration - system.Countdown) / (100f * system.ReactorDuration));
            else
                __instance.PlayerCam.ShakeScreen(0, 0);
        }
        else if (Ship.Systems.ContainsKey(SystemTypes.Reactor) && MapPatches.CurrentMap is 0 or 3 or 5 or 7)
        {
            var system = Ship.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

            if (system.IsActive && CustomGameOptions.ReactorShake != 0f)
                __instance.PlayerCam.ShakeScreen(500, CustomGameOptions.ReactorShake * (system.ReactorDuration - system.Countdown) / (100f * system.ReactorDuration));
            else
                __instance.PlayerCam.ShakeScreen(0, 0);
        }
    }
}