namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class BetterSabotages
    {
        public static void Postfix(HudManager __instance)
        {
            if (IsInGame && ShipStatus.Instance != null)
            {
                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (reactorSystemType.IsActive && CustomGameOptions.ReactorShake != 0f)
                    {
                        __instance.PlayerCam.ShakeScreen(400, CustomGameOptions.ReactorShake * (reactorSystemType.ReactorDuration - reactorSystemType.Countdown) / 75f /
                            reactorSystemType.ReactorDuration);
                    }
                    else
                        __instance.PlayerCam.ShakeScreen(0, 0);
                }
                else if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor) && TownOfUsReworked.NormalOptions.MapId is 4)
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();

                    if (reactorSystemType.IsActive && CustomGameOptions.ReactorShake != 0f)
                    {
                        __instance.PlayerCam.ShakeScreen(400, CustomGameOptions.ReactorShake * (reactorSystemType.Countdown - reactorSystemType.Countdown) / 100f /
                            reactorSystemType.Countdown);
                    }
                    else
                        __instance.PlayerCam.ShakeScreen(0, 0);
                }
                else if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor) && TownOfUsReworked.NormalOptions.MapId is 0 or 6)
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                    if (reactorSystemType.IsActive && CustomGameOptions.ReactorShake != 0f)
                    {
                        __instance.PlayerCam.ShakeScreen(400, CustomGameOptions.ReactorShake * (reactorSystemType.ReactorDuration - reactorSystemType.Countdown) / 100f /
                            reactorSystemType.ReactorDuration);
                    }
                    else
                        __instance.PlayerCam.ShakeScreen(0, 0);
                }
            }
        }
    }
}