#pragma warning disable IDE0059 // Unnecessary assignment of a value
namespace TownOfUsReworked.Options;

public sealed class LayerOption(string hexCode, LayerEnum layer, bool noParts = false, byte min = 1, byte max = 15, byte change = 1) : Option<RoleOptionData>(CustomOptionType.Layer)
{
    private byte Max { get; } = max;
    private byte Min { get; } = min;
    private UColor LayerColor { get; } = CustomColorManager.FromHex(hexCode);
    private bool NoParts { get; } = noParts;
    private string HexCode { get; } = hexCode;
    public LayerEnum Layer { get; } = layer;
    private byte Change { get; } = change;
    private byte CachedCount { get; set; }
    private byte CachedChance { get; set; }
    private GameObject Unique { get; set; }
    private GameObject Active1 { get; set; }
    private GameObject Divider { get; set; }
    private GameObject Chance { get; set; }
    private GameObject Count { get; set; }
    private GameObject Cog { get; set; }
    private SpriteRenderer UniqueCheck { get; set; }
    private SpriteRenderer ActiveCheck { get; set; }
    private Data.GameMode SavedMode { get; set; }
    private PassiveButton Button { get; set; }
    private TextMeshPro CenterTitle { get; set; }
    private TextMeshPro LeftTitle { get; set; }
    private SpriteRenderer CenterBackground { get; set; }
    private GameObject LeftBox { get; set; }
    private GameObject CenterBox { get; set; }
    private GameObject RightBox { get; set; }
    private GameObject RightCheck { get; set; }
    private GameObject RightCross { get; set; }
    private GameObject CenterCheck { get; set; }
    private GameObject CenterCross { get; set; }
    public LayerHeaderOption GroupHeader { get; private set; }

    public static Vector3 Left;
    public static Vector3 Right;
    public static Vector3 Diff;

    public override void OptionCreated()
    {
        base.OptionCreated();
        var role = Setting.Cast<RoleOptionSetting>();
        role.titleText.text = TranslationManager.Translate(ID);
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

        Cog = role.transform.GetChild(5).gameObject;
        Cog.SetActive(GroupHeader?.GroupMembers?.Any(x => x.PartiallyActive()) == true);
        var button = Cog.GetComponent<GameOptionButton>();
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

        var unique = Unique.GetComponent<PassiveButton>();
        var active = Active1.GetComponent<PassiveButton>();

        if (!AmongUsClient.Instance.AmHost || (IsInGame() && !TownOfUsReworked.MciActive))
        {
            role.CountMinusBtn.gameObject.SetActive(false);
            role.CountPlusBtn.gameObject.SetActive(false);
            role.ChanceMinusBtn.gameObject.SetActive(false);
            role.ChancePlusBtn.gameObject.SetActive(false);

            unique.enabled = false;
            active.enabled = false;
        }
        else
        {
            role.CountPlusBtn.OverrideOnClickListeners(IncreaseCount);
            role.CountMinusBtn.OverrideOnClickListeners(DecreaseCount);
            role.ChancePlusBtn.OverrideOnClickListeners(IncreaseChance);
            role.ChanceMinusBtn.OverrideOnClickListeners(DecreaseChance);

            unique.OverrideOnClickListeners(ToggleUnique);
            active.OverrideOnClickListeners(ToggleActive);
        }

        SavedMode = Data.GameMode.None;
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        var view = ViewSetting.Cast<ViewSettingsInfoPanelRoleVariant>();

        LeftBox = view.transform.Find("Left").gameObject;
        LeftTitle = LeftBox.transform.Find("Title").GetComponent<TextMeshPro>();

        RightBox = view.transform.Find("Right").gameObject;
        RightCheck = RightBox.transform.Find("UniqueOn").gameObject;
        RightCross = RightBox.transform.Find("UniqueOff").gameObject;

        CenterBox = view.transform.Find("Centre").gameObject;
        CenterCheck = RightBox.transform.Find("UniqueOn").gameObject;
        CenterCross = RightBox.transform.Find("UniqueOff").gameObject;
        CenterTitle = CenterBox.transform.Find("Title").GetComponent<TextMeshPro>();
        CenterBackground = CenterBox.transform.Find("Sprite").GetComponent<SpriteRenderer>();

        Button = view.transform.Find("Toggle").GetComponent<PassiveButton>();
        Button.gameObject.SetActive(GroupHeader?.GroupMembers?.Any(x => x.PartiallyActive()) == true);
        Button.SelectButton(GroupHeader?.Value == true);
        Button.OverrideOnClickListeners(Toggle);

        view.background.sprite = view.chanceBackground.sprite = CenterBackground.sprite = view.disabledCube;

        if (NoParts)
        {
            LeftBox.SetActive(false);
            RightBox.SetActive(false);
            CenterBox.SetActive(false);
        }

        SavedMode = Data.GameMode.None;
    }

