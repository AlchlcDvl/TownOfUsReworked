using HarmonyLib;
using TownOfUsReworked.Lobby.Extras;

namespace TownOfUsReworked.Patches
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
