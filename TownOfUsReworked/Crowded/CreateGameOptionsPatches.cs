namespace TownOfUsReworked.Crowded
{
    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.Awake))]
    public static class CreateOptionsPicker_Awake
    {
        public static void Postfix(CreateOptionsPicker __instance)
        {
            if (__instance.mode != SettingsMode.Host)
                return;

            var firstButtonRenderer = __instance.MaxPlayerButtons[0];
            firstButtonRenderer.GetComponentInChildren<TextMeshPro>().text = "-";
            firstButtonRenderer.enabled = false;

            var firstButtonButton = firstButtonRenderer.GetComponent<PassiveButton>();
            firstButtonButton.OnClick = new();
            firstButtonButton.OnClick.AddListener((Action)(() =>
            {
                for (var i = 1; i < 11; i++)
                {
                    var playerButton = __instance.MaxPlayerButtons[i];
                    var tmp = playerButton.GetComponentInChildren<TextMeshPro>();
                    var newValue = Mathf.Max(byte.Parse(tmp.text) - 10, byte.Parse(playerButton.name));
                    tmp.text = newValue.ToString();
                }

                __instance.UpdateMaxPlayersButtons(__instance.GetTargetOptions());
            }));

            firstButtonRenderer.Destroy();

            var lastButtonRenderer = __instance.MaxPlayerButtons[^1];
            lastButtonRenderer.GetComponentInChildren<TextMeshPro>().text = "+";
            lastButtonRenderer.enabled = false;

            var lastButtonButton = lastButtonRenderer.GetComponent<PassiveButton>();
            lastButtonButton.OnClick = new();
            lastButtonButton.OnClick.AddListener((Action)(() =>
            {
                for (var i = 1; i < 11; i++)
                {
                    var playerButton = __instance.MaxPlayerButtons[i];
                    var tmp = playerButton.GetComponentInChildren<TextMeshPro>();
                    var newValue = Mathf.Min(byte.Parse(tmp.text) + 10, 113 + byte.Parse(playerButton.name));
                    tmp.text = newValue.ToString();
                }

                __instance.UpdateMaxPlayersButtons(__instance.GetTargetOptions());
            }));

            lastButtonRenderer.Destroy();

            for (var i = 1; i < 11; i++)
            {
                var playerButton = __instance.MaxPlayerButtons[i].GetComponent<PassiveButton>();
                var text = playerButton.GetComponentInChildren<TextMeshPro>();
                playerButton.OnClick = new();
                playerButton.OnClick.AddListener((Action)(() =>
                {
                    var maxPlayers = byte.Parse(text.text);
                    var maxImp = Mathf.Min(__instance.GetTargetOptions().NumImpostors, maxPlayers / 2);
                    __instance.GetTargetOptions().SetInt(Int32OptionNames.NumImpostors, maxImp);
                    __instance.ImpostorButtons[1].TextMesh.text = maxImp.ToString();
                    __instance.SetMaxPlayersButtons(maxPlayers);
                }));
            }

            foreach (var button in __instance.MaxPlayerButtons)
                button.enabled = button.GetComponentInChildren<TextMeshPro>().text == __instance.GetTargetOptions().MaxPlayers.ToString();

            foreach (var imp in __instance.ImpostorButtons)
                imp.gameObject.Destroy();

            __instance.ImpostorText.gameObject.Destroy();
            __instance.SetLanguageFilter((uint)DataManager.Settings.Language.CurrentLanguage);
        }
    }

    [HarmonyPatch(typeof(MapPickerMenu), nameof(MapPickerMenu.SetMap))]
    public static class MapPickerPatch
    {
        public static void Postfix(ref int mapId) => Generate.Map.Set(mapId);
    }

    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.UpdateMaxPlayersButtons))]
    public static class CreateOptionsPicker_UpdateMaxPlayersButtons
    {
        public static bool Prefix(CreateOptionsPicker __instance, [HarmonyArgument(0)] IGameOptions opts)
        {
            if (__instance.CrewArea)
                __instance.CrewArea.SetCrewSize(opts.MaxPlayers, opts.NumImpostors);

            var selectedAsString = opts.MaxPlayers.ToString();

            for (var i = 1; i < __instance.MaxPlayerButtons.Count - 1; i++)
                __instance.MaxPlayerButtons[i].enabled = __instance.MaxPlayerButtons[i].GetComponentInChildren<TextMeshPro>().text == selectedAsString;

            return false;
        }
    }

    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.UpdateImpostorsButtons))]
    public static class CreateOptionsPicker_UpdateImpostorsButtons
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
    public static class GameOptionsMenu_Start2
    {
        public static void Postfix(ref GameOptionsMenu __instance) => __instance.GetComponentsInChildren<NumberOption>().First(o => o.Title == StringNames.GameNumImpostors).ValidRange =
            new(1, 127 / 2);
    }

    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.AreInvalid))]
    public static class InvalidOptionsPatches
    {
        public static bool Prefix(GameOptionsData __instance, [HarmonyArgument(0)] int maxExpectedPlayers)
        {
            return __instance.MaxPlayers > maxExpectedPlayers || __instance.NumImpostors < 1 || __instance.NumImpostors + 1 > maxExpectedPlayers / 2 || __instance.KillDistance is < 0 or >
                2 || __instance.PlayerSpeedMod is <= 0f or > 3f;
        }
    }
}