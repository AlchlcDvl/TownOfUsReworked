namespace TownOfUsReworked.Patches.UI;

[HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
public static class UpdateSplashPatch
{
    public static SpriteRenderer Rend;
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

        Rend = loading.AddComponent<SpriteRenderer>();
        Rend.sprite = GetSprite("Banner");
        Rend.transform.localScale = Vector3.one * 1.8f;
        Rend.color = UColor.clear;
        var cache = Rend.material;

        yield return PerformTimedAction(1f, p => Rend.color = UColor.white.SetAlpha(p));

        Rend.color = UColor.white;
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
        yield return UpdateManager.CheckForUpdates();
        yield return LoadModData();

        Rend.material = cache;

        SetText("Loaded!");

        yield return Wait(0.5f);

        yield return PerformTimedAction(0.5f, p => TMP.color = UColor.white.SetAlpha(1 - p));

        SetText("");

        yield return PerformTimedAction(1f, p => Rend.color = UColor.white.SetAlpha(1 - p));

        Rend.color = UColor.clear;
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
        UpdateRegions();

        yield return null;
    }

    private static IEnumerator PreLoadModData()
    {
        SetText("Pre-Setting Mod Data");
        Message("Pre-setting mod data");

        ReactorCredits.Register<TownOfUsReworked>(x => x is ReactorCredits.Location.MainMenu);
        NormalGameOptionsV09.MinPlayers = Enumerable.Repeat(1, 250).ToArray();
        HideNSeekGameOptionsV09.MinPlayers = Enumerable.Repeat(1, 250).ToArray();
        AllMonos.RegisterMonos();

        if (!Directory.Exists(TownOfUsReworked.Assets))
            Directory.CreateDirectory(TownOfUsReworked.Assets);

        if (!Directory.Exists(TownOfUsReworked.Other))
            Directory.CreateDirectory(TownOfUsReworked.Other);

        if (TownOfUsReworked.IsDev && !Directory.Exists(TownOfUsReworked.Hashes))
            Directory.CreateDirectory(TownOfUsReworked.Hashes);

        if (!Directory.Exists(TownOfUsReworked.Logs))
            Directory.CreateDirectory(TownOfUsReworked.Logs);

        Directory.EnumerateFiles(TownOfUsReworked.Logs).Do(File.Delete);

        yield return null;
    }

    public static void UpdateRegions()
    {
        var mna = new StaticHttpRegionInfo("Modded NA (MNA)", StringNames.NoTranslation, "www.aumods.org", new([new("Http-1", "https://www.aumods.org", 443, false)])).Cast<IRegionInfo>();
        var meu = new StaticHttpRegionInfo("Modded EU (MEU)", StringNames.NoTranslation, "au-eu.duikbo.at", new([new("Http-1", "https://au-eu.duikbo.at", 443, false)])).Cast<IRegionInfo>();
        var mas = new StaticHttpRegionInfo("Modded Asia (MAS)", StringNames.NoTranslation, "au-as.duikbo.at", new([new("Http-1", "https://au-as.duikbo.at", 443, false)])).Cast<IRegionInfo>();
        var custom = new StaticHttpRegionInfo("Custom", StringNames.NoTranslation, TownOfUsReworked.Ip.Value, new([new("Custom", TownOfUsReworked.Ip.Value, TownOfUsReworked.Port.Value,
            false)])).Cast<IRegionInfo>();

        var iRegionInfo1 = ServerManager.Instance.CurrentRegion;

        foreach (var iRegionInfo2 in new[] { mna, meu, mas, custom })
        {
            if (iRegionInfo2 is null)
                Error("Could not add region");
            else
            {
                if (iRegionInfo1 is not null && iRegionInfo2.Name.Equals(iRegionInfo1.Name, StringComparison.OrdinalIgnoreCase))
                    iRegionInfo1 = iRegionInfo2;

                ServerManager.Instance.AddOrUpdateRegion(iRegionInfo2);
            }
        }

        if (iRegionInfo1 is null)
            return;

        Info("Resetting previous region");
        ServerManager.Instance.SetRegion(iRegionInfo1);
    }
}