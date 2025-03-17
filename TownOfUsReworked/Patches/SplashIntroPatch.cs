namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
public static class UpdateSplashPatch
{
    private static bool Loading;
    private static TextMeshPro TMP;

    public static bool Prefix(SplashManager __instance)
    {
        if (__instance.doneLoadingRefdata && !__instance.startedSceneLoad && Time.time - __instance.startTime > 4.2f && !Loading)
            Coroutines.Start(LoadingScreen(__instance));

        return false;
    }

    private static IEnumerator LoadingScreen(SplashManager __instance)
    {
        if (Loading)
            yield break;

        Loading = true;
        LoadAssets();

        var loading = new GameObject("Loading")
        {
            transform =
            {
                localPosition = new(0f, 1.4f, -5f),
                localScale = new(1f, 1f, 1f)
            }
        };

        var rend = loading.AddComponent<SpriteRenderer>();
        rend.sprite = GetSprite("Banner");
        rend.transform.localScale = Vector3.one * 1.8f;
        rend.color = UColor.clear;

        yield return PerformTimedAction(1f, p => rend.color = UColor.white.SetAlpha(p));

        rend.color = UColor.white;
        TMP = UObject.Instantiate(__instance.errorPopup.InfoText, loading.transform);
        TMP.transform.localPosition = new(0f, -1.5f, -10f);
        TMP.fontStyle = FontStyles.Bold;
        TMP.color = UColor.clear;
        TMP.transform.localScale /= 2f;
        AddAsset("Placeholder", TMP.font);

        SetText("Loading...");

        yield return PerformTimedAction(0.5f, p => TMP.color = UColor.white.SetAlpha(p));

        yield return PreLoadModData();
        yield return AssetLoader.RunLoaders();
        yield return ModUpdater.CheckForUpdates();
        yield return LoadModData();

        SetText("Loaded!");

        yield return Wait(0.5f);

        yield return PerformTimedAction(0.5f, p => TMP.color = UColor.white.SetAlpha(1 - p));

        SetText("");

        yield return PerformTimedAction(1f, p => rend.color = UColor.white.SetAlpha(1 - p));

        rend.color = UColor.clear;
        loading.Destroy();
        yield return Wait(0.1f);

        __instance.sceneChanger.AllowFinishLoadingScene();
        __instance.startedSceneLoad = true;
    }

    public static void SetText(string text) => TMP.text = text;

    private static IEnumerator LoadModData()
    {
        SetText("Setting Mod Data");
        Message("Setting mod data");

        ReworkedDataManager.Setup();
        Generate.GenerateAll();
        Modules.Info.SetAllInfo();
        RegionInfoOpenPatch.UpdateRegions();

        yield return EndFrame();
    }

    private static IEnumerator PreLoadModData()
    {
        SetText("Pre-Setting Mod Data");
        Message("Pre-setting mod data");

        ReactorCredits.Register<TownOfUsReworked>(x => x is ReactorCredits.Location.MainMenu);
        NormalGameOptionsV08.MinPlayers = Enumerable.Repeat(1, 250).ToArray();
        HideNSeekGameOptionsV08.MinPlayers = Enumerable.Repeat(1, 250).ToArray();
        ReworkedStart = TranslationManager.GetOrAddName("Translation.ReworkedStart");
        AllMonos.RegisterMonos();

        if (!Directory.Exists(TownOfUsReworked.Assets))
            Directory.CreateDirectory(TownOfUsReworked.Assets);

        if (!Directory.Exists(TownOfUsReworked.Other))
            Directory.CreateDirectory(TownOfUsReworked.Other);

        if (TownOfUsReworked.IsDev && !Directory.Exists(TownOfUsReworked.Hashes))
            Directory.CreateDirectory(TownOfUsReworked.Hashes);

        if (!Directory.Exists(TownOfUsReworked.Logs))
            Directory.CreateDirectory(TownOfUsReworked.Logs);

        Directory.EnumerateFiles(TownOfUsReworked.Logs).ForEach(File.Delete);

        yield return EndFrame();
    }
}