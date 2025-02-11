namespace TownOfUsReworked.Loaders;

public abstract class AssetLoader
{
    protected static readonly string RepositoryUrl = $"https://raw.githubusercontent.com/AlchlcDvl/ReworkedAssets/{(TownOfUsReworked.IsDev ? "dev" : "main")}";

    protected abstract string Manifest { get; }

    protected virtual string DirectoryInfo => "";
    protected virtual string FileExtension => "";
    protected virtual bool Downloading => false;

    protected IEnumerator CoDownloadAssets(IEnumerable<string> files)
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
            var trueName = fileName.Replace(" ", "%20") + (IsNullEmptyOrWhiteSpace(FileExtension) ? "" : $".{FileExtension}");
            Message($"Downloading: {Manifest}/{fileName}");
            var www = UnityWebRequest.Get($"{RepositoryUrl}/{Manifest}/{trueName}");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Error(www.error);
                www.downloadHandler.Dispose();
                www.Dispose();
                continue;
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
    }

    public static IEnumerator RunLoaders()
    {
        using var hasher = MD5.Create();
        yield return new TranslationLoader().CoFetch(hasher);
        yield return new HatLoader().CoFetch(hasher);
        yield return new VisorLoader().CoFetch(hasher);
        yield return new NameplateLoader().CoFetch(hasher);
        yield return new ColorLoader().CoFetch(hasher);
        yield return new PresetLoader().CoFetch(hasher);
        yield return new ImageLoader().CoFetch(hasher);
        yield return new PortalLoader().CoFetch(hasher);
        yield return new SoundLoader().CoFetch(hasher);
        yield return new BundleLoader().CoFetch(hasher);
    }

    public static bool ShouldDownload(string file, string hash, HashAlgorithm hasher) => IsNullEmptyOrWhiteSpace(hash) || !File.Exists(file) || GenerateHash(file, hasher) != hash;

    protected static string GenerateHash(string file, HashAlgorithm hasher) => File.Exists(file) ? BitConverter.ToString(hasher.ComputeHash(File.ReadAllBytes(file))).Replace("-", "") : null;
}