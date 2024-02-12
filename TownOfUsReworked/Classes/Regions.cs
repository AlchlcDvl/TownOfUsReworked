namespace TownOfUsReworked.Classes;

public static class ExtraRegions
{
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
    }
}