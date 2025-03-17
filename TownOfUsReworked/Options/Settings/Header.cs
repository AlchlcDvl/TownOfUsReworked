namespace TownOfUsReworked.Options;

public class HeaderOption(MultiMenu menu, CustomOptionType type = CustomOptionType.Header) : BaseHeaderOption(menu, type)
{
    private TextMeshPro ButtonText { get; set; }
    private PassiveButton Button { get; set; }
    private GameObject Collapse { get; set; }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var header = Setting.Cast<CategoryHeaderMasked>();
        header.Title.text = $"<b>{TranslationManager.Translate(ID)}</b>";
        Collapse = header.transform.FindChild("Collapse").gameObject;
        Collapse.GetComponent<PassiveButton>().OverrideOnClickListeners(Toggle);
        Collapse.SetActive(GroupMembers.Any(x => x.PartiallyActive()));
        ButtonText = Collapse.GetComponentInChildren<TextMeshPro>();
        ButtonText.text = Value ? "-" : "+";
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        Button = ViewSetting.transform.Find("TitleButton").GetComponent<PassiveButton>();
        Button.buttonText.text = $"<b>{TranslationManager.Translate(ID)}</b>";
        Button.OverrideOnClickListeners(Toggle);
        Button.SelectButton(Value);
    }

    public void Toggle()
    {
        Value = !Value;

        if (Setting)
        {
            ButtonText.text = Value ? "-" : "+";
            SettingsPatches.OnValueChanged();
        }

        if (!ViewSetting)
            return;

        Button.SelectButton(Value);
        SettingsPatches.OnValueChangedView();
    }

    public override void Update() => Collapse.SetActive(GroupMembers.Any(x => x.PartiallyActive()));
}