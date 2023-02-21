using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Reactor.Utilities.Extensions;

namespace TownOfUsReworked.Cosmetics
{
    internal static class Loader
    {
        private static string json = "metadata.json";
        public static Assembly Assembly => typeof(TownOfUsReworked).Assembly;

        public static HatMetadataJson LoadHatJson()
        {
            var stream = Assembly.GetManifestResourceStream($"{TownOfUsReworked.Hats}{json}");

            return JsonSerializer.Deserialize<HatMetadataJson>(Encoding.UTF8.GetString(stream.ReadFully()), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });
        }

        public static List<HatMetadataElement> LoadCustomHatData()
        {
            var json = LoadHatJson();
            List<HatMetadataElement> hats = new List<HatMetadataElement>();
            hats.AddRange(json.Credits);
            return hats;
        }

        public static NameplateMetadataJson LoadNameplateJson()
        {
            var stream = Assembly.GetManifestResourceStream($"{TownOfUsReworked.Nameplates}{json}");

            return JsonSerializer.Deserialize<NameplateMetadataJson>(Encoding.UTF8.GetString(stream.ReadFully()), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });
        }

        public static List<NameplateMetadataElement> LoadCustomNameplateData()
        {
            var json = LoadNameplateJson();
            List<NameplateMetadataElement> nameplates = new List<NameplateMetadataElement>();
            nameplates.AddRange(json.Credits);
            return nameplates;
        }

        public static VisorMetadataJson LoadVisorJson()
        {
            var stream = Assembly.GetManifestResourceStream($"{TownOfUsReworked.Visors}{json}");

            return JsonSerializer.Deserialize<VisorMetadataJson>(Encoding.UTF8.GetString(stream.ReadFully()), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });
        }

        public static List<VisorMetadataElement> LoadCustomVisorData()
        {
            var json = LoadVisorJson();
            List<VisorMetadataElement> visors = new List<VisorMetadataElement>();
            visors.AddRange(json.Credits);
            return visors;
        }
    }
}