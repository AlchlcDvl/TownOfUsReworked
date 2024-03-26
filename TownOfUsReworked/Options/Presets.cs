namespace TownOfUsReworked.Options;

public class Preset : CustomButtonOption
{
    private CustomButtonOption Loading { get; set; }
    private List<OptionBehaviour> OldButtons { get; set; } = [];
    public List<CustomButtonOption> SlotButtons = [];

    public Preset() : base(MultiMenu.Main, "Load Preset Settings") => Do = ToDo;

    private List<OptionBehaviour> CreateOptions()
    {
        var options = new List<OptionBehaviour>();

        foreach (var button in SlotButtons)
        {
            if (button.Setting)
                button.Setting.gameObject.SetActive(true);
            else
            {
                button.Setting = CreateButton();
                button.OptionCreated();
            }

            options.Add(button.Setting);
        }

        return options;
    }

    private void Cancel(Func<IEnumerator> flashCoro, string settingsData) => Coroutines.Start(CancelCoro(flashCoro, settingsData));

    private IEnumerator CancelCoro(Func<IEnumerator> flashCoro, string settingsData)
    {
        if (SlotButtons.Count == 0)
            yield break;

        var __instance = UObject.FindObjectOfType<GameOptionsMenu>();
        SlotButtons.Skip(1).ForEach(x => x.Setting.gameObject.Destroy());
        Loading = SlotButtons[0];
        Loading.Do = BlankVoid;
        Loading.Setting.Cast<ToggleOption>().TitleText.text = "Loading...";
        __instance.Children = new[] { Loading.Setting };
        yield return IsNullEmptyOrWhiteSpace(settingsData) ? Wait(0.5f) : CoLoadSettings(settingsData);
        Loading.Setting.gameObject.Destroy();
        OldButtons.ForEach(x => x.gameObject.SetActive(true));
        __instance.Children = OldButtons.ToArray();
        SettingsPatches.SettingsPage = 0;
        yield return EndFrame();
        yield return flashCoro();
        yield break;
    }

    public void ToDo()
    {
        SlotButtons.Clear();
        Directory.EnumerateFiles(TownOfUsReworked.Options).OrderBy(x => x).Select(x => x.SanitisePath()).Where(x => !x.EndsWith(".json")).ForEach(x =>
            SlotButtons.Add(new(MultiMenu.External, x, () => LoadPreset(x))));
        SlotButtons.Add(new(MultiMenu.External, "Cancel", () => Cancel(FlashWhite, "")));
        var options = CreateOptions();
        var __instance = UObject.FindObjectOfType<GameOptionsMenu>();
        var y = __instance.GetComponentsInChildren<OptionBehaviour>().Max(option => option.transform.localPosition.y);
        var x = __instance.Children[1].transform.localPosition.x;
        var z = __instance.Children[1].transform.localPosition.z;
        OldButtons = __instance.Children.ToList();
        OldButtons.ForEach(x => x.gameObject.SetActive(false));
        SettingsPatches.SettingsPage = 10;

        for (var i = 0; i < options.Count; i++)
            options[i].transform.localPosition = new(x, y - (i * 0.5f), z);

        __instance.Children = options.ToArray();
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
                LoadSettings(text);
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