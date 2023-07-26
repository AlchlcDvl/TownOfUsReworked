namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class CalculateLightRadiusPatch
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player, ref float __result)
        {
            if (player == null)
                return false;

            if (IsHnS)
            {
                var hns = GameOptionsManager.Instance.currentHideNSeekGameOptions;

                if (hns.useFlashlight)
                    __result = __instance.MaxLightRadius * (player.IsImpostor() ? hns.ImpostorFlashlightSize : hns.CrewmateFlashlightSize);
                else
                    __result = __instance.MaxLightRadius * (player.IsImpostor() ? hns.ImpostorLightMod : hns.CrewLightMod);

                return false;
            }

            if (player.IsDead)
                __result = __instance.MaxLightRadius;
            else if (player.Object.Is(Faction.Intruder) || (player.Object.Is(RoleAlignment.NeutralKill) && CustomGameOptions.NKHasImpVision) ||
                (player.Object.Is(RoleAlignment.NeutralNeo) && CustomGameOptions.NNHasImpVision) || player.Object.Is(AbilityEnum.Torch))
            {
                __result = __instance.MaxLightRadius * CustomGameOptions.IntruderVision;
            }
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
            //Planning on making flashlights available all the time
            AdjustLighting(__instance);
            return false;
        }

        private static void AdjustLighting(PlayerControl __instance)
        {
            if (CustomPlayer.Local != __instance)
                return;

            var flashlights = false;
            var size = ShipStatus.Instance.CalculateLightRadius(__instance.Data);

            if (__instance.Is(Faction.Crew))
                flashlights = CustomGameOptions.CrewFlashlight;
            else if (__instance.Is(Faction.Intruder))
                flashlights = CustomGameOptions.IntruderFlashlight;
            else if (__instance.Is(Faction.Syndicate))
                flashlights = CustomGameOptions.SyndicateFlashlight;
            else if (__instance.Is(Faction.Neutral))
                flashlights = CustomGameOptions.NeutralFlashlight;

            if (flashlights)
                size /= ShipStatus.Instance.MaxLightRadius;

            flashlights = flashlights && !__instance.Data.IsDead;

            if (!flashlights)
            {
                __instance.TargetFlashlight.gameObject.SetActive(false);
                __instance.StartCoroutine(__instance.EnableRightJoystick(false));
            }
            else
            {
                __instance.TargetFlashlight?.gameObject.SetActive(false);
                __instance.StartCoroutine(__instance.EnableRightJoystick(true));
            }

            __instance.lightSource.SetupLightingForGameplay(flashlights, size, __instance.TargetFlashlight.transform);
        }
    }
}