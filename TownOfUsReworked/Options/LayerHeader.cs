namespace TownOfUsReworked.Options;

public sealed class LayerHeaderOptionAttribute(LayerEnum layer, int priority = -1) : BaseHeaderOptionAttribute(MultiMenu.LayerSubOptions, CustomOptionType.LayerHeader, priority)
{
    public LayerEnum Layer { get; } = layer;
    public PassiveButton Button { get; set; }
    private GameObject Collapse { get; set; }
    private GameObject Info { get; set; }
    private GameObject Desc { get; set; }
    private TextMeshPro ButtonText { get; set; }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var titleText = Setting.transform.GetChild(0).GetComponent<TextMeshPro>();
        titleText.text = $"<b>{TranslationManager.Translate(ID)}</b>";
        titleText.color = (Setting.GetComponent<SpriteRenderer>().color = LayerDictionary[Layer].Color).Alternate();
        Setting.transform.GetChild(2).GetChild(0).GetComponent<TextMeshPro>().text = TranslationManager.Translate($"ShortDesc.{Layer}");
        Collapse = Setting.transform.FindChild("Collapse").gameObject;
        Collapse.GetComponent<PassiveButton>().OverrideOnClickListeners(Toggle);
        Collapse.SetActive(GroupMembers.Any(x => x.PartiallyActive()));
        ButtonText = Collapse.GetComponentInChildren<TextMeshPro>();
        ButtonText.text = Get() ? "-" : "+";
        Info = Setting.transform.GetChild(1).gameObject;
        Desc = Setting.transform.GetChild(2).gameObject;
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
        Value = !Get();

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

    public override void Update()
    {
        Collapse.SetActive(GroupMembers.Any(x => x.PartiallyActive()));
        Info.SetActive(Value);
        Desc.SetActive(Value);
    }
}