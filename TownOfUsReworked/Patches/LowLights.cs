namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
public static class CalculateLightRadiusPatch
{
    public static bool Prefix(ShipStatus __instance, NetworkedPlayerInfo player, ref float __result)
    {
        if (!player)
            return false;

        if (IsHnS())
        {
            var hns = TownOfUsReworked.HNSOptions;

            if (hns.useFlashlight)
                __result = __instance.MaxLightRadius * (player.IsImpostor() ? hns.ImpostorFlashlightSize : hns.CrewmateFlashlightSize);
            else
                __result = __instance.MaxLightRadius * (player.IsImpostor() ? hns.ImpostorLightMod : hns.CrewLightMod);

            return false;
        }

        var pc = player.Object;

        if (!pc.GetRole())
        {
            __result = __instance.MaxLightRadius * (pc.IsImpostor() ? TownOfUsReworked.NormalOptions.ImpostorLightMod : TownOfUsReworked.NormalOptions.CrewLightMod);
            return false;
        }

        if (player.IsDead)
            __result = __instance.MaxLightRadius;
        else if (pc.Is(Faction.Intruder) || (pc.Is(Alignment.NeutralKill) && NeutralKillingSettings.NKHasImpVision) || pc.Is(LayerEnum.Torch) || (pc.Is(Alignment.NeutralNeo) &&
            NeutralNeophyteSettings.NNHasImpVision) || (pc.Is(Alignment.NeutralHarb) && NeutralHarbingerSettings.NHHasImpVision) || (pc.Is(Alignment.NeutralEvil) &&
            NeutralEvilSettings.NEHasImpVision))
        {
            __result = __instance.MaxLightRadius * IntruderSettings.IntruderVision;
        }
        else if (pc.Is(Faction.Syndicate))
            __result = __instance.MaxLightRadius * SyndicateSettings.SyndicateVision;
        else if (pc.Is(Faction.Neutral) && !NeutralSettings.LightsAffectNeutrals)
            __result = __instance.MaxLightRadius * NeutralSettings.NeutralVision;
        else if (pc.Is(LayerEnum.Runner))
            __result = __instance.MaxLightRadius;
        else if (pc.Is(LayerEnum.Hunted))
            __result = __instance.MaxLightRadius * GameModeSettings.HuntedVision;
        else if (pc.Is(LayerEnum.Hunter))
            __result = __instance.MaxLightRadius * (pc.GetLayer<Hunter>().Starting ? 0f : GameModeSettings.HunterVision);
        else
        {
            var t = __instance.MaxLightRadius;

            if (__instance.Systems.TryGetValue(SystemTypes.Electrical, out var system))
                t = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, system.Cast<SwitchSystem>().Value / 255f);

            __result = t * (pc.Is(Faction.Neutral) ? NeutralSettings.NeutralVision : CrewSettings.CrewVision);
        }

        if (MapPatches.CurrentMap is 0 or 3 or 6 && MapSettings.SmallMapHalfVision && !IsTaskRace() && !IsCustomHnS())
            __result *= 0.5f;

        return false;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.AdjustLighting))]
public static class AdjustLightingPatch
{
    public static bool Prefix(PlayerControl __instance)
    {
        if (CustomPlayer.Local != __instance || !Ship())
            return true;

        var flashlights = false;
        var size = Ship().CalculateLightRadius(__instance.Data);

        if (__instance.Is(Faction.Crew))
            flashlights = CrewSettings.CrewFlashlight;
        else if (__instance.Is(Faction.Intruder))
            flashlights = IntruderSettings.IntruderFlashlight;
        else if (__instance.Is(Faction.Syndicate))
            flashlights = SyndicateSettings.SyndicateFlashlight;
        else if (__instance.Is(Faction.Neutral))
            flashlights = NeutralSettings.NeutralFlashlight;
        else if (__instance.Is(LayerEnum.Hunted))
            flashlights = GameModeSettings.HuntedFlashlight;
        else if (__instance.Is(LayerEnum.Hunter))
            flashlights = GameModeSettings.HunterFlashlight;

        flashlights &= !__instance.Data.IsDead;

        if (flashlights)
            size /= Ship().MaxLightRadius;

        __instance.TargetFlashlight.gameObject.SetActive(flashlights);
        __instance.StartCoroutine(__instance.EnableRightJoystick(false));
        __instance.lightSource.SetupLightingForGameplay(flashlights, size, __instance.TargetFlashlight.transform);
        return false;
    }
}