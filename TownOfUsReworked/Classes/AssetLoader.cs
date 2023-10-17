using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TownOfUsReworked.Classes;

public static class AssetLoader
{
    private const string REPO = "https://raw.githubusercontent.com/AlchlcDvl/ReworkedAssets/main";
    private static bool HatsRunning;
    private static bool NameplatesRunning;
    private static bool VisorsRunning;
    private static bool LangRunning;
    public static readonly List<CustomNameplate> NameplateDetails = new();
    public static readonly List<CustomHat> HatDetails = new();
    public static readonly List<CustomVisor> VisorDetails = new();
    public static readonly List<Language> AllTranslations = new();
    private static bool Loaded;
    private static HttpClient Client = null;

    private static void SetUpClient()
    {
        Client = new() { DefaultRequestHeaders = { {"User-Agent", "Loader"} } };
        Client.DefaultRequestHeaders.CacheControl = new() { NoCache = true };
    }

    public static void LoadAssets()
    {
        SoundEffects.Clear();
        Sizes.Clear();
        Sprites.Clear();
        Frequencies.Clear();
        Array.Clear(PortalAnimation, 0, 205);

        foreach (var resourceName in TownOfUsReworked.Core.GetManifestResourceNames())
        {
            if (resourceName.EndsWith(".png"))
            {
                var name = resourceName.Replace(".png", "").Replace(TownOfUsReworked.Buttons, "").Replace(TownOfUsReworked.Misc, "").Replace(TownOfUsReworked.Portal, "");

                if (name is "CurrentSettings" or "Help" or "Plus" or "Minus" or "Wiki")
                    Sizes.Add(name, 180);
                else if (name == "Phone")
                    Sizes.Add(name, 200);
                else if (name == "Cursor")
                    Sizes.Add(name, 115);
                else if (name == "NightVision")
                    Sizes.Add(name, 350);
                else
                    Sizes.Add(name, 100);
            }
            else if (resourceName.EndsWith(".raw"))
            {
                var name = resourceName.Replace(".raw", "").Replace(TownOfUsReworked.Sounds, "");

                if (name.Contains("Intro"))
                    Frequencies.Add(name, 36000);
                else
                    Frequencies.Add(name, 48000);
            }
        }

        var position = 0;

        foreach (var resourceName in TownOfUsReworked.Core.GetManifestResourceNames())
        {
            if ((resourceName.StartsWith(TownOfUsReworked.Buttons) || resourceName.StartsWith(TownOfUsReworked.Misc)) && resourceName.EndsWith(".png"))
                Sprites.Add(resourceName.Replace(".png", "").Replace(TownOfUsReworked.Buttons, "").Replace(TownOfUsReworked.Misc, ""), CreateSprite(resourceName));
            else if (resourceName.StartsWith(TownOfUsReworked.Sounds) && resourceName.EndsWith(".raw"))
                SoundEffects.Add(resourceName.Replace(".raw", "").Replace(TownOfUsReworked.Sounds, ""), CreateAudio(resourceName));
            else if (resourceName.StartsWith(TownOfUsReworked.Portal) && resourceName.EndsWith(".png"))
            {
                if (PortalAnimation[position] == null)
                    PortalAnimation[position] = CreateSprite(resourceName);

                position++;
            }
        }

        Cursor.SetCursor(GetSprite("Cursor").texture, Vector2.zero, CursorMode.Auto);
    }

    public static void LaunchFetchers(bool update)
    {
        if (Loaded)
            return;

        Loaded = true;
        SetUpClient();
        LaunchHatFetcher(update);
        LaunchVisorFetcher(update);
        LaunchNameplateFetcher(update);
        LaunchTranslationFetcher();
        LoadVanillaSounds();
    }

    public static void LoadVanillaSounds()
    {
        SoundEffects.TryAdd("EngineerIntro", GetIntroSound(RoleTypes.Engineer));
        SoundEffects.TryAdd("MorphlingIntro", GetIntroSound(RoleTypes.Shapeshifter));
        SoundEffects.TryAdd("MedicIntro", GetIntroSound(RoleTypes.Scientist));
        SoundEffects.TryAdd("CrewmateIntro", GetIntroSound(RoleTypes.Crewmate));
        SoundEffects.TryAdd("ImpostorIntro", GetIntroSound(RoleTypes.Impostor));
    }

