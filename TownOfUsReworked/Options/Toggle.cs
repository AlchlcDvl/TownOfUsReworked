namespace TownOfUsReworked.Options;

public class ToggleOptionAttribute(MultiMenu menu) : OptionAttribute<bool>(menu, CustomOptionType.Toggle)
{
    public void Toggle() => Set(!Get());

    public override string Format() => Get() ? "On" : "Off";

    public override void OptionCreated()
    {
        base.OptionCreated();
        var toggle = Setting.Cast<ToggleOption>();
        toggle.TitleText.text = TranslationManager.Translate(ID);
        toggle.CheckMark.enabled = Get();
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.text = "";
        viewSettingsInfoPanel.checkMark.gameObject.SetActive(Get());
        viewSettingsInfoPanel.checkMarkOff.gameObject.SetActive(!Get());
    }

    public override void ModifyViewSetting()
    {
        base.ModifyViewSetting();
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.text = "";
        viewSettingsInfoPanel.checkMark.gameObject.SetActive(Get());
        viewSettingsInfoPanel.checkMarkOff.gameObject.SetActive(!Get());
    }

    public override void ModifySetting()
    {
        base.ModifySetting();
        var toggle = Setting.Cast<ToggleOption>();
        var newValue = Get();
        toggle.oldValue = newValue;

        if (toggle.CheckMark)
            toggle.CheckMark.enabled = newValue;
    }
}