    private void IncreaseCount()
    {
        var chance = Value.Chance;
        var count = CycleByte(Max, 0, Value.Count, true, Change);

        if (chance == 0 && count > 0)
            chance = CachedChance == 0 ? (byte)5 : CachedChance;
        else if (count == 0 && chance > 0)
        {
            CachedChance = chance;
            chance = 0;
        }

        if (count.IsInRange(0, Min))
            count = Min;

        var val = Value;
        val.Count = count;
        val.Chance = chance;
        Set(val);
    }

    private void DecreaseCount()
    {
        var chance = Value.Chance;
        var count = CycleByte(Max, 0, Value.Count, false, Change);

        if (chance == 0 && count > 0)
            chance = CachedChance == 0 ? (byte)5 : CachedChance;
        else if (count == 0 && chance > 0)
        {
            CachedChance = chance;
            chance = 0;
        }

        if (count.IsInRange(0, Min))
            count = Min;

        var val = Value;
        val.Count = count;
        val.Chance = chance;
        Set(val);
    }

    private void IncreaseChance()
    {
        var count = Value.Count;
        var chance = CycleByte(100, 0, Value.Chance, true, Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10);

        if (chance == 0 && count > 0)
        {
            CachedCount = count;
            count = 0;
        }
        else if (count == 0 && chance > 0)
            count = CachedCount == 0 ? Min : CachedCount;

        var val = Value;
        val.Count = count;
        val.Chance = chance;
        Set(val);
    }

    private void DecreaseChance()
    {
        var count = Value.Count;
        var chance = CycleByte(100, 0, Value.Chance, false, Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10);

        if (chance == 0 && count > 0)
        {
            CachedCount = count;
            count = 0;
        }
        else if (count == 0 && chance > 0)
            count = CachedCount == 0 ? Min : CachedCount;

        var val = Value;
        val.Count = count;
        val.Chance = chance;
        Set(val);
    }

    public override void Update()
    {
        var data = Value;
        var role = Setting.Cast<RoleOptionSetting>();
        role.chanceText.text = $"{data.Chance}%";
        role.countText.text = $"x{data.Count}";
        role.roleChance = data.Chance;
        UniqueCheck.enabled = data.Unique;
        ActiveCheck.enabled = data.Active;
        Cog.SetActive(GroupHeader?.GroupMembers?.Any(x => x.PartiallyActive()) == true);

        if (SavedMode == GameModeSettings.GameMode)
            return;

        SavedMode = GameModeSettings.GameMode;
        Divider.SetActive(SavedMode is Data.GameMode.Classic or Data.GameMode.AllAny);

        if (NoParts)
            return;

        Chance.SetActive(SavedMode is Data.GameMode.Classic);
        Count.SetActive(SavedMode == Data.GameMode.Classic);
        Unique.SetActive(SavedMode is Data.GameMode.AllAny or Data.GameMode.List);
        Active1.SetActive(SavedMode == Data.GameMode.AllAny);

        switch (SavedMode)
        {
            case Data.GameMode.AllAny:
            {
                Unique.transform.localPosition = Right + new Vector3(0.75f, 0f, 0f);
                Active1.transform.localPosition = Left + new Vector3(0.75f, 0f, 0f);
                break;
            }
            case Data.GameMode.List:
            {
                Unique.transform.localPosition = Right + Diff + new Vector3(0.76f, 0f, 0f);
                break;
            }
        }
    }

    private void ToggleActive()
    {
        var val = Value;
        val.Active = !val.Active;
        ActiveCheck.enabled = val.Active;
        Set(val);
    }

