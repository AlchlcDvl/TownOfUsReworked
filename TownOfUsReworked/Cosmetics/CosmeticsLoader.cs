using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TownOfUsReworked.Cosmetics;

[HarmonyPatch]
public static class CosmeticsLoader
{
    private const string REPO = "https://raw.githubusercontent.com/AlchlcDvl/ReworkedAssets/master";
    private static bool HatsRunning;
    /*private static bool NameplatesRunning;
    private static bool VisorsRunning;
    public static readonly List<CustomNameplate> NameplateDetails = new();*/
    public static readonly List<CustomHat> HatDetails = new();
    //public static readonly List<CustomVisor> VisorDetails = new();

    public static void LaunchFetchers(bool update)
    {
        try
        {
            LaunchHatFetcher(update);
            /*LaunchNameplateFetcher(update);
            LaunchVisorFetcher(update);*/
        } catch {}
    }

    private static void LaunchHatFetcher(bool update)
    {
        if (HatsRunning)
            return;

        HatsRunning = true;
        _ = LaunchHatFetcherAsync(update);
        LogSomething("Fetched hats");
    }

    /*private static void LaunchNameplateFetcher(bool update)
    {
        if (NameplatesRunning)
            return;

        NameplatesRunning = true;
        _ = LaunchNameplateFetcherAsync(update);
        LogSomething("Fetched nameplates");
    }

    private static void LaunchVisorFetcher(bool update)
    {
        if (VisorsRunning)
            return;

        VisorsRunning = true;
        _ = LaunchVisorFetcherAsync(update);
        LogSomething("Fetched visors");
    }*/

    private static async Task LaunchHatFetcherAsync(bool update)
    {
        try
        {
            var status = await FetchHats(update);

            if (status != HttpStatusCode.OK)
                LogSomething("Custom Hats could not be loaded");
        }
        catch (Exception e)
        {
            LogSomething("Unable to fetch hats\n" + e.Message);
        }

        HatsRunning = false;
    }

    /*private static async Task LaunchNameplateFetcherAsync(bool update)
    {
        try
        {
            var status = await FetchNameplates(update);

            if (status != HttpStatusCode.OK)
                LogSomething("Custom Nameplates could not be loaded");
        }
        catch (Exception e)
        {
            LogSomething("Unable to fetch nameplates\n" + e.Message);
        }

        NameplatesRunning = false;
    }

    private static async Task LaunchVisorFetcherAsync(bool update)
    {
        try
        {
            var status = await FetchVisors(update);

            if (status != HttpStatusCode.OK)
                LogSomething("Custom Visors could not be loaded");
        }
        catch (Exception e)
        {
            LogSomething("Unable to fetch visors\n" + e.Message);
        }

        VisorsRunning = false;
    }*/

    private static async Task<HttpStatusCode> FetchHats(bool update)
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.CacheControl = new() { NoCache = true };
        var response = await http.GetAsync(new Uri($"{REPO}/Hats.json"), HttpCompletionOption.ResponseContentRead);

        try
        {
            if (response.StatusCode != HttpStatusCode.OK)
                return response.StatusCode;

            if (response.Content == null)
            {
                LogSomething("Server returned no data: " + response.StatusCode.ToString());
                return HttpStatusCode.ExpectationFailed;
            }

            var json = await response.Content.ReadAsStringAsync();
            var jobj = JObject.Parse(json)["hats"];

            if (jobj == null || !jobj.HasValues)
                return HttpStatusCode.ExpectationFailed;

            var hatdatas = new List<CustomHat>();

            for (var current = jobj.First; current != null; current = current.Next)
            {
                if (current.HasValues)
                {
                    var info = new CustomHat
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
                    hatdatas.Add(info);
                }
            }

            var markedfordownload = new List<string>();

            if (update && Directory.Exists(TownOfUsReworked.Hats))
            {
                var d = new DirectoryInfo(TownOfUsReworked.Hats);
                d.GetFiles("*.png").Select(x => x.FullName).ToArray().ToList().ForEach(File.Delete);
            }

            if (!Directory.Exists(TownOfUsReworked.Hats))
                Directory.CreateDirectory(TownOfUsReworked.Hats);

            foreach (var data in hatdatas)
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
                var hatFileResponse = await http.GetAsync($"{REPO}/hats/{file}.png", HttpCompletionOption.ResponseContentRead);

                if (hatFileResponse.StatusCode != HttpStatusCode.OK)
                    continue;

                var responseStream = await hatFileResponse.Content.ReadAsStreamAsync();
                var fileStream = File.Create($"{TownOfUsReworked.Hats}{file}.png");
                responseStream.CopyTo(fileStream);
            }

