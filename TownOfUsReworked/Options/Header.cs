namespace TownOfUsReworked.Options;

[AttributeUsage(AttributeTargets.Class)]
public class HeaderOptionAttribute(MultiMenu menu) : OptionAttribute<bool>(menu, CustomOptionType.Header)
{
    public string[] GroupMemberStrings { get; set; }
    public OptionAttribute[] GroupMembers { get; set; }
    private GameObject Collapse { get; set; }
    private Type ClassType { get; set; }

    public override void OptionCreated()
    {
        base.OptionCreated();
        Created(Setting);
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        Created(ViewSetting);
    }

    private void Created(MonoBehaviour setting)
    {
        var header = setting.Cast<CategoryHeaderMasked>();
        header.Title.text = $"<b>{TranslationManager.Translate(ID)}</b>";
        Collapse = header.transform.FindChild("Collapse")?.gameObject;

        if (Collapse)
        {
            Collapse.GetComponent<PassiveButton>().OverrideOnClickListeners(Toggle);
            Collapse.GetComponentInChildren<TextMeshPro>().text = Get() ? "-" : "+";
        }
    }

    public void Toggle()
    {
        Value = !Get();
        Collapse.GetComponentInChildren<TextMeshPro>().text = (bool)Value ? "-" : "+";
        SettingsPatches.OnValueChanged(GameSettingMenu.Instance);
    }

    public void SetTypeAndOptions(Type type)
    {
        ClassType = type;
        Name = ClassType.Name;
        Value = DefaultValue = true;
        ID = $"CustomOption.{Name}";
        // OnChanged = AccessTools.GetDeclaredMethods(OnChangedType).Find(x => x.Name == OnChangedName);
        AllOptions.Add(this);
        var members = new List<OptionAttribute>();
        var strings = new List<string>();

        foreach (var prop in  AccessTools.GetDeclaredProperties(type))
        {
            var att = prop.GetCustomAttribute<OptionAttribute>();

            if (att != null)
            {
                att.SetProperty(prop);
                members.Add(att);
                strings.Add(prop.Name);
            }
        }

        GroupMemberStrings = [ .. strings ];
        GroupMembers = [ .. members ];
        OptionParents1.Add((GroupMemberStrings, [ Name ]));
    }

    public override void AddMenuIndex(int index)
    {
        base.AddMenuIndex(index);
        GroupMembers.ForEach(x => x.AddMenuIndex(index));
    }
}