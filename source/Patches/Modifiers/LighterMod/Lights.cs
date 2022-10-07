using HarmonyLib;

namespace TownOfUs.Modifiers.LighterMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class Lights
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player, ref float __result)
        {
            var switchSystem = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

            if (player._object.Is(ModifierEnum.Lighter))
            {
                __result = __instance.MinLightRadius * PlayerControl.GameOptions.ImpostorLightMod;
                return false;
            }

            return false;                
            
        }
    }
}