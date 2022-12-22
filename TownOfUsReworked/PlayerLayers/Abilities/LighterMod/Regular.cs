using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Abilities.LighterMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public class Regular
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player, ref float __result)
        {
            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Lighter))
                __result = __instance.MinLightRadius * PlayerControl.GameOptions.ImpostorLightMod;
            
            return false;
        }
    }
}