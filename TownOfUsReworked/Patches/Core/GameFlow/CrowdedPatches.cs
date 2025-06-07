// using UnityEngine.ProBuilder;

namespace TownOfUsReworked.Patches.Core.GameFlow;

// [HarmonyPatch(typeof(CreateOptionsPicker))]
// public static class CrowdedPatches
// {
//     [HarmonyPatch(nameof(CreateOptionsPicker.Awake))]
//     public static void Postfix(CreateOptionsPicker __instance)
//     {
//         if (__instance.mode != SettingsMode.Host)
//             return;

//         var options = __instance.GetTargetOptions();

//         var firstButtonRenderer = __instance.MaxPlayerButtons[0];
//         firstButtonRenderer.GetComponentInChildren<TextMeshPro>().text = "-";
//         firstButtonRenderer.enabled = false;

//         var firstButtonButton = firstButtonRenderer.GetComponent<PassiveButton>();
//         firstButtonButton.OverrideOnClickListeners(() =>
//         {
//             foreach (var playerButton in __instance.MaxPlayerButtons)
//             {
//                 var tmp = playerButton.GetComponentInChildren<TextMeshPro>();
//                 var newValue = Mathf.Max(byte.Parse(tmp.text) - 10, byte.Parse(playerButton.name) - 3);
//                 tmp.text = $"{newValue}";
//             }

//             __instance.UpdateMaxPlayersButtons(options);
//         });

//         firstButtonRenderer.Destroy();

//         var lastButtonRenderer = __instance.MaxPlayerButtons[^1];
//         lastButtonRenderer.GetComponentInChildren<TextMeshPro>().text = "+";
//         lastButtonRenderer.enabled = false;

//         var lastButtonButton = lastButtonRenderer.GetComponent<PassiveButton>();
//         lastButtonButton.OverrideOnClickListeners(() =>
//         {
//             foreach (var playerButton in __instance.MaxPlayerButtons)
//             {
//                 var tmp = playerButton.GetComponentInChildren<TextMeshPro>();
//                 var newValue = Mathf.Min(byte.Parse(tmp.text) + 10, byte.Parse(playerButton.name) + 235);
//                 tmp.text = $"{newValue}";
//             }

//             __instance.UpdateMaxPlayersButtons(options);
//         });

//         lastButtonRenderer.Destroy();

//         foreach (var button in __instance.MaxPlayerButtons)
//         {
//             var playerButton = button.GetComponent<PassiveButton>();
//             var text = playerButton.GetComponentInChildren<TextMeshPro>();
//             playerButton.OverrideOnClickListeners(() => __instance.SetMaxPlayersButtons(byte.Parse(text.text)));
//         }

//         __instance.MaxPlayerButtons.ForEach(x => x.enabled = x.GetComponentInChildren<TextMeshPro>().text == options.MaxPlayers.ToString());
//         __instance.ImpostorButtons.ForEach(x => x.gameObject.SetActive(false));
//         __instance.ImpostorText.gameObject.SetActive(false);
//     }

//     [HarmonyPatch(nameof(CreateOptionsPicker.SetMap))]
//     public static void Prefix(ref int mapId) => MapSettings.Map = (MapEnum)mapId;

//     [HarmonyPatch(nameof(CreateOptionsPicker.SetMaxPlayersButtons))]
//     public static void Postfix(int maxPlayers) => GameOptions.LobbySize.Set(maxPlayers, false);

//     [HarmonyPatch(nameof(CreateOptionsPicker.UpdateMaxPlayersButtons))]
//     public static bool Prefix(CreateOptionsPicker __instance, IGameOptions opts)
//     {
//         if (__instance.mode != SettingsMode.Host)
//             return true;

//         __instance.CrewArea?.SetCrewSize(opts.MaxPlayers, opts.NumImpostors);
//         __instance.MaxPlayerButtons.ToArray().Skip(1).ForEach(x =>
//         {
//             if (x)
//                 x.enabled = x.GetComponentInChildren<TextMeshPro>()?.text == opts?.MaxPlayers.ToString();
//         });
//         return false;
//     }

//     [HarmonyPatch(nameof(CreateOptionsPicker.UpdateImpostorsButtons))]
//     public static bool Prefix() => false;

