namespace TownOfUsReworked.Options2;

public class PresetOption : ButtonOption
{
    private ButtonOption Loading { get; set; }
    public List<ButtonOption> SlotButtons = [];

    public PresetOption() : base(MultiMenu.Main, "Load Preset Settings") => Do = ToDo;

    private void Cancel(Func<IEnumerator> flashCoro, string settingsData) => Coroutines.Start(CancelCoro(flashCoro, settingsData));

    private IEnumerator CancelCoro(Func<IEnumerator> flashCoro, string settingsData)
    {
        if (SlotButtons.Count == 0)
            yield break;

        SlotButtons.Skip(1).ForEach(x => x.Setting.gameObject.Destroy());
        Loading = SlotButtons[0];
        Loading.Do = BlankVoid;
        SettingsPatches.SettingsPage = 12;
        Loading.Setting.Cast<ToggleOption>().TitleText.text = "Loading...";
        yield return IsNullEmptyOrWhiteSpace(settingsData) ? Wait(0.5f) : OptionAttribute.CoLoadSettings(settingsData);
        Loading.Setting.gameObject.Destroy();
        SettingsPatches.SettingsPage = 0;
        yield return EndFrame();
        yield return flashCoro();
        yield break;
    }

    public void ToDo()
    {
        SlotButtons.Clear();
        Directory.EnumerateFiles(TownOfUsReworked.Options).OrderBy(x => x).Select(x => x.SanitisePath()).Where(x => !x.EndsWith(".json")).ForEach(x => SlotButtons.Add(new(MultiMenu.Presets,
            x, () => LoadPreset(x))));
        SlotButtons.Add(new(MultiMenu.Presets, "Cancel", () => Cancel(FlashWhite, "")));
        SettingsPatches.SettingsPage = 10;
        var __instance = GameSettingMenu.Instance.GameSettingsTab;
        var allOptions = SettingsPatches.CreateOptions();
        var (customOptions, behaviours) = (allOptions.Keys.ToList(), allOptions.Values.ToList());
        var y = 0.713f;

        for (var i = 0; i < allOptions.Count; i++)
        {
            var isHeader = customOptions[i] is CustomHeaderOption;
            behaviours[i].transform.localPosition = new(isHeader ? -0.903f : 0.952f, y, -2f);
            behaviours[i].gameObject.SetActive(true);

            if (behaviours[i] is OptionBehaviour option)
                option.SetClickMask(__instance.ButtonClickMask);

            y -= isHeader ? 0.63f : 0.45f;
        }

        behaviours.RemoveAll(x => x is CategoryHeaderMasked);
        behaviours.Insert(0, __instance.MapPicker);
        __instance.Children = behaviours.Cast<OptionBehaviour>().ToList().ToIl2Cpp();
        __instance.scrollBar.SetYBoundsMax(-1.65f - y);
        __instance.InitializeControllerNavigation();
    }

    public void LoadPreset(string presetName)
    {
        LogMessage($"Loading - {presetName}");
        var text = ReadDiskText(presetName, TownOfUsReworked.Options);

        if (IsNullEmptyOrWhiteSpace(text))
        {
            Cancel(FlashRed, "");
            LogError($"{presetName} no exist");
        }
        else
        {
            CallRpc(CustomRPC.Misc, MiscRPC.LoadPreset, presetName);

            if (Setting)
                Cancel(FlashGreen, text);
            else
                OptionAttribute.LoadSettings(text);
        }
    }

    private IEnumerator FlashGreen()
    {
        Setting.Cast<ToggleOption>().TitleText.color = UColor.green;
        yield return Wait(0.5f);
        Setting.Cast<ToggleOption>().TitleText.color = UColor.white;
    }

    private IEnumerator FlashRed()
    {
        Setting.Cast<ToggleOption>().TitleText.color = UColor.red;
        yield return Wait(0.5f);
        Setting.Cast<ToggleOption>().TitleText.color = UColor.white;
    }

    private IEnumerator FlashWhite() => EndFrame();
}