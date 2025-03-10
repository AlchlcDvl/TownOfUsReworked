#pragma warning disable IDE0059 // Unnecessary assignment of a value
namespace TownOfUsReworked.Options;

public sealed class LayerOptionAttribute(string hexCode, LayerEnum layer, bool noParts = false, byte min = 1, byte max = 15) : OptionAttribute<RoleOptionData>(CustomOptionType.Layer)
{
    private byte CachedCount { get; set; }
    private byte CachedChance { get; set; }
    private byte Max { get; } = max;
    private byte Min { get; } = min;
    private UColor LayerColor { get; } = CustomColorManager.FromHex(hexCode);
    private bool NoParts { get; } = noParts;
    private string HexCode { get; } = hexCode;
    private GameObject Unique { get; set; }
    private GameObject Active1 { get; set; }
    private GameObject Divider { get; set; }
    private GameObject Chance { get; set; }
    private GameObject Count { get; set; }
    private GameObject Cog { get; set; }
    private SpriteRenderer UniqueCheck { get; set; }
    private SpriteRenderer ActiveCheck { get; set; }
    private GameMode SavedMode { get; set; }
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
    public LayerEnum Layer { get; } = layer;
    public LayerHeaderOptionAttribute GroupHeader { get; private set; }

    private static Vector3 Left;
    private static Vector3 Right;
    private static Vector3 Diff;

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

        if (Left == default)
            Left = Count.transform.localPosition;

        if (Right == default)
            Right = Chance.transform.localPosition;

        if (Diff == default)
            Diff = (Left - Right) / 2;

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

