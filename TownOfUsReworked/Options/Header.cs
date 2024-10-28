namespace TownOfUsReworked.Options;

[AttributeUsage(AttributeTargets.Class)]
public class HeaderOptionAttribute(MultiMenu menu) : OptionAttribute<bool>(menu, CustomOptionType.Header), IOptionGroup
{
    public string[] GroupMemberStrings { get; set; }
    public OptionAttribute[] GroupMembers { get; set; }
    private Type ClassType { get; set; }
    private TextMeshPro ButtonText { get; set; }
    public PassiveButton Button { get; set; }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var header = Setting.Cast<CategoryHeaderMasked>();
        header.Title.text = $"<b>{TranslationManager.Translate(ID)}</b>";
        var collapse = header.transform.FindChild("Collapse")?.gameObject;
        collapse.GetComponent<PassiveButton>().OverrideOnClickListeners(Toggle);
        ButtonText = collapse.GetComponentInChildren<TextMeshPro>();
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

    public void SetTypeAndOptions(Type type)
    {
        ClassType = type;
        Name = ClassType.Name;
        Value = DefaultValue = false;
        ID = $"CustomOption.{Name}";
        AllOptions.Add(this);
        var members = new List<OptionAttribute>();

        foreach (var prop in AccessTools.GetDeclaredProperties(type))
        {
            var att = prop.GetCustomAttribute<OptionAttribute>();

            if (att != null)
            {
                att.SetProperty(prop);
                members.Add(att);
            }
        }

        GroupMembers = [ .. members ];
    }

    public override void AddMenuIndex(int index)
    {
        base.AddMenuIndex(index);
        GroupMembers.ForEach(x => x.AddMenuIndex(index));
    }

    public override void PostLoadSetup()
    {
        GroupMemberStrings = [ .. GroupMembers.Select(x => x.Name) ];
        OptionParents1.Add((GroupMemberStrings, [ Name ]));
    }
}