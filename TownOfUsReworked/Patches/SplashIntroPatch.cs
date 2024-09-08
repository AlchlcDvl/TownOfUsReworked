using TownOfUsReworked.Loaders;

namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
public static class UpdateSplashPatch
{
    private static bool Loading;
    private static TextMeshPro TMP;
    private static bool DataSet;

    public static bool Prefix(SplashManager __instance)
    {
        if (__instance.doneLoadingRefdata && !__instance.startedSceneLoad && Time.time - __instance.startTime > 4.2f)
            Coroutines.Start(LoadingScreen(__instance));

        return false;
    }

    private static IEnumerator LoadingScreen(SplashManager __instance)
    {
        if (Loading)
            yield break;

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
            yield return EndFrame();
        }

        rend.color = UColor.white;
        TMP = UObject.Instantiate(__instance.errorPopup.InfoText, loading.transform);
        TMP.transform.localPosition = new(0f, -1.5f, -10f);
        TMP.fontStyle = FontStyles.Bold;
        TMP.color = UColor.clear;
        TMP.transform.localScale /= 2f;
        AddAsset("Placeholder", TMP.font);

        SetText("Loading...");
        yield return EndFrame();

        num = 0f;

        while (num < 1f)
        {
            num += Time.deltaTime;
            TMP.color = UColor.white.AlphaMultiplied(num);
            yield return EndFrame();
        }

        if (!Directory.Exists(TownOfUsReworked.Assets))
            Directory.CreateDirectory(TownOfUsReworked.Assets);

        if (!Directory.Exists(TownOfUsReworked.Other))
            Directory.CreateDirectory(TownOfUsReworked.Other);

        if (!Directory.Exists(TownOfUsReworked.Logs))
            Directory.CreateDirectory(TownOfUsReworked.Logs);

        Directory.EnumerateFiles(TownOfUsReworked.Logs).ForEach(File.Delete);

        yield return AssetLoader.InitLoaders();
        yield return HatLoader.Instance.CoFetch();
        yield return VisorLoader.Instance.CoFetch();
        yield return NameplateLoader.Instance.CoFetch();
        yield return ColorLoader.Instance.CoFetch();
        yield return TranslationLoader.Instance.CoFetch();
        yield return PresetLoader.Instance.CoFetch();
        yield return ImageLoader.Instance.CoFetch();
        yield return PortalLoader.Instance.CoFetch();
        yield return SoundLoader.Instance.CoFetch();
        yield return BundleLoader.Instance.CoFetch();

        yield return ModUpdater.CheckForUpdate("Reworked");
        yield return ModUpdater.CheckForUpdate("Submerged");
        yield return ModUpdater.CheckForUpdate("LevelImpostor");

        yield return LoadModData();

        SetText("Loaded!");
        yield return Wait(0.5f);

        num = 0.5f;

        while (num > 0f)
        {
            num -= Time.deltaTime * 2;
            TMP.color = UColor.white.AlphaMultiplied(num);
            yield return EndFrame();
        }

        SetText("");
        num = 1f;

        while (num > 0f)
        {
            num -= Time.deltaTime;
            rend.color = UColor.white.AlphaMultiplied(num);
            yield return EndFrame();
        }

        rend.color = UColor.clear;
        loading.Destroy();
        yield return Wait(0.1f);

        __instance.sceneChanger.AllowFinishLoadingScene();
        __instance.startedSceneLoad = true;
        yield break;
    }

    public static void SetText(string text) => TMP.SetText(text);

    private static IEnumerator LoadModData()
    {
        if (DataSet)
            yield break;

        SetText("Setting Mod Data");
        LogMessage("Setting mod data");

        ModUpdater.CanDownloadSubmerged = !SubLoaded && ModUpdater.URLs.ContainsKey("Submerged");
        ModUpdater.CanDownloadLevelImpostor = !LILoaded && ModUpdater.URLs.ContainsKey("LevelImpostor");

        Generate.GenerateAll();
        Info.SetAllInfo();
        RegionInfoOpenPatch.UpdateRegions();

        DataSet = true;

        yield return EndFrame();
        yield break;
    }
}