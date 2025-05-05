namespace TownOfUsReworked.Loaders;

public abstract class AssetLoader<T> : AssetLoader
    where T : Asset
{
    public IEnumerator CoFetch(HashAlgorithm hasher)
    {
        UpdateSplashPatch.SetText($"Fetching {Manifest}");

        if (!Directory.Exists(DirectoryInfo))
            Directory.CreateDirectory(DirectoryInfo);

        string jsonText;

        if (ClientOptions.ForceUseLocal)
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
                yield return WaitUntilTaskComplete(task);
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
        yield return null;

        var response = JsonSerializer.Deserialize<T[]>(jsonText);
        float time;

        if (Downloading)
        {
            if (!ClientOptions.ForceUseLocal)
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
                    yield return null;
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

        yield return null;

        UpdateSplashPatch.SetText($"Loading {Manifest}");
        Message($"Found {data.Count} {Debug.ToLower()}");

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
                yield return null;
            }
        }

        AfterLoading(data);
        yield return null;

        Array.Clear(response);
        data.Clear();
    }

    protected virtual void LoadAsset(T item, int i) {}

    protected virtual void GenerateHash(T item, HashAlgorithm hasher) {}

    protected virtual void LoadStreamAssets(List<T> response) {}

    protected virtual void AfterLoading(List<T> response) {}

    protected virtual void BeforeLoading() {}

    protected virtual IEnumerable<string> GenerateDownloadList(T[] response, HashAlgorithm hasher) => [];
}