using System;
using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class BetterSabotages
    {
        public static void Postfix(HudManager __instance)
        {
            if (GameStates.IsInGame && ShipStatus.Instance != null)
            {
                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
                {
                    var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (lifeSuppSystemType.IsActive && CustomGameOptions.OxySlow)
                    {
                        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                        {
                            if (!player.Data.IsDead)
                                player.MyPhysics.Speed = Math.Clamp(2.5f * lifeSuppSystemType.Countdown / lifeSuppSystemType.LifeSuppDuration, 1f, CustomGameOptions.PlayerSpeed);
                        }
                    }
                }

                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (reactorSystemType.IsActive && CustomGameOptions.ReactorShake != 0f)
                    {
                        __instance.PlayerCam.ShakeScreen(400, CustomGameOptions.ReactorShake * (reactorSystemType.ReactorDuration - reactorSystemType.Countdown) /
                            reactorSystemType.ReactorDuration / 75f);
                    }
                    else
                        __instance.PlayerCam.ShakeScreen(0, 0);
                }

                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor) && GameOptionsManager.Instance.currentNormalGameOptions.MapId == 0)
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                    if (reactorSystemType.IsActive && CustomGameOptions.ReactorShake != 0f)
                    {
                        __instance.PlayerCam.ShakeScreen(400, CustomGameOptions.ReactorShake * (reactorSystemType.ReactorDuration - reactorSystemType.Countdown) /
                            reactorSystemType.ReactorDuration / 100f);
                    }
                    else
                        __instance.PlayerCam.ShakeScreen(0, 0);
                }
            }
        }
    }
}