namespace TownOfUsReworked.Options;

public class ToggleOptionAttribute(MultiMenu menu) : OptionAttribute<bool>(menu, CustomOptionType.Toggle)
{
    public void Toggle() => Set(!Get());

    public override string Format() => Get() ? "On" : "Off";

    public override void OptionCreated()
    {
        base.OptionCreated();
        var toggle = Setting.Cast<ToggleOption>();
        toggle.TitleText.SetText(TranslationManager.Translate(ID));

        if ((!AmongUsClient.Instance.AmHost || IsInGame()) && !ClientOnly)
        {
            toggle.CheckMark.transform.parent.GetComponentsInChildren<SpriteRenderer>().ForEach(x => x.enabled = x != toggle.CheckMark);
            toggle.GetComponentInChildren<PassiveButton>().enabled = false;
        }

        Update();
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        ViewUpdate();
    }

    public override void ViewUpdate()
    {
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.SetText("");
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
}