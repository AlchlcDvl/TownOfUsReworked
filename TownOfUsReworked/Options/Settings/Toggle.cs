namespace TownOfUsReworked.Options;

public sealed class ToggleOption(bool defaultValue = false) : Option<bool>(CustomOptionType.Toggle, defaultValue)
{
    private void Toggle() => Set(!Value);

    protected override string FormatValue() => Value ? "On" : "Off";

    public override void OptionCreated()
    {
        base.OptionCreated();
        var toggle = Setting.Cast<global::ToggleOption>();
        toggle.TitleText.text = TranslationManager.Translate(ID);
        var button = toggle.GetComponentInChildren<PassiveButton>();

        if ((!AmongUsClient.Instance.AmHost || IsInGame()) && !(ClientOnly || TownOfUsReworked.MciActive))
            button.enabled = false;
        else
            button.OverrideOnClickListeners(Toggle);
    }

    public override void ViewUpdate()
    {
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.text = "";
        viewSettingsInfoPanel.checkMark.gameObject.SetActive(Value);
        viewSettingsInfoPanel.checkMarkOff.gameObject.SetActive(!Value);
    }

    public override void Update() => Setting.Cast<global::ToggleOption>().CheckMark.enabled = Value;

    public override void ReadValueRpc(NetData reader) => Set(reader.ReadBool(), false);

    protected override void ReadValueString(string value) => Set(bool.Parse(value), false);
}