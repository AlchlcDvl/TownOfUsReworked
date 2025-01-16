namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius)), HarmonyPriority(Priority.Low)]
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
        var role = pc.GetRole();

        if (!role)
        {
            __result = __instance.MaxLightRadius * (pc.IsImpostor() ? TownOfUsReworked.NormalOptions.ImpostorLightMod : TownOfUsReworked.NormalOptions.CrewLightMod);
            return false;
        }

        if (player.IsDead)
            __result = __instance.MaxLightRadius;
        else if (role.Faction == Faction.Intruder || (role.Alignment == Alignment.NeutralKill && NeutralKillingSettings.NKHasImpVision) || pc.Is<Torch>() || (role.Alignment ==
            Alignment.NeutralNeo && NeutralNeophyteSettings.NNHasImpVision) || (role.Alignment == Alignment.NeutralHarb && NeutralHarbingerSettings.NHHasImpVision) || (role.Alignment ==
            Alignment.NeutralEvil && NeutralEvilSettings.NEHasImpVision))
        {
            __result = __instance.MaxLightRadius * IntruderSettings.IntruderVision;
        }
        else if (role.Faction == Faction.Syndicate)
            __result = __instance.MaxLightRadius * SyndicateSettings.SyndicateVision;
        else if (role.Faction == Faction.Neutral && !NeutralSettings.LightsAffectNeutrals)
            __result = __instance.MaxLightRadius * NeutralSettings.NeutralVision;
        else if (role is Runner)
            __result = __instance.MaxLightRadius;
        else if (role is Hunted)
            __result = __instance.MaxLightRadius * GameModeSettings.HuntedVision;
        else if (role is Hunter hunter)
            __result = __instance.MaxLightRadius * (hunter.Starting ? 0.01f : GameModeSettings.HunterVision);
        else
        {
            var t = __instance.MaxLightRadius;

            if (__instance.Systems.TryGetValue(SystemTypes.Electrical, out var system))
                t = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, system.Cast<SwitchSystem>().Level);

            __result = t * (role.Faction == Faction.Neutral ? NeutralSettings.NeutralVision : CrewSettings.CrewVision);
        }

        if (MapPatches.CurrentMap is 0 or 3 or 6 && MapSettings.SmallMapHalfVision && !IsTaskRace() && !IsCustomHnS())
            __result *= 0.5f;

        return false;
    }
}