namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class CalculateLightRadiusPatch
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player, ref float __result)
        {
            if (player == null)
                return false;

            if (ConstantVariables.IsHnS)
            {
                if (GameOptionsManager.Instance.currentHideNSeekGameOptions.useFlashlight)
                {
                    if (player.IsImpostor())
                        __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.ImpostorFlashlightSize;
                    else
                        __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.CrewmateFlashlightSize;
                }
                else if (player.IsImpostor())
                    __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.ImpostorLightMod;
                else
                    __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.CrewLightMod;

                return false;
            }

            if (player.IsDead)
                __result = __instance.MaxLightRadius;
            else if (player.Object.Is(Faction.Intruder) || (player.Object.Is(RoleAlignment.NeutralKill) && CustomGameOptions.NKHasImpVision) || player.Object.Is(AbilityEnum.Torch))
                __result = __instance.MaxLightRadius * CustomGameOptions.IntruderVision;
            else if (player.Object.Is(Faction.Syndicate))
                __result = __instance.MaxLightRadius * CustomGameOptions.SyndicateVision;
            else if (player.Object.Is(Faction.Neutral) && !CustomGameOptions.LightsAffectNeutrals)
                __result = __instance.MaxLightRadius * CustomGameOptions.NeutralVision;
            else if (player != null)
            {
                __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>().Value / 255f) *
                    (player.Object.Is(Faction.Neutral) ? CustomGameOptions.NeutralVision : CustomGameOptions.CrewVision);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.AdjustLighting))]
    public static class AdjustLightingPatch
    {
        public static bool Prefix(PlayerControl __instance)
        {
            //Planning on making flashlight available all the time
            return __instance != null;
        }
    }
}