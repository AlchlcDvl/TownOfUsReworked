namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius)), HarmonyPriority(Priority.Low)]
public static class CalculateLightRadiusPatch
{
    public static bool Prefix(ShipStatus __instance, NetworkedPlayerInfo player, ref float __result)
    {
        if (!player) // No player? No vision
        {
            __result = 0f;
            return false;
        }

        if (IsSubmerged()) // Custom implementation
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
        var baseT = __instance.MaxLightRadius;
        var t = 1f;
        bool affectedByLights;

        if (pc.Is<Role>(out var role))
        {
            affectedByLights = !pc.Is<Torch>() && role.AffectedByLights;
            t *= role.VisionRange;
        }
        else
        {
            affectedByLights = player.IsImpostor();
            t *= affectedByLights ? TownOfUsReworked.NormalOptions.ImpostorLightMod : TownOfUsReworked.NormalOptions.CrewLightMod;
        }

        if (MapPatches.CurrentMap is 0 or 3 or 6 && MapSettings.SmallMapHalfVision)
            t *= 0.5f;

        if (affectedByLights && __instance.Systems.TryGetValue(SystemTypes.Electrical, out var system) && system.TryCast<SwitchSystem>(out var lights))
            baseT = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, lights.Level);

        __result = baseT * t;
        return false;
    }
}