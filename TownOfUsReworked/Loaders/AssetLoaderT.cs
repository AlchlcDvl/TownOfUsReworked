namespace TownOfUsReworked.Loaders;

public abstract class AssetLoader<T> : AssetLoader where T : Asset
{
    private bool Running;

    public IEnumerator CoFetch(HashAlgorithm hasher)
    {
        if (Running)
            yield break;

        Running = true;
        UpdateSplashPatch.SetText($"Fetching {Manifest}");
        yield return EndFrame();

        if (!IsNullEmptyOrWhiteSpace(DirectoryInfo) && !Directory.Exists(DirectoryInfo))
            Directory.CreateDirectory(DirectoryInfo);

        string jsonText;

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
                yield return BeginDownload(response, hasher);
            }

            if (TownOfUsReworked.IsDev)
            {
                yield return GenerateHashes(response, hasher);
                using var stream = File.OpenWrite(Path.Combine(TownOfUsReworked.Hashes, $"{Manifest}.json"));
                JsonSerializer.Serialize(stream, response, new JsonSerializerOptions()
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

    protected virtual IEnumerator BeginDownload(T[] response, HashAlgorithm hasher) => EndFrame();

    protected virtual IEnumerator LoadAssets(T[] response) => EndFrame();

    protected virtual IEnumerator GenerateHashes(T[] response, HashAlgorithm hasher) => EndFrame();
}