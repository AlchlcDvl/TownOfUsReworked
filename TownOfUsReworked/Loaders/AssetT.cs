namespace TownOfUsReworked.Loaders;

public abstract class AssetLoader<T> : AssetLoader
    where T : Asset
{
    public IEnumerator CoFetch(HashAlgorithm hasher)
    {
        UpdateSplashPatch.SetText($"Fetching {Manifest}");

        if (!Directory.Exists(DirectoryInfo))
            Directory.CreateDirectory(DirectoryInfo);

        byte[] json;
        var path = Path.Combine(DirectoryInfo, $"{Manifest}.json");

        if (ClientOptions.ForceUseLocal)
        {
            var task = File.ReadAllBytesAsync(path);
            yield return WaitUntilTaskComplete(task);
            json = task.Result;
        }
        else
        {
            var www = UnityWebRequest.Get($"{RepositoryUrl}/{Manifest}.json");
            yield return www.SendWebRequest();

            var isError = www.result != UnityWebRequest.Result.Success;

            if (isError)
            {
                Error(www.error);
                var task = File.ReadAllBytesAsync(path);
                yield return WaitUntilTaskComplete(task);
                json = task.Result;
            }
            else
            {
                json = www.downloadHandler.data;

                if (json?.Length is null or 0)
                {
                    Warning($"Online JSON for {Manifest} was missing");
                    var task = File.ReadAllBytesAsync(path);
                    yield return WaitUntilTaskComplete(task);
                    json = task.Result;
                }
                else
                {
                    var task = File.WriteAllBytesAsync(path, json);
                    yield return WaitUntilTaskComplete(task);
                }
            }

            www.downloadHandler.Dispose();
            www.Dispose();
        }

        if (json?.Length is null or 0)
        {
            Error($"Unable to load online or local JSON data for {Manifest}");
            yield break;
        }

        BeforeLoading();
        yield return null;

        var response = JsonSerializer.Deserialize<T[]>(json);
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

                UpdateSplashPatch.SetText($"Dumping {Manifest} Hashes");
                using var stream = File.OpenWrite(Path.Combine(TownOfUsReworked.Hashes, $"{Manifest}.json"));
                var task = JsonSerializer.SerializeAsync(stream, response, new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                });
                yield return WaitUntilTaskComplete(task);
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