    private static void LaunchHatFetcher(bool update)
    {
        if (HatsRunning)
            return;

        HatsRunning = true;
        _ = LaunchHatFetcherAsync(update);
        LogMessage("Fetched hats");
    }

    private static void LaunchNameplateFetcher(bool update)
    {
        if (NameplatesRunning)
            return;

        NameplatesRunning = true;
        _ = LaunchNameplateFetcherAsync(update);
        LogMessage("Fetched nameplates");
    }

    private static void LaunchVisorFetcher(bool update)
    {
        if (VisorsRunning)
            return;

        VisorsRunning = true;
        _ = LaunchVisorFetcherAsync(update);
        LogMessage("Fetched visors");
    }

    private static void LaunchTranslationFetcher()
    {
        if (LangRunning)
            return;

        LangRunning = true;
        _ = LaunchTranslationFetcherAsync();
        LogMessage("Fetched translations");
    }

    private static async Task LaunchHatFetcherAsync(bool update)
    {
        try
        {
            var status = await FetchHats(update);

            if (status != HttpStatusCode.OK)
                LogError("Hats could not be loaded");
        }
        catch (Exception e)
        {
            LogError($"Unable to fetch hats\n{e}");
        }

        HatsRunning = false;
    }

    private static async Task LaunchNameplateFetcherAsync(bool update)
    {
        try
        {
            var status = await FetchNameplates(update);

            if (status != HttpStatusCode.OK)
                LogError("Nameplates could not be loaded");
        }
        catch (Exception e)
        {
            LogError($"Unable to fetch nameplates\n{e}");
        }

        NameplatesRunning = false;
    }

    private static async Task LaunchVisorFetcherAsync(bool update)
    {
        try
        {
            var status = await FetchVisors(update);

            if (status != HttpStatusCode.OK)
                LogError("Visors could not be loaded");
        }
        catch (Exception e)
        {
            LogError($"Unable to fetch visors\n{e}");
        }

        VisorsRunning = false;
    }

    private static async Task LaunchTranslationFetcherAsync()
    {
        try
        {
            var status = await FetchTranslations();

            if (status != HttpStatusCode.OK)
                LogError("Translations could not be loaded");
        }
        catch (Exception e)
        {
            LogError($"Unable to fetch translations\n{e}");
        }

        LangRunning = false;
    }

