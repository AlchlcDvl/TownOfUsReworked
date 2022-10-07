using HarmonyLib;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(Constants), nameof(Constants.ShouldHorseAround))]
    class HorseModePatch
    {
        public static bool Prefix(ref bool __result)
        {
            __result = ClientOptions.HorseEnabled;
            return false;
        }
    }
}
