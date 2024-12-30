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
            __instance.RoleSettingsButton.buttonText.SetText(TranslationManager.Translate("GameSettings.Layers"));

            ClientSettingsButton = UObject.Instantiate(__instance.RoleSettingsButton, __instance.RoleSettingsButton.transform.parent);
            ClientSettingsButton.name = "ClientSettingsButton";
            ClientSettingsButton.buttonText.GetComponent<TextTranslatorTMP>()?.Destroy();
            ClientSettingsButton.buttonText.SetText(TranslationManager.Translate("CustomOption.ClientOptions"));
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
                ButtonPrefab.buttonText.SetText("");
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
            SpawnOptionsCreated = false;
            RLOptionsCreated = false;
            PresetsButtons.Clear();
            LayerOptionsCreated.Keys.ForEach(x => LayerOptionsCreated[x] = false);
            RoleListEntryAttribute.ChoiceButtons.Clear();
            ScrollerLocation = default;
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
    public static ToggleOption EntryPrefab;
    public static CategoryHeaderMasked HeaderPrefab;
    public static CategoryHeaderEditRole AlignmentPrefab;
    public static PassiveButton ButtonPrefab;

    [HarmonyPatch(typeof(GameOptionsMenu))]
    public static class GameOptionsMenuPatches
    {
        private static bool LayersSet;

        [HarmonyPatch(nameof(GameOptionsMenu.Awake)), HarmonyPrefix]
        public static bool AwakePrefix(GameOptionsMenu __instance) => __instance.name != "ROLE LIST TAB";

        [HarmonyPatch(nameof(GameOptionsMenu.Awake))]
        public static void Postfix(GameOptionsMenu __instance)
        {
            if (__instance.name == "ROLE LIST TAB")
                return;

            __instance.Children = new();
            var prefabs = new List<MonoBehaviour>();

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

                prefabs.Add(NumberPrefab);
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

                var title = TogglePrefab.transform.GetChild(0);
                title.localPosition = new(-2.0466f, 0f, -2.9968f);
                title.GetComponent<RectTransform>().sizeDelta = new(5.8f, 0.458f);

                var background = TogglePrefab.transform.GetChild(2);
                background.localPosition += new Vector3(-0.8f, 0f, 0f);
                background.localScale += new Vector3(1f, 0f, 0f);

                prefabs.Add(TogglePrefab);
            }

            if (!EntryPrefab)
            {
                // Title = 0, Button = 1, Background = 2, Value Text = 3, Value Box = 4
                EntryPrefab = UObject.Instantiate(TogglePrefab, null).DontUnload().DontDestroy();
                EntryPrefab.name = "RoleListEntryPrefab";
                EntryPrefab.CheckMark.enabled = false;
                EntryPrefab.oldValue = false;

                var button = EntryPrefab.transform.GetChild(1);
                button.localPosition = new(3.8f, 0.042f, -1f);
                button.localScale += new Vector3(0f, 0.05f, 0f);

                UObject.Instantiate(StringPrefab.transform.GetChild(1), EntryPrefab.transform).localPosition = new(2.8f, 0.064f, -1f);
                UObject.Instantiate(StringPrefab.transform.GetChild(5), EntryPrefab.transform).localPosition = new(2.8f, 0.05f, 0f);

                prefabs.Add(EntryPrefab);
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
                newButton.GetComponentInChildren<TextMeshPro>().SetText("-");
                newButton.GetComponent<PassiveButton>().OverrideOnClickListeners(BlankVoid);
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

        [HarmonyPatch(nameof(GameOptionsMenu.Initialize)), HarmonyPrefix]
        public static bool InitializePrefix(GameOptionsMenu __instance)
        {
            if (IsHnS())
                return true;

            if (SettingsPage != 3)
            {
                __instance.MapPicker.Initialize(20);
                __instance.MapPicker.SetUpFromData(GameManager.Instance.GameSettingsList.MapNameSetting, 20);
            }

            // TODO: Make a better fix for this for example caching the options or creating it ourself.
            // AD Says: Done, kinda.
            var behaviours = CreateOptions(__instance.settingsContainer);

            foreach (var behave in behaviours)
            {
                if (behave is OptionBehaviour option)
                    option.SetClickMask(__instance.ButtonClickMask);
            }

            __instance.ControllerSelectable.AddRange(new(__instance.scrollBar.GetComponentsInChildren<UiElement>(true).Pointer));
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

        [HarmonyPatch(nameof(RolesSettingsMenu.Awake)), HarmonyPostfix]
        public static void AwakePostfix(RolesSettingsMenu __instance)
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
                center.transform.localPosition = quota.GetChild(4).localPosition + ((quota.GetChild(1).localPosition - quota.GetChild(4).localPosition) / 2);

                var newButton = UObject.Instantiate(LayersPrefab.CountMinusBtn, AlignmentPrefab.transform);
                newButton.name = "Collapse";
                newButton.transform.localPosition = new(-5.839f, -0.45f, -2f);
                newButton.GetComponentInChildren<TextMeshPro>().SetText("-");
                newButton.OverrideOnClickListeners(BlankVoid);
                newButton.transform.localScale *= 0.7f;

                var newButton2 = UObject.Instantiate(LayersPrefab.transform.GetChild(5), AlignmentPrefab.transform);
                newButton2.name = "AlignmentSubOptions";
                newButton2.transform.FindChild("Text_TMP").gameObject.Destroy();
                newButton2.transform.localPosition = new(-5.239f, -0.45f, -2f);
                newButton2.transform.localScale *= 0.7f;

                prefabs.Add(AlignmentPrefab);
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

        [HarmonyPatch(nameof(RolesSettingsMenu.SetQuotaTab))]
        [HarmonyPatch(nameof(RolesSettingsMenu.OpenChancesTab))]
        [HarmonyPatch(nameof(RolesSettingsMenu.ValueChanged))]
        public static bool Prefix() => false;

        [HarmonyPatch(nameof(RolesSettingsMenu.Update)), HarmonyPostfix]
        public static void UpdatePostfix(RolesSettingsMenu __instance) => __instance.RoleChancesSettings.transform.localPosition = new(SettingsPage >= 5 ? 0.35f : -0.06f, 0f, -5f);
    }

    private static bool SpawnOptionsCreated;
    private static bool RLOptionsCreated;
    private static readonly Dictionary<int, bool> LayerOptionsCreated = [];

    public static IEnumerable<MonoBehaviour> CreateOptions(Transform parent, bool instantiate = true)
    {
        var type = (MultiMenu)SettingsPage;

        if (SettingsPage == 4)
        {
            foreach (var layer in RoleListEntryAttribute.GetLayers())
            {
                var name = $"{layer}";

                if (!RoleListEntryAttribute.ChoiceButtons.TryFinding(x => x.name == name, out var button) && instantiate)
                {
                    button = CreateButton(name, parent);
                    button.buttonText.SetText(TranslationManager.Translate($"RoleList.{layer}"));
                    button.OverrideOnClickListeners(() => SetValue(layer));
                    button.transform.GetChild(2).transform.localScale += new Vector3(0.32f, 0f, 0f);
                    button.transform.GetChild(1).localScale = button.transform.GetChild(2).transform.localScale;
                    button.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new(3f, 0.6123f);

                    if (LayerDictionary.TryGetValue(layer, out var entry))
                        button.buttonText.SetText($"<#{entry.Color.ToHtmlStringRGBA()}>{button.buttonText.text}</color>");

                    RoleListEntryAttribute.ChoiceButtons.Add(button);
                }

                if (button)
                {
                    button.gameObject.SetActive(false);
                    yield return button;
                }
            }
        }
        else
        {
            foreach (var option in OptionAttribute.SortedOptions.Where(x => x.Menus.Contains(type)))
            {
                if (!option.Setting && instantiate)
                {
                    option.Setting = option.Type switch
                    {
                        CustomOptionType.Number => UObject.Instantiate(NumberPrefab, parent),
                        CustomOptionType.String => UObject.Instantiate(StringPrefab, parent),
                        CustomOptionType.Layer => UObject.Instantiate(LayersPrefab, parent),
                        CustomOptionType.Toggle => UObject.Instantiate(TogglePrefab, parent),
                        CustomOptionType.Entry => UObject.Instantiate(EntryPrefab, parent),
                        CustomOptionType.Header => UObject.Instantiate(HeaderPrefab, parent),
                        CustomOptionType.Alignment => UObject.Instantiate(AlignmentPrefab, parent),
                        _ => null,
                    };

                    if (option.Setting is OptionBehaviour behaviour)
                        behaviour.buttons = behaviour.GetComponentsInChildren<PassiveButton>().ToArray();
                }

                if (option.Setting)
                {
                    option.OptionCreated();
                    option.Setting.gameObject.SetActive(false);
                    yield return option.Setting;
                }
            }
        }
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

        __instance.GameSettingsTab.gameObject.SetActive(SettingsPage is 0 or 3 or 4);
        __instance.RoleSettingsTab.gameObject.SetActive(SettingsPage is 1 or >= 5);
        __instance.PresetsTab.gameObject.SetActive(SettingsPage == 2);
        __instance.GameSettingsButton.SelectButton(SettingsPage == 0);
        __instance.RoleSettingsButton.SelectButton(SettingsPage == 1);
        __instance.GamePresetsButton.SelectButton(SettingsPage == 2);
        __instance.GameSettingsTab.MapPicker.gameObject.SetActive(SettingsPage == 0);

        // Gotta love things randomly becoming null

        if (!ReturnButton)
            ReturnButton = __instance.transform.FindChild("ReturnButton")?.gameObject;

        if (ReturnButton)
            ReturnButton.SetActive(SettingsPage >= 4);

        if (!Next)
            Next = __instance.transform.FindChild("NextPageButton")?.gameObject;

        if (Next)
            Next.SetActive(SettingsPage == 2);

        if (!Prev)
            Prev = __instance.transform.FindChild("PrevPageButton")?.gameObject;

        if (Prev)
            Prev.SetActive(SettingsPage == 2);

        if (!ClientSettingsButton)
            ClientSettingsButton = __instance.transform.Find("LeftPanel")?.Find("ClientSettingsButton")?.GetComponent<PassiveButton>();

        if (ClientSettingsButton)
            ClientSettingsButton.SelectButton(SettingsPage == 3);

        CreateOptions(null, false);

        if (!SpawnOptionsCreated && SettingsPage == 1)
        {
            var buttons = new List<PassiveButton>();

            RolesButton = UObject.Instantiate(__instance.GamePresetsButton, __instance.RoleSettingsTab.RoleChancesSettings.transform);
            RolesButton.OverrideOnClickListeners(AllLayers);
            RolesButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            RolesButton.buttonText.alignment = TextAlignmentOptions.Center;
            RolesButton.buttonText.SetText(TranslationManager.Translate("View.AllLayers"));
            RolesButton.name = "AllLayers";
            RolesButton.transform.localPosition = new(0.1117f, 1.626f, -2f);
            buttons.Add(RolesButton);

            AlignmentsButton = UObject.Instantiate(__instance.GamePresetsButton, __instance.RoleSettingsTab.RoleChancesSettings.transform);
            AlignmentsButton.OverrideOnClickListeners(AllAlignments);
            AlignmentsButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            AlignmentsButton.buttonText.alignment = TextAlignmentOptions.Center;
            AlignmentsButton.buttonText.SetText(TranslationManager.Translate("View.AllAlignments"));
            AlignmentsButton.name = "AllAlignments";
            AlignmentsButton.transform.localPosition = new(3.4727f, 1.626f, -2f);
            buttons.Add(AlignmentsButton);

            foreach (var mono in buttons)
            {
                mono.ClickMask = __instance.RoleSettingsTab.ButtonClickMask;

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
            }

            var behaviours = CreateOptions(__instance.RoleSettingsTab.RoleChancesSettings.transform);

            foreach (var behave in behaviours)
            {
                if (behave is OptionBehaviour option)
                    option.SetClickMask(__instance.RoleSettingsTab.ButtonClickMask);
            }

            __instance.RoleSettingsTab.ControllerSelectable.AddRange(new(__instance.RoleSettingsTab.scrollBar.GetComponentsInChildren<UiElement>(true).Pointer));
            __instance.RoleSettingsTab.scrollBar.ScrollToTop();
            SpawnOptionsCreated = true;
        }

        if (!RLOptionsCreated && SettingsPage == 4)
        {
            var behaviours = CreateOptions(__instance.GameSettingsTab.settingsContainer);

            foreach (var mono in behaviours)
            {
                mono.GetComponent<PassiveButton>().ClickMask = __instance.GameSettingsTab.ButtonClickMask;

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
            }

            foreach (var elem in __instance.GameSettingsTab.scrollBar.GetComponentsInChildren<UiElement>(true))
            {
                if (!__instance.GameSettingsTab.ControllerSelectable.Contains(elem))
                    __instance.GameSettingsTab.ControllerSelectable.Add(elem);
            }

            __instance.GameSettingsTab.scrollBar.ScrollToTop();
            RLOptionsCreated = true;
        }

        if ((!LayerOptionsCreated.TryGetValue(SettingsPage, out var layerOptions) || !layerOptions) && SettingsPage >= 5)
        {
            var options = CreateOptions(__instance.RoleSettingsTab.RoleChancesSettings.transform);

            foreach (var option in options)
            {
                if (option is OptionBehaviour behave)
                    behave.SetClickMask(__instance.RoleSettingsTab.ButtonClickMask);
            }

            foreach (var elem in __instance.RoleSettingsTab.scrollBar.GetComponentsInChildren<UiElement>(true))
            {
                if (!__instance.RoleSettingsTab.ControllerSelectable.Contains(elem))
                    __instance.RoleSettingsTab.ControllerSelectable.Add(elem);
            }

            LayerOptionsCreated[SettingsPage] = true;
        }

        if (SettingsPage is 0 or 3 or 4)
        {
            var y = SettingsPage switch
            {
                3 => 2.063f,
                0 => 0.863f,
                _ => 1.9f
            };
            __instance.GameSettingsTab.Children.Clear();

            if (SettingsPage is 0 or 3)
            {
                if (SettingsPage == 0)
                    __instance.GameSettingsTab.Children.Add(__instance.GameSettingsTab.MapPicker);

                foreach (var option in OptionAttribute.SortedOptions)
                {
                    if (option.Setting)
                    {
                        var menu = (MultiMenu)SettingsPage;
                        var flag = option.Menus.Contains(menu) && option.Active();
                        option.Setting.gameObject.SetActive(flag);
                        option.Update();

                        if (!flag)
                            continue;

                        var isHeader = option is HeaderOptionAttribute;
                        option.Setting.transform.localPosition = new(isHeader ? -0.903f : 0.952f, y, -2f);
                        y -= isHeader ? 0.53f : 0.45f;

                        if (option.Setting is OptionBehaviour setting)
                            __instance.GameSettingsTab.Children.Add(setting);
                    }
                }
            }
            else
            {
                for (var i = 0; i < RoleListEntryAttribute.ChoiceButtons.Count; i++)
                {
                    var option = RoleListEntryAttribute.ChoiceButtons[i];
                    option.gameObject.SetActive(true);
                    var isEven = i % 2 == 0;
                    option.transform.localPosition = new(isEven ? -0.2f : 3.2f, y, -2f);

                    if (!isEven)
                        y -= 0.65136f;
                }
            }

            __instance.GameSettingsTab.scrollBar.SetYBoundsMax(-1.65f - y);
            __instance.GameSettingsTab.InitializeControllerNavigation();
        }
        else if (SettingsPage is 1 or >= 5)
        {
            var y = SettingsPage >= 5 ? 1.515f : 1.36f;
            RolesButton.gameObject.SetActive(SettingsPage == 1);
            AlignmentsButton.gameObject.SetActive(SettingsPage == 1);
            __instance.RoleSettingsTab.advancedSettingChildren.Clear();

            foreach (var option in OptionAttribute.SortedOptions)
            {
                if (option.Setting)
                {
                    var menu = (MultiMenu)SettingsPage;
                    var flag = option.Menus.Contains(menu) && option.Active();
                    option.Setting.gameObject.SetActive(flag);
                    option.Update();

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
                        var isAlign = option is AlignmentOptionAttribute;

                        if (isAlign)
                            y -= 0.1f;

                        option.Setting.transform.localPosition = new(isAlign ? 4.986f : -0.15f, y, -2f);
                        y -= isAlign ? 0.525f : 0.43f;
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
        SettingsPage2 = CycleInt(PresetsButtons.Count / 24, 0, SettingsPage2, increment);
        OnPageChanged();
    }

    private static void SetValue(LayerEnum value)
    {
        OptionAttribute.GetOptions<RoleListEntryAttribute>().Find(x => x.ID == RoleListEntryAttribute.SelectedEntry)?.Set(value);
        Return();
    }

    public static GameObject ReturnButton;

    private static void Return()
    {
        SettingsPage = CachedPage;
        var scrollbar = CachedPage is 0 ? GameSettingMenu.Instance.GameSettingsTab.scrollBar : GameSettingMenu.Instance.RoleSettingsTab.scrollBar;
        scrollbar.Inner.transform.localPosition = ScrollerLocation;
        scrollbar.UpdateScrollBars();
        SettingsPage2 = 0;
        RoleListEntryAttribute.SelectedEntry = "";
        RoleListEntryAttribute.ChoiceButtons.ForEach(x => x.gameObject.SetActive(false));
        OnValueChanged();
    }

    private static void AllLayers()
    {
        SettingsPage = 5;
        CachedPage = 1;
        var scrollbar = GameSettingMenu.Instance.RoleSettingsTab.scrollBar;
        ScrollerLocation = scrollbar.Inner.transform.localPosition;
        scrollbar.ScrollToTop();
        OnValueChanged();
    }

    private static void AllAlignments()
    {
        SettingsPage = 250;
        CachedPage = 1;
        var scrollbar = GameSettingMenu.Instance.RoleSettingsTab.scrollBar;
        ScrollerLocation = scrollbar.Inner.transform.localPosition;
        scrollbar.ScrollToTop();
        OnValueChanged();
    }

    private static bool Initialize(OptionBehaviour opt)
    {
        var result = OptionAttribute.AllOptions.TryFinding(option => option.Setting == opt, out var customOption);
        customOption?.OptionCreated();
        return !result;
    }

    [HarmonyPatch(typeof(ToggleOption))]
    public static class ToggleOptionPatches
    {
        [HarmonyPatch(nameof(ToggleOption.Initialize)), HarmonyPrefix]
        public static bool InitializePrefix(ToggleOption __instance) => Initialize(__instance);

        [HarmonyPatch(nameof(ToggleOption.Toggle)), HarmonyPrefix]
        public static bool TogglePrefix(ToggleOption __instance)
        {
            if ((IsInGame() && SettingsPage != 3) || !AmongUsClient.Instance.AmHost)
                return false;

            var option = OptionAttribute.AllOptions.Find(option => option.Setting == __instance);

            if (option is ToggleOptionAttribute toggle)
            {
                toggle.Toggle();
                return false;
            }

            if (option is RoleListEntryAttribute entry)
            {
                entry.ToDo();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(NumberOption))]
    public static class NumberOptionPatches
    {
        [HarmonyPatch(nameof(NumberOption.Initialize)), HarmonyPrefix]
        public static bool InitializePrefix(NumberOption __instance) => Initialize(__instance);

        [HarmonyPatch(nameof(NumberOption.Decrease)), HarmonyPrefix]
        public static bool DecreasePrefix(NumberOption __instance)
        {
            if ((IsInGame() && SettingsPage != 3) || !AmongUsClient.Instance.AmHost)
                return false;

            var result = OptionAttribute.GetOptions<NumberOptionAttribute>().TryFinding(option => option.Setting == __instance, out var num);

            if (result)
                num.Decrease();

            return !result;
        }

        [HarmonyPatch(nameof(NumberOption.Increase)), HarmonyPrefix]
        public static bool IncreasePrefix(NumberOption __instance)
        {
            if ((IsInGame() && SettingsPage != 3) || !AmongUsClient.Instance.AmHost)
                return false;

            var result = OptionAttribute.GetOptions<NumberOptionAttribute>().TryFinding(option => option.Setting == __instance, out var num);

            if (result)
                num.Increase();

            return !result;
        }
    }

    [HarmonyPatch(typeof(StringOption))]
    public static class StringOptionPatches
    {
        [HarmonyPatch(nameof(StringOption.Initialize)), HarmonyPrefix]
        public static bool InitializePrefix(StringOption __instance) => Initialize(__instance);

        [HarmonyPatch(nameof(StringOption.Increase)), HarmonyPrefix]
        public static bool IncreasePrefix(StringOption __instance)
        {
            if ((IsInGame() && SettingsPage != 3) || !AmongUsClient.Instance.AmHost)
                return false;

            var result = OptionAttribute.GetOptions<StringOptionAttribute>().TryFinding(option => option.Setting == __instance, out var str);

            if (result)
                str.Increase();

            return !result;
        }

        [HarmonyPatch(nameof(StringOption.Decrease)), HarmonyPrefix]
        public static bool DecreasePrefix(StringOption __instance)
        {
            if ((IsInGame() && SettingsPage != 3) || !AmongUsClient.Instance.AmHost)
                return false;

            var result = OptionAttribute.GetOptions<StringOptionAttribute>().TryFinding(option => option.Setting == __instance, out var str);

            if (result)
                str.Decrease();

            return !result;
        }
    }

    [HarmonyPatch(typeof(RoleOptionSetting))]
    public static class RoleOptionSettingPatches
    {
        [HarmonyPatch(nameof(RoleOptionSetting.UpdateValuesAndText)), HarmonyPrefix]
        public static bool UpdateValuesAndTextPrefix(RoleOptionSetting __instance) => Initialize(__instance);

        [HarmonyPatch(nameof(RoleOptionSetting.IncreaseChance)), HarmonyPrefix]
        public static bool IncreaseChancePrefix(RoleOptionSetting __instance)
        {
            if ((IsInGame() && SettingsPage != 3) || !AmongUsClient.Instance.AmHost)
                return false;

            var result = OptionAttribute.GetOptions<LayerOptionAttribute>().TryFinding(option => option.Setting == __instance, out var layer);

            if (result)
                layer.IncreaseChance();

            return !result;
        }

        [HarmonyPatch(nameof(RoleOptionSetting.DecreaseChance)), HarmonyPrefix]
        public static bool DecreaseChancePrefix(RoleOptionSetting __instance)
        {
            if ((IsInGame() && SettingsPage != 3) || !AmongUsClient.Instance.AmHost)
                return false;

            var result = OptionAttribute.GetOptions<LayerOptionAttribute>().TryFinding(option => option.Setting == __instance, out var layer);

            if (result)
                layer.DecreaseChance();

            return !result;
        }

        [HarmonyPatch(nameof(RoleOptionSetting.IncreaseCount)), HarmonyPrefix]
        public static bool IncreaseCountPrefix(RoleOptionSetting __instance)
        {
            if ((IsInGame() && SettingsPage != 3) || !AmongUsClient.Instance.AmHost)
                return false;

            var result = OptionAttribute.GetOptions<LayerOptionAttribute>().TryFinding(option => option.Setting == __instance, out var layer);

            if (result)
                layer.IncreaseCount();

            return !result;
        }

        [HarmonyPatch(nameof(RoleOptionSetting.DecreaseCount)), HarmonyPrefix]
        public static bool DecreaseCountPrefix(RoleOptionSetting __instance)
        {
            if ((IsInGame() && SettingsPage != 3) || !AmongUsClient.Instance.AmHost)
                return false;

            var result = OptionAttribute.GetOptions<LayerOptionAttribute>().TryFinding(option => option.Setting == __instance, out var layer);

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
                if (!SentOnce && !ClientOptions.NoWelcome)
                {
                    Run("<#5411F8FF>人 Welcome! 人</color>", "Welcome to Town Of Us Reworked! Type /help to get started!");
                    SentOnce = true;
                }

                return;
            }

            if (GameData.Instance.PlayerCount <= 1 || !AmongUsClient.Instance.AmHost || TownOfUsReworked.MCIActive || IsHnS())
                return;

            SendOptionRPC(targetClientId: __instance.myPlayer.OwnerId);
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
                    CallRpc(CustomRPC.Misc, MiscRPC.SyncCustomSettings, 1, "Map", MapSettings.Map);
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
        public static void Postfix(int mapId) => SetMap(mapId);

        [HarmonyPatch(nameof(GameOptionsMapPicker.SelectMap), typeof(MapIconByName))]
        public static void Postfix(MapIconByName mapInfo) => SetMap((int)mapInfo.Name);
    }

    private static readonly string[] Maps = [ "The Skeld", "Mira HQ", "Polus", "ehT dlekS", "Airship", "Fungle", "Submerged", "Level Impostor", "Random" ];
    private static LobbyNotificationMessage MapChangeNotif;

    private static void SetMap(int mapId) => SetMap((MapEnum)mapId);

    public static void SetMap(MapEnum map)
    {
        if (MapSettings.Map == map || IsInGame() || !CustomPlayer.Local)
            return;

        MapSettings.Map = map;
        TownOfUsReworked.NormalOptions.MapId = (byte)map;
        OnValueChanged();

        var changed = $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">Game Map</font> set to <font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{Maps[(int)map]}</font>";

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
    public static bool Prefix() => false;

    public static void CreateViewOptions(Transform parent, int page = -1)
    {
        if (page == -1)
            page = SettingsPage3;

        var type = (MultiMenu)page;

        foreach (var option in OptionAttribute.SortedOptions.Where(x => x.Menus.Contains(type)))
        {
            if (!option.ViewSetting)
            {
                option.ViewSetting = option.Type switch
                {
                    CustomOptionType.Layer => UObject.Instantiate(LayerViewPrefab, parent),
                    CustomOptionType.Alignment or CustomOptionType.Header => UObject.Instantiate(HeaderViewPrefab, parent),
                    _ => UObject.Instantiate(GenericViewPrefab, parent),
                };
            }

            if (option.ViewSetting)
            {
                option.ViewOptionCreated();
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
                    tmp.SetText(CurrentPreset);
            }

            var ml = GameObject.Find("ModeLabel")?.transform?.GetChild(1).gameObject;

            if (ml)
            {
                var tmp = ml.GetComponent<TextMeshPro>();
                var translation = TranslationManager.Translate($"CustomOption.GameMode.{GameModeSettings.GameMode}");

                if (tmp.text != translation)
                    tmp.SetText(translation);
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
                if (option is not IOptionGroup header || !option.ViewSetting)
                    continue;

                var flag = option.Menus.Contains(menu) && option.Active();
                option.ViewSetting.gameObject.SetActive(flag);
                option.ViewUpdate();

                if (!flag)
                {
                    header.GroupMembers.ForEach(x =>
                    {
                        if (x.ViewSetting)
                            x.ViewSetting.gameObject.SetActive(false);
                    });
                    continue;
                }

                option.ViewSetting.transform.localPosition = new(-9.77f, y, -2f);
                __instance.settingsInfo.Add(option.ViewSetting.gameObject);
                y -= 0.06f;
                var members = header.GroupMembers.Where(x =>
                {
                    if (!x.ViewSetting)
                        return false;

                    var flag2 = x.Menus.Contains(menu) && x.Active();
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
            CreateViewOptions(__instance.settingsContainer, 250);
            CreateViewOptions(__instance.settingsContainer, 5);

            foreach (var option in OptionAttribute.GetOptions<AlignmentOptionAttribute>())
            {
                if (!option.ViewSetting)
                    continue;

                option.ViewUpdate();
                option.ViewSetting.transform.localPosition = new(-9.77f, y, -2f);
                option.ViewSetting.gameObject.SetActive(true);
                __instance.settingsInfo.Add(option.ViewSetting.gameObject);
                y -= 0.16f;

                if (!option.Value)
                {
                    y -= 0.35f;
                    continue;
                }

                if (option.GroupHeader != null)
                {
                    if (option.GroupHeader.ViewSetting)
                    {
                        option.GroupHeader.ViewUpdate();
                        option.GroupHeader.ViewSetting.gameObject.SetActive(false);
                    }

                    var members = option.GroupHeader.GroupMembers.Where(x =>
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

                    y -= members.Length > 0 ? 0.62f : 0.35f;
                }
                else
                    y -= 0.543f;

                foreach (var layer in option.GroupMembers.Cast<LayerOptionAttribute>())
                {
                    layer.ViewUpdate();
                    var active = layer.Active();
                    layer.ViewSetting.gameObject.SetActive(active);

                    if (!active)
                    {
                        layer.GroupHeader?.GroupMembers.ForEach(x =>
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
                        var members = layer.GroupHeader.GroupMembers.Where(x =>
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
            __instance.rolesTabButton.buttonText.SetText(TranslationManager.Translate("GameSettings.Layers"));

            var prefabs = new List<MonoBehaviour>();

            if (!ClientOptionsButton && !IsHnS())
            {
                ClientOptionsButton = UObject.Instantiate(__instance.rolesTabButton, __instance.rolesTabButton.transform.parent);
                ClientOptionsButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
                ClientOptionsButton.name = "ClientOptionsTab";
                ClientOptionsButton.buttonText.SetText(TranslationManager.Translate("CustomOption.ClientOptions"));
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
                button.buttonText.SetText("");
                button.name = "TitleButton";
                button.SelectButton(false);
                button.transform.localPosition = new(-0.75f, -0.05f, 0f);
                button.transform.localScale = new(0.7f, 0.7f, 1f);
                button.OverrideOnClickListeners(BlankVoid);

                prefabs.Add(HeaderViewPrefab);
            }

            if (!LayerViewPrefab)
            {
                // Left Box = 0, Title = 1, Right Box = 2, Label = 3, Role Icon = 4, Button = 5, Center Box = 6
                // ┗------------------------┗----------------------------------------------------┗- Value Text = 0, Background = 1, Other BG = 2, Tick = 3, Cross = 4, Label = 5
                LayerViewPrefab = UObject.Instantiate(__instance.infoPanelRoleOrigin, null).DontUnload().DontDestroy();
                LayerViewPrefab.name = "LayerViewPrefab";

                LayerViewPrefab.iconSprite.gameObject.SetActive(false);
                LayerViewPrefab.background.gameObject.SetActive(false);

                var leftBox = LayerViewPrefab.transform.GetChild(0);
                leftBox.name = "Left";
                leftBox.localPosition -= new Vector3(1.2f, 0f, 0f);

                var leftText = UObject.Instantiate(LayerViewPrefab.titleText, leftBox);
                leftText.name = "LeftText";
                leftText.GetComponent<TextTranslatorTMP>()?.Destroy();
                leftText.transform.SetParent(leftBox);

                LayerViewPrefab.titleText.alignment = TextAlignmentOptions.MidlineLeft;
                LayerViewPrefab.titleText.transform.localPosition = new(-3.4703f, -0.0223f, -2f);

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

                LayerViewPrefab.chanceTitle.name = "RightText";
                LayerViewPrefab.chanceTitle.GetComponent<TextTranslatorTMP>()?.Destroy();
                LayerViewPrefab.chanceTitle.transform.SetParent(rightBox);

                var centerBox = UObject.Instantiate(rightBox, rightBox.parent);
                centerBox.name = "Centre";
                centerBox.localPosition = rightBox.localPosition + ((leftBox.localPosition - rightBox.localPosition) / 2);

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

    public static readonly List<PassiveButton> PresetsButtons = [];
    public static GameObject Prev;
    public static GameObject Next;
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

            if (!Save)
            {
                Save = UObject.Instantiate(ButtonPrefab, __instance.StandardPresetButton.transform.parent);
                Save.OverrideOnClickListeners(OptionAttribute.SaveSettings);
                Save.name = "SaveSettingsButton";
                Save.transform.localPosition = new(0.26345f, 2.6164f, -2f);
                Save.buttonText.SetText(TranslationManager.Translate("ImportExport.Save"));
                Save.gameObject.SetActive(true);
            }

            Directory.GetFiles(TownOfUsReworked.Options).Where(x => x.EndsWith(".txt")).Select(x => x.SanitisePath()).ForEach(CreatePresetButton);

            OnPageChanged();
            return false;
        }
    }

    public static void OnPageChanged()
    {
        for (var i = 0; i < PresetsButtons.Count; i++)
        {
            var button = PresetsButtons[i];

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

        Prev.gameObject.SetActive(PresetsButtons.Count > 24);
        Next.gameObject.SetActive(PresetsButtons.Count > 24);
    }

    public static void CreatePresetButton(string presetName)
    {
        var presetButton = CreateButton(presetName, GameSettingMenu.Instance.PresetsTab.StandardPresetButton.transform.parent);
        presetButton.OverrideOnClickListeners(() => OptionAttribute.LoadPreset(presetName, presetButton.buttonText));
        PresetsButtons.Add(presetButton);
    }

    private static PassiveButton CreateButton(string name, Transform parent)
    {
        var button = UObject.Instantiate(ButtonPrefab, parent);
        button.transform.localScale = new(0.5f, 0.84f, 1f);
        button.buttonText.transform.localScale = new(1.4f, 0.9f, 1f);
        button.buttonText.alignment = TextAlignmentOptions.Center;
        button.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
        button.buttonText.SetText(button.name = name);
        return button;
    }
}