    private static async Task<HttpStatusCode> FetchHats(bool update)
    {
        if (Client == null)
        {
            LogError($"Client was null");
            return HttpStatusCode.NotFound;
        }

        try
        {
            var response = await Client.GetAsync(new Uri($"{REPO}/Hats.json"), HttpCompletionOption.ResponseContentRead);

            if (response.StatusCode != HttpStatusCode.OK)
                return response.StatusCode;

            if (response.Content == null)
            {
                LogError($"Server returned no data: {response.StatusCode}");
                return response.StatusCode;
            }

            var json = await response.Content.ReadAsStringAsync();
            var jobj = JObject.Parse(json)["hats"];

            if (jobj == null || !jobj.HasValues)
                return HttpStatusCode.ExpectationFailed;

            HatDetails.Clear();

            for (var current = jobj.First; current != null; current = current.Next)
            {
                if (current.HasValues)
                {
                    var info = new CustomHat()
                    {
                        Name = current["name"]?.ToString(),
                        ID = current["id"]?.ToString()
                    };

                    if (info.ID == null || info.Name == null) //Required
                        continue;

                    info.BackID = current["backid"]?.ToString();
                    info.ClimbID = current["climbid"]?.ToString();
                    info.FlipID = current["flipid"]?.ToString();
                    info.BackFlipID = current["BackFlipID"]?.ToString();
                    info.Artist = current["artist"]?.ToString();
                    info.Condition = current["condition"]?.ToString();
                    info.FloorID = current["floorid"]?.ToString();
                    info.NoBouce = current["nobounce"] != null;
                    info.Adaptive = current["adaptive"] != null;
                    info.Behind = current["behind"] != null;
                    HatDetails.Add(info);
                }
            }

            var markedfordownload = new List<string>();

            if (update && Directory.Exists(TownOfUsReworked.Hats))
            {
                var d = new DirectoryInfo(TownOfUsReworked.Hats);
                d.GetFiles("*.png").Select(x => x.FullName).ForEach(File.Delete);
            }

            if (!Directory.Exists(TownOfUsReworked.Hats))
                Directory.CreateDirectory(TownOfUsReworked.Hats);

            foreach (var data in HatDetails)
            {
                if (!File.Exists(TownOfUsReworked.Hats + data.ID + ".png"))
                    markedfordownload.Add(data.ID);

                if (data.BackID != null && !File.Exists(TownOfUsReworked.Hats + data.BackID + ".png"))
                    markedfordownload.Add(data.BackID);

                if (data.ClimbID != null && !File.Exists(TownOfUsReworked.Hats + data.ClimbID + ".png"))
                    markedfordownload.Add(data.ClimbID);

                if (data.FlipID != null && !File.Exists(TownOfUsReworked.Hats + data.FlipID + ".png"))
                    markedfordownload.Add(data.FlipID);

                if (data.BackFlipID != null && !File.Exists(TownOfUsReworked.Hats + data.BackFlipID + ".png"))
                    markedfordownload.Add(data.BackFlipID);

                if (data.FloorID != null && !File.Exists(TownOfUsReworked.Hats + data.FloorID + ".png"))
                    markedfordownload.Add(data.FloorID);
            }

            foreach (var file in markedfordownload)
            {
                var hatFileResponse = await Client.GetAsync($"{REPO}/hats/{file}.png", HttpCompletionOption.ResponseContentRead);

                if (hatFileResponse.StatusCode != HttpStatusCode.OK)
                {
                    LogError($"Error downloading {file}: {hatFileResponse.StatusCode}");
                    continue;
                }

                using var responseStream = await hatFileResponse.Content.ReadAsStreamAsync();
                using var fileStream = File.Create($"{TownOfUsReworked.Hats}\\{file}.png");
                responseStream.CopyTo(fileStream);
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }

        return HttpStatusCode.OK;
    }

    private static async Task<HttpStatusCode> FetchVisors(bool update)
    {
        if (Client == null)
        {
            LogError($"Client was null");
            return HttpStatusCode.NotFound;
        }

        try
        {
            var response = await Client.GetAsync(new Uri($"{REPO}/Visors.json"), HttpCompletionOption.ResponseContentRead);

            if (response.StatusCode != HttpStatusCode.OK)
                return response.StatusCode;

            if (response.Content == null)
            {
                LogError($"Server returned no data: {response.StatusCode}");
                return response.StatusCode;
            }

            var json = await response.Content.ReadAsStringAsync();
            var jobj = JObject.Parse(json)["visors"];

            if (jobj == null || !jobj.HasValues)
                return HttpStatusCode.ExpectationFailed;

            VisorDetails.Clear();

            for (var current = jobj.First; current != null; current = current.Next)
            {
                if (current.HasValues)
                {
                    var info = new CustomVisor()
                    {
                        Name = current["name"]?.ToString(),
                        ID = current["id"]?.ToString()
                    };

                    if (info.ID == null || info.Name == null) //Required
                        continue;

                    info.FlipID = current["flipid"]?.ToString();
                    info.Artist = current["artist"]?.ToString();
                    info.FloorID = current["floorid"]?.ToString();
                    info.ClimbID = current["climbid"]?.ToString();
                    info.Adaptive = current["adaptive"] != null;
                    info.InFront = current["infront"] != null;
                    VisorDetails.Add(info);
                }
            }

            var markedfordownload = new List<string>();

            if (update && Directory.Exists(TownOfUsReworked.Visors))
            {
                var d = new DirectoryInfo(TownOfUsReworked.Visors);
                d.GetFiles("*.png").Select(x => x.FullName).ForEach(File.Delete);
            }

            if (!Directory.Exists(TownOfUsReworked.Visors))
                Directory.CreateDirectory(TownOfUsReworked.Visors);

            foreach (var data in VisorDetails)
            {
                if (!File.Exists(TownOfUsReworked.Visors + data.ID + ".png"))
                    markedfordownload.Add(data.ID);

                if (data.FlipID != null && !File.Exists(TownOfUsReworked.Visors + data.FlipID + ".png"))
                    markedfordownload.Add(data.FlipID);

                if (data.ClimbID != null && !File.Exists(TownOfUsReworked.Visors + data.ClimbID + ".png"))
                    markedfordownload.Add(data.ClimbID);

                if (data.FloorID != null && !File.Exists(TownOfUsReworked.Visors + data.FloorID + ".png"))
                    markedfordownload.Add(data.FloorID);
            }

            foreach (var file in markedfordownload)
            {
                var visorFileResponse = await Client.GetAsync($"{REPO}/visors/{file}.png", HttpCompletionOption.ResponseContentRead);

                if (visorFileResponse.StatusCode != HttpStatusCode.OK)
                {
                    LogError($"Error downloading {file}: {visorFileResponse.StatusCode}");
                    continue;
                }

                using var responseStream = await visorFileResponse.Content.ReadAsStreamAsync();
                using var fileStream = File.Create($"{TownOfUsReworked.Visors}\\{file}.png");
                responseStream.CopyTo(fileStream);
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }

        return HttpStatusCode.OK;
    }

    private static async Task<HttpStatusCode> FetchNameplates(bool update)
    {
        if (Client == null)
        {
            LogError($"Client was null");
            return HttpStatusCode.NotFound;
        }

        try
        {
            var response = await Client.GetAsync(new Uri($"{REPO}/Nameplates.json"), HttpCompletionOption.ResponseContentRead);

            if (response.StatusCode != HttpStatusCode.OK)
                return response.StatusCode;

            if (response.Content == null)
            {
                LogError($"Server returned no data: {response.StatusCode}");
                return response.StatusCode;
            }

            var json = await response.Content.ReadAsStringAsync();
            var jobj = JObject.Parse(json)["nameplates"];

            if (jobj == null || !jobj.HasValues)
                return HttpStatusCode.ExpectationFailed;

            NameplateDetails.Clear();

            for (var current = jobj.First; current != null; current = current.Next)
            {
                if (current.HasValues)
                {
                    var info = new CustomNameplate()
                    {
                        Name = current["name"]?.ToString(),
                        ID = current["id"]?.ToString()
                    };

                    if (info.ID == null || info.Name == null) //Required
                        continue;

                    info.Artist = current["artist"]?.ToString();
                    NameplateDetails.Add(info);
                }
            }

            var markedfordownload = new List<string>();

            if (update && Directory.Exists(TownOfUsReworked.Nameplates))
            {
                var d = new DirectoryInfo(TownOfUsReworked.Nameplates);
                d.GetFiles("*.png").Select(x => x.FullName).ForEach(File.Delete);
            }

            if (!Directory.Exists(TownOfUsReworked.Nameplates))
                Directory.CreateDirectory(TownOfUsReworked.Nameplates);

            foreach (var data in NameplateDetails)
            {
                if (!File.Exists(TownOfUsReworked.Nameplates + data.ID + ".png"))
                    markedfordownload.Add(data.ID);
            }

            foreach (var file in markedfordownload)
            {
                var nameplateFileResponse = await Client.GetAsync($"{REPO}/nameplates/{file}.png", HttpCompletionOption.ResponseContentRead);

                if (nameplateFileResponse.StatusCode != HttpStatusCode.OK)
                {
                    LogError($"Error downloading {file}: {nameplateFileResponse.StatusCode}");
                    continue;
                }

                using var responseStream = await nameplateFileResponse.Content.ReadAsStreamAsync();
                using var fileStream = File.Create($"{TownOfUsReworked.Nameplates}\\{file}.png");
                responseStream.CopyTo(fileStream);
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }

        return HttpStatusCode.OK;
    }

    private static async Task<HttpStatusCode> FetchTranslations()
    {
        if (Client == null)
        {
            LogError($"Client was null");
            return HttpStatusCode.NotFound;
        }

        try
        {
            var response = await Client.GetAsync(new Uri($"{REPO}/Languages.json"), HttpCompletionOption.ResponseContentRead);

            if (response.StatusCode != HttpStatusCode.OK)
                return response.StatusCode;

            if (response.Content == null)
            {
                LogError($"Server returned no data: {response.StatusCode}");
                return response.StatusCode;
            }

            var json = await response.Content.ReadAsStringAsync();
            var jobj = JObject.Parse(json)["languages"];

            if (jobj == null || !jobj.HasValues)
                return HttpStatusCode.ExpectationFailed;

            AllTranslations.Clear();

            for (var current = jobj.First; current != null; current = current.Next)
            {
                if (current.HasValues)
                {
                    var info = new Language() { ID = current["id"]?.ToString() };

                    if (info.ID == null) //Required
                        continue;

                    info.English = current["english"]?.ToString();
                    info.SChinese = current["schinese"]?.ToString();
                    AllTranslations.Add(info);
                }
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
        }

        return HttpStatusCode.OK;
    }
}