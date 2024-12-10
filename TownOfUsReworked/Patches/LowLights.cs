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
            var isImp = player.IsImpostor();
            __result = __instance.MaxLightRadius * (hns.useFlashlight ? (isImp ? hns.ImpostorFlashlightSize : hns.CrewmateFlashlightSize) : (isImp ? hns.ImpostorLightMod : hns.CrewLightMod));
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