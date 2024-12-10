namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
public static class VersionShowerPatch
{
    public static TextMeshPro ModVersion;

    public static void Postfix(VersionShower __instance)
    {
        __instance.text.text += $" - <#C50000FF>Reworked</color> {TownOfUsReworked.VersionFinal}";
        var gameObject = GameObject.Find("LOGO-AU");

        if (gameObject && !ModVersion)
        {
            ModVersion = UObject.Instantiate(__instance.text, MainMenuPatches.Logo.transform);
            ModVersion.transform.localPosition = new(0, -2f, 0);
            ModVersion.SetText($"<size=175%><b>{TownOfUsReworked.VersionFinal}\nBy <#C50000FF>AlchlcSystm</color></b></size>");
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
    private static GameObject PingLogo;

    public static bool Prefix(PingTracker __instance)
    {
        if (!__instance.text || !AmongUsClient.Instance)
            return true;

        if (!PingLogo)
        {
            PingLogo = new GameObject("PingLogo") { layer = 5 };
            PingLogo.AddComponent<SpriteRenderer>().sprite = GetSprite("SettingsButton");
            PingLogo.transform.SetParent(__instance.transform);
            PingLogo.transform.localPosition = new(-1f, -0.5f, -1f);
            PingLogo.transform.localScale *= 0.5f;
        }

        __instance.text.SetText($"<size=80%>Ping: {AmongUsClient.Instance.Ping}ms FPS: {Mathf.Round(1f / Time.deltaTime)}</size>");
        return false;
    }
}