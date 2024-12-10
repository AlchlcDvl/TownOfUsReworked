namespace TownOfUsReworked.Classes;

public static class ModUpdater
{
    private static readonly Dictionary<string, bool> Running = new() { { "Reworked", false }, { "Submerged", false }, { "LevelImpostor", false } };
    public static bool ReworkedUpdate;
    public static bool SubmergedUpdate;
    public static bool CanDownloadSubmerged;
    public static bool CanDownloadLevelImpostor;
    public static readonly Dictionary<string, string> URLs = [];
    private static GenericPopup Popup;

    private static string GetLink(string tag) => tag switch
    {
        "Reworked" => "AlchlcDvl/TownOfUsReworked",
        "Submerged" => "SubmergedAmongUs/Submerged",
        "LevelImpostor" => "DigiWorm0/LevelImposter",
        _ => throw new NotImplementedException(tag)
    };

    public static IEnumerator CheckForUpdate(string updateType)
    {
        if (Running[updateType])
            yield break;

        Running[updateType] = true;
        UpdateSplashPatch.SetText($"Fetching {updateType} Data And Loading Compatibility");
        Message($"Getting update info for {updateType}");
        yield return EndFrame();

        // Checks the github api for tags. Compares current version to the latest tag version on GitHub
        var www = UnityWebRequest.Get($"https://api.github.com/repos/{GetLink(updateType)}/releases?per_page=1");
        yield return www.SendWebRequest();

        var isError = www.result != UnityWebRequest.Result.Success;
        var jsonText = "";

        if (ClientOptions.ForceUseLocal)
            jsonText = ReadDiskText($"{updateType}UpdateData.json", TownOfUsReworked.Other);
        else
        {
            if (isError)
            {
                Error(www.error);
                jsonText = ReadDiskText($"{updateType}UpdateData.json", TownOfUsReworked.Other);
            }
            else
            {
                jsonText = www.downloadHandler.text;
                var task = File.WriteAllTextAsync(Path.Combine(TownOfUsReworked.Other, $"{updateType}UpdateData.json"), jsonText);

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
        }

        www.downloadHandler.Dispose();
        www.Dispose();

        if (IsNullEmptyOrWhiteSpace(jsonText) && !isError)
        {
            jsonText = ReadDiskText($"{updateType}UpdateData.json", TownOfUsReworked.Other);
            Warning($"Online JSON for {updateType} was missing");
        }

        if (IsNullEmptyOrWhiteSpace(jsonText))
        {
            Error($"Unable to load online or local JSON data for {updateType}");
            yield break;
        }

        var data = JsonSerializer.Deserialize<GitHubApiObject[]>(jsonText)[0];

        if (data.Tag == null)
        {
            Error($"{updateType} tag doesn't exist");
            yield break; // Something went wrong
        }

        if (data.Description == null)
        {
            Error($"{updateType} description doesn't exist");
            yield break; // Something went wrong part 2
        }

        if (data.Assets == null)
        {
            Error($"No assets found for {updateType}");
            yield break; // Something went wrong part 3
        }

        // Check Reworked version
        if (updateType == "Reworked")
        {
            var version = Version.Parse(data.Tag.Replace("v", ""));
            var diff = TownOfUsReworked.Version.CompareTo(version);
            ReworkedUpdate = diff < 0 || (diff == 0 && TownOfUsReworked.IsDev);
        }
        // Accounts for broken version + checks Submerged version
        else if (updateType == "Submerged" && SubLoaded)
            SubmergedUpdate = SubVersion == null || SubVersion.CompareTo(SemanticVersioning.Version.Parse(data.Tag.Replace("v", ""))) < 0;

        foreach (var asset in data.Assets)
        {
            if (asset.URL == null || (data.Description.Contains("[NoUpdate]") && ReworkedUpdate))
                continue;

            if (asset.URL.EndsWith(".dll"))
            {
                URLs[updateType] = asset.URL;
                break;
            }
        }

        yield return EndFrame();
        yield break;
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

        if (!URLs.TryGetValue(updateType, out var link))
        {
            Error($"No link found for {updateType}");
            Popup.TextAreaTMP.SetText(TranslationManager.Translate("Updates.Mod.Manually"));
            button.SetActive(true);
            yield break;
        }

        var www = UnityWebRequest.Get(link);
        var operation = www.SendWebRequest();

        while (!operation.isDone)
        {
            var stars = Mathf.CeilToInt(www.downloadProgress * 10);
            Popup.TextAreaTMP.SetText($"{TranslationManager.Translate("Updates.Mod.Updating", ("%mod%", updateType))}");
            Popup.TextAreaTMP.text += $"\n{new string((char)0x25A0, stars)}{new string((char)0x25A1, 10 - stars)}";
            yield return EndFrame();
        }

        if (www.isNetworkError || www.isHttpError)
        {
            Popup.TextAreaTMP.SetText(TranslationManager.Translate("Updates.Mod.NoSuccess"));
            Error(www.error);
            www.downloadHandler.Dispose();
            www.Dispose();
            yield break;
        }

        Popup.TextAreaTMP.SetText(TranslationManager.Translate("Updates.Mod.Copying", ("%mod%", updateType)));
        var filePath = Path.Combine(TownOfUsReworked.ModsFolder, $"{updateType}.dll");

        if (File.Exists(filePath + ".old"))
            File.Delete(filePath + ".old");

        if (File.Exists(filePath))
            File.Move(filePath, filePath + ".old");

        var persistTask = File.WriteAllBytesAsync(filePath, www.downloadHandler.data);
        var hasError = false;
        Exception error = null;

        while (!persistTask.IsCompleted)
        {
            if (persistTask.Exception != null)
            {
                hasError = true;
                error = persistTask.Exception;
                break;
            }

            yield return EndFrame();
        }

        www.downloadHandler.Dispose();
        www.Dispose();

        if (hasError)
            Error(error);

        Popup.TextAreaTMP.SetText(hasError ? TranslationManager.Translate("Updates.Mod.Success", ("%mod%", updateType)) : TranslationManager.Translate("Updates.Mod.NoSuccess"));
        button.SetActive(true);
        yield break;
    }
}