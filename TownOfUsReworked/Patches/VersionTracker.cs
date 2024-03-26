namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
public static class VersionShowerPatch
{
    public static TextMeshPro ModVersion;

    public static void Postfix(VersionShower __instance)
    {
        __instance.text.text += $" - <color=#C50000FF>Reworked</color> {TownOfUsReworked.VersionFinal}";
        var gameObject = GameObject.Find("LOGO-AU");

        if (gameObject && !ModVersion)
        {
            ModVersion = UObject.Instantiate(__instance.text, MainMenuStartPatch.Logo.transform);
            ModVersion.transform.localPosition = new(0, -2f, 0);
            ModVersion.text = $"<size=175%><b>{TownOfUsReworked.VersionFinal}\nBy <color=#C50000FF>AlchlcSystm</color></b></size>";
            ModVersion.alignment = TextAlignmentOptions.Center;
            ModVersion.fontStyle = FontStyles.Bold;
            ModVersion.font = GetFont("Placeholder");
            ModVersion.name = "ModVersionText";
        }
    }
}

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
public static class PingTracker_Update
{
    private static float DeltaTime;

    public static bool Prefix(PingTracker __instance)
    {
        if (!__instance || __instance.text == null || !AmongUsClient.Instance || IsFreePlay)
            return true;

        if (!__instance.GetComponentInChildren<SpriteRenderer>())
        {
            var logo = new GameObject("PingLogo") { layer = 5 };
            logo.AddComponent<SpriteRenderer>().sprite = GetSprite("SettingsButton");
            logo.transform.SetParent(__instance.transform);
            logo.transform.localPosition = new(-1f, -0.5f, -1f);
            logo.transform.localScale *= 0.5f;
        }

        DeltaTime += (Time.deltaTime - DeltaTime) * 0.1f;
        var fps = Mathf.Ceil(1f / DeltaTime);

        // try catch my beloved <3
        try
        {
            var host = GameData.Instance?.GetHost();
            __instance.text.text = $"<size=80%>Ping: {AmongUsClient.Instance.Ping}ms FPS: {fps}\n" +
                "<b><color=#00FF00FF>TownOfUs</color><color=#FF00FFFF>Reworked</color></b>\n" +
                $"{(!Meeting ? $"<color=#0000FFFF>{TownOfUsReworked.VersionFinal}</color>\n<color=#C50000FF>By: AlchlcDvl</color>\n" : "")}" +
                (TownOfUsReworked.MCIActive ? (IsLobby ? $"Lobby {(TownOfUsReworked.LobbyCapped ? "C" : "Unc")}apped\nRobots{(TownOfUsReworked.Persistence ? "" : " Don't")} Persist\n" : "") :
                "") + (host != null ? $"Host: {host.PlayerName}</size>" : "");
        } catch {}

        return false;
    }
}