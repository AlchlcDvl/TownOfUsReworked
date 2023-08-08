namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class BetterSabotages
{
    public static void Postfix(HudManager __instance)
    {
        if (IsInGame && ShipStatus.Instance != null)
        {
            if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
            {
                var system = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                if (system.IsActive && CustomGameOptions.ReactorShake != 0f)
                    __instance.PlayerCam.ShakeScreen(500, CustomGameOptions.ReactorShake * (system.ReactorDuration - system.Countdown) / 100f / system.ReactorDuration);
                else
                    __instance.PlayerCam.ShakeScreen(0, 0);
            }
            else if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor) && TownOfUsReworked.NormalOptions.MapId is 4)
            {
                var system = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();

                if (system.IsActive && CustomGameOptions.ReactorShake != 0f)
                    __instance.PlayerCam.ShakeScreen(500, CustomGameOptions.ReactorShake * (system.Countdown - system.Countdown) / 100f / system.Countdown);
                else
                    __instance.PlayerCam.ShakeScreen(0, 0);
            }
            else if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor) && TownOfUsReworked.NormalOptions.MapId is 0 or 6)
            {
                var system = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                if (system.IsActive && CustomGameOptions.ReactorShake != 0f)
                    __instance.PlayerCam.ShakeScreen(500, CustomGameOptions.ReactorShake * (system.ReactorDuration - system.Countdown) / 100f / system.ReactorDuration);
                else
                    __instance.PlayerCam.ShakeScreen(0, 0);
            }
        }
    }
}