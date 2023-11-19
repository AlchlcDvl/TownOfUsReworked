using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace TownOfUsReworked.Classes;

public static class ModUpdater
{
    private static bool Running = false;
    public static bool ReworkedUpdate = false;
    public static bool SubmergedUpdate = false;
    public static bool CanDownloadSubmerged = false;
    private static string ReworkedURI = null;
    private static string SubmergedURI = null;
    private static Task ReworkedTask = null;
    private static Task SubmergedTask = null;
    private static GenericPopup InfoPopup;

    private static string GetLink(string tag) => tag switch
    {
        "Reworked" => "https://api.github.com/repos/AlchlcDvl/TownOfUsReworked/releases/latest",
        "Submerged" => "https://api.github.com/repos/SubmergedAmongUs/Submerged/releases/latest",
        _ => throw new NotImplementedException(tag)
    };

    private static string GetLink2(string tag) => tag switch
    {
        "Reworked" => ReworkedURI,
        "Submerged" => SubmergedURI,
        _ => throw new NotImplementedException(tag)
    };

    public static void LaunchUpdater()
    {
        if (Running)
            return;

        Running = true;
        CanDownloadSubmerged = !SubLoaded;
        CheckForUpdate("Reworked").GetAwaiter();
        CheckForUpdate("Submerged").GetAwaiter();

        if (ReworkedUpdate || SubmergedUpdate || CanDownloadSubmerged)
        {
            InfoPopup = UObject.Instantiate(TwitchManager.Instance.TwitchPopup);
            InfoPopup.TextAreaTMP.fontSize *= 0.7f;
            InfoPopup.TextAreaTMP.enableAutoSizing = false;
        }
    }

    public static void ExecuteUpdate(string updateType)
    {
        var info = Translate("Updates.Mod.Updating").Replace("%mod%", Translate($"Mod.{updateType}"));
        InfoPopup.Show(info);

        if (updateType == "Reworked")
        {
            if (ReworkedTask == null)
            {
                if (ReworkedURI != null)
                    ReworkedTask = DownloadUpdate("Reworked");
                else
                    info = Translate("Updates.Mod.Manually");
            }
            else
                info = Translate("Updates.Mod.InProgress");
        }
        else if (updateType == "Submerged")
        {
            if (SubmergedTask == null)
            {
                if (SubmergedURI != null)
                    SubmergedTask = DownloadUpdate("Submerged");
                else
                    info = Translate("Updates.Mod.Manually");
            }
            else
                info = Translate("Updates.Mod.InProgress");
        }

        InfoPopup.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ => SetPopupText(info))));
    }

    private static async Task<bool> CheckForUpdate(string updateType)
    {
        //Checks the github api for tags. Compares current version to the latest tag version on GitHub
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Updater");
            using var response = await client.GetAsync(GetLink(updateType), HttpCompletionOption.ResponseContentRead);

            if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
            {
                LogError($"Server returned no data: {response.StatusCode} for {updateType}");
                return false;
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<GitHubApiObject>(json);

            var tagname = data.Tag;

            if (tagname == null)
            {
                LogError($"{updateType} tag doesn't exist");
                return false; // Something went wrong
            }

            //Check Reworked version
            if (updateType == "Reworked")
                ReworkedUpdate = TownOfUsReworked.Version.CompareTo(Version.Parse(tagname.Replace("v", ""))) < 0;
            //Accounts for broken version + checks Submerged version
            else if (updateType == "Submerged" && SubLoaded)
                SubmergedUpdate = SubVersion == null || SubVersion.CompareTo(SemanticVersioning.Version.Parse(tagname.Replace("v", ""))) < 0;

            var assets = data.Assets;

            if (assets == null)
                return false;

            foreach (var asset in assets)
            {
                if (asset.URL == null)
                    continue;

                if (asset.URL.EndsWith(".dll"))
                {
                    if (updateType == "Reworked")
                        ReworkedURI = asset.URL;
                    else if (updateType == "Submerged")
                        SubmergedURI = asset.URL;

                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }

        return false;
    }

    private static async Task<bool> DownloadUpdate(string updateType)
    {
        try
        {
            //Downloads the new dll from GitHub into the plugins folder
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Updater");
            using var response = await client.GetAsync(GetLink2(updateType), HttpCompletionOption.ResponseContentRead);

            if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
            {
                LogError($"Server returned no data: {response.StatusCode} for {updateType}");
                ShowPopUp(Translate("Updates.Mod.NoSuccess"));
                return false;
            }

            var fullname = $"{TownOfUsReworked.ModsFolder}{updateType}.dll";

            if (File.Exists(fullname + ".old")) // Clear old file
                File.Delete(fullname + ".old");

            if (File.Exists(fullname)) // Rename current executable to old, if any
                File.Move(fullname, fullname + ".old");

            var array = await response.Content.ReadAsByteArrayAsync();
            File.WriteAllBytes(fullname, array);
            ShowPopUp(Translate("Updates.Mod.Success").Replace("%mod%", Translate($"Mod.{updateType}")));
            return true;
        }
        catch (Exception ex)
        {
            LogError(ex);
            ShowPopUp(Translate("Updates.Mod.NoSuccess"));
            return false;
        }
    }

    private static void ShowPopUp(string message)
    {
        SetPopupText(message);
        InfoPopup.gameObject.SetActive(true);
    }

    public static void SetPopupText(string message)
    {
        if (InfoPopup == null)
            return;

        if (InfoPopup.TextAreaTMP != null)
            InfoPopup.TextAreaTMP.text = message;
    }
}