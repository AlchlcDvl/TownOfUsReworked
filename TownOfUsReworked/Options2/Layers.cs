namespace TownOfUsReworked.Options2;

public class LayersOptionAttribute(MultiMenu menu, string hexCode, LayerEnum layer, string[] groupMemberStrings = null) : OptionAttribute(menu, CustomOptionType.Layers)
{
    private int CachedCount { get; set; }
    private int CachedChance { get; set; }
    private int Max { get; set; } = 15;
    private int Min { get; set; } = 1;
    private LayerEnum Layer { get; } = layer;
    public UColor LayerColor { get; } = CustomColorManager.FromHex(hexCode);
    public string[] GroupMemberStrings { get; } = groupMemberStrings;
    public OptionAttribute[] GroupMembers { get; set; }
    private GameObject Unique { get; set; }
    private GameObject Active1 { get; set; }
    private GameObject Divider { get; set; }
    private GameObject Chance { get; set; }
    private GameObject Count { get; set; }
    private GameObject Cog { get; set; }
    private SpriteRenderer UniqueCheck { get; set; }
    private SpriteRenderer ActiveCheck { get; set; }
    private GameMode SavedMode { get; set; } = GameMode.None;
    private static Vector3 Left { get; set; }
    private static Vector3 Right { get; set; }
    private static Vector3 Diff { get; set; }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var role = Setting.Cast<RoleOptionSetting>();
        role.titleText.text = TranslationManager.Translate(ID);
        role.roleMaxCount = Max;
        var tuple = (RoleOptionData)Value;
        role.chanceText.text = $"{tuple.Chance}%";
        role.countText.text = $"x{tuple.Count}";
        role.role = null;
        role.roleChance = GetChance();
        role.labelSprite.color = LayerColor.Shadow().Shadow();

        Chance = role.transform.GetChild(1).gameObject;
        Count = role.transform.GetChild(2).gameObject;
        Divider = role.transform.GetChild(4).gameObject;
        Cog = role.transform.GetChild(5).gameObject;
        Unique = role.transform.GetChild(6).gameObject;
        Active1 = role.transform.GetChild(7).gameObject;

        if (Left == default)
            Left = Count.transform.localPosition;

        if (Right == default)
            Right = Chance.transform.localPosition;

        if (Diff == default)
            Diff = (Left - Right) / 2;

        Unique.GetComponent<PassiveButton>().OverrideOnClickListeners(ToggleUnique);
        Active1.GetComponent<PassiveButton>().OverrideOnClickListeners(ToggleUnique);

        UpdateParts();
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        var view = ViewSetting.Cast<ViewSettingsInfoPanelRoleVariant>();
        view.iconSprite.gameObject.SetActive(false);
        view.titleText.text = TranslationManager.Translate(ID);
        var tuple = (RoleOptionData)Value;
        view.chanceText.text = $"{tuple.Chance}%";
        view.settingText.text = $"x{tuple.Count}";
    }

    public int GetChance() => IsClassic ? ((RoleOptionData)Value).Chance : 0;

    public int GetCount() => IsCustom ? ((RoleOptionData)Value).Count : 1;

    public bool GetUnique() => (IsAA || IsRoleList) && ((RoleOptionData)Value).Unique;

    public bool GetActive() => (IsAA || IsKilling) && ((RoleOptionData)Value).Unique;

    public void IncreaseCount()
    {
        var chance = GetChance();
        var max = IsCustom ? Max : Min;
        var count = CycleInt(max, 0, GetCount(), true);

        if (chance == 0 && count > 0)
            chance = CachedChance == 0 ? 5 : CachedChance;
        else if (count == 0 && chance > 0)
        {
            CachedChance = chance;
            chance = 0;
        }

        Set(new RoleOptionData(chance, count, GetUnique(), GetActive()));
    }

    public void DecreaseCount()
    {
        var chance = GetChance();
        var max = IsCustom ? Max : Min;
        var count = CycleInt(max, 0, GetCount(), false);

        if (chance == 0 && count > 0)
            chance = CachedChance == 0 ? 5 : CachedChance;
        else if (count == 0 && chance > 0)
        {
            CachedChance = chance;
            chance = 0;
        }

        Set(new RoleOptionData(chance, count, GetUnique(), GetActive()));
    }

    public void IncreaseChance()
    {
        var count = GetCount();
        var chance = CycleInt(100, 0, GetChance(), true, Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10);

        if (chance == 0 && count > 0)
        {
            CachedCount = count;
            count = 0;
        }
        else if (count == 0 && chance > 0)
            count = CachedCount == 0 || !IsCustom ? Min : CachedCount;

        Set(new RoleOptionData(chance, count, GetUnique(), GetActive()));
    }

    public void DecreaseChance()
    {
        var count = GetCount();
        var chance = CycleInt(100, 0, GetChance(), false, Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10);

        if (chance == 0 && count > 0)
        {
            CachedCount = count;
            count = 0;
        }
        else if (count == 0 && chance > 0)
            count = CachedCount == 0 || !IsCustom ? Min : CachedCount;

        Set(new RoleOptionData(chance, count, GetUnique(), GetActive()));
    }

    public void UpdateParts()
    {
        Cog.SetActive(GroupMembers != null && GroupMembers.Length > 0 && (((IsClassic || IsCustom || IsKilling) && GetChance() > 0) || (IsAA && GetActive()) || (IsRoleList &&
            GetOptions<RoleListEntryAttribute>().Any(x => x.Get() == Layer))));

        if (SavedMode == CustomGameOptions2.GameMode)
            return;

        SavedMode = CustomGameOptions2.GameMode;
        Chance.SetActive(SavedMode is GameMode.Classic or GameMode.Custom or GameMode.KillingOnly);
        Count.SetActive(SavedMode == GameMode.Custom);
        Divider.SetActive(SavedMode is GameMode.Custom or GameMode.AllAny);
        Unique.SetActive(SavedMode is GameMode.AllAny or GameMode.RoleList);
        Active1.SetActive(SavedMode == GameMode.AllAny);

        switch(SavedMode)
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

    public void ToggleActive()
    {
        var val = (RoleOptionData)Value;
        val.Active = !val.Active;
        ActiveCheck.enabled = val.Active;
        Set(val);
    }

    public void ToggleUnique()
    {
        var val = (RoleOptionData)Value;
        val.Unique = !val.Unique;
        UniqueCheck.enabled = val.Unique;
        Set(val);
    }

    public override string Format() => CustomGameOptions2.GameMode switch
    {
        GameMode.Classic => $"{((RoleOptionData)Value).Chance}%",
        GameMode.Custom => $"{((RoleOptionData)Value).Chance}% x{((RoleOptionData)Value).Count}",
        GameMode.KillingOnly => $"{(((RoleOptionData)Value).Active ? "A" : "Ina")}ctive",
        GameMode.AllAny => $"{(((RoleOptionData)Value).Active ? "A" : "Ina")}ctive & {(((RoleOptionData)Value).Unique ? "" : "Non-")}Unique",
        GameMode.RoleList => $"{(((RoleOptionData)Value).Unique ? "" : "Non-")}Unique",
        _ => "Invalid"
    };

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();

        if (GroupMemberStrings != null)
            GroupMembers = AllOptions.Where(x => GroupMemberStrings.Contains(x.Property.Name)).ToArray();
    }
}