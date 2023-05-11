using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using TownOfUsReworked.Classes;
using System;

namespace TownOfUsReworked.Cosmetics
{
    public static class CosmeticsLoader
    {
        private const string REPO = "https://raw.githubusercontent.com/AlchlcDvl/ReworkedHats/master";
        public readonly static List<CustomNameplateOnline> NameplateDetails = new();
        public readonly static List<CustomHatOnline> HatDetails = new();
        public readonly static List<CustomVisorOnline> VisorDetails = new();

        public static void LaunchFetchers()
        {
            LaunchNameplateFetcherAsync();
            LaunchHatFetcherAsync();
            LaunchVisorFetcherAsync();
        }

        private static async void LaunchNameplateFetcherAsync()
        {
            try
            {
                var status = await FetchNameplates();

                if (status != HttpStatusCode.OK)
                    Utils.LogSomething("Custom Nameplates could not be loaded");
            }
            catch (Exception e)
            {
                Utils.LogSomething("Unable to fetch nameplates\n" + e.Message);
            }
        }

        private static async void LaunchHatFetcherAsync()
        {
            try
            {
                var status = await FetchHats();

                if (status != HttpStatusCode.OK)
                    Utils.LogSomething("Custom Hats could not be loaded");
            }
            catch (Exception e)
            {
                Utils.LogSomething("Unable to fetch hats\n" + e.Message);
            }
        }

        private static async void LaunchVisorFetcherAsync()
        {
            try
            {
                var status = await FetchVisors();

                if (status != HttpStatusCode.OK)
                    Utils.LogSomething("Custom Visors could not be loaded");
            }
            catch (Exception e)
            {
                Utils.LogSomething("Unable to fetch visors\n" + e.Message);
            }
        }

        public static async Task<HttpStatusCode> FetchHats()
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.CacheControl = new() { NoCache = true };
            var response = await http.GetAsync(new Uri($"{REPO}/CustomHats.json"), HttpCompletionOption.ResponseContentRead);

            try
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    return response.StatusCode;

                if (response.Content == null)
                {
                    Utils.LogSomething("Server returned no data: " + response.StatusCode.ToString());
                    return HttpStatusCode.ExpectationFailed;
                }

                var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var jobj = JObject.Parse(json)["hats"];

                if (jobj == null || jobj?.HasValues == false)
                    return HttpStatusCode.ExpectationFailed;

                var hatdatas = new List<CustomHatOnline>();

                for (var current = jobj.First; current != null; current = current.Next)
                {
                    if (current.HasValues)
                    {
                        var info = new CustomHatOnline
                        {
                            Name = current["name"]?.ToString(),
                            ID = SanitizeResourcePath(current["id"]?.ToString())
                        };

                        if (info.ID == null || info.Name == null) // required
                            continue;

                        info.Reshasha = current["reshasha"]?.ToString();
                        info.BackID = SanitizeResourcePath(current["backid"]?.ToString());
                        info.Reshashb = current["reshashb"]?.ToString();
                        info.ClimbID = SanitizeResourcePath(current["climbid"]?.ToString());
                        info.Reshashc = current["reshashc"]?.ToString();
                        info.FlipID = SanitizeResourcePath(current["flipid"]?.ToString());
                        info.Reshashf = current["reshashf"]?.ToString();
                        info.BackflipID = SanitizeResourcePath(current["backflipid"]?.ToString());
                        info.Reshashbf = current["reshashbf"]?.ToString();

                        info.Artist = current["artist"]?.ToString();
                        info.Condition = current["condition"]?.ToString();
                        info.NoBouce = current["nobounce"] != null;
                        info.Adaptive = current["adaptive"] != null;
                        info.Behind = current["behind"] != null;
                        hatdatas.Add(info);
                    }
                }

                var markedfordownload = new List<string>();
                var filePath = Path.GetDirectoryName(Application.dataPath) + "\\CustomHats\\";

                var md5 = MD5.Create();

