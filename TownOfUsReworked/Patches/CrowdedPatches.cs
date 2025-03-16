namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(CreateOptionsPicker))]
public static class CrowdedPatches
{
    [HarmonyPatch(nameof(CreateOptionsPicker.Awake))]
    public static void Postfix(CreateOptionsPicker __instance)
    {
        if (__instance.mode != SettingsMode.Host)
            return;

        var options = __instance.GetTargetOptions();

        var firstButtonRenderer = __instance.MaxPlayerButtons[0];
        firstButtonRenderer.GetComponentInChildren<TextMeshPro>().text = "-";
        firstButtonRenderer.enabled = false;

        var firstButtonButton = firstButtonRenderer.GetComponent<PassiveButton>();
        firstButtonButton.OverrideOnClickListeners(() =>
        {
            foreach (var playerButton in __instance.MaxPlayerButtons)
            {
                var tmp = playerButton.GetComponentInChildren<TextMeshPro>();
                var newValue = Mathf.Max(byte.Parse(tmp.text) - 10, byte.Parse(playerButton.name) - 3);
                tmp.text = $"{newValue}";
            }

            __instance.UpdateMaxPlayersButtons(options);
        });

        firstButtonRenderer.Destroy();

        var lastButtonRenderer = __instance.MaxPlayerButtons[^1];
        lastButtonRenderer.GetComponentInChildren<TextMeshPro>().text = "+";
        lastButtonRenderer.enabled = false;

        var lastButtonButton = lastButtonRenderer.GetComponent<PassiveButton>();
        lastButtonButton.OverrideOnClickListeners(() =>
        {
            foreach (var playerButton in __instance.MaxPlayerButtons)
            {
                var tmp = playerButton.GetComponentInChildren<TextMeshPro>();
                var newValue = Mathf.Min(byte.Parse(tmp.text) + 10, byte.Parse(playerButton.name) + 235);
                tmp.text = $"{newValue}";
            }

            __instance.UpdateMaxPlayersButtons(options);
        });

        lastButtonRenderer.Destroy();

        foreach (var button in __instance.MaxPlayerButtons)
        {
            var playerButton = button.GetComponent<PassiveButton>();
            var text = playerButton.GetComponentInChildren<TextMeshPro>();
            playerButton.OverrideOnClickListeners(() => __instance.SetMaxPlayersButtons(byte.Parse(text.text)));
        }

        __instance.MaxPlayerButtons.ForEach(x => x.enabled = x.GetComponentInChildren<TextMeshPro>().text == options.MaxPlayers.ToString());
        __instance.ImpostorButtons.ForEach(x => x.gameObject.SetActive(false));
        __instance.ImpostorText.gameObject.SetActive(false);
    }

    [HarmonyPatch(nameof(CreateOptionsPicker.SetMap))]
    public static void Prefix(ref int mapId) => MapSettings.Map = (MapEnum)mapId;

    [HarmonyPatch(nameof(CreateOptionsPicker.SetMaxPlayersButtons))]
    public static void Postfix(int maxPlayers) => OptionAttribute.GetOption<NumberOptionAttribute>("LobbySize").Set(maxPlayers, false);

    [HarmonyPatch(nameof(CreateOptionsPicker.UpdateMaxPlayersButtons))]
    public static bool Prefix(CreateOptionsPicker __instance, IGameOptions opts)
    {
        if (__instance.mode != SettingsMode.Host)
            return true;

        __instance.CrewArea?.SetCrewSize(opts.MaxPlayers, opts.NumImpostors);
        __instance.MaxPlayerButtons.ToArray().Skip(1).ForEach(x =>
        {
            if (x)
                x.enabled = x.GetComponentInChildren<TextMeshPro>()?.text == opts?.MaxPlayers.ToString();
        });
        return false;
    }

    [HarmonyPatch(nameof(CreateOptionsPicker.UpdateImpostorsButtons))]
    public static bool Prefix() => false;

    [HarmonyPatch(nameof(CreateOptionsPicker.Refresh))]
    public static bool Prefix(CreateOptionsPicker __instance)
    {
        var options = __instance.GetTargetOptions();
        __instance.UpdateMaxPlayersButtons(options);
        __instance.UpdateLanguageButton((uint)options.Keywords);
        __instance.MapMenu.UpdateMapButtons(options.MapId);
        __instance.GameModeText.text = TranslationController.Instance.GetString(GameModesHelpers.ModeToName[GameOptionsManager.Instance.CurrentGameOptions.GameMode]);
        return false;
    }
}

[HarmonyPatch(typeof(InnerNetServer), nameof(InnerNetServer.HandleNewGameJoin))]
public static class InnerNetSererHandleNewGameJoin
{
    public static bool Prefix(InnerNetServer __instance, InnerNetServer.Player client)
    {
        if (__instance.Clients.Count < 15)
            return true;

        __instance.Clients.Add(client);
        client.LimboState = LimboStates.PreSpawn;

        if (__instance.HostId == -1)
        {
            __instance.HostId = __instance.Clients[0].Id;

            if (__instance.HostId == client.Id)
                client.LimboState = LimboStates.NotLimbo;
        }

        var writer = MessageWriter.Get(SendOption.Reliable);

        try
        {
            __instance.WriteJoinedMessage(client, writer, true);
            client.Connection.Send(writer);
            __instance.BroadcastJoinMessage(client, writer);
        }
        catch (Exception exception)
        {
            Error($"InnerNetServer::HandleNewGameJoin MessageWriter 2 Exception: {exception}");
        }
        finally
        {
            writer.Recycle();
        }

        return false;
    }
}