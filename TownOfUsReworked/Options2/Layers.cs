namespace TownOfUsReworked.Options2;

public class LayersOptionAttribute(MultiMenu2 menu, string hexCode, LayerEnum layer) : OptionAttribute(menu, CustomOptionType.Layers)
{
    private int CachedCount { get; set; }
    private int CachedChance { get; set; }
    public int Max { get; set; } = 15;
    public int Min { get; set; } = 1;
    public LayerEnum Layer { get; } = layer;
    public UColor LayerColor { get; } = CustomColorManager.FromHex(hexCode);
    public HeaderOptionAttribute GroupHeader { get; set; }
    private GameObject Unique { get; set; }
    private GameObject Active1 { get; set; }
    private GameObject Divider { get; set; }
    private GameObject Chance { get; set; }
    private GameObject Count { get; set; }
    public SpriteRenderer UniqueCheck { get; set; }
    public SpriteRenderer ActiveCheck { get; set; }
    private GameMode SavedMode { get; set; } = GameMode.None;
    private static Vector3 Left;
    private static Vector3 Right;
    private static Vector3 Diff;

    public override void OptionCreated()
    {
        base.OptionCreated();
        var role = Setting.Cast<RoleOptionSetting>();
        role.titleText.text = TranslationManager.Translate(ID);
        role.roleMaxCount = Max;
        var tuple = Get();
        role.chanceText.text = $"{tuple.Chance}%";
        role.countText.text = $"x{tuple.Count}";
        role.role = null;
        role.roleChance = tuple.Chance;
        role.roleMaxCount = Max;
        role.labelSprite.color = LayerColor.Shadow();

        Count = role.transform.GetChild(1).gameObject;
        Chance = role.transform.GetChild(2).gameObject;
        Divider = role.transform.GetChild(4).gameObject;
        Unique = role.transform.GetChild(6).gameObject;
        Active1 = role.transform.GetChild(7).gameObject;

        UniqueCheck = Unique.transform.GetChild(2).GetComponent<SpriteRenderer>();
        ActiveCheck = Active1.transform.GetChild(2).GetComponent<SpriteRenderer>();

        UniqueCheck.enabled = tuple.Unique;
        ActiveCheck.enabled = tuple.Active;

        if (Left == default)
            Left = Count.transform.localPosition;

        if (Right == default)
            Right = Chance.transform.localPosition;

        if (Diff == default)
            Diff = (Left - Right) / 2;

        Unique.GetComponent<PassiveButton>().OverrideOnClickListeners(ToggleUnique);
        Active1.GetComponent<PassiveButton>().OverrideOnClickListeners(ToggleActive);

        var cog = role.transform.GetChild(5).gameObject;
        cog.GetComponent<PassiveButton>().OverrideOnClickListeners(SetUpOptionsMenu);
        cog.SetActive(GroupHeader != null || OptionParents1.Any(x => x.Item2.Contains(Layer)) || OptionParents2.Any(x => x.Item2.Contains(Layer)));

        SavedMode = GameMode.None;
        Update();
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        var view = ViewSetting.Cast<ViewSettingsInfoPanelRoleVariant>();
        view.iconSprite.gameObject.SetActive(false);
        view.titleText.text = TranslationManager.Translate(ID);
        var tuple = Get();
        view.chanceText.text = $"{tuple.Chance}%";
        view.settingText.text = $"x{tuple.Count}";
    }

    public int GetChance() => IsClassic ? Get().Chance : 0;

    public int GetCount() => IsCustom ? Get().Count : 1;

    public bool GetUnique() => (IsAA || IsRoleList) && Get().Unique;

    public bool GetActive() => (IsAA || IsKilling) && Get().Unique;

    public RoleOptionData Get() => (RoleOptionData)Value;

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

        Set(new RoleOptionData(chance, count, GetUnique(), GetActive(), Layer));
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

        Set(new RoleOptionData(chance, count, GetUnique(), GetActive(), Layer));
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

        Set(new RoleOptionData(chance, count, GetUnique(), GetActive(), Layer));
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

        Set(new RoleOptionData(chance, count, GetUnique(), GetActive(), Layer));
    }

    public override void Update()
    {
        if (SavedMode == GameModeSettings.GameMode)
            return;

        SavedMode = GameModeSettings.GameMode;
        Chance.SetActive(SavedMode is GameMode.Classic or GameMode.Custom or GameMode.KillingOnly);
        Count.SetActive(SavedMode == GameMode.Custom);
        Divider.SetActive(SavedMode is GameMode.Custom or GameMode.AllAny);
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

    public void ToggleActive()
    {
        var val = Get();
        val.Active = !val.Active;
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
        base.PostLoadSetup();
        ID =  $"CustomOption.{Layer}On";
        GroupHeader = GetOptions<HeaderOptionAttribute>().Find(x => x.Name == Layer.ToString());
        Value = DefaultValue = new RoleOptionData(0, 0, false, false, Layer);
        var menu = (MultiMenu2)(5 + (int)Layer);
        Property.SetValue(null, Value);

        if (GroupHeader != null)
        {
            GroupHeader.Menu = menu;

            if (!GroupHeader.Menus.Contains(menu))
                GroupHeader.Menus.Add(menu);

            foreach (var elem in GroupHeader.GroupMembers)
            {
                elem.Menu = menu;

                if (!elem.Menus.Contains(menu))
                    elem.Menus.Add(menu);
            }
        }

        if (OptionParents1.TryFinding(x => x.Item2.Contains(Layer), out var option1))
        {
            var option = GetOptionFromPropertyName<HeaderOptionAttribute>(option1.Item1.First());
            option.Menu = menu;

            if (!option.Menus.Contains(menu))
                option.Menus.Add(menu);

            foreach (var elem in option.GroupMembers)
            {
                elem.Menu = menu;

                if (!elem.Menus.Contains(menu))
                    elem.Menus.Add(menu);
            }
        }

        if (OptionParents2.TryFinding(x => x.Item2.Contains(Layer), out option1))
        {
            var option = GetOptionFromPropertyName<HeaderOptionAttribute>(option1.Item1.First());
            option.Menu = menu;

            if (!option.Menus.Contains(menu))
                option.Menus.Add(menu);

            foreach (var elem in option.GroupMembers)
            {
                elem.Menu = menu;

                if (!elem.Menus.Contains(menu))
                    elem.Menus.Add(menu);
            }
        }
    }

    public void SetUpOptionsMenu()
    {
        SettingsPatches.SettingsPage = 5 + (int)Layer;
        GameSettingMenu.Instance.RoleSettingsTab.scrollBar.ScrollToTop();
        SettingsPatches.OnValueChanged();
    }
}