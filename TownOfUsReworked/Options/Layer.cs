namespace TownOfUsReworked.Options;

public class LayerOptionAttribute(string hexCode, LayerEnum layer, bool noParts = false, int min = 1, int max = 15) : OptionAttribute<RoleOptionData>(MultiMenu.Layer, CustomOptionType.Layer)
{
    private int CachedCount { get; set; }
    private int CachedChance { get; set; }
    public int Max { get; set; } = max;
    public int Min { get; set; } = min;
    public LayerEnum Layer { get; } = layer;
    public UColor LayerColor { get; } = CustomColorManager.FromHex(hexCode);
    private bool NoParts { get; set; } = noParts;
    private string HexCode { get; set; } = hexCode;
    public HeaderOptionAttribute GroupHeader { get; set; }
    private GameObject Unique { get; set; }
    private GameObject Active1 { get; set; }
    private GameObject Divider { get; set; }
    private GameObject Chance { get; set; }
    private GameObject Count { get; set; }
    public SpriteRenderer UniqueCheck { get; set; }
    public SpriteRenderer ActiveCheck { get; set; }
    private GameMode SavedMode { get; set; }
    private PassiveButton Button { get; set; }
    private TextMeshPro CenterTitle { get; set; }
    private TextMeshPro CenterValue { get; set; }
    private TextMeshPro LeftTitle { get; set; }
    private SpriteRenderer CenterBackground { get; set; }
    private GameObject LeftBox { get; set; }
    private GameObject CenterBox { get; set; }
    private GameObject RightBox { get; set; }
    private GameObject RightCheck { get; set; }
    private GameObject RightCross { get; set; }
    private GameObject CenterCheck { get; set; }
    private GameObject CenterCross { get; set; }
    private static Vector3 Left;
    private static Vector3 Right;
    private static Vector3 Diff;

    public override void OptionCreated()
    {
        base.OptionCreated();
        var role = Setting.Cast<RoleOptionSetting>();
        role.titleText.SetText(TranslationManager.Translate(ID));
        role.titleText.color = LayerColor.Alternate(0.45f);
        role.roleMaxCount = Max;
        role.labelSprite.color = LayerColor;

        Count = role.transform.GetChild(1).gameObject;
        Chance = role.transform.GetChild(2).gameObject;
        Divider = role.transform.GetChild(4).gameObject;
        Unique = role.transform.GetChild(6).gameObject;
        Active1 = role.transform.GetChild(7).gameObject;

        UniqueCheck = Unique.transform.GetChild(2).GetComponent<SpriteRenderer>();
        ActiveCheck = Active1.transform.GetChild(2).GetComponent<SpriteRenderer>();

        if (Left == default)
            Left = Count.transform.localPosition;

        if (Right == default)
            Right = Chance.transform.localPosition;

        if (Diff == default)
            Diff = (Left - Right) / 2;

        var unique = Unique.GetComponent<PassiveButton>();
        unique.OverrideOnClickListeners(ToggleUnique);

        var active = Active1.GetComponent<PassiveButton>();
        active.OverrideOnClickListeners(ToggleActive);

        var cog = role.transform.GetChild(5).gameObject;
        cog.SetActive(GroupHeader != null || OptionParents1.Any(x => x.Item2.Contains(Layer)) || OptionParents2.Any(x => x.Item2.Contains(Layer)));
        var button = cog.GetComponent<GameOptionButton>();
        button.OverrideOnClickListeners(SetUpOptionsMenu);
        button.interactableHoveredColor = UColor.white;
        button.interactableColor = button.buttonSprite.color = LayerColor.Alternate(0.3f);

        if (NoParts)
        {
            Chance.SetActive(false);
            Count.SetActive(false);
            Unique.SetActive(false);
            Active1.SetActive(false);
        }

        if (!AmongUsClient.Instance.AmHost || IsInGame())
        {
            role.CountMinusBtn.gameObject.SetActive(false);
            role.CountPlusBtn.gameObject.SetActive(false);
            role.ChanceMinusBtn.gameObject.SetActive(false);
            role.ChancePlusBtn.gameObject.SetActive(false);

            unique.GetComponentsInChildren<SpriteRenderer>().ForEach(x => x.enabled = x != UniqueCheck);
            active.GetComponentsInChildren<SpriteRenderer>().ForEach(x => x.enabled = x != ActiveCheck);

            unique.enabled = false;
            active.enabled = false;
        }

        SavedMode = GameMode.None;
        Update();
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        var view = ViewSetting.Cast<ViewSettingsInfoPanelRoleVariant>();

        LeftBox = view.transform.GetChild(0).gameObject;
        LeftTitle = LeftBox.transform.GetChild(5).GetComponent<TextMeshPro>();

        RightBox = view.transform.GetChild(2).gameObject;
        RightCheck = RightBox.transform.GetChild(3).gameObject;
        RightCross = RightBox.transform.GetChild(4).gameObject;

        CenterBox = view.transform.GetChild(6).gameObject;
        CenterCheck = RightBox.transform.GetChild(3).gameObject;
        CenterCross = RightBox.transform.GetChild(4).gameObject;
        CenterValue = CenterBox.transform.GetChild(0).GetComponent<TextMeshPro>();
        CenterTitle = CenterBox.transform.GetChild(5).GetComponent<TextMeshPro>();
        CenterBackground = CenterBox.transform.GetChild(1).GetComponent<SpriteRenderer>();

        Button = view.transform.Find("Toggle").GetComponent<PassiveButton>();
        Button.gameObject.SetActive(GroupHeader != null || OptionParents1.Any(x => x.Item2.Contains(Layer)) || OptionParents2.Any(x => x.Item2.Contains(Layer)));
        Button.SelectButton(GroupHeader?.Get() == true);
        Button.OverrideOnClickListeners(Toggle);

        view.background.sprite = view.chanceBackground.sprite = CenterBackground.sprite = view.disabledCube;

        if (NoParts)
        {
            LeftBox.SetActive(false);
            RightBox.SetActive(false);
            CenterBox.SetActive(false);
        }

        SavedMode = GameMode.None;
        ViewUpdate();
    }

