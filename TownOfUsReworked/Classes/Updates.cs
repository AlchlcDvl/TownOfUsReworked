using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class ModUpdater
    {
        #pragma warning disable
        public static bool running;
        public static bool hasREWUpdate;
        public static bool hasSubmergedUpdate;
        public static string updateREWURI;
        public static string updateSubmergedURI;
        private static Task updateREWTask;
        private static Task updateSubmergedTask;
        public static GenericPopup InfoPopup;
        #pragma warning restore

        public static void LaunchUpdater()
        {
            if (running)
                return;

            running = true;

            try
            {
                CheckForUpdate("REW").GetAwaiter().GetResult();
            } catch {}

            //Only check of Submerged update if Submerged is already installed
            var codeBase = TownOfUsReworked.Executing.Location;
            UriBuilder uri = new(codeBase);

            if (SubmergedCompatibility.Loaded)
            {
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
            var info = "";

            if (updateType == "REW")
            {
                info = "Updating Town Of Us Reworked\nPlease wait...";
                InfoPopup.Show(info);

                if (updateREWTask == null)
                {
                    if (updateREWURI != null)
                        updateREWTask = DownloadUpdate("REW");
                    else
                        info = "Unable to auto-update\nPlease update manually";
                }
                else
                    info = "Update might already\nbe in progress";
            }
            else if (updateType == "Submerged")
            {
                info = "Updating Submerged\nPlease wait...";
                InfoPopup.Show(info);

                if (updateSubmergedTask == null)
                {
                    if (updateSubmergedURI != null)
                        updateSubmergedTask = DownloadUpdate("SUB");
                    else
                        info = "Unable to auto-update\nPlease update manually";
                }
                else
                    info = "Update might already\nbe in progress";
            }
            else
                return;

            InfoPopup.StartCoroutine(Effects.Lerp(0.01f, new Action<float>(_ => SetPopupText(info))));
        }

        public static void ClearOldVersions()
        {
            //Removes any old versions (Denoted by the suffix `.old`)
            try
            {
                DirectoryInfo d = new(Path.GetDirectoryName(Application.dataPath) + @"\BepInEx\plugins");

                foreach (var f in d.GetFiles("*.old").Select(x => x.FullName).ToArray())
                    File.Delete(f);
            }
            catch (Exception e)
            {
                Utils.LogSomething("Exception occured when clearing old versions:\n" + e);
            }
        }

        public static async Task<bool> CheckForUpdate(string updateType = "REW")
        {
            //Checks the github api for Town Of Us Reworked tags. Compares current version (from VersionString in TownOfUsReworked.cs) to the latest tag version (on GitHub)
            try
            {
                string githubURI = "";

                if (updateType == "REW")
                    githubURI = "https://api.github.com/repos/AlchlcDvl/TownOfUsReworked/releases/latest";
                else if (updateType == "Submerged")
                    githubURI = "https://api.github.com/repos/SubmergedAmongUs/Submerged/releases/latest";

                HttpClient http = new();
                http.DefaultRequestHeaders.Add("User-Agent", "Reworked Updater");
                var response = await http.GetAsync(new Uri(githubURI), HttpCompletionOption.ResponseContentRead);

                if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                {
                    Utils.LogSomething("Server returned no data: " + response.StatusCode.ToString());
                    return false;
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<GitHubApiObject>(json);
                var tagname = data.tag_name;

                if (tagname == null)
                    return false; // Something went wrong

                var diff = 0;
                var ver = Version.Parse(tagname.Replace("v", ""));

                if (updateType == "REW")
                {
                    //Check REW version
                    diff = TownOfUsReworked.Version.CompareTo(ver);

                    if (diff < 0)
                    {
                        //REW update required
                        hasREWUpdate = true;
                    }
                }
                else if (updateType == "Submerged")
                {
                    //Accounts for broken version
                    if (SubmergedCompatibility.Version == null)
                        hasSubmergedUpdate = true;
                    else
                    {
                        diff = SubmergedCompatibility.Version.CompareTo(SemanticVersioning.Version.Parse(tagname.Replace("v", "")));

                        if (diff < 0)
                        {
                            // Submerged update required
                            hasSubmergedUpdate = true;
                        }
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
                        if (updateType == "REW")
                            updateREWURI = asset.browser_download_url;
                        else if (updateType == "SUB")
                            updateSubmergedURI = asset.browser_download_url;

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogSomething(ex);
            }

            return false;
        }

        public static async Task<bool> DownloadUpdate(string updateType)
        {
            //Downloads the new Reworked/Submerged dll from GitHub into the plugins folder
            var downloadDLL= "";
            var info = "";

            if (updateType == "REW")
            {
                downloadDLL = updateREWURI;
                info = "Town Of Us Reworked\nupdated successfully.\nPlease RESTART the game.";
            }
            else if (updateType == "SUB")
            {
                downloadDLL = updateSubmergedURI;
                info = "Submerged\nupdated successfully.\nPlease RESTART the game.";
            }

            try
            {
                HttpClient http = new();
                http.DefaultRequestHeaders.Add("User-Agent", "Reworked Updater");
                var response = await http.GetAsync(new Uri(downloadDLL), HttpCompletionOption.ResponseContentRead);

                if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                {
                    Utils.LogSomething("Server returned no data: " + response.StatusCode.ToString());
                    return false;
                }

                var codeBase = TownOfUsReworked.Executing.Location;
                var uri = new UriBuilder(codeBase);
                var fullname = Uri.UnescapeDataString(uri.Path);

                if (updateType == "Submerged")
                    fullname = fullname.Replace("Reworked", "Submerged"); //TODO A better solution than this to correctly name the dll files

                if (File.Exists(fullname + ".old")) //Clear old file in case it wasnt;
                    File.Delete(fullname + ".old");

                File.Move(fullname, fullname + ".old"); //Rename current executable to old
                var responseStream = await response.Content.ReadAsStreamAsync();
                var fileStream = File.Create(fullname);
                responseStream.CopyTo(fileStream);
                ShowPopup(info);
                return true;
            }
            catch (Exception ex)
            {
                Utils.LogSomething(ex);
            }

            ShowPopup("Update wasn't successful\nTry again later,\nor update manually.");
            return false;
        }

        private static void ShowPopup(string message)
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

        #pragma warning disable
        class GitHubApiObject
        {
            [JsonPropertyName("tag_name")]
            public string tag_name;
            [JsonPropertyName("assets")]
            public GitHubApiAsset[] assets;
        }

        class GitHubApiAsset
        {
            [JsonPropertyName("browser_download_url")]
            public string browser_download_url;
        }
        #pragma warning restore
    }
}