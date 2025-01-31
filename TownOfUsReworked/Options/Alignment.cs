namespace TownOfUsReworked.Options;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AlignmentOptionAttribute(RoleListSlot alignment = RoleListSlot.None, bool noParts = false, string colorHex = null, int priority = -1, MultiMenu menu = MultiMenu.Layer) :
    BaseHeaderOptionAttribute(menu, CustomOptionType.Alignment, priority)
{
    public RoleListSlot Alignment { get; } = alignment;
    private bool NoParts { get; } = noParts;
    public BaseHeaderOptionAttribute GroupHeader { get; set; }
    private TextMeshPro Left { get; set; }
    private TextMeshPro Right { get; set; }
    private TextMeshPro Center { get; set; }
    private GameObject Single { get; set; }
    private TextMeshPro ButtonText { get; set; }
    private PassiveButton Button { get; set; }
    private GameMode SavedMode { get; set; }
    private GameObject Cog { get; set; }
    private Transform PlsMnsBtn { get; set; }
    private UColor Color { get; } = CustomColorManager.FromHex(colorHex ?? "#FFFFFFFF");

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

        Center.transform.localPosition = Right.transform.localPosition + ((Left.transform.localPosition - Right.transform.localPosition) / 2);

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
            >= RoleListSlot.CrewSupport and <= RoleListSlot.CrewUtil => CustomColorManager.Crew,
            >= RoleListSlot.IntruderSupport and <= RoleListSlot.IntruderHead => CustomColorManager.Intruder,
            >= RoleListSlot.NeutralPros and <= RoleListSlot.NeutralEvil => CustomColorManager.Neutral,
            >= RoleListSlot.SyndicateKill and <= RoleListSlot.SyndicateUtil => CustomColorManager.Syndicate,
            _ => Color
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

    public override void Toggle()
    {
        Value = !Get();
        (GroupHeader as HeaderOptionAttribute)?.Toggle();

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
            Left.gameObject.SetActive(SavedMode is GameMode.AllAny or GameMode.Classic);
            Right.gameObject.SetActive(SavedMode is GameMode.AllAny or GameMode.Classic);
            Center.gameObject.SetActive(SavedMode == GameMode.RoleList);
            Single.SetActive(SavedMode is not (GameMode.Classic or GameMode.AllAny));

            Center.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
            {
                GameMode.RoleList => "Unique",
                _ => ""
            }));
            Right.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
            {
                GameMode.Classic => "Chance",
                GameMode.AllAny => "Unique",
                _ => ""
            }));
            Left.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
            {
                GameMode.Classic => "Count",
                GameMode.AllAny => "Active",
                _ => ""
            }));
        }
    }

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        GroupHeader = GetOption<HeaderOptionAttribute>($"{Name.Replace("Roles", "")}Settings");
    }

    public void SetUpOptionsMenu()
    {
        SettingsPatches.SettingsPage = 5;
        SettingsPatches.CachedPage = 1;
        var scrollbar = GameSettingMenu.Instance.RoleSettingsTab.scrollBar;
        SettingsPatches.ScrollerLocation = scrollbar.Inner.transform.localPosition;
        scrollbar.ScrollToTop();
        SettingsPatches.SelectedSubOptions = GroupHeader;
        SettingsPatches.OnValueChanged();
    }
}