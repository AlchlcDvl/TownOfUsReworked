using HarmonyLib;
using UnityEngine;
using TMPro;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static class VersionShowerPatch
    {
        public static string Credentials = $"{TownOfUsReworked.versionFinal}\n<size=80%>Created by <color=#C50000>AlchlcDvl</color></size>";

        private static void Postfix(VersionShower __instance)
        {
            GameObject gameObject = GameObject.Find("bannerLogo_AmongUs");

            if (gameObject != null)
            {
                var textMeshPro = UnityEngine.Object.Instantiate<TextMeshPro>(__instance.text);
                textMeshPro.transform.position = new Vector3(0f, -0.3f, 0f);
                textMeshPro.text = Credentials;
                textMeshPro.alignment = TextAlignmentOptions.Center;
                textMeshPro.fontSize *= 0.75f;
                textMeshPro.fontStyle = FontStyles.Bold;
                textMeshPro.transform.SetParent(gameObject.transform);
            }
        }
    }
}