                foreach (var data in hatdatas)
                {
                    if (DoesResourceRequireDownload(filePath + data.ID, data.Reshasha, md5))
                        markedfordownload.Add(data.ID);

                    if (data.BackID != null && DoesResourceRequireDownload(filePath + data.BackID, data.Reshashb, md5))
                        markedfordownload.Add(data.BackID);

                    if (data.ClimbID != null && DoesResourceRequireDownload(filePath + data.ClimbID, data.Reshashc, md5))
                        markedfordownload.Add(data.ClimbID);

                    if (data.FlipID != null && DoesResourceRequireDownload(filePath + data.FlipID, data.Reshashf, md5))
                        markedfordownload.Add(data.FlipID);

                    if (data.BackflipID != null && DoesResourceRequireDownload(filePath + data.BackflipID, data.Reshashbf, md5))
                        markedfordownload.Add(data.BackflipID);
                }

                foreach (var file in markedfordownload)
                {
                    var hatFileResponse = await http.GetAsync($"{REPO}/hats/{file}", HttpCompletionOption.ResponseContentRead);

                    if (hatFileResponse.StatusCode != HttpStatusCode.OK)
                        continue;

                    var responseStream = await hatFileResponse.Content.ReadAsStreamAsync();
                    var fileStream = File.Create($"{filePath}\\{file}");
                    responseStream.CopyTo(fileStream);
                }

                HatDetails.Clear();
                HatDetails.AddRange(hatdatas);
            }
            catch (Exception ex)
            {
                Utils.LogSomething(ex);
            }

