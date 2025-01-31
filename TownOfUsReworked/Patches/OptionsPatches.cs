namespace TownOfUsReworked.Patches;

[HarmonyPatch]
public static class SettingsPatches
{
    public static int SettingsPage;
    public static int SettingsPage2;
    public static int CachedPage;
    public static int SettingsPage3;
    public static string CurrentPreset = "Custom";
    public static Vector3 ScrollerLocation;
    public static BaseHeaderOptionAttribute SelectedSubOptions;

    private static PassiveButton ClientSettingsButton;

    [HarmonyPatch(typeof(GameSettingMenu))]
    public static class GameSettingMenuPatches
    {
        [HarmonyPatch(nameof(GameSettingMenu.Start))]
        public static void Postfix(GameSettingMenu __instance)
        {
            __instance.GameSettingsTab.HideForOnline = new(0);
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

            Prev = UObject.Instantiate(ReturnButton, __instance.transform);
            Prev.GetComponent<PassiveButton>().OverrideOnClickListeners(() => NextPage(false));
            Prev.name = "PrevPageButton";
            Prev.transform.FindChild("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite("ReturnInactive");
            Prev.transform.FindChild("Active").GetComponent<SpriteRenderer>().sprite = GetSprite("ReturnActive");
            Prev.transform.localPosition = new(-4.473f, 1.0795f, -2f);
            Prev.SetActive(false);

            Next = UObject.Instantiate(ReturnButton, __instance.transform);
            Next.GetComponent<PassiveButton>().OverrideOnClickListeners(() => NextPage(true));
            Next.name = "NextPageButton";
            Next.transform.FindChild("Inactive").GetComponent<SpriteRenderer>().sprite = GetSprite("NextInactive");
            Next.transform.FindChild("Active").GetComponent<SpriteRenderer>().sprite = GetSprite("NextActive");
            Next.transform.localPosition = new(-2.2341f, 1.0795f, -2f);
            Next.SetActive(false);

            var pos2 = __instance.GamePresetsButton.transform.localPosition;
            var pos3 = __instance.GameSettingsButton.transform.localPosition;
            var pos4 = __instance.RoleSettingsButton.transform.localPosition;
            var pos1 = pos2 + new Vector3(0f, Mathf.Abs(pos2.y - pos3.y), 0f);

            __instance.GameSettingsButton.transform.localPosition = pos1;
            __instance.RoleSettingsButton.transform.localPosition = pos2;
            __instance.GamePresetsButton.transform.localPosition = pos3;

            __instance.RoleSettingsButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            __instance.RoleSettingsButton.buttonText.text = TranslationManager.Translate("GameSettings.Layers");

            ClientSettingsButton = UObject.Instantiate(__instance.RoleSettingsButton, __instance.RoleSettingsButton.transform.parent);
            ClientSettingsButton.name = "ClientSettingsButton";
            ClientSettingsButton.buttonText.GetComponent<TextTranslatorTMP>()?.Destroy();
            ClientSettingsButton.buttonText.text = TranslationManager.Translate("CustomOption.ClientOptions");
            ClientSettingsButton.OverrideOnClickListeners(() => __instance.ChangeTab(3, false));
            ClientSettingsButton.OverrideOnMouseOverListeners(() => __instance.ChangeTab(3, true));
            ClientSettingsButton.transform.localPosition = pos4;

            if (!ButtonPrefab)
            {
                ButtonPrefab = UObject.Instantiate(__instance.GamePresetsButton, null).DontUnload().DontDestroy();
                ButtonPrefab.OverrideOnClickListeners(BlankVoid);
                ButtonPrefab.name = "ButtonPrefab";
                ButtonPrefab.transform.localScale = new(0.64f, 0.84f, 1f);
                ButtonPrefab.buttonText.transform.localPosition = new(0.0115f, 0.0208f, -1f);
                ButtonPrefab.buttonText.transform.localScale = new(1.4f, 0.9f, 1f);
                ButtonPrefab.buttonText.alignment = TextAlignmentOptions.Center;
                ButtonPrefab.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
                ButtonPrefab.buttonText.text = "";
                ButtonPrefab.gameObject.SetActive(false);
            }

            __instance.ChangeTab(1, false);
        }

        [HarmonyPatch(nameof(GameSettingMenu.ChangeTab))]
        public static void Postfix(GameSettingMenu __instance, int tabNum, bool previewOnly)
        {
            if (tabNum == 3)
                __instance.GameSettingsTab.gameObject.SetActive(true);

            if (previewOnly)
                return;

            if (tabNum == 3)
            {
                __instance.GameSettingsTab.OpenMenu();
                ClientOptionsButton?.SelectButton(true);
            }

            SettingsPage = tabNum switch
            {
                0 => 2,
                2 => 1,
                3 => 3,
                _ => 0
            };
            OnValueChanged(__instance);
        }

        [HarmonyPatch(nameof(GameSettingMenu.Close))]
        public static void Prefix()
        {
            SettingsPage = 0;
            PresetButtons.Clear();
            ScrollerLocation = default;
            Overwriting = false;
            SelectedSubOptions = null;
        }

        [HarmonyPatch(nameof(GameSettingMenu.Update))]
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

    public static RoleOptionSetting LayersPrefab;
    public static NumberOption NumberPrefab;
    public static ToggleOption TogglePrefab;
    public static StringOption StringPrefab;
    public static StringOption MultiSelectPrefab;
    public static ToggleOption MultiOptionPrefab;
    public static CategoryHeaderMasked HeaderPrefab;
    public static CategoryHeaderEditRole AlignmentPrefab;
    public static PassiveButton ButtonPrefab;

    [HarmonyPatch(typeof(GameOptionsMenu))]
    public static class GameOptionsMenuPatches
    {
        private static bool LayersSet;

        [HarmonyPatch(nameof(GameOptionsMenu.Awake))]
        public static void Postfix(GameOptionsMenu __instance)
        {
            __instance.Children = new();
            var prefabs = new List<MonoBehaviour>();

            if (!NumberPrefab)
            {
                // Background = 0, Value Text = 1, Title = 2, - = 3, + = 4, Value Box = 5
                NumberPrefab = UObject.Instantiate(__instance.numberOptionOrigin, null).DontUnload().DontDestroy();
                NumberPrefab.name = "CustomNumbersOptionPrefab";
                NumberPrefab.transform.GetChild(3).localPosition += new Vector3(0.6f, 0f, 0f);
                NumberPrefab.transform.GetChild(4).localPosition += new Vector3(1.5f, 0f, 0f);
                NumberPrefab.PlusBtn.OverrideOnClickListeners(BlankVoid);
                NumberPrefab.MinusBtn.OverrideOnClickListeners(BlankVoid);

                var background = NumberPrefab.transform.GetChild(0);
                background.localPosition += new Vector3(-0.8f, 0f, 0f);
                background.localScale += new Vector3(1f, 0f, 0f);

                var title = NumberPrefab.TitleText;
                title.transform.localPosition = new(-2.0466f, 0f, -2.9968f);
                title.GetComponent<RectTransform>().sizeDelta = new(5.8f, 0.458f);

                var valueBox = NumberPrefab.transform.GetChild(5);
                valueBox.localPosition += new Vector3(1.05f, 0f, 0f);
                valueBox.localScale += new Vector3(0.2f, 0f, 0f);

                prefabs.Add(NumberPrefab);
            }

            if (!StringPrefab)
            {
                // Background = 0, Value Text = 1, Title = 2, < = 3, > = 4, Value Box = 5
                StringPrefab = UObject.Instantiate(__instance.stringOptionOrigin, null).DontUnload().DontDestroy();
                StringPrefab.name = "CustomStringOptionPrefab";
                StringPrefab.PlusBtn.OverrideOnClickListeners(BlankVoid);
                StringPrefab.MinusBtn.OverrideOnClickListeners(BlankVoid);

                var background = StringPrefab.transform.GetChild(0);
                background.localPosition += new Vector3(-0.8f, 0f, 0f);
                background.localScale += new Vector3(1f, 0f, 0f);

                var title = StringPrefab.TitleText;
                title.transform.localPosition = new(-2.0466f, 0f, -2.9968f);
                title.GetComponent<RectTransform>().sizeDelta = new(5.8f, 0.458f);
                title.fontSize = 2.9f; // Why is it different for string options??

                var minus = StringPrefab.MinusBtn;
                minus.ChangeButtonText("<");
                minus.transform.localPosition += new Vector3(0.6f, 0f, 0f);

                var plus = StringPrefab.PlusBtn;
                plus.ChangeButtonText(">");
                plus.transform.localPosition += new Vector3(1.5f, 0f, 0f);

                var valueBox = StringPrefab.transform.GetChild(5);
                valueBox.localPosition += new Vector3(1.05f, 0f, 0f);
                valueBox.localScale += new Vector3(0.2f, 0f, 0f);

                prefabs.Add(StringPrefab);
            }

            if (!TogglePrefab)
            {
                // Title = 0, Toggle = 1, Background = 2
                TogglePrefab = UObject.Instantiate(__instance.checkboxOrigin, null).DontUnload().DontDestroy();
                TogglePrefab.name = "CustomToggleOptionPrefab";
                TogglePrefab.transform.GetChild(1).localPosition += new Vector3(2.2f, 0f, 0f);

                var title = TogglePrefab.TitleText;
                title.transform.localPosition = new(-2.0466f, 0f, -2.9968f);
                title.GetComponent<RectTransform>().sizeDelta = new(5.8f, 0.458f);

                var background = TogglePrefab.transform.GetChild(2);
                background.localPosition += new Vector3(-0.8f, 0f, 0f);
                background.localScale += new Vector3(1f, 0f, 0f);

                prefabs.Add(TogglePrefab);
            }

            if (!MultiOptionPrefab)
            {
                MultiOptionPrefab = UObject.Instantiate(TogglePrefab, null).DontUnload().DontDestroy();
                MultiOptionPrefab.name = "MultiSelectOptionPrefab";

                var background = MultiOptionPrefab.transform.GetChild(2);
                background.localScale = new(1.75f, 1f, 1f);
                background.localPosition = new(-1.5232f, -0.0619f, 0f);

                prefabs.Add(MultiOptionPrefab);
            }

            if (!MultiSelectPrefab)
            {
                // Background = 0, Value Text = 1, Title = 2, < = 3, > = 4, Value Box = 5, Button = 6
                MultiSelectPrefab = UObject.Instantiate(StringPrefab, null).DontUnload().DontDestroy();
                MultiSelectPrefab.name = "RoleListMultiSelectPrefab";
                MultiSelectPrefab.PlusBtn.gameObject.SetActive(false);
                MultiSelectPrefab.MinusBtn.gameObject.SetActive(false);

                var toggle = UObject.Instantiate(TogglePrefab.GetComponentInChildren<PassiveButton>(), MultiSelectPrefab.transform);
                toggle.name = "Button";
                toggle.transform.DestroyChildren();
                toggle.OverrideOnClickListeners(BlankVoid);

                var box = toggle.GetComponent<BoxCollider2D>();
                var prevColliderSize = box.size;
                prevColliderSize.x *= 4.97f;
                box.size = prevColliderSize;

                prefabs.Add(MultiSelectPrefab);
            }

            if (!HeaderPrefab)
            {
                HeaderPrefab = UObject.Instantiate(__instance.categoryHeaderOrigin, null).DontUnload().DontDestroy();
                HeaderPrefab.name = "CustomHeaderOptionPrefab";
                HeaderPrefab.transform.localScale = new(0.63f, 0.63f, 0.63f);
                HeaderPrefab.Background.transform.localScale += new Vector3(0.7f, 0f, 0f);

                var newButton = UObject.Instantiate(StringPrefab.PlusBtn, HeaderPrefab.transform);
                newButton.transform.localScale *= 0.7f;
                newButton.transform.localPosition = new(3.2f, -0.18f, 0f);
                newButton.OverrideOnClickListeners(BlankVoid);
                newButton.name = "Collapse";

                prefabs.Add(HeaderPrefab);
            }

            if (!LayersSet)
            {
                foreach (var mono in prefabs)
                {
                    foreach (var obj in mono.GetComponentsInChildren<SpriteRenderer>(true))
                    {
                        obj.material.SetInt(PlayerMaterial.MaskLayer, 20);
                        obj.material.SetFloat("_StencilComp", 3f);
                        obj.material.SetFloat("_Stencil", 20);
                        obj.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    }

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

        [HarmonyPatch(nameof(GameOptionsMenu.Initialize))]
        public static bool Prefix(GameOptionsMenu __instance)
        {
            if (IsHnS())
                return true;

            if (SettingsPage != 3)
            {
                __instance.MapPicker.Initialize(20);
                __instance.MapPicker.SetUpFromData(GameManager.Instance.GameSettingsList.MapNameSetting, 20);
            }

            // TODO: Make a better fix for this for example caching the options or creating it ourself.
            // AD Says: Done, kinda. It's not perfect but it's better than nothing.
            OnValueChanged();
            return false;
        }

        [HarmonyPatch(nameof(GameOptionsMenu.ValueChanged))]
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(RolesSettingsMenu))]
    public static class RolesSettingsMenuPatches
    {
        private static bool LayersSet;

        [HarmonyPatch(nameof(RolesSettingsMenu.Awake))]
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

            var prefabs = new List<MonoBehaviour>();

            if (!LayersPrefab)
            {
                // Title = 0, Role # = 1, Chance % = 2, Background = 3, Divider = 4, Cog = 5, Unique = 6, Active = 7
                //            ┗-----------┗----------- Value = 0, - = 1, + = 2, Value Box = 3 ┗-----------┗--------- Checkbox = 0
                LayersPrefab = UObject.Instantiate(__instance.roleOptionSettingOrigin, null).DontUnload().DontDestroy();
                LayersPrefab.name = "CustomLayersOptionPrefab";
                LayersPrefab.titleText.alignment = TextAlignmentOptions.Left;
                LayersPrefab.role = null;
                LayersPrefab.transform.GetChild(0).localPosition += new Vector3(-0.1f, 0f, 0f);

                LayersPrefab.CountMinusBtn.OverrideOnClickListeners(BlankVoid);
                LayersPrefab.CountPlusBtn.OverrideOnClickListeners(BlankVoid);
                LayersPrefab.ChanceMinusBtn.OverrideOnClickListeners(BlankVoid);
                LayersPrefab.ChancePlusBtn.OverrideOnClickListeners(BlankVoid);

                var label = LayersPrefab.transform.GetChild(3);
                label.localScale += new Vector3(0.001f, 0f, 0f); // WHY THE FUCK IS THE BACKGROUND EVER SO SLIGHTLY SMALLER THAN THE HEADER?!
                label.localPosition = new(-0.3998f, -0.2953f, 4f);

                var newButton = UObject.Instantiate(LayersPrefab.CountMinusBtn, LayersPrefab.transform);
                newButton.name = "LayerSubSettingsButton";
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

                prefabs.Add(LayersPrefab);
            }

            if (!AlignmentPrefab)
            {
                // Header Label = 0, Header Text = 1, Quota Header = 2, Collapse = 3, Cog = 4
                //                                    ┗---------------- Dark Label = 0, Left = 1, Left Label = 2, Right Label = 3, Right = 4, Long Label = 5, Center = 6
                AlignmentPrefab = UObject.Instantiate(__instance.categoryHeaderEditRoleOrigin, null).DontUnload().DontDestroy();
                AlignmentPrefab.name = "CustomHeaderOptionLayerPrefab";
                AlignmentPrefab.transform.GetChild(0).gameObject.SetActive(false);

                var quota = AlignmentPrefab.transform.GetChild(2);

                var single = UObject.Instantiate(quota.GetChild(3), quota);
                single.localScale += new Vector3(0.5f, 0f, 0f);
                single.localPosition += new Vector3(-0.956f, 0f, 0f);
                single.name = "SingleBG";

                var text = quota.GetChild(1);
                text.GetComponent<TextTranslatorTMP>().Destroy();

                var center = UObject.Instantiate(text, quota);
                center.name = "Center";
                center.GetComponent<TextTranslatorTMP>().Destroy();

                var newButton = UObject.Instantiate(LayersPrefab.CountMinusBtn, AlignmentPrefab.transform);
                newButton.name = "Collapse";
                newButton.transform.localPosition = new(-5.839f, -0.45f, -2f);
                newButton.GetComponentInChildren<TextMeshPro>().text = "-";
                newButton.OverrideOnClickListeners(BlankVoid);
                newButton.transform.localScale *= 0.7f;

                var newButton2 = UObject.Instantiate(LayersPrefab.transform.GetChild(5), AlignmentPrefab.transform);
                newButton2.name = "SubOptions";
                newButton2.transform.FindChild("Text_TMP").gameObject.Destroy();
                newButton2.transform.localPosition = new(-5.239f, -0.45f, -2f);
                newButton2.transform.localScale *= 0.7f;

                prefabs.Add(AlignmentPrefab);
            }

            RolesButton = UObject.Instantiate(GameSettingMenu.Instance.GamePresetsButton, __instance.RoleChancesSettings.transform);
            RolesButton.OverrideOnClickListeners(AllLayers);
            RolesButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            RolesButton.buttonText.alignment = TextAlignmentOptions.Center;
            RolesButton.buttonText.text = TranslationManager.Translate("View.AllLayers");
            RolesButton.name = "AllLayers";
            RolesButton.transform.localPosition = new(0.1117f, 1.626f, -2f);
            RolesButton.ClickMask = __instance.ButtonClickMask;
            prefabs.Add(RolesButton);

            AlignmentsButton = UObject.Instantiate(GameSettingMenu.Instance.GamePresetsButton, __instance.RoleChancesSettings.transform);
            AlignmentsButton.OverrideOnClickListeners(AllAlignments);
            AlignmentsButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            AlignmentsButton.buttonText.alignment = TextAlignmentOptions.Center;
            AlignmentsButton.buttonText.text = TranslationManager.Translate("View.AllAlignments");
            AlignmentsButton.name = "AllAlignments";
            AlignmentsButton.transform.localPosition = new(3.4727f, 1.626f, -2f);
            AlignmentsButton.ClickMask = __instance.ButtonClickMask;
            prefabs.Add(AlignmentsButton);

            if (!LayersSet)
            {
                foreach (var mono in prefabs)
                {
                    foreach (var obj in mono.GetComponentsInChildren<SpriteRenderer>(true))
                    {
                        obj.material.SetInt(PlayerMaterial.MaskLayer, 20);
                        obj.material.SetFloat("_StencilComp", 3f);
                        obj.material.SetFloat("_Stencil", 20);
                        obj.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    }

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

        [HarmonyPatch(nameof(RolesSettingsMenu.SetQuotaTab))]
        [HarmonyPatch(nameof(RolesSettingsMenu.ValueChanged))]
        [HarmonyPrefix]
        public static bool MultiPrefix() => false;

        [HarmonyPatch(nameof(RolesSettingsMenu.Update))]
        public static bool Prefix(RolesSettingsMenu __instance)
        {
            __instance.RoleChancesSettings.transform.localPosition = new(SettingsPage is 4 or 5 ? 0.35f : -0.06f, 0f, -5f);
            return false;
        }

        [HarmonyPatch(nameof(RolesSettingsMenu.OpenChancesTab)), HarmonyPrefix]
        public static bool OpenChancesTabPrefix()
        {
            OnValueChanged();
            return false;
        }
    }

    public static void CreateOptions(Transform parent, Collider2D clickMask, ISystem.List<UiElement> uiElements)
    {
        var type = (MultiMenu)SettingsPage;
        var filtered = OptionAttribute.SortedOptions.Where(x => x.Menu == type);

        if (SelectedSubOptions != null)
            filtered = [ SelectedSubOptions ];

        foreach (var header in filtered)
        {
            if (!header.Setting)
            {
                header.Setting = header.Type switch
                {
                    CustomOptionType.Header => UObject.Instantiate(HeaderPrefab, parent),
                    CustomOptionType.Alignment => UObject.Instantiate(AlignmentPrefab, parent),
                    _ => null,
                };
                header.OptionCreated();

                if (header.Setting is OptionBehaviour option)
                    option.SetClickMask(clickMask);
            }

            if (header.Setting)
                header.Setting.gameObject.SetActive(false);

            foreach (var option in header.GroupMembers)
            {
                if (!option.Setting)
                {
                    option.Setting = option.Type switch
                    {
                        CustomOptionType.Number => UObject.Instantiate(NumberPrefab, parent),
                        CustomOptionType.String => UObject.Instantiate(StringPrefab, parent),
                        CustomOptionType.Layer => UObject.Instantiate(LayersPrefab, parent),
                        CustomOptionType.Toggle => UObject.Instantiate(TogglePrefab, parent),
                        CustomOptionType.Entry or CustomOptionType.MultiSelect => UObject.Instantiate(MultiSelectPrefab, parent),
                        _ => null,
                    };

                    if (option.Setting is OptionBehaviour behaviour)
                        behaviour.buttons = behaviour.GetComponentsInChildren<PassiveButton>().ToArray();

                    option.OptionCreated();
                }

                if (option.Setting)
                    option.Setting.gameObject.SetActive(false);
            }
        }

        parent.GetComponentsInChildren<UiElement>(true).ForEach(x => uiElements.AddUnique(x));
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
        __instance.RoleSettingsTab.gameObject.SetActive(SettingsPage is 1 or 4 or 5);
        __instance.PresetsTab.gameObject.SetActive(SettingsPage == 2);
        __instance.GameSettingsButton.SelectButton(SettingsPage == 0);
        __instance.RoleSettingsButton.SelectButton(SettingsPage == 1);
        __instance.GamePresetsButton.SelectButton(SettingsPage == 2);
        __instance.GameSettingsTab.MapPicker.gameObject.SetActive(SettingsPage == 0);

        // Gotta love things randomly becoming null

        if (!ReturnButton)
            ReturnButton = __instance.transform.FindChild("ReturnButton")?.gameObject;

        if (ReturnButton)
            ReturnButton.SetActive(SettingsPage is 4 or 5);

        if (!Next)
            Next = __instance.transform.FindChild("NextPageButton")?.gameObject;

        if (Next)
            Next.SetActive(PresetButtons.Count > 24 && SettingsPage == 2);

        if (!Prev)
            Prev = __instance.transform.FindChild("PrevPageButton")?.gameObject;

        if (Prev)
            Prev.SetActive(PresetButtons.Count > 24 && SettingsPage == 2);

        if (!ClientSettingsButton)
            ClientSettingsButton = __instance.transform.Find("LeftPanel")?.Find("ClientSettingsButton")?.GetComponent<PassiveButton>();

        if (ClientSettingsButton)
            ClientSettingsButton.SelectButton(SettingsPage == 3);

        CreateOptions
        (
            SettingsPage is 1 or 4 or 5 ? __instance.RoleSettingsTab.RoleChancesSettings.transform : __instance.GameSettingsTab.settingsContainer,
            SettingsPage is 1 or 4 or 5 ? __instance.RoleSettingsTab.ButtonClickMask : __instance.GameSettingsTab.ButtonClickMask,
            SettingsPage is 1 or 4 or 5 ? __instance.RoleSettingsTab.ControllerSelectable : __instance.GameSettingsTab.ControllerSelectable
        );

        var menu = (MultiMenu)SettingsPage;

        if (SettingsPage is 0 or 3)
        {
            var y = SettingsPage == 3 ? 2.063f : 0.863f;
            __instance.GameSettingsTab.Children.Clear();

            if (SettingsPage == 0)
                __instance.GameSettingsTab.Children.Add(__instance.GameSettingsTab.MapPicker);

            foreach (var header in OptionAttribute.SortedOptions)
            {
                if (!header.Setting)
                {
                    header.GroupMembers?.ForEach(x =>
                    {
                        if (x.Setting)
                            x.Setting.gameObject.SetActive(false);
                    });
                    continue;
                }

                var flag = header.Menu == menu && header.Active();
                header.Setting.gameObject.SetActive(flag);
                header.Update();

                if (!flag)
                {
                    header.GroupMembers?.ForEach(x =>
                    {
                        if (x.Setting)
                            x.Setting.gameObject.SetActive(false);
                    });
                    continue;
                }

                header.Setting.transform.localPosition = new(-0.903f, y, -2f);
                y -= 0.53f;

                foreach (var option in header.GroupMembers)
                {
                    if (!option.Setting)
                        continue;

                    var flag2 = flag && option.Active();
                    option.Setting.gameObject.SetActive(flag2);
                    option.Update();

                    if (!flag2)
                        continue;

                    option.Setting.transform.localPosition = new(0.952f, y, -2f);
                    y -= 0.45f;

                    if (option.Setting is OptionBehaviour setting)
                        __instance.GameSettingsTab.Children.Add(setting);

                    if (option is IMultiSelectOption multiSelect)
                    {
                        foreach (var button in multiSelect.Options)
                        {
                            button.transform.localPosition = new(0.952f, y, -2f);
                            y -= 0.45f;
                        }
                    }
                }
            }

            __instance.GameSettingsTab.scrollBar.SetYBoundsMax(-1.65f - y);
            __instance.GameSettingsTab.InitializeControllerNavigation();
        }
        else if (SettingsPage is 1 or 4 or 5)
        {
            var y = SettingsPage is 4 or 5 ? 1.515f : 1.36f;
            RolesButton.gameObject.SetActive(SettingsPage == 1);
            AlignmentsButton.gameObject.SetActive(SettingsPage == 1);
            __instance.RoleSettingsTab.advancedSettingChildren.Clear();

            foreach (var header in OptionAttribute.SortedOptions)
            {
                if (!header.Setting)
                {
                    header.GroupMembers?.ForEach(x =>
                    {
                        if (x.Setting)
                            x.Setting.gameObject.SetActive(false);
                    });
                    continue;
                }

                var flag = header.Menu == menu && (SelectedSubOptions == null || header == SelectedSubOptions) && header.Active();
                header.Setting.gameObject.SetActive(flag);
                header.Update();

                if (!flag)
                {
                    header.GroupMembers?.ForEach(x =>
                    {
                        if (x.Setting)
                            x.Setting.gameObject.SetActive(false);
                    });
                    continue;
                }

                var isHeader = header is HeaderOptionAttribute;

                if (!isHeader)
                    y -= 0.1f;

                header.Setting.transform.localPosition = new(isHeader ? -0.903f : 4.986f, y, -2f);
                y -= isHeader ? 0.53f : 0.525f;

                foreach (var option in header.GroupMembers)
                {
                    if (!option.Setting)
                        continue;

                    var flag2 = flag && option.Active();
                    option.Setting.gameObject.SetActive(flag2);
                    option.Update();

                    if (!flag2)
                        continue;

                    option.Setting.transform.localPosition = new(isHeader ? 0.952f : -0.15f, y, -2f);
                    y -= isHeader ? 0.45f : 0.43f;

                    if (option is IMultiSelectOption multiSelect)
                    {
                        foreach (var button in multiSelect.Options)
                        {
                            button.transform.localPosition = new(0.952f, y, -2f);
                            y -= 0.45f;
                        }
                    }

                    if (option.Setting is OptionBehaviour setting)
                        __instance.RoleSettingsTab.advancedSettingChildren.Add(setting);
                }
            }

            __instance.RoleSettingsTab.scrollBar.SetYBoundsMax(-y + (SettingsPage >= 5 ? -1.65f : -1.2f));
            __instance.RoleSettingsTab.InitializeControllerNavigation();
        }
    }

    private static void NextPage(bool increment)
    {
        SettingsPage2 = CycleInt(PresetButtons.Count / 24, 0, SettingsPage2, increment);
        OnPageChanged();
    }

    public static GameObject ReturnButton;

    private static void Return()
    {
        SettingsPage = CachedPage;
        var scrollbar = CachedPage is 0 ? GameSettingMenu.Instance.GameSettingsTab.scrollBar : GameSettingMenu.Instance.RoleSettingsTab.scrollBar;
        scrollbar.Inner.transform.localPosition = ScrollerLocation;
        scrollbar.UpdateScrollBars();
        SettingsPage2 = 0;
        SelectedSubOptions = null;
        OnValueChanged();
    }

    private static void AllLayers()
    {
        SettingsPage = 4;
        CachedPage = 1;
        var scrollbar = GameSettingMenu.Instance.RoleSettingsTab.scrollBar;
        ScrollerLocation = scrollbar.Inner.transform.localPosition;
        scrollbar.ScrollToTop();
        OnValueChanged();
    }

    private static void AllAlignments()
    {
        SettingsPage = 5;
        CachedPage = 1;
        var scrollbar = GameSettingMenu.Instance.RoleSettingsTab.scrollBar;
        ScrollerLocation = scrollbar.Inner.transform.localPosition;
        scrollbar.ScrollToTop();
        OnValueChanged();
    }

    [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Initialize))]
    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Initialize))]
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Initialize))]
    [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.UpdateValuesAndText))]
    [HarmonyPatch(typeof(NotificationPopper), (nameof(NotificationPopper.AddSettingsChangeMessage)))]
    public static bool Prefix() => false;

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
    public static class PlayerJoinPatch
    {
        private static bool SentOnce;

        public static void Postfix(PlayerPhysics __instance)
        {
            if (!AmongUsClient.Instance || !CustomPlayer.Local || !__instance.myPlayer || IsFreePlay())
                return;

            if (__instance.myPlayer.AmOwner)
            {
                if (!SentOnce && !ClientOptions.NoWelcome)
                {
                    Run("<#5411F8FF>人 Welcome! 人</color>", "Welcome to Town Of Us Reworked! Type /help to get started!");
                    SentOnce = true;
                }

                return;
            }

            if (GameData.Instance.PlayerCount < 2 || !AmongUsClient.Instance.AmHost || TownOfUsReworked.MCIActive || IsHnS())
                return;

            SendOptionRPC(targetClientId: __instance.myPlayer.OwnerId);
            CallTargetedRpc(__instance.myPlayer.OwnerId, CustomRPC.Misc, MiscRPC.SyncMap, MapSettings.Map);
            CallTargetedRpc(__instance.myPlayer.OwnerId, CustomRPC.Misc, MiscRPC.SyncSummary, ReadDiskText("Summary", TownOfUsReworked.Other));

            if (CachedFirstDead != null)
                CallTargetedRpc(__instance.myPlayer.OwnerId, CustomRPC.Misc, MiscRPC.SetFirstKilled, CachedFirstDead);
        }
    }

    [HarmonyPatch(typeof(GameOptionsMapPicker))]
    public static class GameOptionsMapPickerPatches
    {
        [HarmonyPatch(nameof(GameOptionsMapPicker.Initialize))]
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

            if (SubLoaded)
            {
                while (__instance.AllMapIcons.Count(x => x.Name == (MapNames)6) > 1)
                    __instance.AllMapIcons.Remove(__instance.AllMapIcons.Find(x => x.Name == (MapNames)6));
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
                    if (IsInGame() || !AmongUsClient.Instance.AmHost)
                        return;

                    __instance?.selectedButton?.Button?.SelectButton(false);
                    __instance.selectedButton = mapButton;
                    __instance.selectedButton.Button.SelectButton(true);
                    __instance.SelectMap(thisVal);
                    CallRpc(CustomRPC.Misc, MiscRPC.SyncMap, MapSettings.Map);
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

        [HarmonyPatch(nameof(GameOptionsMapPicker.SelectMap), typeof(int))]
        public static void Postfix(int mapId) => SetMap((MapEnum)mapId);

        [HarmonyPatch(nameof(GameOptionsMapPicker.SelectMap), typeof(MapIconByName))]
        public static void Postfix(MapIconByName mapInfo) => SetMap((MapEnum)mapInfo.Name);
    }

    private static readonly string[] Maps = [ "The Skeld", "Mira HQ", "Polus", "ehT dlekS", "Airship", "Fungle", "Submerged", "Level Impostor", "Random" ];
    private static LobbyNotificationMessage MapChangeNotif;

    public static void SetMap(MapEnum map)
    {
        if (IsInGame())
            return;

        MapSettings.Map = map;

        if (!CustomPlayer.Local)
            return;

        TownOfUsReworked.NormalOptions.MapId = (byte)map;
        OnValueChanged();

        var changed = $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">Game Map</font> set to <font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{Maps[(int)map]}</font>";

        if (MapChangeNotif != null)
            MapChangeNotif.UpdateMessage(changed);
        else
        {
            var hud = HUD();
            MapChangeNotif = UObject.Instantiate(hud.Notifier.notificationMessageOrigin, Vector3.zero, Quaternion.identity, hud.Notifier.transform);
            MapChangeNotif.transform.localPosition = new(0f, 0f, -2f);
            MapChangeNotif.SetUp(changed, hud.Notifier.settingsChangeSprite, hud.Notifier.settingsChangeColor, (Action)(() => hud.Notifier.OnMessageDestroy(MapChangeNotif)));
            hud.Notifier.ShiftMessages();
            hud.Notifier.AddMessageToQueue(MapChangeNotif);
        }
    }

    public static void CreateViewOptions(Transform parent, int page = -1)
    {
        if (page == -1)
            page = SettingsPage3;

        var type = (MultiMenu)page;

        foreach (var header in OptionAttribute.SortedOptions.Where(x => x.Menu == type))
        {
            if (!header.ViewSetting)
            {
                header.ViewSetting = UObject.Instantiate(HeaderViewPrefab, parent);
                header.ViewOptionCreated();
            }

            if (header.ViewSetting)
                header.ViewSetting.gameObject.SetActive(false);

            foreach (var option in header.GroupMembers)
            {
                if (!option.ViewSetting)
                {
                    option.ViewSetting = option.Type switch
                    {
                        CustomOptionType.Layer => UObject.Instantiate(LayerViewPrefab, parent),
                        _ => UObject.Instantiate(GenericViewPrefab, parent),
                    };
                    option.ViewOptionCreated();
                }

                if (option.ViewSetting)
                    option.ViewSetting.gameObject.SetActive(false);
            }
        }
    }

    [HarmonyPatch(typeof(LobbyInfoPane), nameof(LobbyInfoPane.Update))]
    public static class LobbyInfoPanePatch
    {
        public static void Postfix()
        {
            if (IsHnS())
                return;

            var gmt = GameObject.Find("GameModeText");

            if (gmt)
            {
                var tmp = gmt.GetComponent<TextMeshPro>();

                if (tmp.text != CurrentPreset)
                    tmp.text = CurrentPreset;
            }

            var ml = GameObject.Find("ModeLabel")?.transform?.GetChild(1).gameObject;

            if (ml)
            {
                var tmp = ml.GetComponent<TextMeshPro>();
                var translation = TranslationManager.Translate($"CustomOption.GameMode.{GameModeSettings.GameMode}");

                if (tmp.text != translation)
                    tmp.text = translation;
            }
        }
    }

    public static void OnValueChangedView(LobbyViewSettingsPane __instance = null)
    {
        if (IsHnS())
            return;

        __instance ??= LobbyInfoPane.Instance.LobbyViewSettingsPane;

        if (!__instance)
            return;

        __instance.taskTabButton.SelectButton(SettingsPage3 == 0);
        __instance.rolesTabButton.SelectButton(SettingsPage3 == 1);
        ClientOptionsButton.SelectButton(SettingsPage3 == 3);

        var y = 1.4f;
        CreateViewOptions(__instance.settingsContainer);

        if (SettingsPage3 is 0 or 3)
        {
            var menu = (MultiMenu)SettingsPage3;

            foreach (var option in OptionAttribute.SortedOptions)
            {
                if (option is not BaseHeaderOptionAttribute header || !option.ViewSetting)
                    continue;

                var flag = option.Menu == menu && option.Active();
                option.ViewSetting.gameObject.SetActive(flag);
                option.ViewUpdate();

                if (!flag)
                {
                    header.GroupMembers?.ForEach(x =>
                    {
                        if (x.ViewSetting)
                            x.ViewSetting.gameObject.SetActive(false);
                    });
                    continue;
                }

                option.ViewSetting.transform.localPosition = new(-9.77f, y, -2f);
                __instance.settingsInfo.Add(option.ViewSetting.gameObject);
                y -= 0.06f;
                var members = header.GroupMembers?.Where(x =>
                {
                    if (!x.ViewSetting)
                        return false;

                    var flag2 = x.Active();
                    x.ViewUpdate();
                    x.ViewSetting.gameObject.SetActive(flag2);
                    return flag2;
                }).ToArray();

                for (var i = 0; i < members.Length; i++)
                {
                    if (i % 2 == 0)
                        y -= 0.59f;

                    var optionn = members[i];
                    optionn.ViewSetting.transform.localPosition = new(i % 2 == 0 ? -8.95f : -3f, y, -2f);
                    __instance.settingsInfo.Add(optionn.ViewSetting.gameObject);
                }

                y -= members.Length > 0 ? 0.42f : 0.35f;
                y -= 0.1f;
            }
        }
        else if (SettingsPage3 == 1)
        {
            CreateViewOptions(__instance.settingsContainer, 5);
            CreateViewOptions(__instance.settingsContainer, 4);

            foreach (var option in OptionAttribute.GetOptions<AlignmentOptionAttribute>())
            {
                if (!option.ViewSetting)
                    continue;

                option.ViewUpdate();
                option.ViewSetting.transform.localPosition = new(-9.77f, y, -2f);
                option.ViewSetting.gameObject.SetActive(true);
                __instance.settingsInfo.Add(option.ViewSetting.gameObject);
                y -= 0.3f;

                if (!option.Value)
                {
                    y -= 0.21f;
                    continue;
                }

                if (option.GroupHeader != null)
                {
                    if (option.GroupHeader.ViewSetting)
                    {
                        option.GroupHeader.ViewUpdate();
                        option.GroupHeader.ViewSetting.gameObject.SetActive(false);
                    }

                    var members = option.GroupHeader.GroupMembers?.Where(x =>
                    {
                        if (!x.ViewSetting)
                            return false;

                        var flag = x.Active();
                        x.ViewUpdate();
                        x.ViewSetting.gameObject.SetActive(flag);
                        return flag;
                    }).ToArray();

                    for (var i = 0; i < members.Length; i++)
                    {
                        if (i % 2 == 0)
                            y -= 0.59f;

                        var optionn = members[i];
                        optionn.ViewSetting.transform.localPosition = new(i % 2 == 0 ? -8.95f : -3f, y, -2f);
                        __instance.settingsInfo.Add(optionn.ViewSetting.gameObject);
                    }

                    y -= members.Length > 0 ? 0.6f : 0.35f;
                }
                else
                    y -= 0.543f;

                foreach (var layer in option.GroupMembers?.Cast<LayerOptionAttribute>())
                {
                    layer.ViewUpdate();
                    var active = layer.Active();
                    layer.ViewSetting.gameObject.SetActive(active);

                    if (!active)
                    {
                        layer.GroupHeader?.GroupMembers?.ForEach(x =>
                        {
                            if (x.ViewSetting)
                                x.ViewSetting.gameObject.SetActive(false);
                        });
                        continue;
                    }

                    layer.ViewSetting.transform.localPosition = new(-6.73f, y, -2f);
                    __instance.settingsInfo.Add(layer.ViewSetting.gameObject);
                    y -= 0.65f;

                    if (layer.GroupHeader != null)
                    {
                        layer.GroupHeader.ViewUpdate();
                        layer.GroupHeader.ViewSetting.gameObject.SetActive(false);

                        y += 0.6f;
                        var members = layer.GroupHeader.GroupMembers?.Where(x =>
                        {
                            if (!x.ViewSetting)
                                return false;

                            var flag = x.Active();
                            x.ViewUpdate();
                            x.ViewSetting.gameObject.SetActive(flag);
                            return flag;
                        }).ToArray();

                        for (var i = 0; i < members.Length; i++)
                        {
                            if (i % 2 == 0)
                                y -= 0.59f;

                            var optionn = members[i];
                            optionn.ViewSetting.transform.localPosition = new(i % 2 == 0 ? -8.95f : -3f, y, -2f);
                            __instance.settingsInfo.Add(optionn.ViewSetting.gameObject);
                        }

                        y -= members.Length > 0 ? 0.62f : 0.59f;
                    }
                    else
                        y += 0.01f;
                }
            }
        }

        __instance.scrollBar.SetYBoundsMax(-y);
    }

    private static CategoryHeaderMasked HeaderViewPrefab;
    private static ViewSettingsInfoPanelRoleVariant LayerViewPrefab;
    private static ViewSettingsInfoPanel GenericViewPrefab;

    private static PassiveButton ClientOptionsButton;

    [HarmonyPatch(typeof(LobbyViewSettingsPane))]
    public static class LobbyViewSettingsPanePatches
    {
        private static bool LayersSet;

        [HarmonyPatch(nameof(LobbyViewSettingsPane.Awake))]
        public static void Postfix(LobbyViewSettingsPane __instance)
        {
            __instance.rolesTabButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            __instance.rolesTabButton.buttonText.text = TranslationManager.Translate("GameSettings.Layers");

            var prefabs = new List<MonoBehaviour>();

            if (!ClientOptionsButton && !IsHnS())
            {
                ClientOptionsButton = UObject.Instantiate(__instance.rolesTabButton, __instance.rolesTabButton.transform.parent);
                ClientOptionsButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
                ClientOptionsButton.name = "ClientOptionsTab";
                ClientOptionsButton.buttonText.text = TranslationManager.Translate("CustomOption.ClientOptions");
                ClientOptionsButton.transform.localPosition = __instance.rolesTabButton.transform.localPosition + new Vector3(__instance.rolesTabButton.transform.localPosition.x -
                    __instance.taskTabButton.transform.localPosition.x, 0f, 0f);
                ClientOptionsButton.OverrideOnClickListeners(() =>
                {
                    if (IsHnS())
                        return;

                    SettingsPage3 = 3;
                    OnValueChangedView(__instance);
                });
            }

            ClientOptionsButton?.gameObject?.SetActive(!IsHnS());

            if (!HeaderViewPrefab)
            {
                HeaderViewPrefab = UObject.Instantiate(__instance.categoryHeaderOrigin, null).DontUnload().DontDestroy();
                HeaderViewPrefab.name = "HeaderViewPrefab";
                HeaderViewPrefab.Background.gameObject.SetActive(false);
                HeaderViewPrefab.Title.gameObject.SetActive(false);
                HeaderViewPrefab.transform.GetChild(1).gameObject.SetActive(false);

                var button = UObject.Instantiate(__instance.rolesTabButton, HeaderViewPrefab.transform);
                button.buttonText.text = "";
                button.name = "TitleButton";
                button.SelectButton(false);
                button.transform.localPosition = new(-0.75f, -0.05f, 0f);
                button.transform.localScale = new(0.7f, 0.7f, 1f);
                button.OverrideOnClickListeners(BlankVoid);

                prefabs.Add(HeaderViewPrefab);
            }

            if (!LayerViewPrefab)
            {
                // Title = 0, Left Box = 1, Center Box = 2, Right Box = 3, Button = 4, Role Icon = 5, Label = 6
                //            ┗-------------┗---------------┗------------ Value Text = 0, Background = 1, _ = 2, Tick = 3, Cross = 4, Title = 5
                LayerViewPrefab = UObject.Instantiate(__instance.infoPanelRoleOrigin, null).DontUnload().DontDestroy();
                LayerViewPrefab.name = "LayerViewPrefab";

                LayerViewPrefab.iconSprite.gameObject.SetActive(false);
                LayerViewPrefab.background.gameObject.SetActive(false);

                var leftBox = LayerViewPrefab.transform.GetChild(0);
                leftBox.name = "Left";
                leftBox.localPosition -= new Vector3(1.2f, 0f, 0f);
                leftBox.SetSiblingIndex(1);

                var leftText = UObject.Instantiate(LayerViewPrefab.titleText, leftBox);
                leftText.name = "Title";
                leftText.GetComponent<TextTranslatorTMP>()?.Destroy();

                LayerViewPrefab.titleText.alignment = TextAlignmentOptions.MidlineLeft;
                LayerViewPrefab.titleText.transform.localPosition = new(-3.4703f, -0.0223f, -2f);
                LayerViewPrefab.titleText.transform.SetSiblingIndex(0);

                var button = UObject.Instantiate(HeaderViewPrefab.transform.Find("TitleButton"), LayerViewPrefab.transform);
                button.name = "Toggle";
                button.transform.localPosition = new(5.7536f, 0.05f, 0f);
                button.transform.GetChild(1).localScale =  button.transform.GetChild(2).localScale =  button.transform.GetChild(3).localScale = new(0.5f, 1f, 1f);

                var buttonButton = button.GetComponent<PassiveButton>();
                buttonButton.buttonText.transform.localPosition = new(-1.55f, 0.048f, -2f);
                buttonButton.buttonText.transform.localScale = Vector3.one;

                LayerViewPrefab.checkMark.name = "ActiveOn";
                LayerViewPrefab.checkMarkOff.name = "ActiveOff";

                LayerViewPrefab.checkMark.transform.localPosition = LayerViewPrefab.checkMarkOff.transform.localPosition = new(1.089f, -0.009f, -1f);

                var rightBox = LayerViewPrefab.transform.GetChild(2);
                rightBox.name = "Right";
                rightBox.localPosition -= new Vector3(0.1f, 0f, 0f);

                UObject.Instantiate(LayerViewPrefab.checkMark, rightBox).name = "UniqueOn";
                UObject.Instantiate(LayerViewPrefab.checkMarkOff, rightBox).name = "UniqueOff";

                LayerViewPrefab.chanceTitle.name = "Title";
                LayerViewPrefab.chanceTitle.GetComponent<TextTranslatorTMP>()?.Destroy();
                LayerViewPrefab.chanceTitle.transform.SetParent(rightBox);

                var centerBox = UObject.Instantiate(rightBox, rightBox.parent);
                centerBox.name = "Centre";
                centerBox.localPosition = rightBox.localPosition + ((leftBox.localPosition - rightBox.localPosition) / 2);
                centerBox.SetSiblingIndex(2);
                centerBox.FindChild("Text_TMP").gameObject.Destroy();

                prefabs.Add(LayerViewPrefab);
            }

            if (!GenericViewPrefab)
            {
                GenericViewPrefab = UObject.Instantiate(__instance.infoPanelOrigin, null).DontUnload().DontDestroy();
                GenericViewPrefab.name = "GenericViewPrefab";
                GenericViewPrefab.labelBackground.gameObject.SetActive(false);

                prefabs.Add(GenericViewPrefab);
            }

            if (!LayersSet)
            {
                foreach (var mono in prefabs)
                {
                    foreach (var obj in mono.GetComponentsInChildren<SpriteRenderer>(true))
                    {
                        obj.material.SetInt(PlayerMaterial.MaskLayer, 61);
                        obj.material.SetFloat("_StencilComp", 3f);
                        obj.material.SetFloat("_Stencil", 61);
                        obj.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    }

                    foreach (var obj in mono.GetComponentsInChildren<TextMeshPro>(true))
                    {
                        obj.fontMaterial.SetFloat("_StencilComp", 3f);
                        obj.fontMaterial.SetFloat("_Stencil", 61);
                    }

                    mono.gameObject.SetActive(false);
                }

                LayersSet = true;
            }
        }

        [HarmonyPatch(nameof(LobbyViewSettingsPane.DrawNormalTab)), HarmonyPrefix]
        public static bool DrawNormalTabPrefix(LobbyViewSettingsPane __instance) => TabPrefix(__instance, 0);

        [HarmonyPatch(nameof(LobbyViewSettingsPane.DrawRolesTab)), HarmonyPrefix]
        public static bool DrawRolesTabPrefix(LobbyViewSettingsPane __instance) => TabPrefix(__instance, 1);

        public static bool TabPrefix(LobbyViewSettingsPane __instance, int page)
        {
            if (IsHnS())
                return true;

            SettingsPage3 = page;
            OnValueChangedView(__instance);
            return false;
        }
    }

    public static readonly List<PassiveButton> PresetButtons = [];
    public static GameObject Prev;
    public static GameObject Next;
    public static PassiveButton Save;
    public static PassiveButton Overwrite;
    public static bool Overwriting;

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

            if (!Save)
            {
                Save = UObject.Instantiate(ButtonPrefab, __instance.StandardPresetButton.transform.parent);
                Save.OverrideOnClickListeners(OptionAttribute.SaveSettings);
                Save.name = "SaveSettingsButton";
                Save.transform.localPosition = new(1.5f, 2.6164f, -2f);
                Save.buttonText.text = TranslationManager.Translate("ImportExport.Save");
                Save.gameObject.SetActive(true);
            }

            if (!Overwrite)
            {
                Overwrite = UObject.Instantiate(ButtonPrefab, __instance.StandardPresetButton.transform.parent);
                Overwrite.OverrideOnClickListeners(BeginOverwriting);
                Overwrite.name = "OverwriteSettingsButton";
                Overwrite.transform.localPosition = new(-0.9731f, 2.6164f, -2f);
                Overwrite.buttonText.text = TranslationManager.Translate("ImportExport.Overwrite");
                Overwrite.gameObject.SetActive(true);
            }

            Directory.GetFiles(TownOfUsReworked.Options).Where(x => x.EndsWith(".txt")).Select(x => x.SanitisePath()).ForEach(CreatePresetButton);

            OnPageChanged();
            return false;
        }
    }

    private static void BeginOverwriting()
    {
        Overwriting = true;
        OnPageChanged();
    }

    public static void OnPageChanged()
    {
        PresetButtons.RemoveAll(x => !x);

        for (var i = 0; i < PresetButtons.Count; i++)
        {
            var button = PresetButtons[i];
            button.SelectButton(Overwriting);

            if (i >= (SettingsPage2 * 24) && i < ((SettingsPage2 + 1) * 24))
            {
                var relativeIndex = i % 24;
                var row = relativeIndex / 4;
                var col = relativeIndex % 4;
                button.transform.localPosition = new(-2.5731f + (col * 1.8911f), 1.7828f - (row * 0.65136f), -2);
                button.gameObject.SetActive(true);
            }
            else
                button.gameObject.SetActive(false);
        }

        Prev.SetActive(PresetButtons.Count > 24 && SettingsPage == 2);
        Next.SetActive(PresetButtons.Count > 24 && SettingsPage == 2);
    }

    public static void CreatePresetButton(string presetName)
    {
        if (PresetButtons.Any(x => x.name == presetName))
            return;

        var presetButton = CreateButton(presetName, GameSettingMenu.Instance.PresetsTab.StandardPresetButton.transform.parent);
        presetButton.OverrideOnClickListeners(() => OptionAttribute.HandlePreset(presetName, presetButton.buttonText));
        PresetButtons.Add(presetButton);
    }

    private static PassiveButton CreateButton(string name, Transform parent)
    {
        var button = UObject.Instantiate(ButtonPrefab, parent);
        button.transform.localScale = new(0.5f, 0.84f, 1f);
        button.buttonText.transform.localScale = new(1.4f, 0.9f, 1f);
        button.buttonText.alignment = TextAlignmentOptions.Center;
        button.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
        button.buttonText.text = button.name = name;
        return button;
    }
}