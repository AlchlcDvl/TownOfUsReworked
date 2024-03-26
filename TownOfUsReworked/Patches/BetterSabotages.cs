namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class BetterSabotages
{
    public static void Postfix(HudManager __instance)
    {
        if (!IsInGame || !Ship)
            return;

        if (Ship.Systems.TryGetValue(SystemTypes.Laboratory, out var lab) && MapPatches.CurrentMap == 2)
        {
            var system = lab.Cast<ReactorSystemType>();

            if (system.IsActive && CustomGameOptions.ReactorShake != 0f)
                __instance.PlayerCam.ShakeScreen(500, CustomGameOptions.ReactorShake * (system.ReactorDuration - system.Countdown) / (100f * system.ReactorDuration));
            else
                __instance.PlayerCam.ShakeScreen(0, 0);
        }
        else if (Ship.Systems.TryGetValue(SystemTypes.Reactor, out var reactor) && MapPatches.CurrentMap is 0 or 1 or 3 or 5 or 7)
        {
            var system = reactor.Cast<ReactorSystemType>();

            if (system.IsActive && CustomGameOptions.ReactorShake != 0f)
                __instance.PlayerCam.ShakeScreen(500, CustomGameOptions.ReactorShake * (system.ReactorDuration - system.Countdown) / (100f * system.ReactorDuration));
            else
                __instance.PlayerCam.ShakeScreen(0, 0);
        }
    }
}