            return HttpStatusCode.OK;
        }

        public static async Task<HttpStatusCode> FetchVisors()
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.CacheControl = new() { NoCache = true };
            var response = await http.GetAsync(new Uri($"{REPO}/CustomVisors.json"), HttpCompletionOption.ResponseContentRead);

            try
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    return response.StatusCode;

                if (response.Content == null)
                {
                    Utils.LogSomething("Server returned no data: " + response.StatusCode.ToString());
                    return HttpStatusCode.ExpectationFailed;
                }

                var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var jobj = JObject.Parse(json)["visors"];

                if (jobj == null || jobj?.HasValues == false)
                    return HttpStatusCode.ExpectationFailed;

                var visordatas = new List<CustomVisorOnline>();

                for (var current = jobj.First; current != null; current = current.Next)
                {
                    if (current.HasValues)
                    {
                        var info = new CustomVisorOnline
                        {
                            Name = current["name"]?.ToString(),
                            ID = SanitizeResourcePath(current["id"]?.ToString())
                        };

                        if (info.ID == null || info.Name == null) // required
                            continue;

                        info.Reshasha = current["reshasha"]?.ToString();
                        info.Reshashb = current["reshashb"]?.ToString();
                        info.Reshashc = current["reshashc"]?.ToString();
                        info.FlipID = SanitizeResourcePath(current["flipid"]?.ToString());
                        info.Reshashf = current["reshashf"]?.ToString();
                        info.Reshashbf = current["reshashbf"]?.ToString();

                        info.Artist = current["artist"]?.ToString();
                        info.Condition = current["condition"]?.ToString();
                        info.Adaptive = current["adaptive"] != null;
                        visordatas.Add(info);
                    }
                }

                var markedfordownload = new List<string>();
                var filePath = Path.GetDirectoryName(Application.dataPath) + "\\CustomHats\\";

                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                var md5 = MD5.Create();

                foreach (var data in visordatas)
                {
                    if (DoesResourceRequireDownload(filePath + data.ID, data.Reshasha, md5))
                        markedfordownload.Add(data.ID);

                    if (data.FlipID != null && DoesResourceRequireDownload(filePath + data.FlipID, data.Reshashf, md5))
                        markedfordownload.Add(data.FlipID);
                }

                foreach (var file in markedfordownload)
                {
                    var hatFileResponse = await http.GetAsync($"{REPO}/visors/{file}", HttpCompletionOption.ResponseContentRead);

                    if (hatFileResponse.StatusCode != HttpStatusCode.OK)
                        continue;

                    var responseStream = await hatFileResponse.Content.ReadAsStreamAsync();
                    var fileStream = File.Create($"{filePath}\\{file}");
                    responseStream.CopyTo(fileStream);
                }

                VisorDetails.Clear();
                VisorDetails.AddRange(visordatas);
            }
            catch (Exception ex)
            {
                Utils.LogSomething(ex);
            }

            return HttpStatusCode.OK;
        }

        private static string SanitizeResourcePath(string res)
        {
            if (res?.EndsWith(".png") != true)
                return null;

            return res.Replace("\\", "").Replace("/", "").Replace("*", "").Replace("..", "");
        }

        public static async Task<HttpStatusCode> FetchNameplates()
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.CacheControl = new() { NoCache = true };
            var response = await http.GetAsync(new Uri($"{REPO}/CustomNameplates.json"), HttpCompletionOption.ResponseContentRead);

            try
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    return response.StatusCode;

                if (response.Content == null)
                {
                    Utils.LogSomething("Server returned no data: " + response.StatusCode.ToString());
                    return HttpStatusCode.ExpectationFailed;
                }

                var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var jobj = JObject.Parse(json)["nameplates"];

                if (jobj == null || jobj?.HasValues == false)
                    return HttpStatusCode.ExpectationFailed;

                var NamePlateDatas = new List<CustomNameplateOnline>();

                for (var current = jobj.First; current != null; current = current.Next)
                {
                    if (current.HasValues)
                    {
                        var info = new CustomNameplateOnline
                        {
                            Name = current["name"]?.ToString(),
                            ID = SanitizeResourcePath(current["id"]?.ToString())
                        };

                        if (info.ID == null || info.Name == null) // required
                            continue;

                        info.Reshasha = current["reshasha"]?.ToString();
                        info.Reshashb = current["reshashb"]?.ToString();
                        info.Reshashc = current["reshashc"]?.ToString();
                        info.Reshashf = current["reshashf"]?.ToString();
                        info.Reshashbf = current["reshashbf"]?.ToString();
                        info.Artist = current["artist"]?.ToString();
                        info.Condition = current["condition"]?.ToString();
                        NamePlateDatas.Add(info);
                    }
                }

                var markedfordownload = new List<string>();
                var filePath = Path.GetDirectoryName(Application.dataPath) + "\\CustomNameplates\\";

                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                var md5 = MD5.Create();

                foreach (var data in NamePlateDatas)
                {
                    if (DoesResourceRequireDownload(filePath + data.ID, data.Reshasha, md5))
                        markedfordownload.Add(data.ID);
                }

                foreach (var file in markedfordownload)
                {
                    var NameplateFileResponse = await http.GetAsync($"{REPO}/nameplates/{file}", HttpCompletionOption.ResponseContentRead);

                    if (NameplateFileResponse.StatusCode != HttpStatusCode.OK)
                        continue;

                    var responseStream = await NameplateFileResponse.Content.ReadAsStreamAsync();
                    var fileStream = File.Create($"{filePath}\\{file}");
                    responseStream.CopyTo(fileStream);
                }

                NameplateDetails.Clear();
                NameplateDetails.AddRange(NamePlateDatas);
            }
            catch (Exception ex)
            {
                Utils.LogSomething(ex);
            }

            return HttpStatusCode.OK;
        }

        private static bool DoesResourceRequireDownload(string respath, string reshash, MD5 md5)
        {
            if (reshash == null || !File.Exists(respath))
                return true;

            var stream = File.OpenRead(respath);
            var hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
            return !reshash.Equals(hash);
        }

        public class CustomNameplateOnline : CustomNameplates.CustomNameplate
        {
            public string Reshasha { get; set; }
            public string Reshashb { get; set; }
            public string Reshashc { get; set; }
            public string Reshashf { get; set; }
            public string Reshashbf { get; set; }
        }

        public class CustomHatOnline : CustomHats.CustomHat
        {
            public string Reshasha { get; set; }
            public string Reshashb { get; set; }
            public string Reshashc { get; set; }
            public string Reshashf { get; set; }
            public string Reshashbf { get; set; }
        }

        public class CustomVisorOnline : CustomVisors.CustomVisor
        {
            public string Reshasha { get; set; }
            public string Reshashb { get; set; }
            public string Reshashc { get; set; }
            public string Reshashf { get; set; }
            public string Reshashbf { get; set; }
        }
    }
}