            HatDetails.Clear();
            HatDetails.AddRange(hatdatas);
        }
        catch (Exception ex)
        {
            LogSomething(ex);
        }

        return HttpStatusCode.OK;
    }

    /*private static async Task<HttpStatusCode> FetchVisors(bool update)
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.CacheControl = new() { NoCache = true };
        var response = await http.GetAsync(new Uri($"{REPO}/Visors.json"), HttpCompletionOption.ResponseContentRead);

        try
        {
            if (response.StatusCode != HttpStatusCode.OK)
                return response.StatusCode;

            if (response.Content == null)
            {
                LogSomething("Server returned no data: " + response.StatusCode.ToString());
                return HttpStatusCode.ExpectationFailed;
            }

            var json = await response.Content.ReadAsStringAsync();
            var jobj = JObject.Parse(json)["visors"];

            if (jobj == null || !jobj.HasValues)
                return HttpStatusCode.ExpectationFailed;

            var visorDatas = new List<CustomVisor>();

            for (var current = jobj.First; current != null; current = current.Next)
            {
                if (current.HasValues)
                {
                    var info = new CustomVisor
                    {
                        Name = current["name"]?.ToString(),
                        ID = current["id"]?.ToString()
                    };

                    if (info.ID == null || info.Name == null) //Required
                        continue;

                    info.FlipID = current["flipid"]?.ToString();
                    info.Artist = current["artist"]?.ToString();
                    info.Condition = current["condition"]?.ToString();
                    info.FloorID = current["floorid"]?.ToString();
                    info.ClimbID = current["climbid"]?.ToString();
                    info.Adaptive = current["adaptive"] != null;
                    visorDatas.Add(info);
                }
            }

            var markedfordownload = new List<string>();

            if (update && Directory.Exists(TownOfUsReworked.Visors))
            {
                var d = new DirectoryInfo(TownOfUsReworked.Visors);
                d.GetFiles("*.png").Select(x => x.FullName).ToArray().ToList().ForEach(File.Delete);
            }

            if (!Directory.Exists(TownOfUsReworked.Visors))
                Directory.CreateDirectory(TownOfUsReworked.Visors);

            foreach (var data in visorDatas)
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
                var hatFileResponse = await http.GetAsync($"{REPO}/visors/{file}.png", HttpCompletionOption.ResponseContentRead);

                if (hatFileResponse.StatusCode != HttpStatusCode.OK)
                    continue;

                var responseStream = await hatFileResponse.Content.ReadAsStreamAsync();
                var fileStream = File.Create($"{TownOfUsReworked.Visors}{file}.png");
                responseStream.CopyTo(fileStream);
            }

            VisorDetails.Clear();
            VisorDetails.AddRange(visorDatas);
        }
        catch (Exception ex)
        {
            LogSomething(ex);
        }

        return HttpStatusCode.OK;
    }

    private static async Task<HttpStatusCode> FetchNameplates(bool update)
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.CacheControl = new() { NoCache = true };
        var response = await http.GetAsync(new Uri($"{REPO}/Nameplates.json"), HttpCompletionOption.ResponseContentRead);

        try
        {
            if (response.StatusCode != HttpStatusCode.OK)
                return response.StatusCode;

            if (response.Content == null)
            {
                LogSomething("Server returned no data: " + response.StatusCode.ToString());
                return HttpStatusCode.ExpectationFailed;
            }

            var json = await response.Content.ReadAsStringAsync();
            var jobj = JObject.Parse(json)["nameplates"];

            if (jobj == null || !jobj.HasValues)
                return HttpStatusCode.ExpectationFailed;

            var namePlateDatas = new List<CustomNameplate>();

            for (var current = jobj.First; current != null; current = current.Next)
            {
                if (current.HasValues)
                {
                    var info = new CustomNameplate
                    {
                        Name = current["name"]?.ToString(),
                        ID = current["id"]?.ToString()
                    };

                    if (info.ID == null || info.Name == null) //Required
                        continue;

                    info.Artist = current["artist"]?.ToString();
                    info.Condition = current["condition"]?.ToString();
                    namePlateDatas.Add(info);
                }
            }

            var markedfordownload = new List<string>();

            if (update && Directory.Exists(TownOfUsReworked.Nameplates))
            {
                var d = new DirectoryInfo(TownOfUsReworked.Nameplates);
                d.GetFiles("*.png").Select(x => x.FullName).ToArray().ToList().ForEach(File.Delete);
            }

            if (!Directory.Exists(TownOfUsReworked.Nameplates))
                Directory.CreateDirectory(TownOfUsReworked.Nameplates);

            foreach (var data in namePlateDatas)
            {
                if (!File.Exists(TownOfUsReworked.Nameplates + data.ID + ".png"))
                    markedfordownload.Add(data.ID);
            }

            foreach (var file in markedfordownload)
            {
                var NameplateFileResponse = await http.GetAsync($"{REPO}/nameplates/{file}.png", HttpCompletionOption.ResponseContentRead);

                if (NameplateFileResponse.StatusCode != HttpStatusCode.OK)
                    continue;

                var responseStream = await NameplateFileResponse.Content.ReadAsStreamAsync();
                var fileStream = File.Create($"{TownOfUsReworked.Nameplates}{file}.png");
                responseStream.CopyTo(fileStream);
            }

            NameplateDetails.Clear();
            NameplateDetails.AddRange(namePlateDatas);
        }
        catch (Exception ex)
        {
            LogSomething(ex);
        }

        return HttpStatusCode.OK;
    }*/
}