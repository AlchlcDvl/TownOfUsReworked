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

        if (!AmongUsClient.Instance.AmHost && !ClientOnly)
        {
            foreach (var button2 in toggle.buttons)
            {
                button2.GetComponentsInChildren<SpriteRenderer>(true).ForEach(x => x.color = Palette.DisabledGrey);

                if (button2 is GameOptionButton goButton)
                {
                    goButton.interactableHoveredColor = goButton.interactableClickColor = Palette.DisabledGrey.Shadow();
                    goButton.interactableColor = Palette.DisabledGrey;
                }
            }
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
}