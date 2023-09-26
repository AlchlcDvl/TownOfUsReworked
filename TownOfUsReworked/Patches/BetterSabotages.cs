namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class BetterSabotages
{
    public static void Postfix(HudManager __instance)
    {
        if (!IsInGame || ShipStatus.Instance == null)
            return;

        if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory) && MapPatches.CurrentMap == 2)
        {
            var system = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

            if (system.IsActive && CustomGameOptions.ReactorShake != 0f)
                __instance.PlayerCam.ShakeScreen(500, (CustomGameOptions.ReactorShake * (system.ReactorDuration - system.Countdown)) / (100f * system.ReactorDuration));
            else
                __instance.PlayerCam.ShakeScreen(0, 0);
        }
        else if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor) && MapPatches.CurrentMap is 0 or 3 or 6)
        {
            var system = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

            if (system.IsActive && CustomGameOptions.ReactorShake != 0f)
                __instance.PlayerCam.ShakeScreen(500, (CustomGameOptions.ReactorShake * (system.ReactorDuration - system.Countdown)) / (100f * system.ReactorDuration));
            else
                __instance.PlayerCam.ShakeScreen(0, 0);
        }
    }
}