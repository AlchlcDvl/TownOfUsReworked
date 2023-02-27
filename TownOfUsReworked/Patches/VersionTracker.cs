using HarmonyLib;
using UnityEngine;
using TMPro;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static class VersionShowerPatch
    {
        private static void Postfix(VersionShower __instance)
        {
            GameObject gameObject = GameObject.Find("bannerLogo_AmongUs");

            if (gameObject != null)
            {
                var textMeshPro = UnityEngine.Object.Instantiate<TextMeshPro>(__instance.text);
                textMeshPro.transform.position = new Vector3(0f, -0.3f, 0f);
                textMeshPro.text = $"{TownOfUsReworked.versionFinal}\n<size=80%>Created by <color=#C50000>AlchlcDvl</color></size>";
                textMeshPro.alignment = TextAlignmentOptions.Center;
                textMeshPro.fontSize *= 0.75f;
                textMeshPro.fontStyle = FontStyles.Bold;
                textMeshPro.transform.SetParent(gameObject.transform);
            }
        }
    }

    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        [HarmonyPostfix]
        public static void Postfix(PingTracker __instance)
        {
            var position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(3.6f, 0.1f, 0);
            position.AdjustPosition();

            __instance.text.text = $"<color=#00FF00FF>TownOfUs</color><color=#FF00FFFF>Reworked</color>\n<color=#0000FFFF>{TownOfUsReworked.versionFinal}</color>\n" +
                $"Ping: {AmongUsClient.Instance.Ping}ms\n" + (!MeetingHud.Instance ? "<color=#FF00FFFF>Reworked By: AlchlcDvl</color>" : "");
        }
    }
}