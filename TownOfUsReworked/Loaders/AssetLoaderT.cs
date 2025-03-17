namespace TownOfUsReworked.Loaders;

public abstract class AssetLoader<T> : AssetLoader
    where T : Asset
{
    public IEnumerator CoFetch(HashAlgorithm hasher)
    {
        UpdateSplashPatch.SetText($"Fetching {Manifest}");
        yield return EndFrame();
        var exists = Directory.Exists(DirectoryInfo);

        if (!exists)
            Directory.CreateDirectory(DirectoryInfo);

        string jsonText;

        if (ClientOptions.ForceUseLocal && exists)
            jsonText = ReadDiskText($"{Manifest}.json", DirectoryInfo);
        else
        {
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

        BeforeLoading();
        yield return EndFrame();

        var response = JsonSerializer.Deserialize<T[]>(jsonText);
        float time;

        if (Downloading)
        {
            if (!ClientOptions.ForceUseLocal || !exists)
            {
                UpdateSplashPatch.SetText($"Downloading {Manifest}");
                yield return CoDownloadAssets(GenerateDownloadList(response, hasher));
            }

            if (TownOfUsReworked.IsDev)
            {
                time = 0f;

                foreach (var (i, item) in response.Indexed())
                {
                    GenerateHash(item, hasher);
                    time += Time.deltaTime;

                    if (time < 1f)
                        continue;

                    time = 0f;
                    UpdateSplashPatch.SetText($"Generating {Manifest} Hashes ({i + 1}/{response.Length})");
                    yield return EndFrame();
                }

                using var stream = File.OpenWrite(Path.Combine(TownOfUsReworked.Hashes, $"{Manifest}.json"));
                JsonSerializer.Serialize(stream, response, new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                });
            }
        }

        var data = new List<T>(response);

        if (TownOfUsReworked.IsStream && HasStreamAssets)
            LoadStreamAssets(data);

        yield return EndFrame();

        UpdateSplashPatch.SetText($"Loading {Manifest}");
        Message($"Found {data.Count} {Manifest.ToLower()}");

        if (data.Any())
        {
            time = 0f;

            foreach (var (i, item) in data.Indexed())
            {
                LoadAsset(item, i);
                time += Time.deltaTime;

                if (time < 1f)
                    continue;

                time = 0f;
                UpdateSplashPatch.SetText($"Loading {Manifest} ({i + 1}/{data.Count})");
                yield return EndFrame();
            }
        }

        yield return EndFrame();

        AfterLoading(data);
        yield return EndFrame();

        Array.Clear(response);
        data.Clear();
        yield return EndFrame();
    }

    protected virtual void LoadAsset(T item, int i) {}

    protected virtual void GenerateHash(T item, HashAlgorithm hasher) {}

    protected virtual void LoadStreamAssets(List<T> response) {}

    protected virtual void AfterLoading(List<T> response) {}

    protected virtual void BeforeLoading() {}

    protected virtual IEnumerable<string> GenerateDownloadList(T[] response, HashAlgorithm hasher) => [];
}