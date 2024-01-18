using BepInEx.Unity.IL2CPP.Utils;

namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
public static class UpdateSplashPatch
{
    public static TMP_FontAsset Font;

    private static bool Loading;
    private static TextMeshPro TMP;
    private static bool SetCPU;

    public static bool Prefix(SplashManager __instance)
    {
        if (__instance.doneLoadingRefdata && !__instance.startedSceneLoad && Time.time - __instance.startTime > 4.2f && !Loading)
            __instance.StartCoroutine(LoadingScreen(__instance));

        return false;
    }

    public static IEnumerator LoadingScreen(SplashManager __instance)
    {
        if (Loading)
            yield break;

        if (ClientGameOptions.OptimisationMode && !SetCPU)
        {
            SetCPUAffinity();
            SetCPU = true;
            yield return Wait(0.1f);
        }

        Loading = true;
        var loading = new GameObject("LoadingLogo");
        loading.transform.localPosition = new(0f, 1.4f, -5f);
        loading.transform.localScale = new(1f, 1f, 1f);
        var rend = loading.AddComponent<SpriteRenderer>();
        rend.sprite = GetSprite("Banner");
        rend.transform.localScale = Vector3.one * 1.8f;
        rend.color = UColor.clear;
        var num = 0f;

        while (num < 1f)
        {
            num += Time.deltaTime;
            rend.color = UColor.white.AlphaMultiplied(num);
            yield return new WaitForEndOfFrame();
        }

        rend.color = UColor.white;
        TMP = UObject.Instantiate(__instance.errorPopup.InfoText, loading.transform);
        TMP.transform.localPosition = new(0f, -1.5f, -10f);
        TMP.fontStyle = FontStyles.Bold;
        TMP.SetText("Loading...");
        TMP.color = UColor.clear;
        TMP.transform.localScale /= 2f;
        Font = TMP.font;
        num = 0f;

        while (num < 1f)
        {
            num += Time.deltaTime;
            TMP.color = UColor.white.AlphaMultiplied(num);
            yield return new WaitForEndOfFrame();
        }

        yield return HatsLoader.Instance.CoFetch();
        yield return VisorsLoader.Instance.CoFetch();
        yield return NameplatesLoader.Instance.CoFetch();
        yield return ColorsLoader.Instance.CoFetch();
        yield return TranslationsLoader.Instance.CoFetch();

        SetText("Setting Mod Data");
        ModCompatibility.Init();
        yield return Wait(0.1f);

        yield return ModUpdater.CheckForUpdate("Reworked");
        //yield return ModUpdater.CheckForUpdate("Submerged");
        yield return ModUpdater.CheckForUpdate("LevelImpostor");

        ModUpdater.CanDownloadSubmerged = !SubLoaded && ModUpdater.URLs.ContainsKey("Submerged");
        ModUpdater.CanDownloadLevelImpostor = !LILoaded && ModUpdater.URLs.ContainsKey("LevelImpostor");

        Generate.GenerateAll();
        Info.SetAllInfo();
        ExtraRegions.UpdateRegions();

        if (!Directory.Exists(TownOfUsReworked.Options))
            Directory.CreateDirectory(TownOfUsReworked.Options);

        Presets.ForEach(x => SaveText(x.AddSpaces(), ReadResourceText(x, "Presets"), TownOfUsReworked.Options));

        //ClientMenu.SetUpClientOptions();

        yield return Wait(1.5f);

        SetText("Loaded!");
        yield return Wait(0.1f);

        for (var i = 0; i < 4; i++)
        {
            SetText("");
            yield return Wait(0.1f);
            SetText("Loaded!");
        }

        num = 0.5f;

        while (num > 0f)
        {
            num -= Time.deltaTime * 2;
            TMP.color = UColor.white.AlphaMultiplied(num);
            yield return new WaitForEndOfFrame();
        }

        SetText("");
        num = 1f;

        while (num > 0f)
        {
            num -= Time.deltaTime;
            rend.color = UColor.white.AlphaMultiplied(num);
            yield return new WaitForEndOfFrame();
        }

        rend.color = UColor.clear;
        rend.Destroy();
        loading.Destroy();
        yield return Wait(0.1f);

        __instance.sceneChanger.AllowFinishLoadingScene();
        __instance.startedSceneLoad = true;
        yield break;
    }

    public static void SetText(string text) => TMP.SetText(text);
}