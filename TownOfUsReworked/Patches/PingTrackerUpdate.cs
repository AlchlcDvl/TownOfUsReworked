using HarmonyLib;
using UnityEngine;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        [HarmonyPostfix]
        public static void Postfix(PingTracker __instance)
        {
            var position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(3.6f, 0.1f, 0);
            position.AdjustPosition();

            __instance.text.text = $"<color=#00FF00FF>TownOfUs</color><color=#FF00FFFF>Reworked</color>\n<color=#0000FFFF>v{TownOfUsReworked.versionFinal}</color>\n" +
                $"Ping: {AmongUsClient.Instance.Ping}ms\n" + (!MeetingHud.Instance ? "<color=#FF00FFFF>Reworked By: AlchlcDvl</color>" : "");
        }
    }
}
