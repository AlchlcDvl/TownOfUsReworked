namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius)), HarmonyPriority(Priority.Low)]
public static class CalculateLightRadiusPatch
{
    public static bool Prefix(ShipStatus __instance, NetworkedPlayerInfo player, ref float __result)
    {
        if (!player)
            return false;

        if (Lobby() || player.IsDead)
        {
            __result = __instance.MaxLightRadius;
            return false;
        }

        if (IsHnS())
        {
            var hns = TownOfUsReworked.HnsOptions;
            var isImp = player.IsImpostor();
            __result = __instance.MaxLightRadius * (hns.useFlashlight ? (isImp ? hns.ImpostorFlashlightSize : hns.CrewmateFlashlightSize) : (isImp ? hns.ImpostorLightMod : hns.CrewLightMod));
            return false;
        }

        var pc = player.Object;
        var t = __instance.MaxLightRadius;

        if (__instance.Systems.TryGetValue(SystemTypes.Electrical, out var system) && system.TryCast<SwitchSystem>(out var lights))
            t = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, lights.Level);

        if (pc.TryGetLayer<Role>(out var role))
        {
            if (pc.Is<Torch>() || !role.AffectedByLights)
                t = __instance.MaxLightRadius;

            t *= role.VisionRange;
        }
        else
            t *= (player.IsImpostor() ? TownOfUsReworked.NormalOptions.ImpostorLightMod : TownOfUsReworked.NormalOptions.CrewLightMod);

        if (MapPatches.CurrentMap is 0 or 3 or 6 && MapSettings.SmallMapHalfVision && !IsTaskRace() && !IsCustomHnS())
            t *= 0.5f;

        __result = t;
        return false;
    }
}