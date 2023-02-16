using UnityEngine;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Reactor.Utilities;
using System.Text.Json.Serialization;

namespace TownOfUsReworked.Classes
{
    public class ModUpdater
    { 
        public static bool running = false;
        public static bool hasTOUUpdate = false;
        public static bool hasSubmergedUpdate = false;
        public static string updateTOUURI = null;
        public static string updateSubmergedURI = null;
        private static Task updateTOUTask = null;
        private static Task updateSubmergedTask = null;
        public static GenericPopup InfoPopup;

        public static void LaunchUpdater()
        {
            if (running)
                return;

            running = true;

            CheckForUpdate("TOU").GetAwaiter().GetResult();

            //Only check of Submerged update if Submerged is already installed
            string codeBase = Assembly.GetExecutingAssembly().Location;
            System.UriBuilder uri = new System.UriBuilder(codeBase);
            string submergedPath = System.Uri.UnescapeDataString(uri.Path.Replace("TownOfUsReworked", "Submerged"));

            if (File.Exists(submergedPath))
                CheckForUpdate("Submerged").GetAwaiter().GetResult();

            ClearOldVersions();
        }

        public static void ExecuteUpdate(string updateType = "TOU")
        {
            string info = "";

            if (updateType == "TOU")
            {
                info = "Updating Town Of Us\nPlease wait...";
                ModUpdater.InfoPopup.Show(info);

                if (updateTOUTask == null)
                {
                    if (updateTOUURI != null)
                        updateTOUTask = DownloadUpdate("TOU");
                    else
                        info = "Unable to auto-update\nPlease update manually";
                }
                else
                    info = "Update might already\nbe in progress";
            }
            else if (updateType == "Submerged")
            {
                info = "Updating Submerged\nPlease wait...";
                ModUpdater.InfoPopup.Show(info);

                if (updateSubmergedTask == null)
                {
                    if (updateSubmergedURI != null)
                        updateSubmergedTask = DownloadUpdate("Submerged");
                    else
                        info = "Unable to auto-update\nPlease update manually";
                }
                else
                    info = "Update might already\nbe in progress";
            }

            ModUpdater.InfoPopup.StartCoroutine(Effects.Lerp(0.01f, new System.Action<float>((p) => { ModUpdater.SetPopupText(info); })));
        }
        
        public static void ClearOldVersions()
        {
            //Removes any old versions (Denoted by the suffix `.old`)
            try
            {
                DirectoryInfo d = new DirectoryInfo(Path.GetDirectoryName(Application.dataPath) + @"\BepInEx\plugins");
                string[] files = d.GetFiles("*.old").Select(x => x.FullName).ToArray();

                foreach (string f in files)
                    File.Delete(f);
            }
            catch (System.Exception e)
            {
                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Exception occured when clearing old versions:\n" + e);
            }
        }

        public static async Task<bool> CheckForUpdate(string updateType = "TOU")
        {
            //Checks the github api for Town Of Us tags. Compares current version (from VersionString in TownOfUsReworked.cs) to the latest tag version (on GitHub)
            try
            {
                string githubURI = "";

                if (updateType == "TOU")
                    githubURI = "https://api.github.com/repos/AlchlcDvl/TownOfUsReworked/releases/latest";
                else if (updateType == "Submerged")
                    githubURI = "https://api.github.com/repos/SubmergedAmongUs/Submerged/releases/latest";

                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "TownOfUsReworked Updater");
                var response = await http.GetAsync(new System.Uri(githubURI), HttpCompletionOption.ResponseContentRead);
                
                if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                {
                    PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Server returned no data: " + response.StatusCode.ToString());
                    return false;
                }

                string json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<GitHubApiObject>(json);
                string tagname = data.tag_name;

                if (tagname == null)
                    return false; // Something went wrong

                int diff = 0;
                System.Version ver = System.Version.Parse(tagname.Replace("v", ""));

                if (updateType == "TOU")
                {
                    //Check TOU version
                    diff = TownOfUsReworked.Version.CompareTo(ver);

                    if (diff < 0)
                    {
                        //TOU update required
                        hasTOUUpdate = true;
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
                        if (updateType == "TOU")
                            updateTOUURI = asset.browser_download_url;
                        else if (updateType == "Submerged")
                            updateSubmergedURI = asset.browser_download_url;
                        
                        return true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage(ex);
            }

            return false;
        }

        public static async Task<bool> DownloadUpdate(string updateType = "TOU")
        {
            //Downloads the new TownOfUsReworked/Submerged dll from GitHub into the plugins folder
            string downloadDLL= "";
            string info = "";

            if (updateType == "TOU")
            {
                downloadDLL = updateTOUURI;
                info = "Town Of Us\nupdated successfully.\nPlease RESTART the game.";
            }
            else if (updateType == "Submerged")
            {
                downloadDLL = updateSubmergedURI;
                info = "Submerged\nupdated successfully.\nPlease RESTART the game.";
            }

            try
            {
                HttpClient http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "TownOfUsReworked Updater");
                var response = await http.GetAsync(new System.Uri(downloadDLL), HttpCompletionOption.ResponseContentRead);

                if (response.StatusCode != HttpStatusCode.OK || response.Content == null)
                {
                    PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Server returned no data: " + response.StatusCode.ToString());
                    return false;
                }

                string codeBase = Assembly.GetExecutingAssembly().Location;
                System.UriBuilder uri = new System.UriBuilder(codeBase);
                string fullname = System.Uri.UnescapeDataString(uri.Path);

                if (updateType == "Submerged")
                    fullname = fullname.Replace("TownOfUsReworked", "Submerged"); //TODO A better solution than this to correctly name the dll files

                if (File.Exists(fullname + ".old")) //Clear old file in case it wasnt;
                    File.Delete(fullname + ".old");

                File.Move(fullname, fullname + ".old"); //Rename current executable to old

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    using (var fileStream = File.Create(fullname))
                        responseStream.CopyTo(fileStream);
                }

                ShowPopup(info);
                return true;
            }
            catch (System.Exception ex)
            {
                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage(ex);
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

        class GitHubApiObject
        {
            [JsonPropertyName("tag_name")]
            public string tag_name { get; set; }
            [JsonPropertyName("assets")]
            public GitHubApiAsset[] assets { get; set; }
        }

        class GitHubApiAsset
        {
            [JsonPropertyName("browser_download_url")]
            public string browser_download_url { get; set; }
        }
    }
}