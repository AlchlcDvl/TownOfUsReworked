namespace TownOfUsReworked.Crowded;

[HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.Awake))]
public static class CreateOptionsPicker_Awake
{
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
            for (var i = 1; i < 11; i++)
            {
                var playerButton = __instance.MaxPlayerButtons[i];
                var tmp = playerButton.GetComponentInChildren<TextMeshPro>();
                var newValue = Mathf.Max(byte.Parse(tmp.text) - 10, byte.Parse(playerButton.name) - 3);
                tmp.text = newValue.ToString();
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
            for (var i = 1; i < 11; i++)
            {
                var playerButton = __instance.MaxPlayerButtons[i];
                var tmp = playerButton.GetComponentInChildren<TextMeshPro>();
                var newValue = Mathf.Min(byte.Parse(tmp.text) + 10, byte.Parse(playerButton.name) + 113);
                tmp.text = newValue.ToString();
            }

            __instance.UpdateMaxPlayersButtons(options);
        });

        lastButtonRenderer.Destroy();

        for (var i = 1; i < 11; i++)
        {
            var playerButton = __instance.MaxPlayerButtons[i].GetComponent<PassiveButton>();
            var text = playerButton.GetComponentInChildren<TextMeshPro>();
            playerButton.OverrideOnClickListeners(() => __instance.SetMaxPlayersButtons(byte.Parse(text.text)));
        }

        __instance.MaxPlayerButtons.ForEach(x => x.enabled = x.GetComponentInChildren<TextMeshPro>().text == options.MaxPlayers.ToString());
        __instance.ImpostorButtons.ForEach(x => x.gameObject.SetActive(false));
        __instance.ImpostorText.gameObject.SetActive(false);
    }
}

[HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.SetMap))]
public static class MapPickerPatch
{
    public static void Postfix(ref int mapId)
    {
        if (mapId == 6 && !SubLoaded)
            mapId = LILoaded ? 7 : 5;

        if (mapId == 7 && !LILoaded)
            mapId = SubLoaded ? 6 : 5;

        Generate.Map.Set(mapId);
    }
}

[HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.SetMaxPlayersButtons))]
public static class LobbySizePatch
{
    public static void Postfix(ref int maxPlayers) => Generate.LobbySize.Set(maxPlayers);
}

[HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.UpdateMaxPlayersButtons))]
public static class CreateOptionsPicker_UpdateMaxPlayersButtons
{
    public static bool Prefix(CreateOptionsPicker __instance, ref IGameOptions opts)
    {
        __instance.CrewArea?.SetCrewSize(opts.MaxPlayers, opts.NumImpostors);

        for (var i = 1; i < __instance.MaxPlayerButtons.Count - 1; i++)
            __instance.MaxPlayerButtons[i].enabled = __instance.MaxPlayerButtons[i].GetComponentInChildren<TextMeshPro>().text == opts.MaxPlayers.ToString();

        return false;
    }
}

[HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.UpdateImpostorsButtons))]
public static class CreateOptionsPicker_UpdateImpostorsButtons
{
    public static bool Prefix() => false;
}