        SavedMode = GameMode.None;
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
    }

    private byte GetChance() => IsClassic() ? Get().Chance : (byte)0;

    private byte GetCount() => IsClassic() ? Get().Count : (IsRoleList() ? (byte)GetOptions<ListEntryAttribute>().Count(x => x.Get().Equals(Layer) && !x.IsBan) : (byte)1);

    private void IncreaseCount()
    {
        var chance = GetChance();
        var max = IsClassic() ? Max : Min;
        var count = CycleByte(max, 0, GetCount(), true);

        if (chance == 0 && count > 0)
            chance = CachedChance == 0 ? (byte)5 : CachedChance;
        else if (count == 0 && chance > 0)
        {
            CachedChance = chance;
            chance = 0;
        }

        if (count.IsInRange(0, Min))
            count = Min;

        var val = Get();
        val.Count = count;
        val.Chance = chance;
        Set(val);
    }

    private void DecreaseCount()
    {
        var chance = GetChance();
        var max = IsClassic() ? Max : Min;
        var count = CycleByte(max, 0, GetCount(), false);

        if (chance == 0 && count > 0)
            chance = CachedChance == 0 ? (byte)5 : CachedChance;
        else if (count == 0 && chance > 0)
        {
            CachedChance = chance;
            chance = 0;
        }

        if (count.IsInRange(0, Min))
            count = 0;

        var val = Get();
        val.Count = count;
        val.Chance = chance;
        Set(val);
    }

    private void IncreaseChance()
    {
        var count = GetCount();
        var chance = CycleByte(100, 0, GetChance(), true, Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10);

        if (chance == 0 && count > 0)
        {
            CachedCount = count;
            count = 0;
        }
        else if (count == 0 && chance > 0)
            count = CachedCount == 0 || !IsClassic() ? Min : CachedCount;

        var val = Get();
        val.Count = count;
        val.Chance = chance;
        Set(val);
    }

    private void DecreaseChance()
    {
        var count = GetCount();
        var chance = CycleByte(100, 0, GetChance(), false, Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10);

        if (chance == 0 && count > 0)
        {
            CachedCount = count;
            count = 0;
        }
        else if (count == 0 && chance > 0)
            count = CachedCount == 0 || !IsClassic() ? Min : CachedCount;

        var val = Get();
        val.Count = count;
        val.Chance = chance;
        Set(val);
    }

    public override void Update()
    {
        var data = Get();
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
        Divider.SetActive(SavedMode is GameMode.Classic or GameMode.AllAny);

        if (NoParts)
            return;

        Chance.SetActive(SavedMode is GameMode.Classic);
        Count.SetActive(SavedMode == GameMode.Classic);
        Unique.SetActive(SavedMode is GameMode.AllAny or GameMode.RoleList);
        Active1.SetActive(SavedMode == GameMode.AllAny);

        switch (SavedMode)
        {
            case GameMode.AllAny:
            {
                Unique.transform.localPosition = Right + new Vector3(0.75f, 0f, 0f);
                Active1.transform.localPosition = Left + new Vector3(0.75f, 0f, 0f);
                break;
            }
            case GameMode.RoleList:
            {
                Unique.transform.localPosition = Right + Diff + new Vector3(0.76f, 0f, 0f);
                break;
            }
        }
    }

    private void ToggleActive()
    {
        var val = Get();
        val.Active = !val.Active;
        CachedCount = val.Count;
        val.Count = 1;
        ActiveCheck.enabled = val.Active;
        Set(val);
    }

    private void ToggleUnique()
    {
        var val = Get();
        val.Unique = !val.Unique;
        UniqueCheck.enabled = val.Unique;
        Set(val);
    }

    protected override string Format()
    {
        var val = Get();
        return GameModeSettings.GameMode switch
        {
            GameMode.Classic => $"{val.Chance}% x{val.Count}",
            GameMode.AllAny => $"{(val.Active ? "A" : "Ina")}ctive & {(val.Unique ? "" : "Non-")}Unique",
            GameMode.RoleList => $"{(val.Unique ? "" : "Non-")}Unique",
            _ => "Invalid"
        };
    }

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        var value = new RoleOptionData(0, 0, false, false, Layer);

        if (IsField)
        {
            Field.SetValue(null, value);
            Value = Field.GetValue<RoleOptionData>(null);
        }
        else if (IsProperty)
        {
            Property.SetValue(null, value);
            Value = Property.GetValue<RoleOptionData>(null);
        }
        else
            Value = value;

        GroupHeader = GetOptions<LayerHeaderOptionAttribute>().Find(x => x.Layer == Layer);
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
        if (GroupHeader == null)
            return;

        GroupHeader.Button = Button;
        GroupHeader.Toggle();
    }

    public override void ViewUpdate()
    {
        var data = Get();
        var view = ViewSetting.Cast<ViewSettingsInfoPanelRoleVariant>();

        Button.gameObject.SetActive(GroupHeader?.GroupMembers?.Any(x => x.PartiallyActive()) == true);

        view.chanceText.text = SavedMode == GameMode.Classic ? $"{data.Chance}%" : "";
        view.settingText.text = SavedMode == GameMode.Classic ? $"x{data.Count}" : "";

        CenterCheck.SetActive(SavedMode == GameMode.RoleList && data.Unique);
        CenterCross.SetActive(SavedMode == GameMode.RoleList && !data.Unique);
        RightCheck.SetActive(SavedMode == GameMode.AllAny && data.Unique);
        RightCross.SetActive(SavedMode == GameMode.AllAny && !data.Unique);
        view.checkMark.gameObject.SetActive(SavedMode == GameMode.AllAny && data.Active);
        view.checkMarkOff.gameObject.SetActive(SavedMode == GameMode.AllAny && !data.Active);

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

        LeftBox.gameObject.SetActive(SavedMode is GameMode.AllAny or GameMode.Classic);
        RightBox.gameObject.SetActive(SavedMode is GameMode.AllAny or GameMode.Classic);
        CenterBox.gameObject.SetActive(SavedMode == GameMode.RoleList);

        CenterTitle.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
        {
            GameMode.RoleList => "Unique",
            _ => ""
        }));
        view.chanceTitle.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
        {
            GameMode.Classic => "Chance",
            GameMode.AllAny => "Unique",
            _ => ""
        }));
        LeftTitle.text = TranslationManager.Translate("RoleOption." + (SavedMode switch
        {
            GameMode.Classic => "Count",
            GameMode.AllAny => "Active",
            _ => ""
        }));
    }

    protected override string SettingNotif() => $"<{HexCode}>{base.SettingNotif()}</color>";

    public override void ReadValueRpc(MessageReader reader) => Set(reader.ReadRoleOptionData(), false);

    public override void WriteValueRpc(MessageWriter writer) => writer.Write(Value);

    protected override void ReadValueString(string value) => Set(RoleOptionData.Parse(value), false);
}