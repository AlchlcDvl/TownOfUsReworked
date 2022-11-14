using HarmonyLib;

namespace TownOfUsReworked.Lobby.Extras.CustomHats.Patches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Awake))]
    public class AmongUsClient_Patches
    {
        private static bool _executed;
        
        public static void Prefix()
        {
            if (!_executed)
            {
                HatLoader.LoadHats();
                _executed = true;
            }
        }        
    }
}