//     [HarmonyPatch(nameof(CreateOptionsPicker.Refresh))]
//     public static bool Prefix(CreateOptionsPicker __instance)
//     {
//         var options = __instance.GetTargetOptions();
//         __instance.UpdateMaxPlayersButtons(options);
//         __instance.UpdateLanguageButton((uint)options.Keywords);
//         __instance.MapMenu.UpdateMapButtons(options.MapId);
//         __instance.GameModeText.text = TranslationController.Instance.GetString(GameModesHelpers.ModeToName[GameOptionsManager.Instance.CurrentGameOptions.GameMode]);
//         return false;
//     }
// }

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
            __instance.HostId = __instance.Clients._items[0].Id;

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

[HarmonyPatch(typeof(CreateOptionsPicker))]
public static class CreateOptionsPickerPatches
{
    [HarmonyPatch(nameof(CreateOptionsPicker.Awake))]
    public static void Prefix(CreateOptionsPicker __instance)
    {
        if (!__instance.MapMenu)
            return;

        var skeld = __instance.MapMenu.MapButtons.Find(x => x.MapId == MapNames.Skeld);

        if (__instance.MapMenu.MapButtons.All(x => x.MapId != MapNames.Dleks))
        {
            var dleksButton = UObject.Instantiate(skeld, skeld.transform.parent);
            dleksButton.ButtonImage.sprite = skeld.ButtonImage.sprite;
            dleksButton.ButtonImage.flipX = true;
            dleksButton.Icon.sprite = skeld.Icon.sprite;
            dleksButton.Icon.flipX = true;
            dleksButton.MapId = MapNames.Dleks;
            __instance.MapMenu.MapButtons = __instance.MapMenu.MapButtons.AddItem(dleksButton).ToArray();
        }

        if (__instance.MapMenu.MapButtons.All(x => x.MapId != (MapNames)8))
        {
            var randomButton = UObject.Instantiate(skeld, skeld.transform.parent);
            randomButton.ButtonImage.sprite = GetSprite("RandomMapIcon");
            randomButton.Icon.sprite = GetSprite("RandomMapIcon");
            randomButton.MapId = (MapNames)8;
            __instance.MapMenu.MapButtons = __instance.MapMenu.MapButtons.AddItem(randomButton).ToArray();
        }
    }

    [HarmonyPatch(nameof(CreateOptionsPicker.SetMap))]
    public static void Prefix(int mapId) => SettingsPatches.SetMap((MapEnum)mapId);
}

[HarmonyPatch(typeof(CreateGameOptions), nameof(CreateGameOptions.Start))]
public static class CreateGameOptionsPatch
{
    public static void Prefix(CreateGameOptions __instance)
    {
        var dleksTooltip = TranslationManager.GetOrAddName("Dleks.Tooltip");

        if (!__instance.mapTooltips.Contains(dleksTooltip))
            __instance.mapTooltips = __instance.mapTooltips.AddItem(dleksTooltip).ToArray();

        var randomTooltip = TranslationManager.GetOrAddName("Random.Tooltip");

        if (!__instance.mapTooltips.Contains(randomTooltip))
            __instance.mapTooltips = __instance.mapTooltips.AddItem(randomTooltip).ToArray();
    }
}

[HarmonyPatch(typeof(GameContainer), nameof(GameContainer.SetupGameInfo))]
public static class GameContainerPatch
{
    public static void Prefix(GameContainer __instance)
    {
        __instance.mapLogoSprites = __instance.mapLogoSprites.Concat([__instance.mapLogoSprites[0], GetSprite("RandomMapIcon")]).ToArray();
        __instance.mapBackgroundSprites = __instance.mapLogoSprites.Concat([__instance.mapBackgroundSprites[0], GetSprite("RandomMapBackground")]).ToArray();
    }
}

[HarmonyPatch(typeof(FilterMapPicker), nameof(FilterMapPicker.Initialize))]
public static class FilterMapPickerPatch
{
    public static void Prefix(FilterMapPicker __instance)
    {
        var dleksTooltip = TranslationManager.GetOrAddName("Dleks.Tooltip");

        if (!__instance.mapStrings.Contains(dleksTooltip))
            __instance.mapStrings = __instance.mapStrings.AddItem(dleksTooltip).ToArray();

        var randomTooltip = TranslationManager.GetOrAddName("Random.Tooltip");

        if (!__instance.mapStrings.Contains(randomTooltip))
            __instance.mapStrings = __instance.mapStrings.AddItem(randomTooltip).ToArray();
    }
}