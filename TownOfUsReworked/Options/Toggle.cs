namespace TownOfUsReworked.Options;

public class ToggleOptionAttribute() : OptionAttribute<bool>(CustomOptionType.Toggle)
{
    private void Toggle() => Set(!Get());

    public override string Format() => Get() ? "On" : "Off";

    public override void OptionCreated()
    {
        base.OptionCreated();
        var toggle = Setting.Cast<ToggleOption>();
        toggle.TitleText.text = TranslationManager.Translate(ID);
        var button = toggle.GetComponentInChildren<PassiveButton>();

        if ((!AmongUsClient.Instance.AmHost || IsInGame()) && !(ClientOnly || TownOfUsReworked.MCIActive))
            button.enabled = false;
        else
            button.OverrideOnClickListeners(Toggle);
    }

    public override void ViewUpdate()
    {
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.text = "";
        viewSettingsInfoPanel.checkMark.gameObject.SetActive(Get());
        viewSettingsInfoPanel.checkMarkOff.gameObject.SetActive(!Get());
    }

    public override void Update()
    {
        var toggle = Setting.Cast<ToggleOption>();
        var newValue = Get();
        toggle.oldValue = newValue;

        if (toggle.CheckMark)
            toggle.CheckMark.enabled = newValue;
    }

    public override void ReadValueRpc(MessageReader reader) => Set(reader.ReadBoolean(), false);

    public override void WriteValueRpc(MessageWriter writer) => writer.Write(Value);

    public override void ReadValueString(string value) => Set(bool.Parse(value), false);
}