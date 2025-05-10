namespace TownOfUsReworked.Managers;

public static class UpdateManager
{
    public static bool ReworkedUpdate;
    public static bool SubmergedUpdate;
    public static bool CanDownloadSubmerged;
    public static bool CanDownloadLevelImpostor;

    private static readonly Dictionary<string, string> Urls = [];
    private static GenericPopup Popup;

    public static IEnumerator CheckForUpdates()
    {
        foreach (var type in new[] { "Reworked", "Submerged", "LevelImpostor" })
            yield return CheckForUpdate(type);

        CanDownloadSubmerged = !SubLoaded && Urls.ContainsKey("Submerged");
        CanDownloadLevelImpostor = !LiLoaded && Urls.ContainsKey("LevelImpostor");
    }

    private static string GetLink(string tag) => tag switch
    {
        "Reworked" => "AlchlcDvl/TownOfUsReworked",
        "Submerged" => "SubmergedAmongUs/Submerged",
        "LevelImpostor" => "DigiWorm0/LevelImposter",
        _ => throw new ArgumentOutOfRangeException(tag)
    };

    private static IEnumerator CheckForUpdate(string updateType)
    {
        UpdateSplashPatch.SetText($"Fetching {updateType} Data");
        Message($"Getting update info for {updateType}");

        string jsonText;

        if (ClientOptions.ForceUseLocal)
            jsonText = ReadDiskText($"{updateType}UpdateData.json", TownOfUsReworked.Other);
        else
        {
            // Checks the GitHub api for tags. Compares the current version to the latest tag version on GitHub
            var www = UnityWebRequest.Get($"https://api.github.com/repos/{GetLink(updateType)}/releases?per_page=1");
            yield return www.SendWebRequest();

            var isError = www.result != UnityWebRequest.Result.Success;

            if (isError)
            {
                Error(www.error);
                jsonText = ReadDiskText($"{updateType}UpdateData.json", TownOfUsReworked.Other);
            }
            else
            {
                jsonText = www.downloadHandler.text;
                var task = File.WriteAllTextAsync(Path.Combine(TownOfUsReworked.Other, $"{updateType}UpdateData.json"), jsonText);
                yield return WaitUntilTaskComplete(task);
            }

            www.downloadHandler.Dispose();
            www.Dispose();

            if (IsNullEmptyOrWhiteSpace(jsonText) && !isError)
            {
                jsonText = ReadDiskText($"{updateType}UpdateData.json", TownOfUsReworked.Other);
                Warning($"Online JSON for {updateType} was missing");
            }
        }

        if (IsNullEmptyOrWhiteSpace(jsonText))
        {
            Failure($"Unable to load online or local JSON data for {updateType}");
            yield break;
        }

        var data = JsonSerializer.Deserialize<GitHubApiObject[]>(jsonText)[0];

        if (data.Tag is null)
        {
            Failure($"{updateType} tag doesn't exist");
            yield break; // Something went wrong
        }

        if (data.Description is null)
        {
            Failure($"{updateType} description doesn't exist");
            yield break; // Something went wrong part 2
        }

        if (data.Assets is null)
        {
            Failure($"No assets found for {updateType}");
            yield break; // Something went wrong part 3
        }

        switch (updateType)
        {
            // Check the Reworked version
            case "Reworked":
            {
                var version = Version.Parse(data.Tag.Replace("v", ""));
                var diff = TownOfUsReworked.ModVer.CompareTo(version);
                ReworkedUpdate = diff < 0 || (diff == 0 && TownOfUsReworked.DebugMode);
                break;
            }
            // Accounts for a broken version + checks the Submerged version
            case "Submerged" when SubLoaded:
            {
                SubmergedUpdate = SubVersion is null || SubVersion.CompareTo(SemanticVersioning.Version.Parse(data.Tag.Replace("v", ""))) < 0;
                break;
            }
        }

        foreach (var asset in data.Assets)
        {
            if (asset.URL is null || (data.Description.Contains("[NoUpdate]") && ReworkedUpdate) || !asset.URL.EndsWith(".dll"))
                continue;

            Urls[updateType] = asset.URL;
            break;
        }
    }

    public static IEnumerator DownloadUpdate(string updateType)
    {
        if (!Popup)
        {
            Popup = UObject.Instantiate(TwitchManager.Instance.TwitchPopup);
            Popup.TextAreaTMP.fontSize *= 0.7f;
            Popup.TextAreaTMP.enableAutoSizing = false;
        }

        Popup.Show(TranslationManager.Translate("Updates.Mod.Updating", ("%mod%", updateType)));

        var button = Popup.transform.GetChild(2).gameObject;
        button.SetActive(false);

        if (!Urls.TryGetValue(updateType, out var link))
        {
            Failure($"No link found for {updateType}");
            Popup.TextAreaTMP.text = TranslationManager.Translate("Updates.Mod.Manually");
            button.SetActive(true);
            yield break;
        }

        var www = UnityWebRequest.Get(link);
        var operation = www.SendWebRequest();

        while (!operation.isDone)
        {
            var stars = Mathf.CeilToInt(www.downloadProgress * 10);
            Popup.TextAreaTMP.text = $"{TranslationManager.Translate("Updates.Mod.Updating", ("%mod%", updateType))}";
            Popup.TextAreaTMP.text += $"\n{new string((char)0x25A0, stars)}{new string((char)0x25A1, 10 - stars)}";
            yield return null;
        }

        var hasError = www.result != UnityWebRequest.Result.Success;

        if (hasError)
            Error(www.error);
        else
        {
            Popup.TextAreaTMP.text = TranslationManager.Translate("Updates.Mod.Copying", ("%mod%", updateType));
            var filePath = Path.Combine(TownOfUsReworked.ModsFolder, $"{updateType}.dll");

            if (File.Exists(filePath + ".old"))
                File.Delete(filePath + ".old");

            if (File.Exists(filePath))
                File.Move(filePath, filePath + ".old");

            var persistTask = File.WriteAllBytesAsync(filePath, www.downloadHandler.data);
            yield return WaitUntilTaskComplete(persistTask);

            if (persistTask.Exception is not null)
            {
                Error(persistTask.Exception);
                hasError = true;
            }
        }

        www.downloadHandler.Dispose();
        www.Dispose();

        Popup.TextAreaTMP.text = hasError ? TranslationManager.Translate("Updates.Mod.NoSuccess") : TranslationManager.Translate("Updates.Mod.Success", ("%mod%", updateType));
        button.SetActive(true);
    }
}