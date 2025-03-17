namespace TownOfUsReworked.Options;

public sealed class ToggleOption() : Option<bool>(CustomOptionType.Toggle)
{
    private void Toggle() => Set(!Value);

    protected override string Format() => Value ? "On" : "Off";

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

    public override void ReadValueRpc(MessageReader reader) => Set(reader.ReadBoolean(), false);

    public override void WriteValueRpc(MessageWriter writer) => writer.Write(Value);

    protected override void ReadValueString(string value) => Set(bool.Parse(value), false);
}