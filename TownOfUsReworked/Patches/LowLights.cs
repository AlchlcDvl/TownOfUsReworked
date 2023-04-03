using HarmonyLib;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class LowLights
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player, ref float __result)
        {
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

            if (player?.IsDead != false)
            {
                __result = __instance.MaxLightRadius;
                return false;
            }
            else if (player._object.Is(Faction.Intruder) || (player._object.Is(RoleAlignment.NeutralKill) && CustomGameOptions.NKHasImpVision) || player._object.Is(AbilityEnum.Torch))
            {
                __result = __instance.MaxLightRadius * CustomGameOptions.IntruderVision;
                return false;
            }
            else if (player._object.Is(Faction.Syndicate))
            {
                __result = __instance.MaxLightRadius * CustomGameOptions.SyndicateVision;
                return false;
            }
            else if (player._object.Is(Faction.Neutral) && !CustomGameOptions.LightsAffectNeutrals)
            {
                __result = __instance.MaxLightRadius * CustomGameOptions.NeutralVision;
                return false;
            }
            else if (player != null)
            {
                var switchSystem = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                var t = switchSystem.Value / 255f;
                __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, t) * (player._object.Is(Faction.Neutral) ? CustomGameOptions.NeutralVision :
                    CustomGameOptions.CrewVision);
            }

            return false;
        }
    }
}