    public int GetChance() => IsClassic() || IsCustom() || IsKilling() ? Get().Chance : 0;

    public int GetCount() => IsCustom() ? Get().Count : (IsRoleList() ? GetOptions<RoleListEntryAttribute>().Count(x => x.Get() == Layer && !x.IsBan) : 1);

    public void IncreaseCount()
    {
        var val = Get();
        var chance = GetChance();
        var max = IsCustom() ? Max : Min;
        var count = CycleInt(max, 0, GetCount(), true);

        if (chance == 0 && count > 0)
            chance = CachedChance == 0 ? 5 : CachedChance;
        else if (count == 0 && chance > 0)
        {
            CachedChance = chance;
            chance = 0;
        }

        if (count.IsInRange(0, Min))
            count = Min;

        val.Count = count;
        val.Chance = chance;
        Set(val);
    }

    public void DecreaseCount()
    {
        var val = Get();
        var chance = GetChance();
        var max = IsCustom() ? Max : Min;
        var count = CycleInt(max, 0, GetCount(), false);

        if (chance == 0 && count > 0)
            chance = CachedChance == 0 ? 5 : CachedChance;
        else if (count == 0 && chance > 0)
        {
            CachedChance = chance;
            chance = 0;
        }

        if (count.IsInRange(0, Min))
            count = 0;

        val.Count = count;
        val.Chance = chance;
        Set(val);
    }

    public void IncreaseChance()
    {
        var val = Get();
        var count = GetCount();
        var chance = CycleInt(100, 0, GetChance(), true, Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10);

        if (chance == 0 && count > 0)
        {
            CachedCount = count;
            count = 0;
        }
        else if (count == 0 && chance > 0)
            count = CachedCount == 0 || !IsCustom() ? Min : CachedCount;

        val.Count = count;
        val.Chance = chance;
        Set(val);
    }

    public void DecreaseChance()
    {
        var val = Get();
        var count = GetCount();
        var chance = CycleInt(100, 0, GetChance(), false, Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10);

        if (chance == 0 && count > 0)
        {
            CachedCount = count;
            count = 0;
        }
        else if (count == 0 && chance > 0)
            count = CachedCount == 0 || !IsCustom() ? Min : CachedCount;

        val.Count = count;
        val.Chance = chance;
        Set(val);
    }

