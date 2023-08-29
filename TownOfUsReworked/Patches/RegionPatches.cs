namespace TownOfUsReworked.Patches;

//Adapted from The Other Roles and Mini.RegionInstall
[HarmonyPatch(typeof(RegionMenu), nameof(RegionMenu.Open))]
public static class RegionInfoOpenPatch
{
    private static TextBoxTMP IPField;
    private static TextBoxTMP PortField;

    public static void Postfix(RegionMenu __instance)
    {
        if (!__instance.TryCast<RegionMenu>())
            return;

        var flag = ServerManager.Instance.CurrentRegion.Name == "Custom";
        IPField?.gameObject?.SetActive(flag);
        PortField?.gameObject?.SetActive(flag);
        var joinGameButton1 = DestroyableSingleton<JoinGameButton>.Instance;

        foreach (var joinGameButton2 in UObject.FindObjectsOfType<JoinGameButton>())
        {
            if (joinGameButton2.GameIdText != null && joinGameButton2.GameIdText.Background != null)
            {
                joinGameButton1 = joinGameButton2;
                break;
            }
        }

        if (joinGameButton1 == null || joinGameButton1.GameIdText == null)
            return;

        if (IPField == null || IPField.gameObject == null)
        {
            IPField = UObject.Instantiate(joinGameButton1.GameIdText, __instance.transform);
            IPField.gameObject.name = "IpTextBox";
            var child = IPField.transform.FindChild("arrowEnter");

            if (child == null || child.gameObject == null)
                return;

            child.gameObject.DestroyImmediate();
            IPField.transform.localPosition = new(-2.5f, -1.55f, -100f);
            IPField.characterLimit = 30;
            IPField.AllowSymbols = true;
            IPField.ForceUppercase = false;
            __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ =>
            {
                IPField.outputText.SetText(TownOfUsReworked.Ip.Value, true);
                IPField.SetText(TownOfUsReworked.Ip.Value);
            })));
            IPField.ClearOnFocus = false;
            IPField.OnEnter = IPField.OnChange = new();
            IPField.OnFocusLost = new();
            IPField.OnChange.AddListener(new Action(ChangeIP));
            IPField.OnFocusLost.AddListener(new Action(ExtraRegions.UpdateRegions));
            IPField.gameObject.SetActive(flag);
        }

        if (PortField != null && PortField.gameObject != null)
            return;

        PortField = UObject.Instantiate(joinGameButton1.GameIdText, __instance.transform);
        PortField.gameObject.name = "PortTextBox";
        var child1 = PortField.transform.FindChild("arrowEnter");

        if (child1 == null || child1.gameObject == null)
            return;

        child1.gameObject.DestroyImmediate();
        PortField.transform.localPosition = new(2.8f, -1.55f, -100f);
        PortField.characterLimit = 5;
        __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ =>
        {
            PortField.outputText.SetText($"{TownOfUsReworked.Port.Value}", true);
            PortField.SetText($"{TownOfUsReworked.Port.Value}");
        })));
        PortField.ClearOnFocus = false;
        PortField.OnEnter = PortField.OnChange = new();
        PortField.OnFocusLost = new();
        PortField.OnChange.AddListener(new Action(ChangePort));
        PortField.OnFocusLost.AddListener(new Action(ExtraRegions.UpdateRegions));
        PortField.gameObject.SetActive(flag);
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
}

[HarmonyPatch(typeof(RegionMenu), nameof(RegionMenu.ChooseOption))]
public static class RegionMenuChooseOptionPatch
{
    public static bool Prefix(RegionMenu __instance, ref IRegionInfo region)
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

[HarmonyPatch(typeof(DnsRegionInfo), nameof(DnsRegionInfo.PopulateServers))]
public static class DnsRegionInfoPatch
{
    public static void Postfix(DnsRegionInfo __instance)
    {
        LogInfo($"DRI Populate Servers: {__instance.Fqdn}");

        foreach (var server in __instance.Servers)
            LogInfo($"Configured server: {server.ToString()}");
    }
}

[HarmonyPatch(typeof(ServerManager), nameof(ServerManager.ReselectServer))]
public static class SMReselectPatch
{
    public static void Prefix(ServerManager __instance) => ExtraRegions.CorrectCurrentRegion(__instance);

    public static void Postfix(ServerManager __instance)
    {
        var server = __instance.CurrentUdpServer;
        LogInfo($"Current server: {server.ToString()}");
    }
}

[HarmonyPatch(typeof(ServerManager), nameof(ServerManager.LoadServers))]
public static class ServerManagerLoadServersPatch
{
    public static void Postfix(ServerManager __instance)
    {
        ExtraRegions.CorrectCurrentRegion(__instance);
        __instance.CurrentUdpServer = __instance.CurrentRegion.Servers[0];
    }
}