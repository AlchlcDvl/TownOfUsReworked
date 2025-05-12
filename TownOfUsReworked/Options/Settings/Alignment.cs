namespace TownOfUsReworked.Options;

public sealed class AlignmentOption(ListSlot alignment, bool noParts = false) : BaseHeaderOption(MultiMenu.Layer, CustomOptionType.Alignment)
{
    public ListSlot Alignment { get; } = alignment;
    private bool NoParts { get; } = noParts;
    public AlignmentHeaderOption GroupHeader { get; private set; }
    private TextMeshPro Left { get; set; }
    private TextMeshPro Right { get; set; }
    private TextMeshPro Center { get; set; }
    private GameObject Single { get; set; }
    private TextMeshPro ButtonText { get; set; }
    private PassiveButton Button { get; set; }
    private Mode SavedMode { get; set; }
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
        ButtonText.text = Value ? "-" : "+";

        var color = Alignment switch
        {
            >= ListSlot.CrewSupport and <= ListSlot.CrewUtil => CustomColorManager.Crew,
            >= ListSlot.IntruderSupport and <= ListSlot.IntruderHead => CustomColorManager.Intruder,
            >= ListSlot.NeutralPros and <= ListSlot.NeutralNeo => CustomColorManager.Neutral,
            >= ListSlot.SyndicateKill and <= ListSlot.SyndicateUtil => CustomColorManager.Syndicate,
            ListSlot.ApocDeity or ListSlot.ApocHarb => CustomColorManager.Apocalypse,
            ListSlot.Modifiers => CustomColorManager.Modifier,
            ListSlot.Abilities => CustomColorManager.Ability,
            ListSlot.Dispositions => CustomColorManager.Disposition,
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

        SavedMode = Mode.None;
    }

    private bool ChildrenActive() => GroupHeader?.GroupMembers?.Any(x => x.PartiallyActive()) == true;

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        Button = ViewSetting.transform.Find("TitleButton").GetComponent<PassiveButton>();
        Button.buttonText.text = $"<b>{TranslationManager.Translate(ID)}</b>";
        Button.OverrideOnClickListeners(Toggle);
        Button.SelectButton(Value);

        SavedMode = Mode.None;
    }

    private void Toggle()
    {
        Value = !Value;
        GroupHeader?.Toggle();

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
        var flag = ChildrenActive();
        Cog.SetActive(flag);
        PlsMnsBtn.localPosition = flag ? DefaultPos : new(-5.539f, -0.45f, -2f);

        if (!Value)
        {
            Left.gameObject.SetActive(false);
            Right.gameObject.SetActive(false);
            Center.gameObject.SetActive(false);
            Single.gameObject.SetActive(true);
            return;
        }

        if (SavedMode == GameModeSettings.GameMode)
            return;

        SavedMode = GameModeSettings.GameMode;

        if (NoParts)
            return;

        Left.gameObject.SetActive(SavedMode is Mode.AllAny or Mode.Classic);
        Right.gameObject.SetActive(SavedMode is Mode.AllAny or Mode.Classic);
        Center.gameObject.SetActive(SavedMode == Mode.List);
        Single.SetActive(SavedMode is not (Mode.Classic or Mode.AllAny));

        Center.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
        {
            Mode.List => "Unique",
            _ => ""
        }));
        Right.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
        {
            Mode.Classic => "Chance",
            Mode.AllAny => "Unique",
            _ => ""
        }));
        Left.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
        {
            Mode.Classic => "Count",
            Mode.AllAny => "Active",
            _ => ""
        }));
    }

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        GroupHeader = GetOptions<AlignmentHeaderOption>().Find(x => x.Alignment == Alignment);
        GroupMembers?.Cast<LayerOption>().Do(x => x.HideDivider = NoParts);
    }

    protected override bool Visible()
    {
        if (GameModeSettings.GameMode == Mode.Vanilla)
            return false;

        var mode = GameModeSettings.GameMode is Mode.HideAndSeek or Mode.TaskRace;
        return Alignment == ListSlot.GameMode ? mode : !mode;
    }

    private void SetUpOptionsMenu()
    {
        SettingsPatches.SettingsPage = 4;
        SettingsPatches.CachedPage = 1;
        var scrollbar = GameSettingMenu.Instance.RoleSettingsTab.scrollBar;
        SettingsPatches.ScrollerLocation = scrollbar.Inner.transform.localPosition;
        scrollbar.ScrollToTop();
        SettingsPatches.SelectedSubOptions = GroupHeader;
        SettingsPatches.OnValueChanged();
    }
}