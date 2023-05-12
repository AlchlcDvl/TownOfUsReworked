namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class LowLights
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
                var switchSystem = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                var t = switchSystem.Value / 255f;
                __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, t) * (player.Object.Is(Faction.Neutral) ? CustomGameOptions.NeutralVision :
                    CustomGameOptions.CrewVision);
            }

            return false;
        }
    }
}