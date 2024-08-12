namespace TownOfUsReworked.Options2;

public static class SettingsPatches
{
    public static int SettingsPage;
    public static LayerEnum ActiveLayer = LayerEnum.None;
    public static string CurrentPreset = "Custom";

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
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
    public static class OnChangingTabs
    {
        public static void Postfix(GameSettingMenu __instance, ref int tabNum, ref bool previewOnly)
        {
            if (previewOnly)
                return;

            SettingsPage = tabNum switch
            {
                0 => 2,
                2 => 1,
                _ => 0
            };
            ActiveLayer = LayerEnum.None;
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

    public static List<MonoBehaviour> CreateOptions(Transform parent)
    {
        var options = new List<MonoBehaviour>();
        var type = (MultiMenu2)SettingsPage;

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

            options.Add(option.Setting);
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

                var newButton = UObject.Instantiate(StringPrefab.transform.GetChild(3), BaseHeaderPrefab.transform);
                newButton.localPosition += new Vector3(1.5f, -0.14f, 0f);
                newButton.localScale *= 0.7f;
                newButton.GetComponentInChildren<TextMeshPro>().text = "-";
                newButton.GetComponent<PassiveButton>().OverrideOnClickListeners(BlankVoid);
                newButton.name = "Collapse";

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
            __instance.roleChances = new();
            __instance.roleTabs = new();

            var bg = __instance.transform.FindChild("Scroller").FindChild("MaskBg");
            bg.localPosition += new Vector3(0f, 0.4f, 0f);
            bg.localScale += new Vector3(0f, 0.8f, 0f);

            __instance.ButtonClickMask.transform.localScale += new Vector3(0f, 0.5f, 0f);

            if (!LayersPrefab)
            {
                // Title = 0, Role # = 1, Chance % = 2, Background = 3, Divider = 4, Cog = 5, Unique = 6, Active = 7
                //            ┗------------┗----------- Value = 0, - = 1, + = 2, Value Box = 3
                LayersPrefab = UObject.Instantiate(__instance.roleOptionSettingOrigin, null).DontUnload().DontDestroy();
                LayersPrefab.name = "CustomLayersOptionPrefab";
                LayersPrefab.titleText.alignment = TextAlignmentOptions.Left;
                LayersPrefab.buttons = LayersPrefab.GetComponentsInChildren<PassiveButton>().ToArray();
                LayersPrefab.transform.GetChild(0).localPosition += new Vector3(-0.1f, 0f, 0f);
                LayersPrefab.transform.GetChild(3).localScale += new Vector3(0.001f, 0f, 0f); // WHY THE FUCK IS THE BACKGROUND EVER SO SLIGHTLY SMALLER THAN THE HEADER?!

                var newButton = UObject.Instantiate(LayersPrefab.buttons[0], LayersPrefab.transform);
                newButton.name = "LayersSubSettingsButton";
                newButton.transform.localPosition = new(0.3419f, -0.2582f, -2f);
                newButton.transform.GetChild(0).gameObject.Destroy();
                newButton.transform.FindChild("InactiveSprite").GetComponent<SpriteRenderer>().sprite = GetSprite("Cog");
                newButton.transform.FindChild("ActiveSprite").GetComponent<SpriteRenderer>().sprite = GetSprite("CogActive");
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

            if (!LayerHeaderPrefab)
            {
                LayerHeaderPrefab = UObject.Instantiate(__instance.categoryHeaderEditRoleOrigin, null).DontUnload().DontDestroy();
                LayerHeaderPrefab.name = "CustomHeaderOptionLayerPrefab";

                var quota = LayerHeaderPrefab.transform.GetChild(2);

                var single = UObject.Instantiate(quota.GetChild(3), quota);
                single.localScale += new Vector3(0.5f, 0f, 0f);
                single.localPosition += new Vector3(-0.956f, 0f, 0f);
                single.name = "SingleBG";

                var count = quota.GetChild(1).GetComponent<TextMeshPro>();

                var active = UObject.Instantiate(count, quota);
                active.name = "Active";
                active.GetComponent<TextTranslatorTMP>().Destroy();
                active.text = "Is Active?";

                var unique = UObject.Instantiate(count, quota);
                unique.name = "Unique";
                unique.GetComponent<TextTranslatorTMP>().Destroy();
                unique.text = "Is Unique?";

                var newButton = UObject.Instantiate(LayersPrefab.buttons[0], LayerHeaderPrefab.transform);
                newButton.name = "Collapse";
                newButton.transform.localPosition = new(-5.639f, -0.44f, -2f);
                newButton.GetComponentInChildren<TextMeshPro>().text = "-";
                newButton.OverrideOnClickListeners(BlankVoid);
                newButton.transform.localScale *= 0.7f;

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

            __instance.MapPicker.Initialize(20);
            __instance.MapPicker.SetUpFromData(GameManager.Instance.GameSettingsList.MapNameSetting, 20);

            // TODO: Make a better fix for this for example caching the options or creating it ourself.
            // AD Says: Done, kinda.
            var behaviours = CreateOptions(__instance.settingsContainer);

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
            __instance.AllButton.gameObject.SetActive(false);
            __instance.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            var behaviours = CreateOptions(__instance.RoleChancesSettings.transform);

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

    public static void OnValueChanged(GameSettingMenu __instance = null)
    {
        if (IsHnS)
            return;

        __instance ??= GameSettingMenu.Instance;

        if (!__instance)
            return;

        __instance.GameSettingsTab.gameObject.SetActive(SettingsPage is 0 or 3);
        __instance.RoleSettingsTab.gameObject.SetActive(SettingsPage is 1 or 5);
        __instance.PresetsTab.gameObject.SetActive(SettingsPage == 2);
        __instance.GameSettingsButton.SelectButton(SettingsPage == 0);
        __instance.RoleSettingsButton.SelectButton(SettingsPage == 1);
        __instance.GamePresetsButton.SelectButton(SettingsPage == 2);

        if (!ReturnButton)
            ReturnButton = __instance.transform.FindChild("ReturnButton")?.gameObject; // For some reason this damn thing is becoming null even though it definitely exists???

        if (ReturnButton)
            ReturnButton.SetActive(SettingsPage == 5);

        if (SettingsPage is 0 or 3)
        {
            var y = 0.713f;

            try
            {
                __instance.GameSettingsTab.Children.Clear();
            } catch {}

            __instance.GameSettingsTab.Children = new();
            __instance.GameSettingsTab.Children.Add(__instance.GameSettingsTab.MapPicker);

            foreach (var option in OptionAttribute.AllOptions)
            {
                if (option.Setting)
                {
                    if (option.Menu != (MultiMenu2)SettingsPage || !option.Active())
                    {
                        option.Setting.gameObject.SetActive(false);
                        continue;
                    }

                    var isHeader = option is HeaderOptionAttribute;
                    option.Setting.gameObject.SetActive(true);
                    option.Setting.transform.localPosition = new(isHeader ? -0.903f : 0.952f, y, -2f);
                    y -= isHeader ? 0.63f : 0.45f;
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
        else if (SettingsPage is 1 or 5)
        {
            var y = SettingsPage == 5 ? 1.513f : 1.96f;
            __instance.RoleSettingsTab.quotaHeader.gameObject.SetActive(false);

            try
            {
                __instance.RoleSettingsTab.advancedSettingChildren.Clear();
            } catch {}

            __instance.RoleSettingsTab.advancedSettingChildren = new();

            foreach (var option in OptionAttribute.AllOptions)
            {
                if (option.Setting)
                {
                    if (option.Menu != (MultiMenu2)SettingsPage || !option.Active())
                    {
                        option.Setting.gameObject.SetActive(false);
                        continue;
                    }

                    var isHeader = option is HeaderOptionAttribute;

                    if (SettingsPage == 5)
                    {
                        option.Setting.transform.localPosition = new(isHeader ? -0.903f : 0.952f, y, -2f);
                        y -= isHeader ? 0.63f : 0.45f;
                    }
                    else
                    {
                        if (isHeader)
                            y -= 0.1f;

                        option.Setting.transform.localPosition = new(isHeader ? 4.986f : -0.15f, y, -2f);
                        y -= isHeader ? 0.496f : 0.404f;
                    }

                    option.Setting.gameObject.SetActive(true);
                    option.Update();

                    if (option.Setting is OptionBehaviour setting)
                        __instance.RoleSettingsTab.advancedSettingChildren.Add(setting);
                }
            }

            __instance.RoleSettingsTab.scrollBar.SetYBoundsMax(-y + (SettingsPage == 5 ? -1.65f : 0f));
            __instance.RoleSettingsTab.InitializeControllerNavigation();
        }
    }

    public static GameObject ReturnButton;

    private static void Return()
    {
        ActiveLayer = LayerEnum.None;
        SettingsPage = 1;
        OnValueChanged();
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Update))]
    public static class RolesSettingsMenu_Update
    {
        public static void Postfix(RolesSettingsMenu __instance) => __instance.RoleChancesSettings.transform.localPosition = new(SettingsPage == 5 ? 0.35f : 0f, 0f, -5f);
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
        MapSettings.Map = (MapEnum)mapId;
        OnValueChanged();

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

    // public static CategoryHeaderMasked ViewHeaderPrefab;

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.DrawNormalTab))]
    public static class OverrideNormalViewSettingsTab
    {
        public static bool Prefix(LobbyViewSettingsPane __instance)
        {
            var num = 1.44f;
            var num2 = -8.95f;

            foreach (var header in OptionAttribute.AllOptions.Where(x => x is HeaderOptionAttribute && x.Menu is MultiMenu2.Main or MultiMenu2.Client).Cast<HeaderOptionAttribute>())
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

    // public static void OnValueChanged(LobbyViewSettingsPane __instance)
    // {
    //     var num = 1.44f;
    //     var num2 = -8.95f;

    //     foreach (var header in OptionAttribute.AllOptions.Where(x => x is HeaderOptionAttribute && x.Menu == MultiMenu2.Main).Cast<HeaderOptionAttribute>())
    //     {
    //         var categoryHeaderMasked = UObject.Instantiate(__instance.categoryHeaderOrigin, __instance.settingsContainer);
    //         categoryHeaderMasked.transform.localScale = Vector3.one;
    //         categoryHeaderMasked.transform.localPosition = new(-9.77f, num, -2f);
    //         header.ViewSetting = categoryHeaderMasked;
    //         header.ViewOptionCreated();
    //         __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
    //         num -= 0.85f;

    //         for (var i = 0; i < header.GroupMembers.Length; i++)
    //         {
    //             var option = header.GroupMembers[i];
    //             var viewSettingsInfoPanel = UObject.Instantiate(__instance.infoPanelOrigin, __instance.settingsContainer);
    //             viewSettingsInfoPanel.transform.localScale = Vector3.one;

    //             if (i % 2 == 0)
    //             {
    //                 num2 = -8.95f;

    //                 if (i > 0)
    //                     num -= 0.59f;
    //             }
    //             else
    //                 num2 = -3f;

    //             viewSettingsInfoPanel.transform.localPosition = new(num2, num, -2f);
    //             option.ViewSetting = viewSettingsInfoPanel;
    //             option.ViewOptionCreated();
    //             __instance.settingsInfo.Add(viewSettingsInfoPanel.gameObject);
    //         }

    //         num -= 0.59f;
    //     }

    //     __instance.scrollBar.SetYBoundsMax(-num);
    // }

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

            // var presets = Directory.EnumerateFiles(TownOfUsReworked.Options).OrderBy(x => x).Where(x => !x.EndsWith(".json")).Select(x => x.SanitisePath());
            var saveButton = UObject.Instantiate(GameSettingMenu.Instance.GamePresetsButton, __instance.StandardPresetButton.transform.parent);
            saveButton.OverrideOnClickListeners(OptionAttribute.SaveSettings);
            saveButton.name = "SaveSettingsButton";
            saveButton.transform.localPosition = new(0.6909f, 2.6164f, -2f);
            saveButton.transform.localScale = new(0.64f, 0.84f, 1f);
            var saveText = saveButton.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>();
            saveText.transform.localPosition = new(0.0115f, 0.0208f, -1f);
            saveText.transform.localScale = new(1.4f, 0.9f, 1f);
            saveText.alignment = TextAlignmentOptions.Center;
            saveText.GetComponent<TextTranslatorTMP>().Destroy(); // Yeah because this darn thing exists
            saveText.text = "Save Settings"; // WHY ARE THE TMPS NOT CHANGING TEXTS EVEN THROUGH FUCKING COROUTINES AAAAAAAAAAAAAAAAAAAAAAAA
            return false;
        }
    }
}