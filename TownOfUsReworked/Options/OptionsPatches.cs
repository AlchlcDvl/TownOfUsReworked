namespace TownOfUsReworked.Options;

public static class SettingsPatches
{
    public static int SettingsPage;
    public static string CurrentPreset = "Custom";
    public static int SettingsPage2;
    public static int CachedPage;
    public static int SettingsPage3;

    private static PassiveButton ClientSettingsButton;

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    public static class OptionsMenuBehaviour_Start
    {
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
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
    public static class OnChangingTabs
    {
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
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Close))]
    public static class OptionsMenuBehaviour_Close
    {
        public static void Prefix()
        {
            if (SettingsPage is 3 or 4 or 5)
                SettingsPage = 0;

            SpawnOptionsCreated = false;
            RLOptionsCreated = false;
            PresetsButtons.Clear();
            LayerOptionsCreated.Keys.ForEach(x => LayerOptionsCreated[x] = false);
            RoleListEntryAttribute.ChoiceButtons.Clear();
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

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Awake))]
    public static class DefinePrefabs1
    {
        private static bool LayersSet;

        public static bool Prefix(GameOptionsMenu __instance) => __instance.name != "ROLE LIST TAB";

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

                var minus = StringPrefab.transform.GetChild(3);
                minus.GetComponentInChildren<TextMeshPro>().text = "<";
                minus.localPosition += new Vector3(0.6f, 0f, 0f);

                var plus = StringPrefab.transform.GetChild(4);
                plus.GetComponentInChildren<TextMeshPro>().text = ">";
                plus.localPosition += new Vector3(1.5f, 0f, 0f);

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
                EntryPrefab.CheckMark.gameObject.Destroy();
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
                newButton.GetComponentInChildren<TextMeshPro>().text = "-";
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

            var prefabs = new List<MonoBehaviour>();

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

                prefabs.Add(LayersPrefab);
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
    }

    private static bool SpawnOptionsCreated;
    private static bool RLOptionsCreated;
    private static readonly Dictionary<int, bool> LayerOptionsCreated = [];

    public static List<MonoBehaviour> CreateOptions(Transform parent)
    {
        var options = new List<MonoBehaviour>();
        var type = (MultiMenu)SettingsPage;

        if (SettingsPage == 4)
        {
            foreach (var layer in RoleListEntryAttribute.GetLayers())
            {
                var name = $"{layer}";

                if (!RoleListEntryAttribute.ChoiceButtons.TryFinding(x => x.name == name, out var button))
                {
                    button = CreateButton(name, parent);
                    button.buttonText.text = TranslationManager.Translate($"RoleList.{layer}");
                    button.OverrideOnClickListeners(() => SetValue(layer));
                    button.transform.GetChild(2).transform.localScale += new Vector3(0.32f, 0f, 0f);
                    button.transform.GetChild(1).localScale = button.transform.GetChild(2).transform.localScale;
                    button.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new(3f, 0.6123f);

                    if (LayerDictionary.TryGetValue(layer, out var entry))
                        button.buttonText.text = $"<color=#{entry.Color.ToHtmlStringRGBA()}>{button.buttonText.text}</color>";

                    RoleListEntryAttribute.ChoiceButtons.Add(button);
                }

                options.Add(button);
            }
        }
        else
        {
            foreach (var option in OptionAttribute.AllOptions.Where(x => x.Menus.Contains(type)))
            {
                if (!option.Setting)
                {
                    var setting = option.Type switch
                    {
                        CustomOptionType.Number => UObject.Instantiate(NumberPrefab, parent),
                        CustomOptionType.String => UObject.Instantiate(StringPrefab, parent),
                        CustomOptionType.Layers => UObject.Instantiate(LayersPrefab, parent),
                        CustomOptionType.Toggle => UObject.Instantiate(TogglePrefab, parent),
                        CustomOptionType.Entry => UObject.Instantiate(EntryPrefab, parent),
                        CustomOptionType.Header => UObject.Instantiate(HeaderPrefab, parent),
                        CustomOptionType.Alignment => UObject.Instantiate(AlignmentPrefab, parent),
                        _ => (MonoBehaviour)null,
                    };
                    option.Setting = setting;
                    option.OptionCreated();
                }

                options.Add(option.Setting);
            }
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
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.SetQuotaTab))]
    public static class RolesSettingsMenuPatches1
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.OpenChancesTab))]
    public static class RolesSettingsMenuPatches2
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
    public static class DisableCustomNotify1
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.ValueChanged))]
    public static class DisableCustomNotify2
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

        RoleListEntryAttribute.ChoiceButtons.ForEach(x => x.gameObject.SetActive(false));

        foreach (var opt in OptionAttribute.AllOptions)
        {
            if (opt.Setting)
                opt.Setting.gameObject.SetActive(false);
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

    private static void NextPage(bool increment)
    {
        SettingsPage2 = CycleInt(PresetsButtons.Count / 24, 0, SettingsPage2, increment);
        OnPageChanged();
    }

    private static void SetValue(LayerEnum value)
    {
        OptionAttribute.GetOptions<RoleListEntryAttribute>().Find(x => x.ID == RoleListEntryAttribute.SelectedEntry).Set(value);
        Return();
    }

    public static GameObject ReturnButton;

    private static void Return()
    {
        SettingsPage = CachedPage;
        GameSettingMenu.Instance.RoleSettingsTab.scrollBar.ScrollToTop();
        GameSettingMenu.Instance.GameSettingsTab.scrollBar.ScrollToTop();
        SettingsPage2 = 0;
        RoleListEntryAttribute.SelectedEntry = "";
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
        var result = OptionAttribute.AllOptions.TryFinding(option => option.Setting == opt, out var customOption);
        customOption?.OptionCreated();
        return !result;
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
            if (IsInGame())
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

    [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.FixedUpdate))]
    public static class OverrideToggles
    {
        public static bool Prefix(ToggleOption __instance) => !OptionAttribute.AllOptions.Any(option => option.Setting == __instance);
    }

    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
    public static class NumberOptionPatchIncrease
    {
        public static bool Prefix(NumberOption __instance)
        {
            if (IsInGame())
                return false;

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
            if (IsInGame())
                return false;

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
            if (IsInGame())
                return false;

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
            if (IsInGame())
                return false;

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
            if (IsInGame())
                return false;

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
            if (IsInGame())
                return false;

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
            if (IsInGame())
                return false;

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
            if (IsInGame())
                return false;

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

            if (AllPlayers().Count <= 1 || !AmongUsClient.Instance.AmHost || TownOfUsReworked.MCIActive || IsHnS())
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
                    if (IsInGame())
                        return;

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

    private static void SetMap(int mapId) => SetMap((MapEnum)mapId);

    public static void SetMap(MapEnum map)
    {
        if (MapSettings.Map == map || IsInGame())
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
    public static class DisableVanilaNotifs
    {
        public static bool Prefix() => false;
    }

    public static List<MonoBehaviour> CreateViewOptions(Transform parent, int page = -1)
    {
        if (page == -1)
            page = SettingsPage3;

        var options = new List<MonoBehaviour>();
        var type = (MultiMenu)page;

        foreach (var option in OptionAttribute.AllOptions.Where(x => x.Menus.Contains(type)))
        {
            if (!option.ViewSetting)
            {
                var setting = option.Type switch
                {
                    CustomOptionType.Layers => UObject.Instantiate(LayerViewPrefab, parent),
                    CustomOptionType.Alignment or CustomOptionType.Header => UObject.Instantiate(HeaderViewPrefab, parent),
                    _ => (MonoBehaviour)UObject.Instantiate(GenericViewPrefab, parent),
                };
                option.ViewSetting = setting;
                option.ViewOptionCreated();
            }

            options.Add(option.ViewSetting);
        }

        return options;
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.DrawNormalTab))]
    public static class OverrideNormalViewSettingsTab
    {
        public static bool Prefix(LobbyViewSettingsPane __instance)
        {
            if (IsHnS())
                return true;

            SettingsPage3 = 0;
            OnValueChangedView(__instance);
            return false;
        }
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.DrawRolesTab))]
    public static class OverrideRolesViewSettingsTab
    {
        public static bool Prefix(LobbyViewSettingsPane __instance)
        {
            if (IsHnS())
                return true;

            SettingsPage3 = 1;
            OnValueChangedView(__instance);
            return false;
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

        var num = 1.4f;
        CreateViewOptions(__instance.settingsContainer);

        foreach (var option in OptionAttribute.AllOptions)
        {
            if (option.ViewSetting)
                option.ViewSetting.gameObject.SetActive(false);
        }

        if (SettingsPage3 is 0 or 3)
        {
            var menu = (MultiMenu)SettingsPage3;

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
                num += 0.59f;

                for (var i = 0; i < members.Length; i++)
                {
                    if (i % 2 == 0)
                        num -= 0.59f;

                    var optionn = members[i];
                    optionn.ViewSetting.transform.localPosition = new(i % 2 == 0 ? -8.95f : -3f, num, -2f);
                    __instance.settingsInfo.Add(optionn.ViewSetting.gameObject);
                }

                num -= members.Length > 0 ? 0.62f : 0.25f;
            }
        }
        else if (SettingsPage == 1)
        {
            foreach (var option in OptionAttribute.GetOptions<AlignsOptionAttribute>())
            {
                CreateViewOptions(__instance.settingsContainer, (int)option.Alignment + 6);

                option.ViewSetting.transform.localPosition = new(-9.77f, num, -2f);
                __instance.settingsInfo.Add(option.ViewSetting.gameObject);
                num -= 0.85f;

                if (option.GroupHeader != null)
                {
                    option.GroupHeader.ViewSetting.gameObject.SetActive(false);

                    var members = option.GroupHeader.GroupMembers.Where(x =>
                    {
                        if (!x.ViewSetting)
                            return false;

                        var flag2 = x.Active();
                        x.ViewSetting.gameObject.SetActive(flag2);
                        return flag2;
                    }).ToArray();
                    option.GroupHeader.GroupMembers.Except(members).ForEach(x =>
                    {
                        if (x.ViewSetting)
                            x.ViewSetting.gameObject.SetActive(false);
                    });

                    for (var i = 0; i < members.Length; i++)
                    {
                        if (i % 2 == 0)
                            num -= 0.59f;

                        var optionn = members[i];
                        optionn.ViewSetting.transform.localPosition = new(i % 2 == 0 ? -8.95f : -3f, num, -2f);
                        __instance.settingsInfo.Add(optionn.ViewSetting.gameObject);
                    }

                    num -= members.Length > 0 ? 0.62f : 0.25f;
                }

                var layers = option.GroupMembers.Cast<LayersOptionAttribute>();

                foreach (var layer in layers)
                {
                    CreateViewOptions(__instance.settingsContainer, (int)layer.Layer + 6);

                    layer.ViewSetting.transform.localPosition = new(-9.77f, num, -2f);
                    __instance.settingsInfo.Add(layer.ViewSetting.gameObject);
                    num -= 0.85f;

                    if (layer.GroupHeader != null)
                    {
                        layer.GroupHeader.ViewSetting.gameObject.SetActive(false);

                        var members2 = layer.GroupHeader.GroupMembers.Where(x =>
                        {
                            if (!x.ViewSetting)
                                return false;

                            var flag2 = x.Active();
                            x.ViewSetting.gameObject.SetActive(flag2);
                            return flag2;
                        }).ToArray();
                        layer.GroupHeader.GroupMembers.Except(members2).ForEach(x =>
                        {
                            if (x.ViewSetting)
                                x.ViewSetting.gameObject.SetActive(false);
                        });

                        for (var i = 0; i < members2.Length; i++)
                        {
                            if (i % 2 == 0)
                                num -= 0.59f;

                            var optionn = members2[i];
                            optionn.ViewSetting.transform.localPosition = new(i % 2 == 0 ? -8.95f : -3f, num, -2f);
                            __instance.settingsInfo.Add(optionn.ViewSetting.gameObject);
                        }

                        num -= members2.Length > 0 ? 0.62f : 0.25f;
                    }
                }
            }
        }

        __instance.scrollBar.SetYBoundsMax(-num);
    }

    private static CategoryHeaderMasked HeaderViewPrefab;
    private static ViewSettingsInfoPanelRoleVariant LayerViewPrefab;
    private static ViewSettingsInfoPanel GenericViewPrefab;

    private static PassiveButton ClientOptionsButton;

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Awake))]
    public static class DefinePrefabs3
    {
        private static bool LayersSet;

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
            else if (ClientOptionsButton)
                ClientOptionsButton.gameObject.SetActive(!IsHnS());

            if (!HeaderViewPrefab)
            {
                HeaderViewPrefab = UObject.Instantiate(__instance.categoryHeaderOrigin, null).DontUnload().DontDestroy();
                HeaderViewPrefab.transform.localScale = Vector3.one;
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
                LayerViewPrefab = UObject.Instantiate(__instance.infoPanelRoleOrigin, null).DontUnload().DontDestroy();
                LayerViewPrefab.transform.localScale = Vector3.one;
                LayerViewPrefab.name = "LayerViewPrefab";
                LayerViewPrefab.iconSprite.gameObject.SetActive(false);

                prefabs.Add(LayerViewPrefab);
            }

            if (!GenericViewPrefab)
            {
                GenericViewPrefab = UObject.Instantiate(__instance.infoPanelOrigin, null).DontUnload().DontDestroy();
                GenericViewPrefab.transform.localScale = Vector3.one;
                GenericViewPrefab.name = "GenericViewPrefab";

                prefabs.Add(GenericViewPrefab);
            }

            if (!LayersSet)
            {
                foreach (var mono in prefabs)
                {
                    if (mono.name != "GenericViewPrefab")
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
                    }

                    mono.gameObject.SetActive(false);
                }

                LayersSet = true;
            }
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
                Save.buttonText.text = TranslationManager.Translate("ImportExport.Save");
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
        button.buttonText.text = button.name = name;
        return button;
    }
}