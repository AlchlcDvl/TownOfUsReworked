namespace TownOfUsReworked.CustomOptions;

public class Preset : CustomButtonOption
{
    private CustomButtonOption Loading { get; set; }
    private List<OptionBehaviour> OldButtons { get; set; } = new();
    public List<CustomButtonOption> SlotButtons = new();

    public Preset() : base(MultiMenu.Main, "Load Preset Settings") => Do = ToDo;

    private List<OptionBehaviour> CreateOptions()
    {
        var options = new List<OptionBehaviour>();
        var togglePrefab = UObject.FindObjectOfType<ToggleOption>();

        foreach (var button in SlotButtons)
        {
            if (button.Setting != null)
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

    private void Cancel(Func<IEnumerator> flashCoro) => Coroutines.Start(CancelCoro(flashCoro));

    private IEnumerator CancelCoro(Func<IEnumerator> flashCoro)
    {
        if (SlotButtons.Count == 0)
            yield break;

        var __instance = UObject.FindObjectOfType<GameOptionsMenu>();
        SlotButtons.Skip(1).ForEach(x => x.Setting.gameObject.Destroy());
        Loading = SlotButtons[0];
        Loading.Do = () => {};
        Loading.Setting.Cast<ToggleOption>().TitleText.text = "Loading...";
        __instance.Children = new[] { Loading.Setting };
        yield return new WaitForSeconds(0.5f);
        Loading.Setting.gameObject.Destroy();
        OldButtons.ForEach(x => x.gameObject.SetActive(true));
        __instance.Children = OldButtons.ToArray();
        SettingsPatches.SettingsPage = 0;
        yield return new WaitForEndOfFrame();
        yield return flashCoro();
        yield break;
    }

    public void ToDo()
    {
        SlotButtons.Clear();
        Presets.Keys.ForEach(x => SlotButtons.Add(new(MultiMenu.External, x, delegate { LoadPreset(x); })));
        SlotButtons.Add(new(MultiMenu.External, "Cancel", delegate { Cancel(FlashWhite); }));
        var options = CreateOptions();
        var __instance = UObject.FindObjectOfType<GameOptionsMenu>();
        var y = __instance.GetComponentsInChildren<OptionBehaviour>().Max(option => option.transform.localPosition.y);
        var x = __instance.Children[1].transform.localPosition.x;
        var z = __instance.Children[1].transform.localPosition.z;
        OldButtons = __instance.Children.ToList();
        OldButtons.ForEach(x => x.gameObject.SetActive(false));
        SettingsPatches.SettingsPage = 9;

        for (var i = 0; i < options.Count; i++)
            options[i].transform.localPosition = new(x, y - (i * 0.5f), z);

        __instance.Children = new(options.ToArray());
    }

    public void LoadPreset(string presetName, bool inLobby = false)
    {
        LogInfo($"Loading - {presetName}");
        string text = null;

        try
        {
            text = Presets[presetName];
        }
        catch
        {
            text = "";
        }

        if (IsNullEmptyOrWhiteSpace(text))
        {
            Cancel(FlashRed);
            LogError($"{presetName} no exist");
        }
        else
        {
            LoadSettings(text);

            if (!inLobby)
                Cancel(FlashGreen);
        }
    }

    private IEnumerator FlashGreen()
    {
        Setting.Cast<ToggleOption>().TitleText.color = UColor.green;
        yield return new WaitForSeconds(0.5f);
        Setting.Cast<ToggleOption>().TitleText.color = UColor.white;
    }

    private IEnumerator FlashRed()
    {
        Setting.Cast<ToggleOption>().TitleText.color = UColor.red;
        yield return new WaitForSeconds(0.5f);
        Setting.Cast<ToggleOption>().TitleText.color = UColor.white;
    }

    private IEnumerator FlashWhite() => null;
}