using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class ModUpdater
    {
        public static bool Running;
        public static bool HasReworkedUpdate;
        public static bool HasSubmergedUpdate;
        public static string UpdateReworkedURI;
        public static string UpdateSubmergedURI;
        private static Task UpdateReworkedTask;
        private static Task UpdateSubmergedTask;
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

            //Only check of Submerged/LevelImpostor update if Submerged/LevelImpostor is already installed
            var codeBase = TownOfUsReworked.Executing.Location;
            var uri = new UriBuilder(codeBase);

            if (ModCompatibility.SubLoaded)
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

            if (updateType == "Reworked")
            {
                info = "Updating Town Of Us Reworked\nPlease wait...";
                InfoPopup.Show(info);

                if (UpdateReworkedTask == null)
                {
                    if (UpdateReworkedURI != null)
                        UpdateReworkedTask = DownloadUpdate("Reworked");
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

                if (UpdateSubmergedTask == null)
                {
                    if (UpdateSubmergedURI != null)
                        UpdateSubmergedTask = DownloadUpdate("Submerged");
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
                Utils.LogSomething(e);
            }
        }

        public static async Task<bool> CheckForUpdate(string updateType = "Reworked")
        {
            //Checks the github api for Town Of Us Reworked tags. Compares current version (from VersionString in TownOfUsReworked.cs) to the latest tag version (on GitHub)
            try
            {
                var githubURI = "";

                if (updateType == "Reworked")
                    githubURI = "https://api.github.com/repos/AlchlcDvl/TownOfUsReworked/releases/latest";
                else if (updateType == "Submerged")
                    githubURI = "https://api.github.com/repos/SubmergedAmongUs/Submerged/releases/latest";

                HttpClient http = new();
                http.DefaultRequestHeaders.Add("User-Agent", $"{updateType} Updater");
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
                    return false; //Something went wrong

                var diff = 0;
                var ver = Version.Parse(tagname.Replace("v", ""));

                if (updateType == "Reworked")
                {
                    //Check Reworked version
                    diff = TownOfUsReworked.Version.CompareTo(ver);

                    if (diff < 0)
                    {
                        //Reworked update required
                        HasReworkedUpdate = true;
                    }
                }
                else if (updateType == "Submerged")
                {
                    //Accounts for broken version
                    if (ModCompatibility.SubVersion == null)
                        HasSubmergedUpdate = true;
                    else
                    {
                        diff = ModCompatibility.SubVersion.CompareTo(SemanticVersioning.Version.Parse(tagname.Replace("v", "")));

                        if (diff < 0)
                        {
                            //Submerged update required
                            HasSubmergedUpdate = true;
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
                        if (updateType == "Reworked")
                            UpdateReworkedURI = asset.browser_download_url;
                        else if (updateType == "Submerged")
                            UpdateSubmergedURI = asset.browser_download_url;

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
            //Downloads the new dll from GitHub into the plugins folder
            var downloadDLL= "";
            var info = "";

            if (updateType == "Reworked")
            {
                downloadDLL = UpdateReworkedURI;
                info = "Town Of Us Reworked\nupdated successfully.\nPlease RESTART the game.";
            }
            else if (updateType == "Submerged")
            {
                downloadDLL = UpdateSubmergedURI;
                info = "Submerged\nupdated successfully.\nPlease RESTART the game.";
            }

            try
            {
                HttpClient http = new();
                http.DefaultRequestHeaders.Add("User-Agent", $"{updateType} Updater");
                var response = await http.GetAsync(new Uri(downloadDLL), HttpCompletionOption.ResponseContentRead);

                if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                {
                    Utils.LogSomething("Server returned no data: " + response.StatusCode.ToString());
                    return false;
                }

                var codeBase = TownOfUsReworked.Executing.Location;
                var uri = new UriBuilder(codeBase);
                var fullname = Uri.UnescapeDataString(uri.Path);

                //TODO: A better solution than this to correctly name the dll files

                if (updateType == "Submerged")
                    fullname = fullname.Replace("Reworked", "Submerged");

                if (File.Exists(fullname + ".old")) //Clear old file in case it wasnt
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
    }
}