using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace TownOfUsReworked.Classes;

public class ModUpdater
{
    public static bool Running = false;
    public static bool ReworkedUpdate = false;
    public static bool SubmergedUpdate = false;
    public static string ReworkedURI = null;
    public static string SubmergedURI = null;
    private static Task ReworkedTask = null;
    private static Task SubmergedTask = null;
    public static GenericPopup InfoPopup;

    public static void LaunchUpdater()
    {
        if (Running)
            return;

        Running = true;

        try
        {
            CheckForUpdate("Reworked").GetAwaiter().GetResult();
        } catch {}

        if (SubLoaded)
        {
            //Only check of Submerged update if Submerged is already installed
            var codeBase = TownOfUsReworked.Executing.Location;
            var uri = new UriBuilder(codeBase);
            var submergedPath = Uri.UnescapeDataString(uri.Path.Replace("Reworked", "Submerged"));

            if (File.Exists(submergedPath))
            {
                try
                {
                    CheckForUpdate("Submerged").GetAwaiter().GetResult();
                } catch {}
            }
        }

        ClearOldVersions();
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

        InfoPopup.StartCoroutine(Effects.Lerp(0.01f, new Action<float>((p) => { SetPopupText(info); })));
    }

    private static void ClearOldVersions()
    {
        //Removes any old versions (Denoted by the suffix `.old`)
        try
        {
            var d = new DirectoryInfo(TownOfUsReworked.DataPath + "BepInEx\\plugins");
            d.GetFiles("*.old").Select(x => x.FullName).ForEach(File.Delete);
        }
        catch (Exception e)
        {
            LogError("Exception occured when clearing old versions:\n" + e);
        }
    }

    private static async Task<bool> CheckForUpdate(string updateType)
    {
        //Checks the github api for Reworked tags. Compares current version (from VersionString in TownOfUsReworked.cs) to the latest tag version (on GitHub)
        try
        {
            var githubURI = "";

            if (updateType == "Reworked")
                githubURI = "https://api.github.com/repos/AlchlcDvl/TownOfUsReworked/releases/latest";
            else if (updateType == "Submerged")
                githubURI = "https://api.github.com/repos/SubmergedAmongUs/Submerged/releases/latest";

            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent", $"{updateType} Updater");
            var response = await http.GetAsync(new Uri(githubURI), HttpCompletionOption.ResponseContentRead);

            if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
            {
                LogError($"Server returned no data: {response.StatusCode}");
                return false;
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<GitHubApiObject>(json);

            var tagname = data.tag_name;

            if (tagname == null)
                return false; // Something went wrong

            var diff = 0;
            var ver = Version.Parse(tagname.Replace("v", ""));

            if (updateType == "Reworked")
            {
                //Check Reworked version
                diff = TownOfUsReworked.Version.CompareTo(ver);

                if (diff < 0) // Reworked update required
                    ReworkedUpdate = true;
            }
            else if (updateType == "Submerged")
            {
                //account for broken version
                if (SubVersion == null)
                    SubmergedUpdate = true;
                else
                {
                    diff = SubVersion.CompareTo(SemanticVersioning.Version.Parse(tagname.Replace("v", "")));

                    if (diff < 0) // Submerged update required
                        SubmergedUpdate = true;
                }
            }

            var assets = data.assets;

            if (assets == null)
                return false;

            foreach (var asset in assets)
            {
                if (asset.browser_download_url == null)
                    continue;

                if (asset.browser_download_url.EndsWith(".dll"))
                {
                    if (updateType == "Reworked")
                        ReworkedURI = asset.browser_download_url;
                    else if (updateType == "Submerged")
                        SubmergedURI = asset.browser_download_url;

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
        //Downloads the new Reworked/Submerged dll from GitHub into the plugins folder
        var downloadDLL = updateType == "Reworked" ? ReworkedURI : SubmergedURI;
        var info = Translate("Updates.Mod.Success").Replace("%mod%", Translate($"Mod.{updateType}"));

        try
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent", $"{updateType} Updater");
            var response = await http.GetAsync(new Uri(downloadDLL), HttpCompletionOption.ResponseContentRead);

            if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
            {
                LogError($"Server returned no data: {response.StatusCode}");
                return false;
            }

            var codeBase = TownOfUsReworked.Executing.Location;
            var uri = new UriBuilder(codeBase);
            var fullname = Uri.UnescapeDataString(uri.Path);

            if (updateType == "Submerged")
                fullname = fullname.Replace("Reworked", "Submerged"); //TODO A better solution than this to correctly name the dll files

            if (File.Exists(fullname + ".old")) // Clear old file in case it wasnt;
                File.Delete(fullname + ".old");

            File.Move(fullname, fullname + ".old"); // rename current executable to old
            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = File.Create(fullname);
            responseStream.CopyTo(fileStream);
            ShowPopUp(info);
            return true;
        }
        catch (Exception ex)
        {
            LogError(ex);
        }

        ShowPopUp(Translate("Updates.Mod.NoSuccess"));
        return false;
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