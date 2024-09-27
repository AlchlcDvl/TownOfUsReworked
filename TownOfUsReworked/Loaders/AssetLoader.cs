namespace TownOfUsReworked.Loaders;

public abstract class AssetLoader
{
    public static string RepositoryUrl => $"https://raw.githubusercontent.com/AlchlcDvl/ReworkedAssets/{(TownOfUsReworked.IsDev ? "dev" : "main")}";

    public virtual string Manifest => "";
    public virtual string DirectoryInfo => "";
    public virtual string FileExtension => "";
    public virtual bool Downloading => false;

    public IEnumerator CoDownloadAsset(IEnumerable<string> files)
    {
        foreach (var fileName in files)
        {
            var trueName = $"{fileName.Replace(" ", "%20")}{(IsNullEmptyOrWhiteSpace(FileExtension) ? "" : $".{FileExtension}")}";
            Message($"Downloading: {Manifest}/{fileName}");
            var www = UnityWebRequest.Get($"{RepositoryUrl}/{Manifest}/{trueName}");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Error(www.error);
                www.downloadHandler.Dispose();
                www.Dispose();
                yield break;
            }

            var filePath = Path.Combine(DirectoryInfo, trueName);
            filePath = filePath.Replace("%20", " ");
            var persistTask = File.WriteAllBytesAsync(filePath, www.downloadHandler.data);

            while (!persistTask.IsCompleted)
            {
                if (persistTask.Exception != null)
                {
                    Error(persistTask.Exception);
                    break;
                }

                yield return EndFrame();
            }

            www.downloadHandler.Dispose();
            www.Dispose();
            yield return EndFrame();
        }

        yield break;
    }

    public static IEnumerator InitLoaders()
    {
        UpdateSplashPatch.SetText("Initialising Loaders");
        ColorLoader.Instance = new();
        HatLoader.Instance = new();
        ImageLoader.Instance = new();
        NameplateLoader.Instance = new();
        PortalLoader.Instance = new();
        PresetLoader.Instance = new();
        SoundLoader.Instance = new();
        TranslationLoader.Instance = new();
        VisorLoader.Instance = new();
        BundleLoader.Instance = new();
        yield return EndFrame();
        yield break;
    }

    public virtual IEnumerator BeginDownload(object response) => EndFrame();

    public virtual IEnumerator AfterLoading(object response) => EndFrame();
}

public abstract class AssetLoader<T> : AssetLoader where T : Asset
{
    private bool Running;

    public IEnumerator CoFetch()
    {
        if (Running)
            yield break;

        Running = true;
        UpdateSplashPatch.SetText($"Fetching {Manifest}");
        yield return EndFrame();

        if (!Directory.Exists(DirectoryInfo))
            Directory.CreateDirectory(DirectoryInfo);

        Message($"Downloading manifest at: {RepositoryUrl}/{Manifest}.json");
        var www = UnityWebRequest.Get($"{RepositoryUrl}/{Manifest}.json");
        yield return www.SendWebRequest();

        var isError = www.result != UnityWebRequest.Result.Success;
        var jsonText = isError ? ReadDiskText($"{Manifest}.json", DirectoryInfo) : www.downloadHandler.text;

        if (isError)
            Error(www.error);
        else
        {
            var task = File.WriteAllTextAsync(Path.Combine(DirectoryInfo, $"{Manifest}.json"), www.downloadHandler.text);

            while (!task.IsCompleted)
            {
                if (task.Exception != null)
                {
                    Error(task.Exception);
                    break;
                }

                yield return EndFrame();
            }
        }

        www.downloadHandler.Dispose();
        www.Dispose();

        if (IsNullEmptyOrWhiteSpace(jsonText) && !isError)
        {
            jsonText = ReadDiskText($"{Manifest}.json", DirectoryInfo);
            Warning($"Online JSON for {Manifest} was missing");
        }

        if (IsNullEmptyOrWhiteSpace(jsonText))
        {
            Error($"Unable to load online or local JSON data for {Manifest}");
            yield break;
        }

        var response = JsonSerializer.Deserialize<List<T>>(jsonText);

        if (Downloading)
        {
            UpdateSplashPatch.SetText($"Downloading {Manifest}");
            yield return BeginDownload(response);
        }

        UpdateSplashPatch.SetText($"Preloading {Manifest}");
        yield return AfterLoading(response);
        yield return EndFrame();
        yield break;
    }
}