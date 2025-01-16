namespace TownOfUsReworked.Options;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AlignmentOptionAttribute(LayerEnum alignment, bool noParts = false) : OptionAttribute<bool>(MultiMenu.Layer, CustomOptionType.Alignment), IOptionGroup
{
    public LayerEnum Alignment { get; } = alignment;
    private bool NoParts { get; set; } = noParts;
    public HeaderOptionAttribute GroupHeader { get; set; }
    public IEnumerable<OptionAttribute> GroupMembers { get; set; }
    private TextMeshPro Left { get; set; }
    private TextMeshPro Right { get; set; }
    private TextMeshPro Center { get; set; }
    private GameObject Single { get; set; }
    private TextMeshPro ButtonText { get; set; }
    private PassiveButton Button { get; set; }
    private GameMode SavedMode { get; set; }
    private GameObject Cog { get; set; }
    private Transform PlsMnsBtn { get; set; }

    private static Vector3 DefaultPos { get; set; } = Vector3.zero;

    public override void OptionCreated()
    {
        base.OptionCreated();
        var header = Setting.Cast<CategoryHeaderMasked>();
        header.Title.text = $"<b>{TranslationManager.Translate(ID)}</b>";

        var quota = header.transform.GetChild(2);

        Left = quota.GetChild(1).GetComponent<TextMeshPro>();
        Right = quota.GetChild(4).GetComponent<TextMeshPro>();
        Center = quota.GetChild(6).GetComponent<TextMeshPro>();

        Single = quota.GetChild(5).gameObject;

        var flag = ChildrenActive();
        Cog = header.transform.GetChild(4).gameObject;
        Cog.SetActive(flag);
        var button = Cog.GetComponent<GameOptionButton>();
        button.OverrideOnClickListeners(SetUpOptionsMenu);
        PlsMnsBtn = header.transform.GetChild(3);

        if (DefaultPos == Vector3.zero)
            DefaultPos = PlsMnsBtn.localPosition;

        if (!flag)
            PlsMnsBtn.localPosition = new(-5.539f, -0.45f, -2f);

        var collapse = header.transform.FindChild("Collapse");
        collapse.GetComponent<PassiveButton>().OverrideOnClickListeners(Toggle);
        ButtonText = collapse.GetComponentInChildren<TextMeshPro>();
        ButtonText.text = Get() ? "-" : "+";

        var color = Alignment switch
        {
            LayerEnum.CrewSupport or LayerEnum.CrewInvest or LayerEnum.CrewProt or LayerEnum.CrewKill or LayerEnum.CrewUtil or LayerEnum.CrewSov => CustomColorManager.Crew,
            LayerEnum.IntruderSupport or LayerEnum.IntruderConceal or LayerEnum.IntruderDecep or LayerEnum.IntruderKill or LayerEnum.IntruderUtil or LayerEnum.IntruderHead =>
                CustomColorManager.Intruder,
            LayerEnum.NeutralKill or LayerEnum.NeutralNeo or LayerEnum.NeutralEvil or LayerEnum.NeutralBen or LayerEnum.NeutralPros or LayerEnum.NeutralApoc or LayerEnum.NeutralHarb =>
                CustomColorManager.Neutral,
            LayerEnum.SyndicateKill or LayerEnum.SyndicateSupport or LayerEnum.SyndicateDisrup or LayerEnum.SyndicatePower or LayerEnum.SyndicateUtil => CustomColorManager.Syndicate,
            LayerEnum.Ability => CustomColorManager.Ability,
            LayerEnum.Modifier => CustomColorManager.Modifier,
            LayerEnum.Disposition => CustomColorManager.Disposition,
            _ => UColor.white
        };

        quota.GetChild(0).GetComponent<SpriteRenderer>().color = color.Alternate(0.35f);
        quota.GetChild(2).GetComponent<SpriteRenderer>().color = quota.GetChild(3).GetComponent<SpriteRenderer>().color = quota.GetChild(5).GetComponent<SpriteRenderer>().color =
            color.Alternate();
        button.interactableHoveredColor = UColor.white;
        button.interactableColor = button.buttonSprite.color = color;

        if (NoParts)
        {
            Left.gameObject.SetActive(false);
            Right.gameObject.SetActive(false);
            Center.gameObject.SetActive(false);
            Single.SetActive(true);
        }

        SavedMode = GameMode.None;
        Update();
    }

    private bool ChildrenActive() => GroupHeader?.GroupMembers?.Any(x => x.PartiallyActive()) == true;

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        Button = ViewSetting.transform.Find("TitleButton").GetComponent<PassiveButton>();
        Button.buttonText.text = $"<b>{TranslationManager.Translate(ID)}</b>";
        Button.OverrideOnClickListeners(Toggle);
        Button.SelectButton(Value);

        SavedMode = GameMode.None;
        ViewUpdate();
    }

    public void Toggle()
    {
        Value = !Get();
        GroupHeader?.Toggle();

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

    public override void Update()
    {
        var flag = ChildrenActive();
        Cog.SetActive(flag);
        PlsMnsBtn.localPosition = flag ? DefaultPos : new(-5.539f, -0.45f, -2f);

        if (SavedMode == GameModeSettings.GameMode)
            return;

        SavedMode = GameModeSettings.GameMode;

        if (!NoParts)
        {
            Left.gameObject.SetActive(SavedMode is GameMode.AllAny or GameMode.Custom);
            Right.gameObject.SetActive(SavedMode is GameMode.AllAny or GameMode.Custom);
            Center.gameObject.SetActive(SavedMode is GameMode.Classic or GameMode.RoleList or GameMode.KillingOnly);
            Single.SetActive(SavedMode is not (GameMode.Custom or GameMode.AllAny));

            Center.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
            {
                GameMode.Classic or GameMode.KillingOnly => "Chance",
                GameMode.RoleList => "Unique",
                _ => ""
            }));
            Right.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
            {
                GameMode.Custom => "Chance",
                GameMode.AllAny => "Unique",
                _ => ""
            }));
            Left.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
            {
                GameMode.Custom => "Count",
                GameMode.AllAny => "Active",
                _ => ""
            }));
        }
    }

    public override void PostLoadSetup()
    {
        TargetType = typeof(bool);
        GroupHeader = GetOptions<HeaderOptionAttribute>().Find(x => x.Name == Name.Replace("Roles", "") + "Settings");
        GroupHeader?.AddMenuIndex(6 + (int)Alignment);
        OptionParents3.Add((GroupMembers, this));
    }

    public void SetUpOptionsMenu()
    {
        SettingsPatches.SettingsPage = 6 + (int)Alignment;
        SettingsPatches.CachedPage = 1;
        var scrollbar = GameSettingMenu.Instance.RoleSettingsTab.scrollBar;
        SettingsPatches.ScrollerLocation = scrollbar.Inner.transform.localPosition;
        scrollbar.ScrollToTop();
        SettingsPatches.OnValueChanged();
    }

    public override void AddMenuIndex(int index)
    {
        base.AddMenuIndex(index);
        GroupHeader?.AddMenuIndex(index);
    }
}