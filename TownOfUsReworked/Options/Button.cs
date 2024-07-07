namespace TownOfUsReworked.Options;

public class CustomButtonOption(MultiMenu menu, string name, Action toDo = null) : CustomOption(menu, name, CustomOptionType.Button, null)
{
    public Action Do { get; set; } = toDo ?? BlankVoid;

    public override void OptionCreated()
    {
        base.OptionCreated();
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

        SaveText($"SavedSettings{(pathoverridden ? i : "")}", SettingsToString(), TownOfUsReworked.Options);
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