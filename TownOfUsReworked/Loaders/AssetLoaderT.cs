namespace TownOfUsReworked.Loaders;

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