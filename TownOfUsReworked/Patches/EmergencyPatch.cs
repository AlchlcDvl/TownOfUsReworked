using HarmonyLib;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.Patches
{
    //Yet another instance of me stealing twix's code lmaoo
    [HarmonyPatch]
    public static class EmergencyPatch
    {
        private static float Time { get; set; }

        [HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.Begin))]
        [HarmonyPostfix]
        public static void Postfix(EmergencyMinigame __instance)
        {
            if (Time < CustomGameOptions.InitialCooldowns)
                __instance.ForceClose();
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]
        public static void Postfix2()
        {
            if (!GameManager.Instance.GameHasStarted || Time >= CustomGameOptions.InitialCooldowns)
                return;

            Time += UnityEngine.Time.deltaTime;
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        [HarmonyPostfix]
        public static void Postfix3() => Time = 0f;
    }
}