    private void ToggleUnique()
    {
        var val = Value;
        val.Unique = !val.Unique;
        UniqueCheck.enabled = val.Unique;
        Set(val);
    }

    protected override string Format()
    {
        var val = Value;
        return GameModeSettings.GameMode switch
        {
            Data.GameMode.Classic => $"{val.Chance}% x{val.Count}",
            Data.GameMode.AllAny => $"{(val.Active ? "A" : "Ina")}ctive & {(val.Unique ? "" : "Non-")}Unique",
            Data.GameMode.List => $"{(val.Unique ? "" : "Non-")}Unique",
            _ => "Invalid"
        };
    }

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        var value = new RoleOptionData(0, 0, false, false, Layer);
        Member.SetValue(null, value);
        Value = Member.GetValue<RoleOptionData>(null);
        GroupHeader = GetOptions<LayerHeaderOption>().Find(x => x.Layer == Layer);
    }

    private void SetUpOptionsMenu()
    {
        SettingsPatches.SettingsPage = 3;
        SettingsPatches.CachedPage = 1;
        var scrollbar = GameSettingMenu.Instance.RoleSettingsTab.scrollBar;
        SettingsPatches.ScrollerLocation = scrollbar.Inner.transform.localPosition;
        scrollbar.ScrollToTop();
        SettingsPatches.SelectedSubOptions = GroupHeader;
        SettingsPatches.OnValueChanged();
    }

    private void Toggle()
    {
        if (GroupHeader is null)
            return;

        GroupHeader.Button = Button;
        GroupHeader.Toggle();
    }

    public override void ViewUpdate()
    {
        var data = Value;
        var view = ViewSetting.Cast<ViewSettingsInfoPanelRoleVariant>();

        Button.gameObject.SetActive(GroupHeader?.GroupMembers?.Any(x => x.PartiallyActive()) == true);

        view.chanceText.text = SavedMode == Data.GameMode.Classic ? $"{data.Chance}%" : "";
        view.settingText.text = SavedMode == Data.GameMode.Classic ? $"x{data.Count}" : "";

        CenterCheck.SetActive(SavedMode == Data.GameMode.List && data.Unique);
        CenterCross.SetActive(SavedMode == Data.GameMode.List && !data.Unique);
        RightCheck.SetActive(SavedMode == Data.GameMode.AllAny && data.Unique);
        RightCross.SetActive(SavedMode == Data.GameMode.AllAny && !data.Unique);
        view.checkMark.gameObject.SetActive(SavedMode == Data.GameMode.AllAny && data.Active);
        view.checkMarkOff.gameObject.SetActive(SavedMode == Data.GameMode.AllAny && !data.Active);

        var isActive = RoleGenManager.GetSpawnItem(Layer).IsActive();
        var color = isActive ? LayerColor : Palette.DisabledGrey.Shadow();
        view.labelBackground.color = color;
        view.titleText.color = view.chanceText.color = view.chanceTitle.color = view.settingText.color = LeftTitle.color = CenterTitle.color = color.Alternate(0.45f);
        view.background.color = view.chanceBackground.color = CenterBackground.color = color.Alternate(0.3f);

        if (SavedMode == GameModeSettings.GameMode)
            return;

        SavedMode = GameModeSettings.GameMode;

        if (NoParts)
            return;

        LeftBox.gameObject.SetActive(SavedMode is Data.GameMode.AllAny or Data.GameMode.Classic);
        RightBox.gameObject.SetActive(SavedMode is Data.GameMode.AllAny or Data.GameMode.Classic);
        CenterBox.gameObject.SetActive(SavedMode == Data.GameMode.List);

        CenterTitle.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
        {
            Data.GameMode.List => "Unique",
            _ => ""
        }));
        view.chanceTitle.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
        {
            Data.GameMode.Classic => "Chance",
            Data.GameMode.AllAny => "Unique",
            _ => ""
        }));
        LeftTitle.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
        {
            Data.GameMode.Classic => "Count",
            Data.GameMode.AllAny => "Active",
            _ => ""
        }));
    }

    protected override string SettingNotif() => $"<{HexCode}>{base.SettingNotif()}</color>";

    public override void ReadValueRpc(NetData reader) => Set(reader.ReadRoleOptionData(), false);

    protected override void ReadValueString(string value) => Set(RoleOptionData.Parse(value), false);
}