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
        if (__instance.text == null || !AmongUsClient.Instance || IsFreePlay())
            return true;

        if (!__instance.GetComponentInChildren<SpriteRenderer>())
        {
            var logo = new GameObject("PingLogo") { layer = 5 };
            logo.AddComponent<SpriteRenderer>().sprite = GetSprite("SettingsButton");
            logo.transform.SetParent(__instance.transform);
            logo.transform.localPosition = new(-1f, -0.5f, -1f);
            logo.transform.localScale *= 0.5f;
        }

        // try catch my beloved <3
        try
        {
            DeltaTime += (Time.deltaTime - DeltaTime) * 0.1f;
            var fps = Mathf.Ceil(1f / DeltaTime);
            var host = GameData.Instance?.GetHost();
            __instance.text.text = $"<size=80%>Ping: {AmongUsClient.Instance.Ping}ms FPS: {fps}" + (host !=  null ? $"\nHost: {host.PlayerName}" : "") +
                "</size>";
        } catch {}

        return false;
    }
}