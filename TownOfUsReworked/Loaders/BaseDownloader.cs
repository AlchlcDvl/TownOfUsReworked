namespace TownOfUsReworked.Loaders;

public class BaseDownloader(string dir, string manifest, string ext) : AssetLoader<DownloadableAsset>
{
    protected override string DirectoryInfo { get; } = dir;
    protected override string Manifest { get; } = manifest;
    protected override string FileExtension { get; } = ext;

    protected override bool Downloading => true;

    protected override IEnumerable<string> GenerateDownloadList(DownloadableAsset[] response, HashAlgorithm hasher) => response.Where(x => ShouldDownload(Path.Combine(DirectoryInfo,
        $"{x.ID}.{FileExtension}"), x.Hash, hasher)).Select(x => x.ID);

    protected override void GenerateHash(DownloadableAsset item, HashAlgorithm hasher) => item.Hash = GenerateHash(Path.Combine(DirectoryInfo, $"{item.ID}.{FileExtension}"), hasher);

    protected override void LoadAsset(DownloadableAsset item, int i) => AddPath(item.ID, Path.Combine(DirectoryInfo, $"{item.ID}.{FileExtension}"));
}