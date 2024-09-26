namespace TownOfUsReworked.Options;

[AttributeUsage(AttributeTargets.Class)]
public class AlignsOptionAttribute(MultiMenu menu, LayerEnum alignment, bool noParts = false) : OptionAttribute<bool>(menu, CustomOptionType.Alignment), IOptionGroup
{
    public LayerEnum Alignment { get; } = alignment;
    private bool NoParts { get; set; } = noParts;
    public HeaderOptionAttribute GroupHeader { get; set; }
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

    public override void OptionCreated()
    {
        base.OptionCreated();
        var header = Setting.Cast<CategoryHeaderMasked>();
        header.Title.text = $"<b>{TranslationManager.Translate(ID)}</b>";
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

        var flag = GroupHeader != null || OptionParents1.Any(x => x.Item2.Contains(Alignment)) || OptionParents2.Any(x => x.Item2.Contains(Alignment));
        var cog = header.transform.GetChild(4).gameObject;
        cog.SetActive(flag);
        cog.GetComponent<PassiveButton>().OverrideOnClickListeners(SetUpOptionsMenu);

        if (!flag)
            header.transform.GetChild(3).localPosition = new(-5.539f, -0.45f, -2f);

        Collapse = header.transform.FindChild("Collapse").gameObject;
        Collapse.GetComponent<PassiveButton>().OverrideOnClickListeners(Toggle);
        Collapse.GetComponentInChildren<TextMeshPro>().text = Get() ? "-" : "+";

        var color = Alignment switch
        {
            LayerEnum.CrewSupport or LayerEnum.CrewInvest or LayerEnum.CrewProt or LayerEnum.CrewKill or LayerEnum.CrewUtil or LayerEnum.CrewSov or LayerEnum.CrewAudit =>
                CustomColorManager.Crew,
            LayerEnum.IntruderSupport or LayerEnum.IntruderConceal or LayerEnum.IntruderDecep or LayerEnum.IntruderKill or LayerEnum.IntruderUtil or LayerEnum.IntruderHead =>
                CustomColorManager.Intruder,
            LayerEnum.NeutralKill or LayerEnum.NeutralNeo or LayerEnum.NeutralEvil or LayerEnum.NeutralBen or LayerEnum.NeutralPros or LayerEnum.NeutralApoc or LayerEnum.NeutralHarb =>
                CustomColorManager.Neutral,
            LayerEnum.SyndicateKill or LayerEnum.SyndicateSupport or LayerEnum.SyndicateDisrup or LayerEnum.SyndicatePower or LayerEnum.SyndicateUtil => CustomColorManager.Syndicate,
            LayerEnum.Ability => CustomColorManager.Ability,
            LayerEnum.Modifier => CustomColorManager.Modifier,
            LayerEnum.Objectifier => CustomColorManager.Objectifier,
            _ => UColor.white
        };

        quota.GetChild(0).GetComponent<SpriteRenderer>().color = color.Shadow(0.35f);
        quota.GetChild(2).GetComponent<SpriteRenderer>().color = quota.GetChild(3).GetComponent<SpriteRenderer>().color = quota.GetChild(5).GetComponent<SpriteRenderer>().color =
            color.Shadow();

        Update();
    }

    public void Toggle()
    {
        Value = !Get();
        Collapse.GetComponentInChildren<TextMeshPro>().text = Value ? "-" : "+";
        SettingsPatches.OnValueChanged();
        SettingsPatches.OnValueChangedView();
    }

    public override void Update()
    {
        if (SavedMode == GameModeSettings.GameMode)
            return;

        SavedMode = GameModeSettings.GameMode;

        if (NoParts)
        {
            Chance.SetActive(false);
            Count.SetActive(false);
            Single.SetActive(true);
            Unique.SetActive(false);
            Active1.SetActive(false);
        }
        else
        {
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

        foreach (var prop in AccessTools.GetDeclaredProperties(ClassType))
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

    public override void PostLoadSetup()
    {
        GroupHeader = GetOptions<HeaderOptionAttribute>().Find(x => x.Name == Name.Replace("Roles", "") + "Settings");
        GroupHeader?.AddMenuIndex(6 + (int)Alignment);
    }

    public void SetUpOptionsMenu()
    {
        SettingsPatches.SettingsPage = 6 + (int)Alignment;
        SettingsPatches.CachedPage = 1;
        GameSettingMenu.Instance.RoleSettingsTab.scrollBar.ScrollToTop();
        SettingsPatches.OnValueChanged();
    }

    public override void AddMenuIndex(int index)
    {
        base.AddMenuIndex(index);
        GroupHeader?.AddMenuIndex(index);
    }
}