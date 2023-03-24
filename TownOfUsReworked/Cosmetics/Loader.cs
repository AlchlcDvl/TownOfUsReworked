using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Reactor.Utilities.Extensions;

namespace TownOfUsReworked.Cosmetics
{
    public static class Loader
    {
        private const string json = "metadata.json";

        public static HatMetadataJson LoadHatJson()
        {
            var stream = TownOfUsReworked.Assembly.GetManifestResourceStream(TownOfUsReworked.Hats + json);

            return JsonSerializer.Deserialize<HatMetadataJson>(Encoding.UTF8.GetString(stream.ReadFully()), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });
        }

        public static List<HatMetadataElement> LoadCustomHatData()
        {
            var json = LoadHatJson();
            List<HatMetadataElement> hats = new();
            hats.AddRange(json.Credits);
            return hats;
        }

        public static NameplateMetadataJson LoadNameplateJson()
        {
            var stream = TownOfUsReworked.Assembly.GetManifestResourceStream(TownOfUsReworked.Nameplates + json);

            return JsonSerializer.Deserialize<NameplateMetadataJson>(Encoding.UTF8.GetString(stream.ReadFully()), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });
        }

        public static List<NameplateMetadataElement> LoadCustomNameplateData()
        {
            var json = LoadNameplateJson();
            List<NameplateMetadataElement> nameplates = new();
            nameplates.AddRange(json.Credits);
            return nameplates;
        }

        public static VisorMetadataJson LoadVisorJson()
        {
            var stream = TownOfUsReworked.Assembly.GetManifestResourceStream(TownOfUsReworked.Visors + json);

            return JsonSerializer.Deserialize<VisorMetadataJson>(Encoding.UTF8.GetString(stream.ReadFully()), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });
        }

        public static List<VisorMetadataElement> LoadCustomVisorData()
        {
            var json = LoadVisorJson();
            List<VisorMetadataElement> visors = new();
            visors.AddRange(json.Credits);
            return visors;
        }
    }
}