namespace TownOfUsReworked.Options2;

public static class SettingsPatches
{
    private static readonly string[] Menus = [ "Global", "Crew", "Neutral", "Intruder", "Syndicate", "Modifier", "Objectifier", "Ability", "Role List", "Client", "Presets",
        "Role List Entry" ];
    private static readonly List<int> CreatedPages = [];

    public static PresetOption PresetButton = new();
    public static ButtonOption SaveSettings = new(MultiMenu.Main, "Save Current Settings", ButtonOption.SaveSettings);

    public static int SettingsPage;

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    public static class OptionsMenuBehaviour_Start
    {
        public static void Postfix(GameSettingMenu __instance)
        {
            __instance.GamePresetsButton.gameObject.SetActive(false);
            __instance.PresetsTab.gameObject.SetActive(false);

            if (SettingsPage == 9)
            {
                __instance.GameSettingsButton.gameObject.SetActive(false);
                __instance.RoleSettingsButton.gameObject.SetActive(false);
            }

            if (SettingsPage is 0 or 9 or 10)
                __instance.GameSettingsTab.gameObject.SetActive(true);
            else
            {
                __instance.RoleSettingsTab.gameObject.SetActive(true);
                ToggleTabs(__instance.RoleSettingsTab, SettingsPage);
            }
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
    public static class OnChangingTabs
    {
        public static void Postfix(ref int tabNum)
        {
            if (tabNum == 1)
                SettingsPage = 0;
            else if (tabNum == 2 && SettingsPage == 0)
                SettingsPage = 1;
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Close))]
    public static class OptionsMenuBehaviour_Close
    {
        public static void Prefix()
        {
            if (SettingsPage is 9 or 10 || (SettingsPage == 8 && !IsRoleList) || (SettingsPage == 7 && !CustomGameOptions.EnableAbilities) || (SettingsPage == 6 &&
                !CustomGameOptions.EnableObjectifiers) || (SettingsPage == 5 && !CustomGameOptions.EnableModifiers))
            {
                SettingsPage = 0;
            }

            PresetButton.SlotButtons.Clear();
            LobbyConsole.ClientOptionsActive = false;
            CreatedPages.Clear();
        }
    }

    public static RoleOptionSetting LayersPrefab;
    public static NumberOption NumberPrefab;
    public static ToggleOption TogglePrefab;
    public static ToggleOption ButtonPrefab;
    public static StringOption StringPrefab;
    public static CategoryHeaderMasked BaseHeaderPrefab;
    public static CategoryHeaderEditRole LayerHeaderPrefab;
    private static readonly List<MonoBehaviour> Prefabs1 = [];
    private static readonly List<MonoBehaviour> Prefabs2 = [];

    public static Dictionary<IOption, MonoBehaviour> CreateOptions(Transform parent = null)
    {
        var options = new Dictionary<IOption, MonoBehaviour>();

        if (SettingsPage == 10)
            return options;

        var type = (MultiMenu)SettingsPage;
        parent ??= GameObject.Find($"Main Camera/{(type == MultiMenu.Client ? "ClientOptionsMenu" : "PlayerOptionsMenu(Clone)")}/MainArea/{(type is MultiMenu.Main or MultiMenu.Client or MultiMenu.Presets ? "GAME SETTINGS" : "ROLES")} TAB/Scroller/SliderInner").transform;

        if (type == MultiMenu.Main)
        {
            if (!SaveSettings.Setting)
            {
                SaveSettings.Setting = UObject.Instantiate(ButtonPrefab, parent);
                SaveSettings.OptionCreated();
            }

            if (!PresetButton.Setting)
            {
                PresetButton.Setting = UObject.Instantiate(ButtonPrefab, parent);
                PresetButton.OptionCreated();
            }

            options.Add(SaveSettings, SaveSettings.Setting);
            options.Add(PresetButton, PresetButton.Setting);
        }

        foreach (var option in OptionAttribute.AllOptions.Where(x => x.Menu == type))
        {
            if (!option.Setting)
            {
                MonoBehaviour setting = null;

                switch (option.Type)
                {
                    case CustomOptionType.Number:
                        setting = UObject.Instantiate(NumberPrefab, parent);
                        break;

                    case CustomOptionType.String:
                        setting = UObject.Instantiate(StringPrefab, parent);
                        break;

                    case CustomOptionType.Layers:
                        setting = UObject.Instantiate(LayersPrefab, parent);
                        break;

                    case CustomOptionType.Toggle:
                        setting = UObject.Instantiate(TogglePrefab, parent);
                        break;

                    case CustomOptionType.Header:
                        setting = UObject.Instantiate(((HeaderOptionAttribute)option).HeaderType == HeaderType.General ? BaseHeaderPrefab : LayerHeaderPrefab, parent);
                        break;

                    case CustomOptionType.Entry:
                        setting = UObject.Instantiate(ButtonPrefab, parent);
                        break;
                }

                option.Setting = setting;
                option.OptionCreated();
            }

            options.Add(option, option.Setting);
        }

        return options;
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Awake))]
    public static class DefinePrefabs1
    {
        private static bool LayersSet;

        public static void Postfix(GameOptionsMenu __instance)
        {
            if (!NumberPrefab)
            {
                // Background = 0, Value Text = 1, Title = 2, - = 3, + = 4, Value Box = 5
                NumberPrefab = UObject.Instantiate(__instance.numberOptionOrigin, null).DontUnload().DontDestroy();
                NumberPrefab.name = "CustomNumbersOptionPrefab";

                var background = NumberPrefab.transform.GetChild(0);
                background.localPosition += new Vector3(-0.8f, 0f, 0f);
                background.localScale += new Vector3(1f, 0f, 0f);

                NumberPrefab.transform.GetChild(1).localPosition += new Vector3(1.05f, 0f, 0f);

                var title = NumberPrefab.transform.GetChild(2);
                title.localPosition += new Vector3(-3.1f, 0f, 0f);
                title.GetComponent<RectTransform>().sizeDelta = new(10f, 0.458f);

                NumberPrefab.transform.GetChild(3).localPosition += new Vector3(0.6f, 0f, 0f);
                NumberPrefab.transform.GetChild(4).localPosition += new Vector3(1.5f, 0f, 0f);

                var valueBox = NumberPrefab.transform.GetChild(5);
                valueBox.localPosition += new Vector3(1.05f, 0f, 0f);
                valueBox.localScale += new Vector3(0.2f, 0f, 0f);

                Prefabs1.Add(NumberPrefab);
            }

            if (!StringPrefab)
            {
                // Background = 0, Value Text = 1, Title = 2, < = 3, > = 4, Value Box = 5
                StringPrefab = UObject.Instantiate(__instance.stringOptionOrigin, null).DontUnload().DontDestroy();
                StringPrefab.name = "CustomStringOptionPrefab";

                var background = StringPrefab.transform.GetChild(0);
                background.localPosition += new Vector3(-0.8f, 0f, 0f);
                background.localScale += new Vector3(1f, 0f, 0f);

                StringPrefab.transform.GetChild(1).localPosition += new Vector3(1.05f, 0f, 0f);

                var title = StringPrefab.transform.GetChild(2);
                title.localPosition += new Vector3(-3.1f, 0f, 0f);
                title.GetComponent<RectTransform>().sizeDelta = new(10f, 0.458f);

                var minus = StringPrefab.transform.GetChild(3);
                minus.GetComponentInChildren<TextMeshPro>().text = "<";
                minus.localPosition += new Vector3(0.6f, 0f, 0f);

                var plus = StringPrefab.transform.GetChild(4);
                plus.GetComponentInChildren<TextMeshPro>().text = ">";
                plus.localPosition += new Vector3(1.5f, 0f, 0f);

                var valueBox = StringPrefab.transform.GetChild(5);
                valueBox.localPosition += new Vector3(1.05f, 0f, 0f);
                valueBox.localScale += new Vector3(0.2f, 0f, 0f);

                Prefabs1.Add(StringPrefab);
            }

            if (!TogglePrefab)
            {
                // Title = 0, Toggle = 1, Background = 2
                TogglePrefab = UObject.Instantiate(__instance.checkboxOrigin, null).DontUnload().DontDestroy();
                TogglePrefab.name = "CustomToggleOptionPrefab";

                var title = TogglePrefab.transform.GetChild(0);
                title.localPosition += new Vector3(-3.1f, 0f, 0f);
                title.GetComponent<RectTransform>().sizeDelta = new(10f, 0.458f);

                TogglePrefab.transform.GetChild(1).localPosition += new Vector3(2.2f, 0f, 0f);

                var background = TogglePrefab.transform.GetChild(2);
                background.localPosition += new Vector3(-0.8f, 0f, 0f);
                background.localScale += new Vector3(1f, 0f, 0f);

                Prefabs1.Add(TogglePrefab);
            }

            if (!ButtonPrefab)
            {
                // Title = 0, Toggle = 1, Background = 2
                ButtonPrefab = UObject.Instantiate(__instance.checkboxOrigin, null).DontUnload().DontDestroy();
                ButtonPrefab.name = "ButtonPrefab";

                ButtonPrefab.transform.GetChild(0).localPosition += new Vector3(0.3f, 0f, 0f);

                var click = ButtonPrefab.transform.GetChild(1);
                click.GetChild(2).gameObject.SetActive(false);
                click.localPosition += new Vector3(-15f, 0f, 0f);
                click.localScale += new Vector3(14f, 0f, 0f);

                ButtonPrefab.transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
                ButtonPrefab.transform.GetChild(2).gameObject.SetActive(false);

                Prefabs1.Add(ButtonPrefab);
            }

            if (!BaseHeaderPrefab)
            {
                BaseHeaderPrefab = UObject.Instantiate(__instance.categoryHeaderOrigin, null).DontUnload().DontDestroy();
                BaseHeaderPrefab.name = "CustomHeaderOptionBasePrefab";
                BaseHeaderPrefab.transform.localScale = new(0.63f, 0.63f, 0.63f);
                BaseHeaderPrefab.Background.transform.localScale += new Vector3(0.7f, 0f, 0f);

                Prefabs1.Add(BaseHeaderPrefab);
            }

            if (!LayersSet)
            {
                foreach (var mono in Prefabs1)
                {
                    mono.GetComponentsInChildren<SpriteRenderer>(true).ForEach(x => x.material.SetInt(PlayerMaterial.MaskLayer, 20));

                    foreach (var obj in mono.GetComponentsInChildren<TextMeshPro>(true))
                    {
                        obj.fontMaterial.SetFloat("_StencilComp", 3f);
                        obj.fontMaterial.SetFloat("_Stencil", 20);
                    }

                    mono.gameObject.SetActive(false);
                }

                LayersSet = true;
            }
        }
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Awake))]
    public static class DefinePrefabs2
    {
        private static bool LayersSet;

        public static void Postfix(RolesSettingsMenu __instance)
        {
            if (!LayersPrefab)
            {
                // Title = 0, Role # = 1, Chance % = 2, Background = 3, Divider = 4
                //            ┗------------┗----------- Value = 0, - = 1, + = 2, Value Box = 3
                LayersPrefab = UObject.Instantiate(__instance.roleOptionSettingOrigin, null).DontUnload().DontDestroy();
                LayersPrefab.name = "CustomLayersOptionPrefab";
                LayersPrefab.titleText.alignment = TextAlignmentOptions.Left;
                LayersPrefab.buttons = LayersPrefab.GetComponentsInChildren<PassiveButton>().ToArray();

                var newButton = UObject.Instantiate(LayersPrefab.buttons[0], LayersPrefab.transform);
                newButton.name = "LayersSubSettingsButton";
                newButton.transform.localPosition = new(0.2419f, -0.2582f, -2f);
                newButton.transform.FindChild("Plus_TMP").gameObject.Destroy();
                newButton.transform.FindChild("InactiveSprite").GetComponent<SpriteRenderer>().sprite = GetSprite("Cog");
                newButton.transform.FindChild("ActiveSprite").GetComponent<SpriteRenderer>().sprite = GetSprite("CogActive");
                newButton.OverrideOnClickListeners(BlankVoid);

                var unique = UObject.Instantiate(TogglePrefab.transform.GetChild(1).GetComponent<PassiveButton>(), LayersPrefab.transform);
                unique.name = "Unique";
                unique.OverrideOnClickListeners(BlankVoid);

                var active = UObject.Instantiate(TogglePrefab.transform.GetChild(1).GetComponent<PassiveButton>(), LayersPrefab.transform);
                active.name = "Active";
                active.OverrideOnClickListeners(BlankVoid);

                Prefabs2.Add(LayersPrefab);
            }

            if (!LayerHeaderPrefab)
            {
                LayerHeaderPrefab = UObject.Instantiate(__instance.categoryHeaderEditRoleOrigin, null).DontUnload().DontDestroy();
                LayerHeaderPrefab.name = "CustomHeaderOptionLayerPrefab";

                Prefabs2.Add(LayerHeaderPrefab);
            }

            if (!LayersSet)
            {
                foreach (var mono in Prefabs2)
                {
                    mono.GetComponentsInChildren<SpriteRenderer>(true).ForEach(x => x.material.SetInt(PlayerMaterial.MaskLayer, 20));

                    foreach (var obj in mono.GetComponentsInChildren<TextMeshPro>(true))
                    {
                        obj.fontMaterial.SetFloat("_StencilComp", 3f);
                        obj.fontMaterial.SetFloat("_Stencil", 20);
                    }

                    mono.gameObject.SetActive(false);
                }

                LayersSet = true;
            }
        }
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Initialize))]
    public static class GameOptionsMenu_Initialize
    {
        public static bool Prefix(GameOptionsMenu __instance)
        {
            if (IsHnS)
                return true;

            __instance.Children = new();
            __instance.MapPicker.Initialize(20);
            __instance.MapPicker.SetUpFromData(GameManager.Instance.GameSettingsList.MapNameSetting, 20);
            __instance.Children.Add(__instance.MapPicker);

            // TODO: Make a better fix for this for example caching the options or creating it ourself.
            // AD Says: Done, kinda.
            var allOptions = CreateOptions();
            var (customOptions, behaviours) = (allOptions.Keys.ToList(), allOptions.Values.ToList());
            var y = 0.713f;

            for (var i = 0; i < allOptions.Count; i++)
            {
                var isHeader = customOptions[i] is HeaderAttribute;
                behaviours[i].transform.localPosition = new(isHeader ? -0.903f : 0.952f, y, -2f);
                behaviours[i].gameObject.SetActive(true);

                if (behaviours[i] is OptionBehaviour option)
                {
                    option.SetClickMask(__instance.ButtonClickMask);
                    __instance.Children.Add(option);
                }

                y -= isHeader ? 0.63f : 0.45f;
            }

            __instance.scrollBar.SetYBoundsMax(-1.65f - y);
            __instance.ControllerSelectable.AddRange(new(__instance.scrollBar.GetComponentsInChildren<UiElement>().Pointer));
            __instance.InitializeControllerNavigation();
            OnValueChanged();
            return false;
        }
    }

    public static void ToggleTabs(RolesSettingsMenu __instance, int pos)
    {
        if (pos == SettingsPage)
            return;

        var previous = SettingsPage;
        SettingsPage = pos;

        if (__instance)
        {
            var color = GetSettingColor(pos);
            __instance.quotaHeader.Background.color = color.Shadow();
            __instance.roleTabs[pos - 1].SelectButton(true);

            if (previous > 0)
                __instance.roleTabs[previous - 1].SelectButton(false);

            __instance.scrollBar.ScrollToTop();
            __instance.quotaHeader.gameObject.SetActive(false);

            if (CreatedPages.Contains(pos))
                return;

            foreach (var option in OptionAttribute.AllOptions)
            {
                if (option.Setting)
                    option.Setting.gameObject.SetActive(false);
            }

            var allOptions = CreateOptions(__instance.RoleChancesSettings.transform);
            OnValueChanged();
            var (customOptions, behaviours) = (allOptions.Keys.ToList(), allOptions.Values.ToList());
            var y = 0.61f;

            for (var i = 0; i < allOptions.Count; i++)
            {
                var isHeader = customOptions[i] is HeaderOptionAttribute;
                var header = isHeader ? (HeaderOptionAttribute)customOptions[i] : null;
                var isGen = header?.HeaderType == HeaderType.General;
                var isLayer = customOptions[i] is LayersOptionAttribute;

                if (i > 0)
                    y -= isHeader ? (isGen ? 0.63f : 0.6f) : (isLayer ? 0.47f : 0.45f);

                behaviours[i].transform.localPosition = new(isLayer ? -0.15f : (isHeader ? (isGen ? -0.623f : 4.986f) : 1.232f), y, -2f);
                behaviours[i].gameObject.SetActive(true);

                if (behaviours[i] is OptionBehaviour option)
                    option.SetClickMask(__instance.ButtonClickMask);
            }

            __instance.scrollBar.SetYBoundsMax(-1.65f - y);
            __instance.ControllerSelectable.AddRange(new(__instance.scrollBar.GetComponentsInChildren<UiElement>().Pointer));
            __instance.InitializeControllerNavigation();
            CreatedPages.Add(pos);
        }
    }

    public static UColor GetSettingColor(int index) => index switch
    {
        1 => CustomColorManager.Crew,
        2 => CustomColorManager.Neutral,
        3 => CustomColorManager.Intruder,
        4 => CustomColorManager.Syndicate,
        5 => CustomColorManager.Modifier,
        6 => CustomColorManager.Objectifier,
        7 => CustomColorManager.Abilities,
        8 => CustomColorManager.RoleList,
        _ => UColor.white
    };

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.SetQuotaTab))]
    public static class RolesSettingsMenu_SetQuotaTab
    {
        public static bool Prefix(RolesSettingsMenu __instance)
        {
            // var num = 0.662f;
            var tabXPos = -2.69f;
            __instance.roleTabs = new();
            __instance.AllButton.gameObject.SetActive(false);

            for (var i = 1; i < 9; i++)
            {
                var tab = UObject.Instantiate(i % 2 == 0 ? __instance.roleSettingsTabButtonOrigin : __instance.roleSettingsTabButtonOriginImpostor, __instance.tabParent);
                tab.transform.localPosition = new(tabXPos, 2.27f, -2f);
                var cache = i;
                tab.icon.sprite =  GameManager.Instance.GameSettingsList.AllRoles.ToSystem().Find(cat => cat.Role.Role == Icons(i)).Role.RoleIconWhite;
                tab.icon.color = GetSettingColor(i).Light();
                tab.button.OverrideOnClickListeners(() => ToggleTabs(__instance, cache));
                tabXPos += 0.762f;
                __instance.roleTabs.Add(tab.Button);
            }

            ToggleTabs(__instance, SettingsPage);
            return false;
        }

        private static RoleTypes Icons(int index) => index switch
        {
            1 => RoleTypes.GuardianAngel,
            2 => RoleTypes.Phantom,
            3 => RoleTypes.Shapeshifter,
            4 => RoleTypes.Tracker,
            5 or 6 or 7 => RoleTypes.Noisemaker,
            8 => RoleTypes.Engineer,
            _ => RoleTypes.Crewmate
        };
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Update))]
    public static class FixIssue
    {
        public static bool Prefix(GameSettingMenu __instance)
        {
            if (Controller.currentTouchType != 0)
            {
                __instance.ToggleLeftSideDarkener(false);
                __instance.ToggleRightSideDarkener(false);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.ValueChanged))]
    public static class DisableCustomNotify1
    {
        public static bool Prefix(ref OptionBehaviour option)
        {
            var optionn = option;
            return OptionAttribute.AllOptions.Any(x => x.Setting == optionn);
        }
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.ValueChanged))]
    public static class DisableCustomNotify2
    {
        public static bool Prefix(ref OptionBehaviour obj)
        {
            var optionn = obj;
            return OptionAttribute.AllOptions.Any(x => x.Setting == optionn);
        }
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.OpenChancesTab))]
    public static class RandomNullRefBOOOOOOOO
    {
        public static bool Prefix() => false;
    }

    public static void OnValueChanged()
    {
        if (IsHnS || !GameSettingMenu.Instance)
            return;

        if (SettingsPage is 0 or 9 or 10 or 12)
        {
            var y = 0.713f;
            GameSettingMenu.Instance.GameSettingsTab.Children = new();
            GameSettingMenu.Instance.GameSettingsTab.Children.Add(GameSettingMenu.Instance.GameSettingsTab.MapPicker);

            if (SettingsPage == 0)
            {
                SaveSettings.Setting.gameObject.SetActive(true);
                SaveSettings.Setting.transform.localPosition = new(0.952f, y, -2f);
                y -= 0.45f;


                PresetButton.Setting.gameObject.SetActive(true);
                PresetButton.Setting.transform.localPosition = new(0.952f, y, -2f);
                y -= 0.45f;
            }

            foreach (var option in OptionAttribute.AllOptions)
            {
                if (option != null && option.Setting && option.Setting.gameObject)
                {
                    if (option.Menu != (MultiMenu)SettingsPage || !option.Active())
                    {
                        option.Setting.gameObject.SetActive(false);
                        continue;
                    }

                    var isHeader = option is HeaderOptionAttribute;
                    option.Setting.gameObject.SetActive(true);
                    option.Setting.transform.localPosition = new(isHeader ? -0.903f : 0.952f, y, -2f);
                    y -= isHeader ? 0.63f : 0.45f;

                    if (option.Setting is OptionBehaviour setting)
                        GameSettingMenu.Instance.GameSettingsTab.Children.Add(setting);
                }
            }

            GameSettingMenu.Instance.GameSettingsTab.scrollBar.SetYBoundsMax(-1.65f - y);
            GameSettingMenu.Instance.GameSettingsTab.ControllerSelectable.AddRange(new(GameSettingMenu.Instance.GameSettingsTab.scrollBar.GetComponentsInChildren<UiElement>().Pointer));
            GameSettingMenu.Instance.GameSettingsTab.InitializeControllerNavigation();
        }
        else
        {
            var y = 0.61f;
            GameSettingMenu.Instance.RoleSettingsTab.quotaHeader.gameObject.SetActive(false);
            // GameSettingMenu.Instance.RoleSettingsTab.quotaHeader.Title.text = $"{Menus[SettingsPage]} Settings";
            // GameSettingMenu.Instance.RoleSettingsTab.quotaHeader.Title.color = GameSettingMenu.Instance.RoleSettingsTab.quotaHeader.Background.color.Light().Light();

            for (var i = 0; i < OptionAttribute.AllOptions.Count; i++)
            {
                var option = OptionAttribute.AllOptions[i];

                if (option != null && option.Setting && option.Setting.gameObject)
                {
                    var isLayer = option is LayersOptionAttribute;

                    if (isLayer)
                        ((LayersOptionAttribute)option).UpdateParts();

                    if (option.Menu != (MultiMenu)SettingsPage || !option.Active())
                    {
                        option.Setting.gameObject.SetActive(false);
                        continue;
                    }

                    var isHeader = option is HeaderOptionAttribute;
                    var header = isHeader ? (HeaderOptionAttribute)option : null;
                    var isGen = header?.HeaderType == HeaderType.General;

                    if (i > 0)
                        y -= isHeader ? (isGen ? 0.63f : 0.6f) : (isLayer ? 0.47f : 0.45f);

                    option.Setting.gameObject.SetActive(true);
                    option.Setting.transform.localPosition = new(isLayer ? -0.15f : (isHeader ? (isGen ? -0.623f : 4.986f) : 1.232f), y, -2f);
                }
            }

            GameSettingMenu.Instance.RoleSettingsTab.scrollBar.SetYBoundsMax(-1.65f - y);
            GameSettingMenu.Instance.RoleSettingsTab.ControllerSelectable.AddRange(new(GameSettingMenu.Instance.RoleSettingsTab.scrollBar.GetComponentsInChildren<UiElement>().Pointer));
            GameSettingMenu.Instance.RoleSettingsTab.InitializeControllerNavigation();
        }
    }

    // private static float Timer;

    // [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Update))]
    // public static class RolesSettingsMenu_Update
    // {
    //     public static bool Prefix(RolesSettingsMenu __instance)
    //     {
    //         if (IsHnS || SettingsPage is 0 or 9 or 10 or 12)
    //             return true;

    //         Timer += Time.deltaTime;

    //         if (Timer < 0.1f)
    //             return false;

    //         Timer = 0f;
    //         var y = 0.65f;
    //         __instance.quotaHeader.Title.text = $"{Menus[SettingsPage]} Settings";
    //         __instance.quotaHeader.Title.color = __instance.quotaHeader.Background.color.Light().Light();

    //         foreach (var option in OptionAttribute.AllOptions)
    //         {
    //             if (option.Setting && option.Setting.gameObject && option is LayersOptionAttribute layer)
    //                 layer.UpdateParts();
    //         }

    //         return false;
    //     }
    // }

    // [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
    // public static class GameOptionsMenu_Update
    // {
    //     public static bool Prefix(GameOptionsMenu __instance)
    //     {
    //         if (IsHnS || SettingsPage is not (0 or 9 or 10 or 12))
    //             return true;

    //         Timer += Time.deltaTime;

    //         if (Timer < 0.1f)
    //             return false;

    //         Timer = 0f;
    //         var y = 0.713f;
    //         __instance.Children = new();
    //         __instance.Children.Add(__instance.MapPicker);

    //         if (SettingsPage == 0)
    //         {
    //             SaveSettings.Setting.gameObject.SetActive(true);
    //             SaveSettings.Setting.transform.localPosition = new(0.952f, y, -2f);
    //             y -= 0.45f;


    //             PresetButton.Setting.gameObject.SetActive(true);
    //             PresetButton.Setting.transform.localPosition = new(0.952f, y, -2f);
    //             y -= 0.45f;
    //         }

    //         foreach (var option in OptionAttribute.AllOptions)
    //         {
    //             if (option != null && option.Setting && option.Setting.gameObject)
    //             {
    //                 if (option.Menu != (MultiMenu)SettingsPage || !option.Active())
    //                 {
    //                     option.Setting.gameObject.SetActive(false);
    //                     continue;
    //                 }

    //                 var isHeader = option is HeaderOptionAttribute;
    //                 option.Setting.gameObject.SetActive(true);
    //                 option.Setting.transform.localPosition = new(isHeader ? -0.903f : 0.952f, y, -2f);
    //                 y -= isHeader ? 0.63f : 0.45f;

    //                 if (option.Setting is OptionBehaviour setting)
    //                     __instance.Children.Add(setting);
    //             }
    //         }

    //         __instance.scrollBar.SetYBoundsMax(-1.65f - y);
    //         __instance.ControllerSelectable.AddRange(new(__instance.scrollBar.GetComponentsInChildren<UiElement>().Pointer));
    //         __instance.InitializeControllerNavigation();
    //         return false;
    //     }
    // }

    private static bool Initialize(OptionBehaviour opt)
    {
        if (opt == PresetButton.Setting)
        {
            PresetButton.OptionCreated();
            return false;
        }

        if (opt == SaveSettings.Setting)
        {
            SaveSettings.OptionCreated();
            return false;
        }

        var customOption = (OptionAttribute.AllOptions.Find(option => option.Setting == opt) as IOption
            ?? PresetButton.SlotButtons.Find(option => option.Setting == opt))
            ?? RoleListEntryAttribute.EntryButtons.Find(option => option.Setting == opt);

        if (customOption == null)
            return true;

        customOption.OptionCreated();
        return false;
    }

    [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Initialize))]
    public static class ToggleOption_Initialize
    {
        public static bool Prefix(ToggleOption __instance) => Initialize(__instance);
    }

    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Initialize))]
    public static class NumberOption_Initialize
    {
        public static bool Prefix(NumberOption __instance) => Initialize(__instance);
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Initialize))]
    public static class StringOption_Initialize
    {
        public static bool Prefix(StringOption __instance) => Initialize(__instance);
    }

    [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.UpdateValuesAndText))]
    public static class RoleOptionSetting_UpdateValuesAndText
    {
        public static bool Prefix(RoleOptionSetting __instance) => Initialize(__instance);
    }

    [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Toggle))]
    public static class ToggleButtonPatch
    {
        public static bool Prefix(ToggleOption __instance)
        {
            if (!AmongUsClient.Instance.AmHost)
                return false;

            var option = OptionAttribute.AllOptions.Find(option => option.Setting == __instance);

            if (option is ToggleOptionAttribute toggle)
            {
                toggle.Toggle();
                return false;
            }

            if (option is RoleListEntryAttribute role)
            {
                role.ToDo();
                return false;
            }

            if (__instance == PresetButton.Setting)
            {
                PresetButton.Do();
                return false;
            }

            if (__instance == SaveSettings.Setting)
            {
                SaveSettings.Do();
                return false;
            }

            var option1 = PresetButton.SlotButtons.Find(option => option.Setting == __instance);

            if (option1 is ButtonOption button)
            {
                button.Do();
                return false;
            }

            var option2 = RoleListEntryAttribute.EntryButtons.Find(option => option.Setting == __instance);

            if (option2 is ButtonOption button1)
            {
                button1.Do();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
    public static class NumberOptionPatchIncrease
    {
        public static bool Prefix(NumberOption __instance)
        {
            var option = OptionAttribute.AllOptions.Find(option => option.Setting == __instance);

            if (option is NumberOptionAttribute number)
            {
                number.Increase();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
    public static class NumberOptionPatchDecrease
    {
        public static bool Prefix(NumberOption __instance)
        {
            var option = OptionAttribute.AllOptions.Find(option => option.Setting == __instance);

            if (option is NumberOptionAttribute number)
            {
                number.Decrease();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
    public static class KeyValueOptionPatchIncrease
    {
        public static bool Prefix(StringOption __instance)
        {
            var option = OptionAttribute.AllOptions.Find(option => option.Setting == __instance);

            if (option is StringOptionAttribute str)
            {
                str.Increase();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
    public static class KeyValueOptionOptionPatchDecrease
    {
        public static bool Prefix(StringOption __instance)
        {
            var option = OptionAttribute.AllOptions.Find(option => option.Setting == __instance);

            if (option is StringOptionAttribute str)
            {
                str.Decrease();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.IncreaseChance))]
    public static class RoleOptionOptionPatchIncreaseChance
    {
        public static bool Prefix(RoleOptionSetting __instance)
        {
            var option = OptionAttribute.AllOptions.Find(option => option.Setting == __instance);

            if (option is LayersOptionAttribute layer)
            {
                layer.IncreaseChance();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.DecreaseChance))]
    public static class RoleOptionOptionPatchDecreaseChance
    {
        public static bool Prefix(RoleOptionSetting __instance)
        {
            var option = OptionAttribute.AllOptions.Find(option => option.Setting == __instance);

            if (option is LayersOptionAttribute layer)
            {
                layer.DecreaseChance();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.IncreaseCount))]
    public static class RoleOptionOptionPatchIncreaseCount
    {
        public static bool Prefix(RoleOptionSetting __instance)
        {
            var option = OptionAttribute.AllOptions.Find(option => option.Setting == __instance);

            if (option is LayersOptionAttribute layer)
            {
                layer.IncreaseCount();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.DecreaseCount))]
    public static class RoleOptionOptionPatchDecreaseCount
    {
        public static bool Prefix(RoleOptionSetting __instance)
        {
            var option = OptionAttribute.AllOptions.Find(option => option.Setting == __instance);

            if (option is LayersOptionAttribute layer)
            {
                layer.DecreaseCount();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
    public static class PlayerJoinPatch
    {
        private static bool SentOnce;

        public static void Postfix(PlayerPhysics __instance)
        {
            if (!AmongUsClient.Instance || !CustomPlayer.Local || !__instance.myPlayer)
                return;

            if (__instance.myPlayer == CustomPlayer.Local)
            {
                if (!SentOnce)
                {
                    Run("<color=#5411F8FF>人 Welcome! 人</color>", "Welcome to Town Of Us Reworked! Type /help to get started!");
                    SentOnce = true;
                }

                return;
            }

            if (CustomPlayer.AllPlayers.Count <= 1 || !AmongUsClient.Instance.AmHost || TownOfUsReworked.MCIActive)
                return;

            SendOptionRPC(setting: (OptionAttribute)null);
            CallRpc(CustomRPC.Misc, MiscRPC.SyncSummary, ReadDiskText("Summary", TownOfUsReworked.Other));

            if (CachedFirstDead != null)
                CallRpc(CustomRPC.Misc, MiscRPC.SetFirstKilled, CachedFirstDead);
        }
    }

    [HarmonyPatch(typeof(GameOptionsMapPicker), nameof(GameOptionsMapPicker.Initialize))]
    public static class GameOptionsMapPickerInitializePatch
    {
        public static void Postfix(GameOptionsMapPicker __instance, ref int maskLayer)
        {
            if (!__instance.AllMapIcons.Any(x => x.Name == MapNames.Dleks))
            {
                __instance.AllMapIcons.Add(new()
                {
                    Name = MapNames.Dleks,
                    MapImage = GetSprite("DleksBackground"),
                    MapIcon = GetSprite("DleksMapIcon"),
                    NameImage = GetSprite("Dleks")
                });
            }

            if (!__instance.AllMapIcons.Any(x => x.Name == (MapNames)8))
            {
                __instance.AllMapIcons.Add(new()
                {
                    Name = (MapNames)8,
                    MapImage = GetSprite("RandomMapBackground"),
                    MapIcon = GetSprite("RandomMapIcon"),
                    NameImage = GetSprite("Random")
                });
            }

            __instance.mapButtons.ForEach(x => x.gameObject.Destroy());
            __instance.mapButtons.Clear();
            __instance.transform.GetChild(1).localPosition = new(-1.134f, 0.733f, -1);

            for (var k = 0; k < __instance.AllMapIcons.Count; k++)
            {
                var thisVal = __instance.AllMapIcons[k];
                var mapButton = UObject.Instantiate(__instance.MapButtonOrigin, Vector3.zero, Quaternion.identity, __instance.transform);
                mapButton.SetImage(thisVal.MapIcon, maskLayer);
                mapButton.transform.localPosition = new(__instance.StartPosX + (k * __instance.SpacingX) - 0.7f, 0.74f, -2f);
                mapButton.Button.ClickMask = __instance.ButtonClickMask;
                mapButton.Button.OverrideOnClickListeners(() =>
                {
                    __instance?.selectedButton?.Button?.SelectButton(false);
                    __instance.selectedButton = mapButton;
                    __instance.selectedButton.Button.SelectButton(true);
                    __instance.SelectMap(thisVal);
                });

                if (k > 0)
                {
                    mapButton.Button.ControllerNav.selectOnLeft = __instance.mapButtons[k - 1].Button;
                    __instance.mapButtons[k - 1].Button.ControllerNav.selectOnRight = mapButton.Button;
                }

                __instance.mapButtons.Add(mapButton);

                if (thisVal.Name == (MapNames)__instance.selectedMapId)
                {
                    mapButton.Button.SelectButton(true);
                    __instance.SelectMap(__instance.selectedMapId);
                    __instance.selectedButton = mapButton;
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameOptionsMapPicker), nameof(GameOptionsMapPicker.SelectMap), typeof(int))]
    public static class GameOptionsMapPickerSelectMapPatch1
    {
        public static void Postfix(ref int mapId) => SetMap(mapId);
    }

    [HarmonyPatch(typeof(GameOptionsMapPicker), nameof(GameOptionsMapPicker.SelectMap), typeof(MapIconByName))]
    public static class GameOptionsMapPickerSelectMapPatch2
    {
        public static void Postfix(ref MapIconByName mapInfo) => SetMap((int)mapInfo.Name);
    }

    private static readonly string[] Maps = [ "The Skeld", "Mira HQ", "Polus", "ehT dlekS", "Airship", "Fungle", "Submerged", "Level Impostor", "Random" ];
    private static LobbyNotificationMessage MapChangeNotif;

    private static void SetMap(int mapId)
    {
        CustomGameOptions2.Map = (MapEnum)mapId;

        var changed = $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">Game Map</font> set to <font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{Maps[mapId]}</font>";

        if (MapChangeNotif != null)
            MapChangeNotif.UpdateMessage(changed);
        else
        {
            MapChangeNotif = UObject.Instantiate(HUD.Notifier.notificationMessageOrigin, Vector3.zero, Quaternion.identity, HUD.Notifier.transform);
            MapChangeNotif.transform.localPosition = new(0f, 0f, -2f);
            MapChangeNotif.SetUp(changed, HUD.Notifier.settingsChangeSprite, HUD.Notifier.settingsChangeColor, (Action)(() => HUD.Notifier.OnMessageDestroy(MapChangeNotif)));
            HUD.Notifier.ShiftMessages();
            HUD.Notifier.AddMessageToQueue(MapChangeNotif);
        }
    }

    [HarmonyPatch(typeof(NotificationPopper), (nameof(NotificationPopper.AddSettingsChangeMessage)))]
    public static class DisableVanilaNotifs
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.DrawNormalTab))]
    public static class OverrideNormalViewSettingsTab
    {
        public static bool Prefix(LobbyViewSettingsPane __instance)
        {
            var num = 1.44f;
            var num2 = -8.95f;

            foreach (var header in OptionAttribute.AllOptions.Where(x => x is HeaderOptionAttribute && x.Menu == MultiMenu.Main).Cast<HeaderOptionAttribute>())
            {
                var categoryHeaderMasked = UObject.Instantiate(__instance.categoryHeaderOrigin, __instance.settingsContainer);
                categoryHeaderMasked.transform.localScale = Vector3.one;
                categoryHeaderMasked.transform.localPosition = new(-9.77f, num, -2f);
                header.ViewSetting = categoryHeaderMasked;
                header.ViewOptionCreated();
                __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
                num -= 0.85f;

                for (var i = 0; i < header.GroupMembers.Length; i++)
                {
                    var option = header.GroupMembers[i];
                    var viewSettingsInfoPanel = UObject.Instantiate(__instance.infoPanelOrigin, __instance.settingsContainer);
                    viewSettingsInfoPanel.transform.localScale = Vector3.one;

                    if (i % 2 == 0)
                    {
                        num2 = -8.95f;

                        if (i > 0)
                            num -= 0.59f;
                    }
                    else
                        num2 = -3f;

                    viewSettingsInfoPanel.transform.localPosition = new(num2, num, -2f);
                    option.ViewSetting = viewSettingsInfoPanel;
                    option.ViewOptionCreated();
                    __instance.settingsInfo.Add(viewSettingsInfoPanel.gameObject);
                }

                num -= 0.59f;
            }

            __instance.scrollBar.SetYBoundsMax(-num);
            return false;
        }
    }
}