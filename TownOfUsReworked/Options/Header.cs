namespace TownOfUsReworked.Options;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class HeaderOptionAttribute(MultiMenu menu, int priority = -1) : OptionAttribute<bool>(menu, CustomOptionType.Header, priority), IOptionGroup
{
    public string[] GroupMemberStrings { get; set; }
    public OptionAttribute[] GroupMembers { get; set; }
    private TextMeshPro ButtonText { get; set; }
    public PassiveButton Button { get; set; }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var header = Setting.Cast<CategoryHeaderMasked>();
        header.Title.SetText($"<b>{TranslationManager.Translate(ID)}</b>");
        var collapse = header.transform.FindChild("Collapse")?.gameObject;
        collapse.GetComponent<PassiveButton>().OverrideOnClickListeners(Toggle);
        ButtonText = collapse.GetComponentInChildren<TextMeshPro>();
        ButtonText.SetText(Get() ? "-" : "+");
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        Button = ViewSetting.transform.Find("TitleButton").GetComponent<PassiveButton>();
        Button.buttonText.SetText($"<b>{TranslationManager.Translate(ID)}</b>");
        Button.OverrideOnClickListeners(Toggle);
        Button.SelectButton(Value);
    }

    public void Toggle()
    {
        Value = !Get();

        if (Setting)
        {
            ButtonText.SetText(Value ? "-" : "+");
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
        GroupMemberStrings = [ .. GroupMembers.Select(x => x.Name) ];
        OptionParents1.Add((GroupMemberStrings, [ Name ]));
    }
}