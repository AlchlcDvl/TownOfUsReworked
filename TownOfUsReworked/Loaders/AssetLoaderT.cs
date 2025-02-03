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

        var jsonText = "";

        if (ClientOptions.ForceUseLocal)
            jsonText = ReadDiskText($"{Manifest}.json", DirectoryInfo);
        else
        {
            Message($"Downloading manifest at: {RepositoryUrl}/{Manifest}.json");
            var www = UnityWebRequest.Get($"{RepositoryUrl}/{Manifest}.json");
            yield return www.SendWebRequest();

            var isError = www.result != UnityWebRequest.Result.Success;

            if (isError)
            {
                Error(www.error);
                jsonText = ReadDiskText($"{Manifest}.json", DirectoryInfo);
            }
            else
            {
                jsonText = www.downloadHandler.text;
                var task = File.WriteAllTextAsync(Path.Combine(DirectoryInfo, $"{Manifest}.json"), jsonText);

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
        }

        if (IsNullEmptyOrWhiteSpace(jsonText))
        {
            Error($"Unable to load online or local JSON data for {Manifest}");
            yield break;
        }

        var response = JsonSerializer.Deserialize<T[]>(jsonText);

        if (Downloading)
        {
            if (!ClientOptions.ForceUseLocal)
            {
                UpdateSplashPatch.SetText($"Downloading {Manifest}");
                yield return BeginDownload(response);
            }

            if (TownOfUsReworked.IsDev)
            {
                yield return GenerateHashes(response);
                JsonSerializer.Serialize(File.OpenWrite(Path.Combine(TownOfUsReworked.Hashes, $"{Manifest}.json")), response, new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                });
            }
        }

        UpdateSplashPatch.SetText($"Loading {Manifest}");
        yield return LoadAssets(response);

        Array.Clear(response);
        yield return EndFrame();
    }

    public virtual IEnumerator BeginDownload(T[] response) => EndFrame();

    public virtual IEnumerator LoadAssets(T[] response) => EndFrame();

    public virtual IEnumerator GenerateHashes(T[] response) => EndFrame();
}