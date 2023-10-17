using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace TownOfUsReworked.Classes;

public static class ExtraRegions
{
    private static ReadOnlyDictionary<string, IRegionInfo> ParsedRegions;

    public static void UpdateRegions()
    {
        var mna = new StaticHttpRegionInfo("Modded NA (MNA)", StringNames.NoTranslation, "www.aumods.us", new(new[] { new ServerInfo("Http-1", "https://www.aumods.us", 443, false)
            })).Cast<IRegionInfo>();

        var meu = new StaticHttpRegionInfo("Modded EU (MEU)", StringNames.NoTranslation, "au-eu.duikbo.at", new(new[] { new ServerInfo("Http-1", "https://au-eu.duikbo.at", 443, false)
            })).Cast<IRegionInfo>();

        var mas = new StaticHttpRegionInfo("Modded Asia (MAS)", StringNames.NoTranslation, "au-as.duikbo.at", new(new[] { new ServerInfo("Http-1", "https://au-as.duikbo.at", 443, false)
            })).Cast<IRegionInfo>();

        var custom = new StaticHttpRegionInfo("Custom", StringNames.NoTranslation, TownOfUsReworked.Ip.Value, new(new[] { new ServerInfo("Custom", TownOfUsReworked.Ip.Value,
            TownOfUsReworked.Port.Value, false) })).Cast<IRegionInfo>();

        //play.scumscyb.org

        var iregionInfoArray = new IRegionInfo[] { mna, meu, mas, custom };
        var iregionInfo1 = ServerManager.Instance.CurrentRegion;

        foreach (var iregionInfo2 in iregionInfoArray)
        {
            if (iregionInfo2 == null)
                LogError("Could not add region");
            else
            {
                if (iregionInfo1 != null && iregionInfo2.Name.Equals(iregionInfo1.Name, StringComparison.OrdinalIgnoreCase))
                    iregionInfo1 = iregionInfo2;

                ServerManager.Instance.AddOrUpdateRegion(iregionInfo2);
            }
        }

        if (iregionInfo1 == null)
            return;

        LogInfo("Resetting previous region");
        ServerManager.Instance.SetRegion(iregionInfo1);
        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) => SetUpExtraRegions(scene)));
    }

    public static void SetUpExtraRegions(Scene scene)
    {
        if (scene.name != "MainMenu")
            return;

        // Remove regions first in case the user accidentally also adds a region with the same name.
        if (TownOfUsReworked.RegionsToRemove != null)
        {
            var rmRegions = TownOfUsReworked.RegionsToRemove.Value.Split(",");
            LogInfo($"Removing User Regions: {string.Join("\", \"", rmRegions)}");
            RemoveRegions(rmRegions);
        }

        if (TownOfUsReworked.Regions != null && TownOfUsReworked.Regions.Value.Length != 0)
        {
            LogInfo("Adding User Regions");
            var parsed = ParseRegions(TownOfUsReworked.Regions.Value);
            AddRegions(parsed);
            var regionsDict = new Dictionary<string, IRegionInfo>();
            parsed.ForEach(region => regionsDict[region.Name] = region);
            ParsedRegions = new(regionsDict);
        }
    }

    private static void AddRegions(IRegionInfo[] regions)
    {
        var serverMngr = ServerManager.Instance;
        var currentRegion = serverMngr.CurrentRegion;
        LogInfo($"Adding {regions.Length} regions");

        foreach (var region in regions)
        {
            if (region == null)
                LogError("Could not add region");
            else
            {
                if (currentRegion != null && region.Name.Equals(currentRegion.Name, StringComparison.OrdinalIgnoreCase))
                    currentRegion = region;

                serverMngr.AddOrUpdateRegion(region);
            }
        }

        // AU remembers the previous region that was set, so we need to restore it
        if (currentRegion != null)
        {
            LogInfo("Resetting previous region");
            serverMngr.SetRegion(currentRegion);
        }
    }

    private static void RemoveRegions(string[] regionNames)
    {
        var newRegions = ServerManager.Instance.AvailableRegions.Where(r => Array.FindIndex(regionNames, name => name.Equals(r.Name, StringComparison.OrdinalIgnoreCase)) == -1);
        ServerManager.Instance.AvailableRegions = newRegions.ToArray();
    }

    private static IRegionInfo[] ParseRegions(string regions)
    {
        LogInfo($"Parsing {regions}");

        switch (regions[0])
        {
            // The entire JsonServerData
            case '{':
                LogInfo("Loading server data");
                var result = JsonConvert.DeserializeObject<ServerManager.JsonServerData>(regions, new() { TypeNameHandling = TypeNameHandling.Auto });

                foreach (var region in result.Regions)
                    LogInfo($"Region \"{region.Name}\" @ {region.Servers[0].Ip}:{region.Servers[0].Port}");

                return result.Regions;

            // Only the IRegionInfo array
            case '[':
                LogInfo("Loading region array");
                // Sadly AU does not have a Generic that parses IRegionInfo[] directly, so instead we wrap the array into a JsonServerData structure.
                return ParseRegions($"{{\"CurrentRegionIdx\":0,\"Regions\":{regions}}}");

            default:
                LogError("Could not detect format of configured regions");
                return Array.Empty<IRegionInfo>();
        }
    }

    public static void CorrectCurrentRegion(ServerManager __instance)
    {
        var region = __instance.CurrentRegion;
        LogInfo($"Current region: {region.Name} ({region.Servers.Length} servers)");
        LogInfo($"Region \"{region.Servers[0].Name}\" @ {region.Servers[0].Ip}:{region.Servers[0].Port}");

        if (ParsedRegions != null && ParsedRegions.ContainsKey(region.Name))
        {
            __instance.CurrentRegion = ParsedRegions[region.Name];
            LogInfo("Loading region from cache instead of from file");

            if (region.Servers[0].Port != __instance.CurrentRegion.Servers[0].Port)
                LogInfo($"Port corrected from {region.Servers[0].Port} to {__instance.CurrentRegion.Servers[0].Port}");
        }
    }
}