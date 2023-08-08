namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
public static class VersionShowerPatch
{
    public static TextMeshPro ModVersion;

    public static void Postfix(VersionShower __instance)
    {
        var gameObject = GameObject.Find("LOGO-AU");

        if (gameObject && !ModVersion)
        {
            ModVersion = UObject.Instantiate(__instance.text, MainMenuStartPatch.Logo.transform);
            ModVersion.transform.localPosition = new(0, -2f, 0);
            ModVersion.text = $"<size=175%><b>{TownOfUsReworked.VersionFinal}\nCreated by <color=#C50000FF>AlchlcDvl</color></b></size>";
            ModVersion.alignment = TextAlignmentOptions.Center;
            ModVersion.fontStyle = FontStyles.Bold;
            ModVersion.name = "ModVersion";
        }
    }
}

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
public static class PingTracker_Update
{
    private static float deltaTime;

    public static void Prefix(PingTracker __instance)
    {
        if (!__instance.GetComponentInChildren<SpriteRenderer>())
        {
            var logo = new GameObject("Logo") { layer = 5 };
            logo.AddComponent<SpriteRenderer>().sprite = GetSprite("SettingsButton");
            logo.transform.SetParent(__instance.transform);
            logo.transform.localPosition = new(-1f, -0.5f, -1);
            logo.transform.localScale *= 0.5f;
        }
    }

    public static void Postfix(PingTracker __instance)
    {
        if (!__instance || __instance.text == null)
            return;

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        var fps = Mathf.Ceil(1.0f / deltaTime);
        var position = __instance.GetComponent<AspectPosition>();
        position.DistanceFromEdge = new(3.6f, 0.1f, 0);
        position.AdjustPosition();
        var host = GameData.Instance.GetHost();

        __instance.text.text = $"<size=80%>Ping: {AmongUsClient.Instance?.Ping}ms FPS: {fps}\n" +
            "<b><color=#00FF00FF>TownOfUs</color><color=#FF00FFFF>Reworked</color></b>\n" +
            $"{(!Meeting ? $"<color=#0000FFFF>{TownOfUsReworked.VersionFinal}</color>\n" : "")}" +
            $"{(!Meeting ? "<color=#C50000FF>By: AlchlcDvl</color>\n" : "")}" + (TownOfUsReworked.MCIActive ? (IsLobby ?
            $"Lobby {(TownOfUsReworked.LobbyCapped ? "C" : "Unc")}apped\nRobots{(TownOfUsReworked.Persistence ? "" : " Don't")} Persist\n" : "") : "") +
            $"Host: {host.PlayerName}</size>";
    }
}