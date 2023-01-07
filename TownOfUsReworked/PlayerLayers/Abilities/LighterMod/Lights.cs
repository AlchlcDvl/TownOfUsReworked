using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Abilities.LighterMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class Lights
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player, ref float __result)
        {
            var switchSystem = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

            if (player._object.Is(AbilityEnum.Lighter))
            {
                __result = __instance.MinLightRadius * CustomGameOptions.IntruderVision;
                return false;
            }

            return false;                
            
        }
    }
}