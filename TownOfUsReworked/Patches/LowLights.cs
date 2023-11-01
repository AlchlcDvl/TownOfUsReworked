namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
public static class CalculateLightRadiusPatch
{
    public static bool Prefix(ShipStatus __instance, ref GameData.PlayerInfo player, ref float __result)
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

        var pc = player.Object;

        if (player.IsDead)
            __result = __instance.MaxLightRadius;
        else if (pc.Is(Faction.Intruder) || (pc.Is(Alignment.NeutralKill) && CustomGameOptions.NKHasImpVision) || pc.Is(LayerEnum.Torch) || (pc.Is(Alignment.NeutralNeo) &&
            CustomGameOptions.NNHasImpVision))
        {
            __result = __instance.MaxLightRadius * CustomGameOptions.IntruderVision;
        }
        else if (pc.Is(Faction.Syndicate))
            __result = __instance.MaxLightRadius * CustomGameOptions.SyndicateVision;
        else if (pc.Is(Faction.Neutral) && !CustomGameOptions.LightsAffectNeutrals)
            __result = __instance.MaxLightRadius * CustomGameOptions.NeutralVision;
        else if (pc.Is(LayerEnum.Runner))
            __result = __instance.MaxLightRadius;
        else if (pc.Is(LayerEnum.Hunted))
            __result = __instance.MaxLightRadius * CustomGameOptions.HuntedVision;
        else if (pc.Is(LayerEnum.Hunter))
            __result = __instance.MaxLightRadius * (Role.GetRole<Hunter>(pc).Starting ? 0f : CustomGameOptions.HunterVision);
        else
        {
            var t = __instance.MaxLightRadius;

            if (__instance.Systems.ContainsKey(SystemTypes.Electrical))
                t = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>().Value / 255f);

            __result = t * (pc.Is(Faction.Neutral) ? CustomGameOptions.NeutralVision : CustomGameOptions.CrewVision);
        }

        if (MapPatches.CurrentMap is 0 or 3 or 6 && CustomGameOptions.SmallMapHalfVision && !IsTaskRace && !IsCustomHnS)
            __result *= 0.5f;

        return false;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.AdjustLighting))]
public static class AdjustLightingPatch
{
    public static bool Prefix(PlayerControl __instance)
    {
        if (CustomPlayer.Local != __instance)
            return true;

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
        else if (__instance.Is(LayerEnum.Hunted))
            flashlights = CustomGameOptions.HuntedFlashlight;
        else if (__instance.Is(LayerEnum.Hunter))
            flashlights = CustomGameOptions.HunterFlashlight;

        flashlights &= !__instance.Data.IsDead;

        if (flashlights)
            size /= ShipStatus.Instance.MaxLightRadius;

        __instance.TargetFlashlight.gameObject.SetActive(flashlights);
        __instance.StartCoroutine(__instance.EnableRightJoystick(flashlights));
        __instance.lightSource.SetupLightingForGameplay(flashlights, size, __instance.TargetFlashlight.transform);
        return false;
    }
}