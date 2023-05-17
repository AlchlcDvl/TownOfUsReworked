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
                var textMeshPro = UObject.Instantiate(__instance.text);
                textMeshPro.transform.position = new(0f, -0.85f, 0f);
                textMeshPro.text = $"{TownOfUsReworked.versionFinal}\n<size=85%>Created by <color=#C50000FF>AlchlcDvl</color></size>";
                textMeshPro.alignment = TextAlignmentOptions.Center;
                textMeshPro.fontSize *= 0.75f;
                textMeshPro.fontStyle = FontStyles.Bold;
                textMeshPro.transform.parent = gameObject.transform;
            }
        }
    }

    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        public static void Postfix(PingTracker __instance)
        {
            __instance.text.text = "<size=80%><b><color=#00FF00FF>TownOfUs</color><color=#FF00FFFF>Reworked</color></b>\n" +
                $"{(!MeetingHud.Instance ? $"<color=#0000FFFF>{TownOfUsReworked.versionFinal}</color>\n" : "")}" +
                $"{(!MeetingHud.Instance ? "<color=#C50000FF>By: AlchlcDvl</color>\n" : "")}" +
                $"Ping: {AmongUsClient.Instance.Ping}ms\n" + (TownOfUsReworked.MCIActive ?
                (ConstantVariables.IsLobby ? $"Lobby {(TownOfUsReworked.LobbyCapped ? "C" : "Unc")}apped\nRobots{(TownOfUsReworked.Persistence ? "" : " Don't")} Persist" : "") : "") +
                "</size>";
        }

        public static void Prefix(PingTracker __instance)
        {
            if (!__instance.GetComponentInChildren<SpriteRenderer>())
            {
                var logo = new GameObject("Logo") { layer = 5 };
                logo.AddComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("SettingsButton");
                logo.transform.parent = __instance.transform;
                logo.transform.localPosition = new(-1f, -0.5f, -1);
                logo.transform.localScale *= 0.5f;
            }
        }
    }
}