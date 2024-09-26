namespace TownOfUsReworked.Options;

public static class SettingsPatches
{
    public static int SettingsPage;
    public static string CurrentPreset = "Custom";
    public static int SettingsPage2;
    public static int CachedPage;
    public static int SettingsPage3;

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    public static class OptionsMenuBehaviour_Start
    {
        public static void Postfix(GameSettingMenu __instance)
        {
            __instance.transform.GetChild(3).gameObject.SetActive(false);
            __instance.transform.GetChild(7).gameObject.Destroy();
            ReturnButton = UObject.Instantiate(__instance.transform.FindChild("CloseButton").gameObject, __instance.transform);
            ReturnButton.name = "ReturnButton";
            ReturnButton.GetComponent<PassiveButton>().OverrideOnClickListeners(Return);
            ReturnButton.GetComponent<CloseButtonConsoleBehaviour>().Destroy();
            ReturnButton.transform.localPosition += new Vector3(-6.7f, 0f, 0f);
            ReturnButton.transform.FindChild("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite("ReturnInactive");
            ReturnButton.transform.FindChild("Active").GetComponent<SpriteRenderer>().sprite = GetSprite("ReturnActive");
            ReturnButton.SetActive(false);

            var pos1 = __instance.GamePresetsButton.transform.localPosition;
            var pos2 = __instance.GameSettingsButton.transform.localPosition;
            var pos3 = __instance.RoleSettingsButton.transform.localPosition;

            __instance.GameSettingsButton.transform.localPosition = pos1;
            __instance.RoleSettingsButton.transform.localPosition = pos2;
            __instance.GamePresetsButton.transform.localPosition = pos3;

            __instance.ChangeTab(1, false);

            __instance.RoleSettingsButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            __instance.RoleSettingsButton.buttonText.text = TranslationManager.Translate("GameSettings.Layers");
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
    public static class OnChangingTabs
    {
        public static void Postfix(GameSettingMenu __instance, int tabNum, bool previewOnly)
        {
            if (previewOnly)
                return;

            SettingsPage = tabNum switch
            {
                0 => 2,
                2 => 1,
                _ => 0
            };
            OnValueChanged(__instance);
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Close))]
    public static class OptionsMenuBehaviour_Close
    {
        public static void Prefix()
        {
            if (SettingsPage is 3 or 4 or 5)
                SettingsPage = 0;

            LobbyConsole.ClientOptionsActive = false;
            SpawnOptionsCreated = false;
            PresetsButtons.Clear();
            LayerOptionsCreated.Keys.ForEach(x => LayerOptionsCreated[x] = false);
        }
    }

    public static RoleOptionSetting LayersPrefab;
    public static NumberOption NumberPrefab;
    public static ToggleOption TogglePrefab;
    public static StringOption StringPrefab;
    public static CategoryHeaderMasked HeaderPrefab;
    public static CategoryHeaderEditRole AlignmentPrefab;
    private static readonly List<MonoBehaviour> Prefabs1 = [];
    private static readonly List<MonoBehaviour> Prefabs2 = [];

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Awake))]
    public static class DefinePrefabs1
    {
        private static bool LayersSet;

        public static void Postfix(GameOptionsMenu __instance)
        {
            __instance.Children = new();

            if (!NumberPrefab)
            {
                // Background = 0, Value Text = 1, Title = 2, - = 3, + = 4, Value Box = 5
                NumberPrefab = UObject.Instantiate(__instance.numberOptionOrigin, null).DontUnload().DontDestroy();
                NumberPrefab.name = "CustomNumbersOptionPrefab";
                NumberPrefab.transform.GetChild(1).localPosition += new Vector3(1.05f, 0f, 0f);
                NumberPrefab.transform.GetChild(3).localPosition += new Vector3(0.6f, 0f, 0f);
                NumberPrefab.transform.GetChild(4).localPosition += new Vector3(1.5f, 0f, 0f);

                var background = NumberPrefab.transform.GetChild(0);
                background.localPosition += new Vector3(-0.8f, 0f, 0f);
                background.localScale += new Vector3(1f, 0f, 0f);

                var title = NumberPrefab.transform.GetChild(2);
                title.localPosition = new(-2.0466f, 0f, -2.9968f);
                title.GetComponent<RectTransform>().sizeDelta = new(5.8f, 0.458f);

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
                StringPrefab.transform.GetChild(1).localPosition += new Vector3(1.05f, 0f, 0f);

                var background = StringPrefab.transform.GetChild(0);
                background.localPosition += new Vector3(-0.8f, 0f, 0f);
                background.localScale += new Vector3(1f, 0f, 0f);

                var title = StringPrefab.transform.GetChild(2);
                title.localPosition = new(-2.0466f, 0f, -2.9968f);
                title.GetComponent<RectTransform>().sizeDelta = new(5.8f, 0.458f);
                title.GetComponent<TextMeshPro>().fontSize = 2.9f; // Why is it different for string options??

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
                TogglePrefab.transform.GetChild(1).localPosition += new Vector3(2.2f, 0f, 0f);

                var title = TogglePrefab.transform.GetChild(0);
                title.localPosition = new(-2.0466f, 0f, -2.9968f);
                title.GetComponent<RectTransform>().sizeDelta = new(5.8f, 0.458f);

                var background = TogglePrefab.transform.GetChild(2);
                background.localPosition += new Vector3(-0.8f, 0f, 0f);
                background.localScale += new Vector3(1f, 0f, 0f);

                Prefabs1.Add(TogglePrefab);
            }

            if (!HeaderPrefab)
            {
                HeaderPrefab = UObject.Instantiate(__instance.categoryHeaderOrigin, null).DontUnload().DontDestroy();
                HeaderPrefab.name = "CustomHeaderOptionPrefab";
                HeaderPrefab.transform.localScale = new(0.63f, 0.63f, 0.63f);
                HeaderPrefab.Background.transform.localScale += new Vector3(0.7f, 0f, 0f);

                var newButton = UObject.Instantiate(StringPrefab.transform.GetChild(3), HeaderPrefab.transform);
                newButton.localPosition += new Vector3(1.5f, -0.14f, 0f);
                newButton.localScale *= 0.7f;
                newButton.GetComponentInChildren<TextMeshPro>().text = "-";
                newButton.GetComponent<PassiveButton>().OverrideOnClickListeners(BlankVoid);
                newButton.name = "Collapse";

                Prefabs1.Add(HeaderPrefab);
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
            __instance.roleChances = new();
            __instance.roleTabs = new();
            __instance.advancedSettingChildren = new();

            var bg = __instance.transform.FindChild("Scroller").FindChild("MaskBg");
            bg.localPosition += new Vector3(0f, 0.4f, 0f);
            bg.localScale += new Vector3(0f, 0.8f, 0f);

            __instance.ButtonClickMask.transform.localScale += new Vector3(0f, 0.5f, 0f);
            __instance.AllButton.gameObject.SetActive(false);
            __instance.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            __instance.quotaHeader.gameObject.SetActive(false);

            if (!LayersPrefab)
            {
                // Title = 0, Role # = 1, Chance % = 2, Background = 3, Divider = 4, Cog = 5, Unique = 6, Active = 7
                //            ┗-----------┗----------- Value = 0, - = 1, + = 2, Value Box = 3 ┗-----------┗--------- Checkbox = 0
                LayersPrefab = UObject.Instantiate(__instance.roleOptionSettingOrigin, null).DontUnload().DontDestroy();
                LayersPrefab.name = "CustomLayersOptionPrefab";
                LayersPrefab.titleText.alignment = TextAlignmentOptions.Left;
                LayersPrefab.buttons = LayersPrefab.GetComponentsInChildren<PassiveButton>().ToArray();
                LayersPrefab.transform.GetChild(0).localPosition += new Vector3(-0.1f, 0f, 0f);

                var label = LayersPrefab.transform.GetChild(3);
                label.localScale += new Vector3(0.001f, 0f, 0f); // WHY THE FUCK IS THE BACKGROUND EVER SO SLIGHTLY SMALLER THAN THE HEADER?!
                label.localPosition = new(-0.3998f, -0.2953f, 4f);

                var newButton = UObject.Instantiate(LayersPrefab.buttons[0], LayersPrefab.transform);
                newButton.name = "LayersSubSettingsButton";
                newButton.transform.localPosition = new(0.4719f, -0.2982f, -2f);
                newButton.transform.FindChild("Text_TMP").gameObject.Destroy();
                newButton.transform.FindChild("ButtonSprite").GetComponent<SpriteRenderer>().sprite = GetSprite("Cog");
                newButton.OverrideOnClickListeners(BlankVoid);

                var check = __instance.checkboxOrigin.transform.GetChild(1);

                var unique = UObject.Instantiate(check, LayersPrefab.transform);
                unique.name = "Unique";
                unique.GetComponent<PassiveButton>().OverrideOnClickListeners(BlankVoid);
                unique.transform.localScale = new(0.6f, 0.6f, 1f);

                var active = UObject.Instantiate(check, LayersPrefab.transform);
                active.name = "Active";
                active.GetComponent<PassiveButton>().OverrideOnClickListeners(BlankVoid);
                active.transform.localScale = new(0.6f, 0.6f, 1f);

                Prefabs2.Add(LayersPrefab);
            }

            if (!AlignmentPrefab)
            {
                // Header Label = 0, Header Text = 1, Quota Header = 2, Collapse = 3, Cog = 4
                //                                    ┗---------------- Dark Label = 0, Count Text = 1, Left Label = 2, Right Label = 3, Chance Text = 4, Long Label = 5, Active = 6,
                //                                                          Unique = 7
                AlignmentPrefab = UObject.Instantiate(__instance.categoryHeaderEditRoleOrigin, null).DontUnload().DontDestroy();
                AlignmentPrefab.name = "CustomHeaderOptionLayerPrefab";
                AlignmentPrefab.transform.GetChild(0).gameObject.SetActive(false);

                var quota = AlignmentPrefab.transform.GetChild(2);

                var single = UObject.Instantiate(quota.GetChild(3), quota);
                single.localScale += new Vector3(0.5f, 0f, 0f);
                single.localPosition += new Vector3(-0.956f, 0f, 0f);
                single.name = "SingleBG";

                var count = quota.GetChild(1).GetComponent<TextMeshPro>();

                count.GetComponent<TextTranslatorTMP>().Destroy();
                count.text = "# Layer Count";

                var active = UObject.Instantiate(count, quota);
                active.name = "Active";
                active.GetComponent<TextTranslatorTMP>().Destroy();
                active.text = "Is Active?";

                var unique = UObject.Instantiate(count, quota);
                unique.name = "Unique";
                unique.GetComponent<TextTranslatorTMP>().Destroy();
                unique.text = "Is Unique?";

                var newButton = UObject.Instantiate(LayersPrefab.buttons[0], AlignmentPrefab.transform);
                newButton.name = "Collapse";
                newButton.transform.localPosition = new(-5.839f, -0.45f, -2f);
                newButton.GetComponentInChildren<TextMeshPro>().text = "-";
                newButton.OverrideOnClickListeners(BlankVoid);
                newButton.transform.localScale *= 0.7f;

                var newButton2 = UObject.Instantiate(LayersPrefab.transform.GetChild(5), AlignmentPrefab.transform);
                newButton2.name = "AlignmentSubOptions";
                newButton2.transform.FindChild("Text_TMP").gameObject.Destroy();
                newButton2.transform.localPosition = new(-5.239f, -0.45f, -2f);
                newButton2.transform.localScale *= 0.7f;

                Prefabs2.Add(AlignmentPrefab);
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

    private static bool SpawnOptionsCreated;
    private static readonly Dictionary<int, bool> LayerOptionsCreated = [];

    public static List<MonoBehaviour> CreateOptions(Transform parent)
    {
        var options = new List<MonoBehaviour>();
        var type = (MultiMenu)SettingsPage;

        foreach (var option in OptionAttribute.AllOptions.Where(x => x.Menus.Contains(type)))
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

                    case CustomOptionType.Toggle or CustomOptionType.Entry:
                        setting = UObject.Instantiate(TogglePrefab, parent);
                        break;

                    case CustomOptionType.Header:
                        setting = UObject.Instantiate(HeaderPrefab, parent);
                        break;

                    case CustomOptionType.Alignment:
                        setting = UObject.Instantiate(AlignmentPrefab, parent);
                        break;
                }

                option.Setting = setting;
                option.OptionCreated();
            }

            options.Add(option.Setting);
        }

        return options;
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Initialize))]
    public static class GameOptionsMenu_Initialize
    {
        public static bool Prefix(GameOptionsMenu __instance)
        {
            if (IsHnS())
                return true;

            __instance.MapPicker.Initialize(20);
            __instance.MapPicker.SetUpFromData(GameManager.Instance.GameSettingsList.MapNameSetting, 20);

            // TODO: Make a better fix for this for example caching the options or creating it ourself.
            // AD Says: Done, kinda.
            var behaviours = CreateOptions(__instance.MapPicker.transform.parent);

            foreach (var behave in behaviours)
            {
                if (behave is OptionBehaviour option)
                    option.SetClickMask(__instance.ButtonClickMask);
            }

            __instance.ControllerSelectable.AddRange(new(__instance.scrollBar.GetComponentsInChildren<UiElement>().Pointer));
            OnValueChanged();
            return false;
        }
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.SetQuotaTab))]
    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.OpenChancesTab))]
    public static class RolesSettingsMenuPatches
    {
        public static bool Prefix() => false;
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
    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.ValueChanged))]
    public static class DisableCustomNotify
    {
        public static bool Prefix() => false;
    }

    private static PassiveButton RolesButton;
    private static PassiveButton AlignmentsButton;

    public static void OnValueChanged(GameSettingMenu __instance = null)
    {
        if (IsHnS())
            return;

        __instance ??= GameSettingMenu.Instance;

        if (!__instance)
            return;

        __instance.GameSettingsTab.gameObject.SetActive(SettingsPage is 0 or 3);
        __instance.RoleSettingsTab.gameObject.SetActive(SettingsPage is 1 or >= 5);
        __instance.PresetsTab.gameObject.SetActive(SettingsPage == 2);
        __instance.GameSettingsButton.SelectButton(SettingsPage == 0);
        __instance.RoleSettingsButton.SelectButton(SettingsPage == 1);
        __instance.GamePresetsButton.SelectButton(SettingsPage == 2);

        if (!ReturnButton)
            ReturnButton = __instance.transform.FindChild("ReturnButton")?.gameObject; // For some reason this damn thing is becoming null even though it definitely exists???

        if (ReturnButton)
            ReturnButton.SetActive(SettingsPage >= 4);

        if (!SpawnOptionsCreated && SettingsPage == 1)
        {
            var buttons = new List<PassiveButton>();

            RolesButton = UObject.Instantiate(__instance.GamePresetsButton, __instance.RoleSettingsTab.RoleChancesSettings.transform);
            RolesButton.OverrideOnClickListeners(AllLayers);
            RolesButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            RolesButton.buttonText.alignment = TextAlignmentOptions.Center;
            RolesButton.buttonText.text = TranslationManager.Translate("View.AllLayers");
            RolesButton.name = "AllLayers";
            RolesButton.transform.localPosition = new(0.1117f, 1.626f, -2f);
            buttons.Add(RolesButton);

            AlignmentsButton = UObject.Instantiate(__instance.GamePresetsButton, __instance.RoleSettingsTab.RoleChancesSettings.transform);
            AlignmentsButton.OverrideOnClickListeners(AllAlignments);
            AlignmentsButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            AlignmentsButton.buttonText.alignment = TextAlignmentOptions.Center;
            AlignmentsButton.buttonText.text = TranslationManager.Translate("View.AllAlignments");
            AlignmentsButton.name = "AllAlignments";
            AlignmentsButton.transform.localPosition = new(3.4727f, 1.626f, -2f);
            buttons.Add(AlignmentsButton);

            foreach (var mono in buttons)
            {
                mono.GetComponentsInChildren<SpriteRenderer>(true).ForEach(x => x.material.SetInt(PlayerMaterial.MaskLayer, 20));

                foreach (var obj in mono.GetComponentsInChildren<TextMeshPro>(true))
                {
                    obj.fontMaterial.SetFloat("_StencilComp", 3f);
                    obj.fontMaterial.SetFloat("_Stencil", 20);
                }
            }

            var behaviours = CreateOptions(__instance.RoleSettingsTab.RoleChancesSettings.transform);

            foreach (var behave in behaviours)
            {
                if (behave is OptionBehaviour option)
                    option.SetClickMask(__instance.RoleSettingsTab.ButtonClickMask);
            }

            __instance.RoleSettingsTab.ControllerSelectable.AddRange(new(__instance.RoleSettingsTab.scrollBar.GetComponentsInChildren<UiElement>().Pointer));
            __instance.RoleSettingsTab.scrollBar.ScrollToTop();
            SpawnOptionsCreated = true;
        }

        if ((!LayerOptionsCreated.TryGetValue(SettingsPage, out var layerOptions) || !layerOptions) && SettingsPage >= 5)
        {
            var options = CreateOptions(__instance.RoleSettingsTab.RoleChancesSettings.transform);

            foreach (var option in options)
            {
                if (option is OptionBehaviour behave)
                    behave.SetClickMask(__instance.RoleSettingsTab.ButtonClickMask);
            }

            foreach (var elem in __instance.RoleSettingsTab.scrollBar.GetComponentsInChildren<UiElement>())
            {
                if (!__instance.RoleSettingsTab.ControllerSelectable.Contains(elem))
                    __instance.RoleSettingsTab.ControllerSelectable.Add(elem);
            }

            LayerOptionsCreated[SettingsPage] = true;
        }

        if (SettingsPage is 0 or 3)
        {
            var y = 0.863f;
            __instance.GameSettingsTab.Children.Clear();
            __instance.GameSettingsTab.Children.Add(__instance.GameSettingsTab.MapPicker);

            foreach (var option in OptionAttribute.AllOptions)
            {
                if (option.Setting)
                {
                    var menu = (MultiMenu)SettingsPage;
                    var flag = option.Menus.Contains(menu) && option.Active();
                    option.Setting.gameObject.SetActive(flag);

                    if (!flag)
                        continue;

                    var isHeader = option is HeaderOptionAttribute;
                    option.Setting.transform.localPosition = new(isHeader ? -0.903f : 0.952f, y, -2f);
                    y -= isHeader ? 0.53f : 0.45f;
                    option.Update();

                    if (option.Setting is OptionBehaviour setting)
                        __instance.GameSettingsTab.Children.Add(setting);
                }
            }

            __instance.GameSettingsTab.scrollBar.SetYBoundsMax(-1.65f - y);
            __instance.GameSettingsTab.InitializeControllerNavigation();

            if (SettingsPage == 3)
            {
                __instance.GameSettingsButton.gameObject.SetActive(false);
                __instance.RoleSettingsButton.gameObject.SetActive(false);
                __instance.GamePresetsButton.gameObject.SetActive(false);
            }
        }
        else if (SettingsPage is 1 or >= 5)
        {
            var y = SettingsPage >= 5 ? 1.515f : 1.36f;
            RolesButton.gameObject.SetActive(SettingsPage == 1);
            AlignmentsButton.gameObject.SetActive(SettingsPage == 1);
            __instance.RoleSettingsTab.advancedSettingChildren.Clear();

            foreach (var option in OptionAttribute.AllOptions)
            {
                if (option.Setting)
                {
                    var menu = (MultiMenu)SettingsPage;
                    var flag = option.Menus.Contains(menu) && option.Active();
                    option.Setting.gameObject.SetActive(flag);

                    if (!flag)
                        continue;

                    if (SettingsPage >= 5)
                    {
                        var isHeader = option is HeaderOptionAttribute;
                        option.Setting.transform.localPosition = new(isHeader ? -0.903f : 0.952f, y, -2f);
                        y -= isHeader ? 0.53f : 0.45f;
                    }
                    else
                    {
                        var isAlign = option is AlignsOptionAttribute;

                        if (isAlign)
                            y -= 0.1f;

                        option.Setting.transform.localPosition = new(isAlign ? 4.986f : -0.15f, y, -2f);
                        y -= isAlign ? 0.525f : 0.43f;
                    }

                    option.Update();

                    if (option.Setting is OptionBehaviour setting)
                        __instance.RoleSettingsTab.advancedSettingChildren.Add(setting);
                }
            }

            __instance.RoleSettingsTab.scrollBar.SetYBoundsMax(-y + (SettingsPage >= 5 ? -1.65f : -1.2f));
            __instance.RoleSettingsTab.InitializeControllerNavigation();
        }
    }

    public static GameObject ReturnButton;

    private static void Return()
    {
        SettingsPage = CachedPage;
        GameSettingMenu.Instance.RoleSettingsTab.scrollBar.ScrollToTop();
        OnValueChanged();
    }

    private static void AllLayers()
    {
        SettingsPage = 5;
        CachedPage = 1;
        GameSettingMenu.Instance.RoleSettingsTab.scrollBar.ScrollToTop();
        OnValueChanged();
    }

    private static void AllAlignments()
    {
        SettingsPage = 250;
        CachedPage = 1;
        GameSettingMenu.Instance.RoleSettingsTab.scrollBar.ScrollToTop();
        OnValueChanged();
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Update))]
    public static class RolesSettingsMenu_Update
    {
        public static void Postfix(RolesSettingsMenu __instance) => __instance.RoleChancesSettings.transform.localPosition = new(SettingsPage >= 5 ? 0.35f : -0.06f, 0f, -5f);
    }

    private static bool Initialize(OptionBehaviour opt)
    {
        var customOption = OptionAttribute.AllOptions.Find(option => option.Setting == opt);
        customOption?.OptionCreated();
        return customOption == null;
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
            var option = OptionAttribute.AllOptions.Find(option => option.Setting == __instance);

            if (option is ToggleOptionAttribute toggle)
            {
                toggle.Toggle();
                return false;
            }

            if (option is RoleListEntryAttribute roleListEntry)
            {
                roleListEntry.ToDo();
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
            var result = OptionAttribute.GetOptions<NumberOptionAttribute>().TryFinding(option => option.Setting == __instance, out var num);

            if (result)
                num.Increase();

            return !result;
        }
    }

    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
    public static class NumberOptionPatchDecrease
    {
        public static bool Prefix(NumberOption __instance)
        {
            var result = OptionAttribute.GetOptions<NumberOptionAttribute>().TryFinding(option => option.Setting == __instance, out var num);

            if (result)
                num.Decrease();

            return !result;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
    public static class KeyValueOptionPatchIncrease
    {
        public static bool Prefix(StringOption __instance)
        {
            var result = OptionAttribute.GetOptions<StringOptionAttribute>().TryFinding(option => option.Setting == __instance, out var str);

            if (result)
                str.Increase();

            return !result;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
    public static class KeyValueOptionOptionPatchDecrease
    {
        public static bool Prefix(StringOption __instance)
        {
            var result = OptionAttribute.GetOptions<StringOptionAttribute>().TryFinding(option => option.Setting == __instance, out var str);

            if (result)
                str.Decrease();

            return !result;
        }
    }

    [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.IncreaseChance))]
    public static class RoleOptionOptionPatchIncreaseChance
    {
        public static bool Prefix(RoleOptionSetting __instance)
        {
            var result = OptionAttribute.GetOptions<LayersOptionAttribute>().TryFinding(option => option.Setting == __instance, out var layer);

            if (result)
                layer.IncreaseChance();

            return !result;
        }
    }

    [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.DecreaseChance))]
    public static class RoleOptionOptionPatchDecreaseChance
    {
        public static bool Prefix(RoleOptionSetting __instance)
        {
            var result = OptionAttribute.GetOptions<LayersOptionAttribute>().TryFinding(option => option.Setting == __instance, out var layer);

            if (result)
                layer.DecreaseChance();

            return !result;
        }
    }

    [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.IncreaseCount))]
    public static class RoleOptionOptionPatchIncreaseCount
    {
        public static bool Prefix(RoleOptionSetting __instance)
        {
            var result = OptionAttribute.GetOptions<LayersOptionAttribute>().TryFinding(option => option.Setting == __instance, out var layer);

            if (result)
                layer.IncreaseCount();

            return !result;
        }
    }

    [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.DecreaseCount))]
    public static class RoleOptionOptionPatchDecreaseCount
    {
        public static bool Prefix(RoleOptionSetting __instance)
        {
            var result = OptionAttribute.GetOptions<LayersOptionAttribute>().TryFinding(option => option.Setting == __instance, out var layer);

            if (result)
                layer.DecreaseCount();

            return !result;
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

            if (__instance.myPlayer.AmOwner)
            {
                if (!SentOnce)
                {
                    Run("<color=#5411F8FF>人 Welcome! 人</color>", "Welcome to Town Of Us Reworked! Type /help to get started!");
                    SentOnce = true;
                }

                return;
            }

            if (AllPlayers().Count <= 1 || !AmongUsClient.Instance.AmHost || TownOfUsReworked.MCIActive)
                return;

            SendOptionRPC();
            CallRpc(CustomRPC.Misc, MiscRPC.SyncSummary, ReadDiskText("Summary", TownOfUsReworked.Other));

            if (CachedFirstDead != null)
                CallRpc(CustomRPC.Misc, MiscRPC.SetFirstKilled, CachedFirstDead);
        }
    }

    [HarmonyPatch(typeof(GameOptionsMapPicker), nameof(GameOptionsMapPicker.Initialize))]
    public static class GameOptionsMapPickerInitializePatch
    {
        public static void Postfix(GameOptionsMapPicker __instance, int maskLayer)
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
            __instance.selectedMapId = (int)MapSettings.Map;

            for (var k = 0; k < __instance.AllMapIcons.Count; k++)
            {
                var thisVal = __instance.AllMapIcons[k];
                var mapButton = UObject.Instantiate(__instance.MapButtonOrigin, Vector3.zero, Quaternion.identity, __instance.transform);
                mapButton.SetImage(thisVal.MapIcon, maskLayer);
                mapButton.transform.localPosition = new(__instance.StartPosX + (k * __instance.SpacingX) - 0.7f, 0.74f, -2f);
                mapButton.name = $"{__instance.AllMapIcons[k].Name}";
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

                if (thisVal.Name == (MapNames)MapSettings.Map)
                {
                    mapButton.Button.SelectButton(true);
                    __instance.SelectMap((int)MapSettings.Map);
                    __instance.selectedButton = mapButton;
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameOptionsMapPicker), nameof(GameOptionsMapPicker.SelectMap), typeof(int))]
    public static class GameOptionsMapPickerSelectMapPatch1
    {
        public static void Postfix(int mapId) => SetMap(mapId);
    }

    [HarmonyPatch(typeof(GameOptionsMapPicker), nameof(GameOptionsMapPicker.SelectMap), typeof(MapIconByName))]
    public static class GameOptionsMapPickerSelectMapPatch2
    {
        public static void Postfix(MapIconByName mapInfo) => SetMap((int)mapInfo.Name);
    }

    private static readonly string[] Maps = [ "The Skeld", "Mira HQ", "Polus", "ehT dlekS", "Airship", "Fungle", "Submerged", "Level Impostor", "Random" ];
    private static LobbyNotificationMessage MapChangeNotif;

    private static void SetMap(int mapId)
    {
        var map = (MapEnum)mapId;

        if (MapSettings.Map == map)
            return;

        MapSettings.Map = map;
        OnValueChanged();

        var changed = $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">Game Map</font> set to <font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{Maps[mapId]}</font>";

        if (MapChangeNotif != null)
            MapChangeNotif.UpdateMessage(changed);
        else
        {
            MapChangeNotif = UObject.Instantiate(HUD().Notifier.notificationMessageOrigin, Vector3.zero, Quaternion.identity, HUD().Notifier.transform);
            MapChangeNotif.transform.localPosition = new(0f, 0f, -2f);
            MapChangeNotif.SetUp(changed, HUD().Notifier.settingsChangeSprite, HUD().Notifier.settingsChangeColor, (Action)(() => HUD().Notifier.OnMessageDestroy(MapChangeNotif)));
            HUD().Notifier.ShiftMessages();
            HUD().Notifier.AddMessageToQueue(MapChangeNotif);
        }
    }

    [HarmonyPatch(typeof(NotificationPopper), (nameof(NotificationPopper.AddSettingsChangeMessage)))]
    public static class DisableVanilaNotifs
    {
        public static bool Prefix() => false;
    }

    public static List<MonoBehaviour> CreateViewOptions(Transform parent)
    {
        var options = new List<MonoBehaviour>();
        var type = (MultiMenu)SettingsPage3;

        foreach (var option in OptionAttribute.AllOptions.Where(x => x.Menus.Contains(type)))
        {
            if (!option.ViewSetting)
            {
                MonoBehaviour setting = null;

                switch (option.Type)
                {
                    case CustomOptionType.Layers:
                        setting = UObject.Instantiate(LayerViewPrefab, parent);
                        break;

                    case CustomOptionType.Alignment or CustomOptionType.Header:
                        setting = UObject.Instantiate(HeaderViewPrefab, parent);
                        break;

                    default:
                        setting = UObject.Instantiate(GenericViewPrefab, parent);
                        break;
                };

                option.ViewSetting = setting;
                option.ViewOptionCreated();
            }

            options.Add(option.ViewSetting);
        }

        return options;
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.DrawNormalTab))]
    // [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.DrawRolesTab))]
    public static class OverrideNormalViewSettingsTab
    {
        public static bool Prefix(LobbyViewSettingsPane __instance)
        {
            SettingsPage3 = 0;
            OnValueChangedView(__instance);
            return false;
        }
    }

    public static void OnValueChangedView(LobbyViewSettingsPane __instance = null)
    {
        if (IsHnS())
            return;

        __instance ??= LobbyInfoPane.Instance.LobbyViewSettingsPane;

        if (!__instance)
            return;

        var num = 1.44f;
        var num2 = -8.95f;
        var menu = (MultiMenu)SettingsPage3;
        CreateViewOptions(__instance.settingsContainer);

        foreach (var option in OptionAttribute.AllOptions)
        {
            if (option is not IOptionGroup header || !option.ViewSetting)
                continue;

            var flag = option.Menus.Contains(menu) && option.Active();
            option.ViewSetting.gameObject.SetActive(flag);

            if (!flag)
            {
                header.GroupMembers.ForEach(x =>
                {
                    if (x.ViewSetting)
                        x.ViewSetting.gameObject.SetActive(false);
                });
                continue;
            }

            option.ViewSetting.transform.localPosition = new(-9.77f, num, -2f);
            __instance.settingsInfo.Add(option.ViewSetting.gameObject);
            num -= 0.85f;
            var members = header.GroupMembers.Where(x =>
            {
                if (!x.ViewSetting)
                    return false;

                var flag2 = x.Menus.Contains(menu) && x.Active();
                x.ViewSetting.gameObject.SetActive(flag2);
                return flag2;
            }).ToArray();
            header.GroupMembers.Except(members).ForEach(x =>
            {
                if (x.ViewSetting)
                    x.ViewSetting.gameObject.SetActive(false);
            });

            for (var i = 0; i < members.Length; i++)
            {
                var optionn = members[i];

                if (i % 2 == 0)
                {
                    num2 = -8.95f;

                    if (i > 0)
                        num -= 0.59f;
                }
                else
                    num2 = -3f;

                optionn.ViewSetting.transform.localPosition = new(num2, num, -2f);
                __instance.settingsInfo.Add(optionn.ViewSetting.gameObject);
            }

            num -= 0.59f;
        }

        __instance.scrollBar.SetYBoundsMax(-num);
    }

    private static CategoryHeaderMasked HeaderViewPrefab;
    private static CategoryHeaderRoleVariant LayerViewPrefab;
    private static ViewSettingsInfoPanel GenericViewPrefab;

    private static PassiveButton ClientOptionsButton;

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Awake))]
    public static class DefinePrefabs3
    {
        public static void Postfix(LobbyViewSettingsPane __instance)
        {
            if (LobbyInfoPane.Instance)
            {
                var child = LobbyInfoPane.Instance.gameObject.transform.GetChild(0).GetChild(6).GetChild(1);
                child.GetComponent<TextTranslatorTMP>()?.Destroy();
                child.GetComponent<TextMeshPro>().text = TranslationManager.Translate($"CustomOption.GameMode.{GameModeSettings.GameMode}");
            }

            __instance.rolesTabButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            __instance.rolesTabButton.buttonText.text = TranslationManager.Translate("GameSettings.Layers");

            if (!ClientOptionsButton)
            {
                ClientOptionsButton = UObject.Instantiate(__instance.rolesTabButton, __instance.rolesTabButton.transform.parent);
                ClientOptionsButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
                ClientOptionsButton.name = "ClientOptionsTab";
                ClientOptionsButton.buttonText.text = TranslationManager.Translate("CustomOption.ClientOptions");
                ClientOptionsButton.transform.localPosition = __instance.rolesTabButton.transform.localPosition + new Vector3(__instance.rolesTabButton.transform.localPosition.x -
                    __instance.taskTabButton.transform.localPosition.x, 0f, 0f);
                ClientOptionsButton.OverrideOnClickListeners(() =>
                {
                    SettingsPage3 = 3;
                    OnValueChangedView(__instance);
                });
            }

            if (!HeaderViewPrefab)
            {
                HeaderViewPrefab = UObject.Instantiate(__instance.categoryHeaderOrigin, null).DontUnload().DontDestroy();
                HeaderViewPrefab.transform.localScale = Vector3.one;
                HeaderViewPrefab.name = "HeaderViewPrefab";
                HeaderViewPrefab.Title.name = "Collapse";
                HeaderViewPrefab.Title.gameObject.AddComponent<PassiveButton>();

                HeaderViewPrefab.gameObject.SetActive(false);
            }

            if (!LayerViewPrefab)
            {
                LayerViewPrefab = UObject.Instantiate(__instance.categoryHeaderRoleOrigin, null).DontUnload().DontDestroy();
                LayerViewPrefab.transform.localScale = Vector3.one;
                LayerViewPrefab.name = "LayerViewPrefab";
                LayerViewPrefab.gameObject.SetActive(false);
            }

            if (!GenericViewPrefab)
            {
                GenericViewPrefab = UObject.Instantiate(__instance.infoPanelOrigin, null).DontUnload().DontDestroy();
                GenericViewPrefab.transform.localScale = Vector3.one;
                GenericViewPrefab.name = "GenericViewPrefab";
                GenericViewPrefab.gameObject.SetActive(false);
            }
        }
    }

    public static readonly List<PassiveButton> PresetsButtons = [];
    public static PassiveButton Prev;
    public static PassiveButton Next;
    public static PassiveButton Save;

    [HarmonyPatch(typeof(GamePresetsTab), nameof(GamePresetsTab.Start))]
    public static class GamePresetsStart
    {
        public static bool Prefix(GamePresetsTab __instance)
        {
            __instance.StandardRulesSprites.ForEach(x => x.gameObject.SetActive(false));
            __instance.AlternateRulesSprites.ForEach(x => x.gameObject.SetActive(false));
            __instance.SpritesToDesaturate.ForEach(x => x.gameObject.SetActive(false));
            __instance.StandardPresetButton.gameObject.SetActive(false);
            __instance.StandardRulesText.gameObject.SetActive(false);
            __instance.AlternateRulesText.gameObject.SetActive(false);
            __instance.SecondPresetButton.gameObject.SetActive(false);
            __instance.PresetDescriptionText.gameObject.SetActive(false);

            Save = UObject.Instantiate(GameSettingMenu.Instance.GamePresetsButton, __instance.StandardPresetButton.transform.parent);
            Save.OverrideOnClickListeners(OptionAttribute.SaveSettings);
            Save.name = "SaveSettingsButton";
            Save.transform.localPosition = new(0.26345f, 2.6164f, -2f);
            Save.transform.localScale = new(0.64f, 0.84f, 1f);
            Save.buttonText.transform.localPosition = new(0.0115f, 0.0208f, -1f);
            Save.buttonText.transform.localScale = new(1.4f, 0.9f, 1f);
            Save.buttonText.alignment = TextAlignmentOptions.Center;
            Save.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            Save.buttonText.text = TranslationManager.Translate("ImportExport.Save");

            Prev = UObject.Instantiate(GameSettingMenu.Instance.GamePresetsButton, __instance.StandardPresetButton.transform.parent);
            Prev.OverrideOnClickListeners(() => NextPage(false));
            Prev.name = "PreviousPageButton";
            Prev.transform.localPosition = new(-2.2875f, 2.6164f, -2f);
            Prev.transform.localScale = new(0.64f, 0.84f, 1f);
            Prev.buttonText.transform.localPosition = new(0.0115f, 0.0208f, -1f);
            Prev.buttonText.transform.localScale = new(1.4f, 0.9f, 1f);
            Prev.buttonText.alignment = TextAlignmentOptions.Center;
            Prev.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            Prev.buttonText.text = TranslationManager.Translate("ImportExport.Previous");

            Next = UObject.Instantiate(GameSettingMenu.Instance.GamePresetsButton, __instance.StandardPresetButton.transform.parent);
            Next.OverrideOnClickListeners(() => NextPage(true));
            Next.name = "NextPageButton";
            Next.transform.localPosition = new(2.9625f, 2.6164f, -2f);
            Next.transform.localScale = new(0.64f, 0.84f, 1f);
            Next.buttonText.transform.localPosition = new(0.0115f, 0.0208f, -1f);
            Next.buttonText.transform.localScale = new(1.4f, 0.9f, 1f);
            Next.buttonText.alignment = TextAlignmentOptions.Center;
            Next.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            Next.buttonText.text = TranslationManager.Translate("ImportExport.Next");

            var presets = Directory.EnumerateFiles(TownOfUsReworked.Options).Where(x => x.EndsWith(".txt")).OrderBy(x => x).Select(x => x.SanitisePath()).ToList();

            for (var i = 0; i < presets.Count; i++)
            {
                var preset = presets[i];
                var presetButton = UObject.Instantiate(Save, __instance.StandardPresetButton.transform.parent);
                presetButton.transform.localScale = new(0.5f, 0.84f, 1f);
                presetButton.buttonText.transform.localScale = new(1.4f, 0.9f, 1f);
                presetButton.buttonText.alignment = TextAlignmentOptions.Center;
                presetButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
                presetButton.buttonText.text = presetButton.name = preset;
                presetButton.OverrideOnClickListeners(() => OptionAttribute.LoadPreset(preset));

                if (i >= (SettingsPage2 * 25) && i < ((SettingsPage2 + 1) * 25))
                {
                    var relativeIndex = i % 25;
                    var row = relativeIndex / 4;
                    var col = relativeIndex % 4;
                    presetButton.transform.localPosition = new(-2.5731f + (col * 1.8911f), 1.7828f - (row * 0.65136f), -2);
                }
                else
                    presetButton.gameObject.SetActive(false);

                PresetsButtons.Add(presetButton);
            }

            Prev.gameObject.SetActive(PresetsButtons.Count > 25);
            Next.gameObject.SetActive(PresetsButtons.Count > 25);
            return false;
        }
    }

    private static void NextPage(bool increment)
    {
        SettingsPage2 = CycleInt(PresetsButtons.Count / 25, 0, SettingsPage2, increment);

        for (var i = 0; i < PresetsButtons.Count; i++)
        {
            var preset = PresetsButtons[i];

            if (i >= (SettingsPage2 * 25) && i < ((SettingsPage2 + 1) * 25))
            {
                var relativeIndex = i % 25;
                var row = relativeIndex / 4;
                var col = relativeIndex % 4;
                preset.transform.localPosition = new(-2.5731f + (col * 1.8911f), 1.7828f - (row * 0.65136f), -2);
            }
            else
                preset.gameObject.SetActive(false);
        }
    }
}