namespace TownOfUsReworked.Loaders;

public abstract class AssetLoader
{
    public static readonly string RepositoryUrl = $"https://raw.githubusercontent.com/AlchlcDvl/ReworkedAssets/{(TownOfUsReworked.IsDev ? "dev" : "main")}";

    public virtual string Manifest => "";
    public virtual string DirectoryInfo => "";
    public virtual string FileExtension => "";
    public virtual bool Downloading => false;

    public IEnumerator CoDownloadAssets(IEnumerable<string> files)
    {
        var count = files.Count();

        if (count == 0)
            yield break;

        Message($"Downloading {count} files");
        var i = 0;

        foreach (var fileName in files)
        {
            i++;
            UpdateSplashPatch.SetText($"Downloading {Manifest} ({i}/{count})");
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
}