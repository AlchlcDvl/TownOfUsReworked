namespace TownOfUsReworked.Patches;

// Adapted from The Other Roles
[HarmonyPatch(typeof(RegionMenu))]
public static class RegionInfoOpenPatch
{
    private static TextBoxTMP IPField;
    private static TextBoxTMP PortField;

    [HarmonyPatch(nameof(RegionMenu.Open))]
    public static void Postfix(RegionMenu __instance)
    {
        JoinGameButton joinGameButton1 = null;

        foreach (var joinGameButton2 in UObject.FindObjectsOfType<JoinGameButton>())
        {
            if (joinGameButton2.GameIdText && joinGameButton2.GameIdText.Background)
            {
                joinGameButton1 = joinGameButton2;
                break;
            }
        }

        if (!joinGameButton1 || !joinGameButton1.GameIdText)
            return;

        var flag = ServerManager.Instance.CurrentRegion.Name == "Custom";

        if (!IPField || !IPField.gameObject)
        {
            IPField = UObject.Instantiate(joinGameButton1.GameIdText, __instance.transform);
            IPField.name = "IPTextBox";
            var child = IPField.transform.FindChild("arrowEnter");

            if (child && child.gameObject)
            {
                child.gameObject.DestroyImmediate();
                IPField.transform.localPosition = new(-2.5f, -1.55f, -100f);
                IPField.characterLimit = 30;
                IPField.AllowSymbols = true;
                IPField.ForceUppercase = false;
                IPField.outputText.SetText(TownOfUsReworked.Ip.Value);
                IPField.SetText(TownOfUsReworked.Ip.Value);
                IPField.ClearOnFocus = false;
                IPField.OnEnter = IPField.OnChange = new();
                IPField.OnFocusLost = new();
                IPField.OnChange.AddListener((Action)ChangeIP);
                IPField.OnFocusLost.AddListener((Action)UpdateRegions);
            }
        }

        if (!PortField || !PortField.gameObject)
        {
            PortField = UObject.Instantiate(joinGameButton1.GameIdText, __instance.transform);
            PortField.name = "PortTextBox";
            var child1 = PortField.transform.FindChild("arrowEnter");

            if (child1 && child1.gameObject)
            {
                child1.gameObject.DestroyImmediate();
                PortField.transform.localPosition = new(2.8f, -1.55f, -100f);
                PortField.characterLimit = 5;
                PortField.outputText.SetText($"{TownOfUsReworked.Port.Value}");
                PortField.SetText($"{TownOfUsReworked.Port.Value}");
                PortField.ClearOnFocus = false;
                PortField.OnEnter = PortField.OnChange = new();
                PortField.OnFocusLost = new();
                PortField.OnChange.AddListener((Action)ChangePort);
                PortField.OnFocusLost.AddListener((Action)UpdateRegions);
            }
        }

        IPField?.gameObject?.SetActive(flag);
        PortField?.gameObject?.SetActive(flag);
    }

    private static void ChangeIP() => TownOfUsReworked.Ip.Value = IPField.text;

    private static void ChangePort()
    {
        if (ushort.TryParse(PortField.text, out var result))
        {
            TownOfUsReworked.Port.Value = result;
            PortField.outputText.color = UColor.white;
        }
        else
            PortField.outputText.color = UColor.red;
    }

    public static void UpdateRegions()
    {
        var mna = new StaticHttpRegionInfo("Modded NA (MNA)", StringNames.NoTranslation, "www.aumods.us", new([new("Http-1", "https://www.aumods.us", 443, false)])).Cast<IRegionInfo>();
        var meu = new StaticHttpRegionInfo("Modded EU (MEU)", StringNames.NoTranslation, "au-eu.duikbo.at", new([new("Http-1", "https://au-eu.duikbo.at", 443, false)])).Cast<IRegionInfo>();
        var mas = new StaticHttpRegionInfo("Modded Asia (MAS)", StringNames.NoTranslation, "au-as.duikbo.at", new([new("Http-1", "https://au-as.duikbo.at", 443, false)])).Cast<IRegionInfo>();
        var custom = new StaticHttpRegionInfo("Custom", StringNames.NoTranslation, TownOfUsReworked.Ip.Value, new([new("Custom", TownOfUsReworked.Ip.Value, TownOfUsReworked.Port.Value,
            false)])).Cast<IRegionInfo>();

        var iRegionInfo1 = ServerManager.Instance.CurrentRegion;

        foreach (var iRegionInfo2 in new[] { mna, meu, mas, custom })
        {
            if (iRegionInfo2 == null)
                Error("Could not add region");
            else
            {
                if (iRegionInfo1 != null && iRegionInfo2.Name.Equals(iRegionInfo1.Name, StringComparison.OrdinalIgnoreCase))
                    iRegionInfo1 = iRegionInfo2;

                ServerManager.Instance.AddOrUpdateRegion(iRegionInfo2);
            }
        }

        if (iRegionInfo1 == null)
            return;

        Info("Resetting previous region");
        ServerManager.Instance.SetRegion(iRegionInfo1);
    }

    [HarmonyPatch(nameof(RegionMenu.ChooseOption))]
    public static bool Prefix(RegionMenu __instance, IRegionInfo region)
    {
        if (region.Name != "Custom" || ServerManager.Instance.CurrentRegion.Name == "Custom")
            return true;

        ServerManager.Instance.SetRegion(region);
        __instance.RegionText.text = "Custom";

        foreach (var button in __instance.ButtonPool.activeChildren)
        {
            var serverListButton = button.TryCast<ServerListButton>();
            serverListButton?.SetSelected(serverListButton.Text.text == "Custom");
        }

        __instance.Open();
        return false;
    }
}