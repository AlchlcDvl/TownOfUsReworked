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
        var role = pc.GetRole();

        if (!role)
        {
            __result = __instance.MaxLightRadius * (pc.IsImpostor() ? TownOfUsReworked.NormalOptions.ImpostorLightMod : TownOfUsReworked.NormalOptions.CrewLightMod);
            return false;
        }

        if (role.Faction == Faction.Intruder || (role is NKilling && NeutralKillingSettings.NkHaveImpVision) || pc.Is<Torch>() || (role.Alignment == Alignment.Neophyte &&
            NeutralNeophyteSettings.NnHaveImpVision) || (role.Alignment == Alignment.Harbinger && NeutralHarbingerSettings.NhHaveImpVision) || (role.Alignment == Alignment.Evil &&
            NeutralEvilSettings.NeHaveImpVision) || role.Alignment == Alignment.Apocalypse)
        {
            __result = __instance.MaxLightRadius * IntruderSettings.IntruderVision;
        }
        else switch (role.Faction)
        {
            case Faction.Syndicate:
            {
                __result = __instance.MaxLightRadius * SyndicateSettings.SyndicateVision;
                break;
            }
            case Faction.Neutral when !NeutralSettings.LightsAffectNeutrals:
            {
                __result = __instance.MaxLightRadius * NeutralSettings.NeutralVision;
                break;
            }
            default:
            {
                switch (role)
                {
                    case Runner:
                    {
                        __result = __instance.MaxLightRadius;
                        break;
                    }
                    case Hunted:
                    {
                        __result = __instance.MaxLightRadius * GameModeSettings.HuntedVision;
                        break;
                    }
                    case Hunter hunter:
                    {
                        __result = __instance.MaxLightRadius * (hunter.Starting ? 0.01f : GameModeSettings.HunterVision);
                        break;
                    }
                    default:
                    {
                        var t = __instance.MaxLightRadius;

                        if (__instance.Systems.TryGetValue(SystemTypes.Electrical, out var system) && system.TryCast<SwitchSystem>(out var lights))
                            t = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, lights.Level);

                        __result = t * (role.Faction == Faction.Neutral ? NeutralSettings.NeutralVision : CrewSettings.CrewVision);
                        break;
                    }
                }

                break;
            }
        }

        if (MapPatches.CurrentMap is 0 or 3 or 6 && MapSettings.SmallMapHalfVision && !IsTaskRace() && !IsCustomHnS())
            __result *= 0.5f;

        return false;
    }
}