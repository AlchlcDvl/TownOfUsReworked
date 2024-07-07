namespace TownOfUsReworked.Options2;

public class ButtonOption(MultiMenu menu, string name, Action toDo = null) : IOption
{
    public Action Do { get; set; } = toDo ?? BlankVoid;
    public string Name { get; set; } = name;
    public MultiMenu Menu { get; } = menu;
    public OptionBehaviour Setting { get; set; }

    public void OptionCreated()
    {
        Setting.name = Setting.gameObject.name = Name.Replace(" ", "_");
        Setting.Title = (StringNames)999999999;
        Setting.OnValueChanged = (Action<OptionBehaviour>)BlankVoid; // The cast here is not redundant, idk why the compiler refuses to accept this
        Setting.Cast<ToggleOption>().TitleText.text = Name;
    }

    public static void SaveSettings() => Coroutines.Start(StartSaving());

    private static IEnumerator StartSaving()
    {
        if (!SettingsPatches.SaveSettings.Setting)
            yield break;

        SettingsPatches.SaveSettings.Do = BlankVoid;
        SettingsPatches.SaveSettings.Setting.Cast<ToggleOption>().TitleText.text = "Loading...";
        var filePath = Path.Combine(TownOfUsReworked.Options, "SavedSettings");
        var i = 0;
        var pathoverridden = false;

        while (File.Exists(filePath))
        {
            filePath = Path.Combine(TownOfUsReworked.Options, $"SavedSettings{i}");
            i++;
            pathoverridden = true;
        }

        SaveText($"SavedSettings{(pathoverridden ? i : "")}", OptionAttribute.SettingsToString(), TownOfUsReworked.Options);
        yield return Wait(0.5f);
        SettingsPatches.SaveSettings.Setting.Cast<ToggleOption>().TitleText.text = "Save Current Settings";
        SettingsPatches.SaveSettings.Setting.Cast<ToggleOption>().TitleText.color = UColor.green;
        yield return Wait(0.5f);
        SettingsPatches.SaveSettings.Setting.Cast<ToggleOption>().TitleText.color = UColor.white;
        SettingsPatches.SaveSettings.Do = SaveSettings;
        yield return EndFrame();
        yield break;
    }
}