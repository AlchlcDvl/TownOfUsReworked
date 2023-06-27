namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static class VersionShowerPatch
    {
        public static TextMeshPro ModVersion;

        public static void Postfix(VersionShower __instance)
        {
            var gameObject = GameObject.Find("LOGO-AU");

            if (gameObject && !ModVersion)
            {
                ModVersion = UObject.Instantiate(__instance.text);
                var pos = MainMenuStartPatch.Logo.transform.position;
                pos.y -= 2f;
                ModVersion.transform.position = pos;
                ModVersion.text = $"<size=175%><b>{TownOfUsReworked.VersionFinal}\nCreated by <color=#C50000FF>AlchlcDvl</color></b></size>";
                ModVersion.alignment = TextAlignmentOptions.Center;
                ModVersion.fontStyle = FontStyles.Bold;
                ModVersion.name = "ModVersion";
                ModVersion.transform.SetParent(MainMenuStartPatch.Logo.transform.parent);
            }
        }
    }

    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        private static float deltaTime;

        public static void Postfix(PingTracker __instance)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            var fps = Mathf.Ceil(1.0f / deltaTime);

            __instance.text.text = $"Ping: {AmongUsClient.Instance.Ping}ms FPS: {fps}\n" +
                "<size=80%><b><color=#00FF00FF>TownOfUs</color><color=#FF00FFFF>Reworked</color></b>\n" +
                $"{(!MeetingHud.Instance ? $"<color=#0000FFFF>{TownOfUsReworked.VersionFinal}</color>\n" : "")}" +
                $"{(!MeetingHud.Instance ? "<color=#C50000FF>By: AlchlcDvl</color>\n" : "")}" + (TownOfUsReworked.MCIActive ? (ConstantVariables.IsLobby ?
                $"Lobby {(TownOfUsReworked.LobbyCapped ? "C" : "Unc")}apped\nRobots{(TownOfUsReworked.Persistence ? "" : " Don't")} Persist" : "") : "") + "</size>";
        }

        public static void Prefix(PingTracker __instance)
        {
            if (!__instance.GetComponentInChildren<SpriteRenderer>())
            {
                var logo = new GameObject("Logo") { layer = 5 };
                logo.AddComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("SettingsButton");
                logo.transform.SetParent(__instance.transform);
                logo.transform.localPosition = new(-1f, -0.5f, -1);
                logo.transform.localScale *= 0.5f;
            }
        }
    }
}