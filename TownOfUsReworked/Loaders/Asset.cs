// ReSharper disable HeuristicUnreachableCode
namespace TownOfUsReworked.Loaders;

public abstract class AssetLoader
{
    protected static readonly string RepositoryUrl = $"https://raw.githubusercontent.com/AlchlcDvl/ReworkedAssets/{(TownOfUsReworked.IsDev ? "dev" : "main")}";

    protected abstract string Manifest { get; }
    protected abstract string DirectoryInfo { get; }

    protected virtual string FileExtension => "";
    protected virtual bool Downloading => false;
    protected virtual bool HasStreamAssets => false;
    protected virtual string Debug => Manifest;

    protected IEnumerator CoDownloadAssets(IEnumerable<string> files)
    {
        var count = files.Count();

        if (count == 0)
            yield break;

        Message($"Downloading {count} files");
        files.Do(x => Coroutines.Start(CoRun(DownloadFile(x), OnEnd)));
        var max = count;

        while (count > 0)
        {
            UpdateSplashPatch.SetText($"Downloading {Manifest} ({max - count}/{max})");
            yield return null;
        }

        void OnEnd() => count--;
    }

    private IEnumerator DownloadFile(string fileName)
    {
        var trueName = fileName + (IsNullEmptyOrWhiteSpace(FileExtension) ? "" : $".{FileExtension}");
        yield return CoDownloadItem($"{RepositoryUrl}/{Manifest}/{trueName.Replace(" ", "%20")}", Path.Combine(DirectoryInfo, trueName));
    }

    public static IEnumerator RunLoaders()
    {
        using var hasher = MD5.Create();
        yield return new TranslationLoader().CoFetch(hasher);
        yield return new BundleLoader().CoFetch(hasher);
        yield return new HatLoader().CoFetch(hasher);
        yield return new VisorLoader().CoFetch(hasher);
        yield return new NameplateLoader().CoFetch(hasher);
        yield return new ColorLoader().CoFetch(hasher);
        yield return new BaseDownloader(TownOfUsReworked.Options, "Presets", "txt").CoFetch(hasher);
        yield return new BaseDownloader(TownOfUsReworked.Images, "Images", "png").CoFetch(hasher);
        yield return new BaseDownloader(TownOfUsReworked.Sounds, "Sounds", "wav").CoFetch(hasher);
    }

    protected static bool ShouldDownload(string file, string hash, HashAlgorithm hasher) => IsNullEmptyOrWhiteSpace(hash) || !File.Exists(file) || GenerateHash(file, hasher) != hash;

    protected static string GenerateHash(string file, HashAlgorithm hasher) => File.Exists(file) ? BitConverter.ToString(hasher.ComputeHash(File.ReadAllBytes(file))).Replace("-", "")
        .ToLowerInvariant() : null;
}