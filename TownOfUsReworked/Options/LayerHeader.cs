namespace TownOfUsReworked.Options;

public sealed class LayerHeaderOptionAttribute(LayerEnum layer, int priority = -1) : BaseHeaderOptionAttribute(MultiMenu.LayerSubOptions, CustomOptionType.LayerHeader, priority)
{
    public LayerEnum Layer { get; } = layer;
    public PassiveButton Button { get; set; }
    private GameObject Collapse { get; set; }
    private GameObject Info { get; set; }
    private GameObject Desc { get; set; }
    private TextMeshPro ButtonText { get; set; }
    private SpriteRenderer Label { get; set; }
    public static Sprite OgLabel;
    public static Vector3 OgPosition;

    public override void OptionCreated()
    {
        base.OptionCreated();
        var titleText = Setting.transform.FindChild("Title").GetComponent<TextMeshPro>();
        titleText.text = $"<b>{TranslationManager.Translate(ID)}</b>";
        Collapse = Setting.transform.FindChild("Collapse").gameObject;
        Collapse.GetComponent<PassiveButton>().OverrideOnClickListeners(Toggle);
        Collapse.SetActive(GroupMembers.Any(x => x.PartiallyActive()));
        ButtonText = Collapse.GetComponentInChildren<TextMeshPro>();
        ButtonText.text = Get() ? "-" : "+";
        Info = Setting.transform.FindChild("Info").gameObject;
        Info.GetComponentInChildren<TextMeshPro>().text = TranslationManager.Translate($"ShortDesc.{Layer}");
        Desc = Setting.transform.FindChild("Desc").gameObject;
        Label = Setting.transform.FindChild("Label").GetComponent<SpriteRenderer>();
        titleText.color = (Label.color = LayerDictionary[Layer].Color).Alternate();
        Label.sprite = Value ? OgLabel : GetSprite("Unopened");
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
        Label.sprite = Value ? OgLabel : GetSprite("Unopened");

        if (Value)
            Label.transform.localPosition = OgPosition;
        else
            Label.transform.SetLocalX(-1.1706f);
    }
}