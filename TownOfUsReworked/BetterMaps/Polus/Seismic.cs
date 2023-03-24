using HarmonyLib;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.BetterMaps.Polus
{
    [HarmonyPatch(typeof(ReactorSystemType), nameof(ReactorSystemType.RepairDamage))]
    public static class Seismic
    {
        public static bool Prefix(ReactorSystemType __instance, byte opCode)
        {
            if (ShipStatus.Instance.Type == ShipStatus.MapType.Pb && opCode == 128 && !__instance.IsActive)
            {
                __instance.Countdown = CustomGameOptions.SeismicTimer;
                __instance.UserConsolePairs.Clear();
                __instance.IsDirty = true;
                return false;
            }

            return true;
        }
    }
}