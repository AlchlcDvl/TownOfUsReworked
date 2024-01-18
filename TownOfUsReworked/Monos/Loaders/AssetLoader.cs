using System.Text.Json;
using UnityEngine.Networking;

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
        UpdateSplashPatch.SetText($"Fetching {ManifestFileName}");
        yield return new WaitForEndOfFrame();

        if (DirectoryInfo != "" && !Directory.Exists(DirectoryInfo))
            Directory.CreateDirectory(DirectoryInfo);

        var www = new UnityWebRequest();
        www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
        LogMessage($"Downloading manifest at: {RepositoryUrl}/{ManifestFileName}.json");
        www.SetUrl($"{RepositoryUrl}/{ManifestFileName}.json");
        www.downloadHandler = new DownloadHandlerBuffer();
        var operation = www.SendWebRequest();

        while (!operation.isDone)
            yield return new WaitForEndOfFrame();

        var filePath = Path.Combine(DirectoryInfo, $"{ManifestFileName}.json");
        var isError = www.isNetworkError || www.isHttpError;
        var jsonText = isError ? ReadDiskText($"{ManifestFileName}.json", DirectoryInfo) : www.downloadHandler.text;

        if (isError)
            LogError(www.error);

        var response = JsonSerializer.Deserialize(jsonText, JSONType, new JsonSerializerOptions() { AllowTrailingCommas = true });
        www.downloadHandler.Dispose();
        www.Dispose();

        if (!isError)
        {
            var task = File.WriteAllTextAsync(filePath, jsonText);

            while (!task.IsCompleted)
            {
                if (task.Exception != null)
                {
                    LogError(task.Exception.Message);
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        if (Downloading)
        {
            UpdateSplashPatch.SetText($"Downloading {ManifestFileName}");
            yield return BeginDownload(response);
        }

        UpdateSplashPatch.SetText($"Preloading {ManifestFileName}");
        AfterLoading(response);
        response = null;
        jsonText = null;
        yield return new WaitForEndOfFrame();
        yield break;
    }

    [HideFromIl2Cpp]
    public virtual IEnumerator BeginDownload(object response) => null;

    [HideFromIl2Cpp]
    public virtual void AfterLoading(object response) {}

    [HideFromIl2Cpp]
    public static IEnumerator CoDownloadAsset(string fileName, AssetLoader downloader)
    {
        var www = new UnityWebRequest();
        www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
        fileName = fileName.Replace(" ", "%20");
        LogMessage($"Downloading: {downloader.FolderDownloadName}/{fileName}");
        www.SetUrl($"{RepositoryUrl}/{downloader.FolderDownloadName}/{fileName}.png");
        www.downloadHandler = new DownloadHandlerBuffer();
        var operation = www.SendWebRequest();

        while (!operation.isDone)
            yield return new WaitForEndOfFrame();

        if (www.isNetworkError || www.isHttpError)
        {
            LogError(www.error);
            yield break;
        }

        var filePath = Path.Combine(downloader.DirectoryInfo, $"{fileName}.png");
        filePath = filePath.Replace("%20", " ");
        var persistTask = File.WriteAllBytesAsync(filePath, www.downloadHandler.data);

        while (!persistTask.IsCompleted)
        {
            if (persistTask.Exception != null)
            {
                LogError(persistTask.Exception.Message);
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        www.downloadHandler.Dispose();
        www.Dispose();
        yield return new WaitForEndOfFrame();
        yield break;
    }
}