    public override void Update()
    {
        var data = Get();
        var role = Setting.Cast<RoleOptionSetting>();
        role.chanceText.SetText($"{data.Chance}%");
        role.countText.SetText($"x{data.Count}");
        role.roleChance = data.Chance;
        UniqueCheck.enabled = data.Unique;
        ActiveCheck.enabled = data.Active;

        if (SavedMode == GameModeSettings.GameMode)
            return;

        SavedMode = GameModeSettings.GameMode;
        Divider.SetActive(SavedMode is GameMode.Custom or GameMode.AllAny);

        if (!NoParts)
        {
            Chance.SetActive(SavedMode is GameMode.Classic or GameMode.Custom or GameMode.KillingOnly);
            Count.SetActive(SavedMode == GameMode.Custom);
            Unique.SetActive(SavedMode is GameMode.AllAny or GameMode.RoleList);
            Active1.SetActive(SavedMode == GameMode.AllAny);

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
                    Unique.transform.localPosition = Right + new Vector3(0.75f, 0f, 0f);
                    Active1.transform.localPosition = Left + new Vector3(0.75f, 0f, 0f);
                    break;

                case GameMode.RoleList:
                    Unique.transform.localPosition = Right + Diff + new Vector3(0.91f, 0f, 0f);
                    break;

                case GameMode.KillingOnly:
                    Chance.transform.localPosition = Right + Diff;
                    break;
            }
        }
    }

    public void ToggleActive()
    {
        var val = Get();
        val.Active = !val.Active;
        CachedCount = val.Count;
        val.Count = 1;
        ActiveCheck.enabled = val.Active;
        Set(val);
    }

    public void ToggleUnique()
    {
        var val = Get();
        val.Unique = !val.Unique;
        UniqueCheck.enabled = val.Unique;
        Set(val);
    }

    public override string Format()
    {
        var val = Get();
        return GameModeSettings.GameMode switch
        {
            GameMode.Classic => $"{val.Chance}%",
            GameMode.Custom => $"{val.Chance}% x{val.Count}",
            GameMode.KillingOnly => $"{(val.Active ? "A" : "Ina")}ctive",
            GameMode.AllAny => $"{(val.Active ? "A" : "Ina")}ctive & {(val.Unique ? "" : "Non-")}Unique",
            GameMode.RoleList => $"{(val.Unique ? "" : "Non-")}Unique",
            _ => "Invalid"
        };
    }

    public override void PostLoadSetup()
    {
        Value = DefaultValue = new RoleOptionData(0, 0, false, false, Layer);
        Property.SetValue(null, Value);
        GroupHeader = GetOptions<HeaderOptionAttribute>().Find(x => x.Name.Contains($"{Layer}"));
        GroupHeader?.AddMenuIndex(6 + (int)Layer);
    }

    public void SetUpOptionsMenu()
    {
        SettingsPatches.SettingsPage = 6 + (int)Layer;
        SettingsPatches.CachedPage = 1;
        var scrollbar = GameSettingMenu.Instance.RoleSettingsTab.scrollBar;
        SettingsPatches.ScrollerLocation = scrollbar.Inner.transform.localPosition;
        scrollbar.ScrollToTop();
        SettingsPatches.OnValueChanged();
    }

    public void Toggle()
    {
        if (GroupHeader == null)
            return;

        GroupHeader.Button = Button;
        GroupHeader.Toggle();
    }

    public override void ViewUpdate()
    {
        var data = Get();
        var view = ViewSetting.Cast<ViewSettingsInfoPanelRoleVariant>();

        view.chanceText.SetText(SavedMode == GameMode.Custom ? $"{data.Chance}%" : "");
        view.settingText.SetText(SavedMode == GameMode.Custom ? $"x{data.Count}" : "");
        CenterValue.SetText(SavedMode is GameMode.Classic or GameMode.KillingOnly ? $"{data.Chance}%" : "");

        CenterCheck.SetActive(SavedMode == GameMode.RoleList && data.Unique);
        CenterCross.SetActive(SavedMode == GameMode.RoleList && !data.Unique);
        RightCheck.SetActive(SavedMode == GameMode.AllAny && data.Unique);
        RightCross.SetActive(SavedMode == GameMode.AllAny && !data.Unique);
        view.checkMark.gameObject.SetActive(SavedMode == GameMode.AllAny && data.Active);
        view.checkMarkOff.gameObject.SetActive(SavedMode == GameMode.AllAny && !data.Active);

        var isActive = RoleGenManager.GetSpawnItem(Layer).IsActive();
        var color = isActive ? LayerColor : Palette.DisabledGrey.Shadow();
        view.labelBackground.color = color;
        view.titleText.color = view.chanceText.color = view.chanceTitle.color = view.settingText.color = LeftTitle.color = CenterValue.color = CenterTitle.color = color.Alternate(0.45f);
        view.background.color = view.chanceBackground.color = CenterBackground.color = color.Alternate(0.3f);

        if (SavedMode == GameModeSettings.GameMode)
            return;

        SavedMode = GameModeSettings.GameMode;

        if (!NoParts)
        {
            LeftBox.gameObject.SetActive(SavedMode is GameMode.AllAny or GameMode.Custom);
            RightBox.gameObject.SetActive(SavedMode is GameMode.AllAny or GameMode.Custom);
            CenterBox.gameObject.SetActive(SavedMode is GameMode.Classic or GameMode.RoleList or GameMode.KillingOnly);

            CenterTitle.SetText(TranslationManager.Translate("RoleOption." + (SavedMode switch
            {
                GameMode.Classic or GameMode.KillingOnly => "Chance",
                GameMode.RoleList => "Unique",
                _ => ""
            })));
            view.chanceTitle.SetText(TranslationManager.Translate("RoleOption." + (SavedMode switch
            {
                GameMode.Custom => "Chance",
                GameMode.AllAny => "Unique",
                _ => ""
            })));
            LeftTitle.SetText(TranslationManager.Translate("RoleOption." + (SavedMode switch
            {
                GameMode.Custom => "Count",
                GameMode.AllAny => "Active",
                _ => ""
            })));
        }
    }

    public override string SettingNotif() => $"<{HexCode}>{base.SettingNotif()}</color>";
}