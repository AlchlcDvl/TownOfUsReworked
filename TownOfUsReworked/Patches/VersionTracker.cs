using HarmonyLib;
using UnityEngine;
using TMPro;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static class VersionShowerPatch
    {
        public static void Postfix(VersionShower __instance)
        {
            var gameObject = GameObject.Find("bannerLogo_AmongUs");

            if (gameObject != null)
            {
                var textMeshPro = Object.Instantiate(__instance.text);
                textMeshPro.transform.position = new Vector3(0f, -0.3f, 0f);
                textMeshPro.text = $"{TownOfUsReworked.versionFinal}\n<size=85%>Created by <color=#C50000FF>AlchlcDvl</color></size>";
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
            __instance.text.text = "<color=#00FF00FF>TownOfUs</color><color=#FF00FFFF>Reworked</color>\n" +
                $"{(!MeetingHud.Instance ? $"<color=#0000FFFF>{TownOfUsReworked.versionFinal}</color>\n" : "")}" +
                $"{(!MeetingHud.Instance ? "<color=#C50000FF>By: AlchlcDvl</color>\n" : "")}" +
                $"Ping: {AmongUsClient.Instance.Ping}ms\n" + (TownOfUsReworked.MCIActive ?
                (GameStates.IsLobby ? $"Lobby {(TownOfUsReworked.LobbyCapped ? "C" : "Unc")}apped\nRobots{(TownOfUsReworked.Persistence ? "" : " Don't")} Persist" : "") : "");
        }
    }
}