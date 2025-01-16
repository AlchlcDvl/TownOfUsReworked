namespace TownOfUsReworked.Options;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class HeaderOptionAttribute(MultiMenu menu, int priority = -1) : OptionAttribute<bool>(menu, CustomOptionType.Header, priority), IOptionGroup
{
    public IEnumerable<OptionAttribute> GroupMembers { get; set; }
    private TextMeshPro ButtonText { get; set; }
    public PassiveButton Button { get; set; }
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
        ButtonText.text = Get() ? "-" : "+";
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

        if (ViewSetting)
        {
            Button.SelectButton(Value);
            SettingsPatches.OnValueChangedView();
        }
    }

    public override void AddMenuIndex(int index)
    {
        base.AddMenuIndex(index);
        GroupMembers.ForEach(x => x.AddMenuIndex(index));
    }

    public override void PostLoadSetup()
    {
        TargetType = typeof(bool);
        OptionParents3.Add((GroupMembers, this));
    }

    public override void Update() => Collapse.SetActive(GroupMembers.Any(x => x.PartiallyActive()));
}