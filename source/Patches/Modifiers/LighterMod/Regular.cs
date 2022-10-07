using HarmonyLib;

namespace TownOfUs.Modifiers.LighterMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public class Regular
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player, ref float __result)
        {
            __result = __instance.MinLightRadius * PlayerControl.GameOptions.ImpostorLightMod;
            return false;
        }
    }
}