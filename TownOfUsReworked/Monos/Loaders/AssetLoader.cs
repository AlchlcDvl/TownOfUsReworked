namespace TownOfUsReworked.Monos;

public abstract class AssetLoader : MonoBehaviour
{
    private const string RepositoryUrl = "https://raw.githubusercontent.com/AlchlcDvl/ReworkedAssets/main";

    private bool Running;

    public virtual string ManifestFileName => "";
    public virtual string FolderDownloadName => "";
    public virtual string DirectoryInfo => "";
    public virtual bool Downloading => false;

    [HideFromIl2Cpp]
    public virtual Type JSONType => null;

    public AssetLoader(IntPtr ptr) : base(ptr) {}

    [HideFromIl2Cpp]
    public IEnumerator CoFetch()
    {
        if (Running)
            yield break;

        Running = true;

        if (JSONType == null)
        {
            LogError($"Missing JSON type for {ManifestFileName}");
            yield break;
        }

        if (IsNullEmptyOrWhiteSpace(DirectoryInfo))
        {
            LogError($"Missing DirectoryInfo for {ManifestFileName}");
            yield break;
        }

        UpdateSplashPatch.SetText($"Fetching {ManifestFileName}");
        yield return EndFrame();

        if (!Directory.Exists(DirectoryInfo))
            Directory.CreateDirectory(DirectoryInfo);

        LogMessage($"Downloading manifest at: {RepositoryUrl}/{ManifestFileName}.json");
        var www = UnityWebRequest.Get($"{RepositoryUrl}/{ManifestFileName}.json");
        yield return www.SendWebRequest();

        var isError = www.result != UnityWebRequest.Result.Success;
        var jsonText = isError && DirectoryInfo != "" ? ReadDiskText($"{ManifestFileName}.json", DirectoryInfo) : www.downloadHandler.text;

        if (isError)
            LogError(www.error);
        else if (DirectoryInfo != "")
        {
            var task = File.WriteAllTextAsync(Path.Combine(DirectoryInfo, $"{ManifestFileName}.json"), www.downloadHandler.text);

            while (!task.IsCompleted)
            {
                if (task.Exception != null)
                {
                    LogError(task.Exception);
                    break;
                }

                yield return EndFrame();
            }
        }

        www.downloadHandler.Dispose();
        www.Dispose();

        if (IsNullEmptyOrWhiteSpace(jsonText) && DirectoryInfo != "" && !isError)
        {
            jsonText = ReadDiskText($"{ManifestFileName}.json", DirectoryInfo);
            LogWarning($"Online JSON for {ManifestFileName} was missing");
        }

        if (IsNullEmptyOrWhiteSpace(jsonText))
        {
            LogError($"Unable to load online or local JSON data for {ManifestFileName}");
            yield break;
        }

        var response = JsonSerializer.Deserialize(jsonText, JSONType);

        if (Downloading)
        {
            UpdateSplashPatch.SetText($"Downloading {ManifestFileName}");
            yield return BeginDownload(response);
        }

        UpdateSplashPatch.SetText($"Preloading {ManifestFileName}");
        yield return AfterLoading(response);
        yield break;
    }

    [HideFromIl2Cpp]
    public virtual IEnumerator BeginDownload(object response) => null;

    [HideFromIl2Cpp]
    public virtual IEnumerator AfterLoading(object response) => null;

    [HideFromIl2Cpp]
    public static IEnumerator CoDownloadAsset(string fileName, AssetLoader downloader, string fileType)
    {
        fileName = fileName.Replace(" ", "%20");
        LogMessage($"Downloading: {downloader.FolderDownloadName}/{fileName}");
        var www = UnityWebRequest.Get($"{RepositoryUrl}/{downloader.FolderDownloadName}/{fileName}.{fileType}");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            LogError(www.error);
            yield break;
        }

        var filePath = Path.Combine(downloader.DirectoryInfo, $"{fileName}.{fileType}");
        filePath = filePath.Replace("%20", " ").Replace(".txt", "");
        var persistTask = File.WriteAllBytesAsync(filePath, www.downloadHandler.data);

        while (!persistTask.IsCompleted)
        {
            if (persistTask.Exception != null)
            {
                LogError(persistTask.Exception);
                break;
            }

            yield return EndFrame();
        }

        www.downloadHandler.Dispose();
        www.Dispose();
        yield return EndFrame();
        yield break;
    }
}