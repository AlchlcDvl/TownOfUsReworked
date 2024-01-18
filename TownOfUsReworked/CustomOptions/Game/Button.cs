namespace TownOfUsReworked.CustomOptions;

public class CustomButtonOption : CustomOption
{
    public Action Do { get; set; }

    public CustomButtonOption(MultiMenu menu, string name, Action toDo = null) : base(menu, name, CustomOptionType.Button, null) => Do = toDo ?? BlankVoid;

    public override void OptionCreated()
    {
        base.OptionCreated();
        Setting.Cast<ToggleOption>().TitleText.text = Name;
    }

    public static ToggleOption CreateButton()
    {
        var togglePrefab = UObject.FindObjectOfType<ToggleOption>();
        var toggle = UObject.Instantiate(togglePrefab, togglePrefab.transform.parent);
        toggle.transform.GetChild(0).localPosition = new(-1.05f, 0f, 0f);
        toggle.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new(5.5f, 0.37f);
        toggle.transform.GetChild(1).localScale = new(1.6f, 1f, 1f);
        toggle.transform.GetChild(2).gameObject.SetActive(false);
        toggle.gameObject.GetComponent<BoxCollider2D>().size = new(7.91f, 0.45f);
        return toggle;
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
        yield return new WaitForEndOfFrame();
        yield break;
    }
}