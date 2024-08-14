namespace TownOfUsReworked.Options2;

[AttributeUsage(AttributeTargets.Class)]
public class HeaderOptionAttribute(MultiMenu2 menu, HeaderType type = HeaderType.General) : OptionAttribute(menu, CustomOptionType.Header)
{
    public HeaderType HeaderType { get; } = type;
    public string[] GroupMemberStrings { get; set; }
    public OptionAttribute[] GroupMembers { get; set; }
    private GameObject Chance { get; set; }
    private GameObject Count { get; set; }
    private GameObject Active1 { get; set; }
    private GameObject Unique { get; set; }
    private GameObject Single { get; set; }
    private GameObject Collapse { get; set; }
    private Type ClassType { get; set; }
    private GameMode SavedMode { get; set; } = GameMode.None;
    private static Vector3 Left;
    private static Vector3 Right;
    private static Vector3 Diff;

    public bool Get() => (bool)Value;

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

        if (HeaderType == HeaderType.Layer)
        {
            var quota = header.transform.GetChild(2);
            Count = quota.GetChild(1).gameObject;
            Chance = quota.GetChild(4).gameObject;
            Single = quota.GetChild(5).gameObject;
            Active1 = quota.GetChild(6).gameObject;
            Unique = quota.GetChild(7).gameObject;

            if (Left == default)
                Left = Count.transform.localPosition;

            if (Right == default)
                Right = Chance.transform.localPosition;

            if (Diff == default)
                Diff = (Left - Right) / 2;

            Update();
        }

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

    public override void Update()
    {
        if (SavedMode == GameModeSettings.GameMode || HeaderType == HeaderType.General)
            return;

        SavedMode = GameModeSettings.GameMode;
        Chance.SetActive(SavedMode is GameMode.Classic or GameMode.Custom or GameMode.KillingOnly);
        Count.SetActive(SavedMode == GameMode.Custom);
        Unique.SetActive(SavedMode is GameMode.AllAny or GameMode.RoleList);
        Active1.SetActive(SavedMode == GameMode.AllAny);
        Single.SetActive(SavedMode is not (GameMode.Custom or GameMode.AllAny));

        switch (SavedMode)
        {
            case GameMode.Classic:
                Chance.transform.localPosition = Right + Diff;
                break;

            case GameMode.Custom:
                Chance.transform.localPosition = Right;
                Count.transform.localPosition = Left;
                break;

            case GameMode.AllAny:
                Unique.transform.localPosition = Right;
                Active1.transform.localPosition = Left;
                break;

            case GameMode.RoleList:
                Unique.transform.localPosition = Right + Diff;
                break;

            case GameMode.KillingOnly:
                Chance.transform.localPosition = Right + Diff;
                break;
        }
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
}