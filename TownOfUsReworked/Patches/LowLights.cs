using HarmonyLib;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class LowLights
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player, ref float __result)
        {
            if (player == null | player.IsDead)
            {
                __result = __instance.MaxLightRadius;
                return false;
            }

            var switchSystem = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

            if (player._object.Is(Faction.Intruders) | player._object.Is(RoleAlignment.NeutralKill) | player._object.Is(Faction.Intruders))
            {
                __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
                return false;
            }

            if (SubmergedCompatibility.isSubmerged())
            {
                if (player._object.Is(AbilityEnum.Torch))
                    __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;

                if (player._object.Is(AbilityEnum.Lighter))
                    __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1) * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
                return false;
            }

            var t = switchSystem.Value / 255f;
            
            if (player._object.Is(AbilityEnum.Torch))
                t = 1;

            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, t) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
            return false;
        }
    }
}