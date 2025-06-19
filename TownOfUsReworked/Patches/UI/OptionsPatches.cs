namespace TownOfUsReworked.Patches.UI;

[HarmonyPatch]
public static class SettingsPatches
{
    public static int SettingsPage;
    private static int SettingsPage2;
    public static int CachedPage;
    private static int SettingsPage3;
    public static string CurrentPreset = "Custom";
    public static Vector3 ScrollerLocation;
    public static BaseHeaderOption SelectedSubOptions;

    private static PassiveButton ClientSettingsButton;

    [HarmonyPatch(typeof(GameSettingMenu))]
    public static class GameSettingMenuPatches
    {
        [HarmonyPatch(nameof(GameSettingMenu.Start))]
        public static void Postfix(GameSettingMenu __instance)
        {
            __instance.GameSettingsTab.HideForOnline = new(0);
            __instance.transform.GetChild(3).gameObject.SetActive(false);

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

            __instance.GameSettingsButton.transform.localPosition = pos2 + new Vector3(0f, Mathf.Abs(pos2.y - pos3.y), 0f);
            __instance.RoleSettingsButton.transform.localPosition = pos2;
            __instance.GamePresetsButton.transform.localPosition = pos4;

            __instance.RoleSettingsButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
            __instance.RoleSettingsButton.buttonText.text = TranslationManager.Translate("GameSettings.Layers");

            ClientSettingsButton = UObject.Instantiate(__instance.RoleSettingsButton, __instance.RoleSettingsButton.transform.parent);
            ClientSettingsButton.name = "ClientSettingsButton";
            ClientSettingsButton.buttonText.GetComponent<TextTranslatorTMP>()?.Destroy();
            ClientSettingsButton.buttonText.text = TranslationManager.Translate("CustomOption.ClientOptions");
            ClientSettingsButton.OverrideOnClickListeners(() => __instance.ChangeTab(3, false));
            ClientSettingsButton.OverrideOnMouseOverListeners(() => __instance.ChangeTab(3, true));
            ClientSettingsButton.transform.localPosition = pos3;

            if (!ButtonPrefab)
            {
                ButtonPrefab = UObject.Instantiate(__instance.GamePresetsButton, null).DontDestroy();
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
                __instance.GameSettingsTab.OpenMenu();

            SettingsPage = tabNum switch
            {
                0 => 5,
                2 => 1,
                3 => 2,
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
            RolesButton = null;
            AlignmentsButton = null;
            ReturnButton = null;
        }

        [HarmonyPatch(nameof(GameSettingMenu.Update))]
        public static bool Prefix(GameSettingMenu __instance)
        {
            if (Controller.currentTouchType == 0)
                return false;

            __instance.ToggleLeftSideDarkener(false);
            __instance.ToggleRightSideDarkener(false);
            return false;
        }
    }

    public static RoleOptionSetting LayersPrefab;
    public static NumberOption NumberPrefab;
    public static ToggleOption TogglePrefab;
    public static StringOption StringPrefab;
    public static StringOption MultiSelectPrefab;
    public static BlankBehaviour MultiOptionPrefab;
    public static CategoryHeaderMasked HeaderPrefab;
    public static BlankBehaviour SpecialHeaderPrefab;
    public static CategoryHeaderEditRole AlignmentPrefab;
    private static PassiveButton ButtonPrefab;

    [HarmonyPatch(typeof(GameOptionsMenu))]
    public static class GameOptionsMenuPatches
    {
        [HarmonyPatch(nameof(GameOptionsMenu.Awake))]
        public static void Postfix(GameOptionsMenu __instance) => __instance.Children = new();

        [HarmonyPatch(nameof(GameOptionsMenu.OnEnable))]
        public static bool Prefix(GameOptionsMenu __instance)
        {
            if (IsHnS())
                return true;

            if (SettingsPage != 3)
            {
                __instance.MapPicker.Initialize(20);
                __instance.MapPicker.SetUpFromData(GameManager.Instance.GameSettingsList.MapNameSetting, 20);
            }

            OnValueChanged();
            return false;
        }

        [HarmonyPatch(nameof(GameOptionsMenu.ValueChanged))]
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(RolesSettingsMenu))]
    public static class RolesSettingsMenuPatches
    {
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

            if (!RolesButton)
            {
                RolesButton = UObject.Instantiate(GameSettingMenu.Instance.GamePresetsButton, __instance.RoleChancesSettings.transform);
                RolesButton.OverrideOnClickListeners(AllLayers);
                RolesButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
                RolesButton.buttonText.alignment = TextAlignmentOptions.Center;
                RolesButton.buttonText.text = TranslationManager.Translate("View.AllLayers");
                RolesButton.name = "AllLayers";
                RolesButton.transform.localPosition = new(0.1117f, 1.626f, -2f);
                RolesButton.ClickMask = __instance.ButtonClickMask;
                prefabs.Add(RolesButton);
            }

            if (!AlignmentsButton)
            {
                AlignmentsButton ??= UObject.Instantiate(GameSettingMenu.Instance.GamePresetsButton, __instance.RoleChancesSettings.transform);
                AlignmentsButton.OverrideOnClickListeners(AllAlignments);
                AlignmentsButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
                AlignmentsButton.buttonText.alignment = TextAlignmentOptions.Center;
                AlignmentsButton.buttonText.text = TranslationManager.Translate("View.AllAlignments");
                AlignmentsButton.name = "AllAlignments";
                AlignmentsButton.transform.localPosition = new(3.4727f, 1.626f, -2f);
                AlignmentsButton.ClickMask = __instance.ButtonClickMask;
                prefabs.Add(AlignmentsButton);
            }

            foreach (var mono in prefabs)
            {
                foreach (var obj in mono.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    obj.material.SetInt(PlayerMaterial.MaskLayer, 20);
                    obj.material.SetFloat(StencilComp, 3f);
                    obj.material.SetFloat(Stencil, 20);
                    obj.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }

                foreach (var obj in mono.GetComponentsInChildren<TextMeshPro>(true))
                {
                    obj.fontMaterial.SetFloat(StencilComp, 3f);
                    obj.fontMaterial.SetFloat(Stencil, 20);
                }

                mono.gameObject.SetActive(false);
            }
        }

        [HarmonyPatch(nameof(RolesSettingsMenu.SetQuotaTab))]
        [HarmonyPatch(nameof(RolesSettingsMenu.ValueChanged))]
        public static bool Prefix() => false;

        [HarmonyPatch(nameof(RolesSettingsMenu.Update))]
        public static bool Prefix(RolesSettingsMenu __instance)
        {
            __instance.RoleChancesSettings.transform.localPosition = new(SettingsPage is 3 or 4 ? 0.35f : -0.06f, 0f, -5f);
            return false;
        }

        [HarmonyPatch(nameof(RolesSettingsMenu.OpenChancesTab)), HarmonyPrefix]
        public static bool OpenChancesTabPrefix()
        {
            OnValueChanged();
            return false;
        }
    }

    private static void CreateOptions(Transform parent, Collider2D clickMask, ISystem.List<UiElement> uiElements)
    {
        var type = (MultiMenu)SettingsPage;

        foreach (var header in SelectedSubOptions is null ? Option.SortedOptions.Where(x => x.Menu == type) : [ SelectedSubOptions ])
        {
            if (!header.Setting)
            {
                header.Setting = UObject.Instantiate((MonoBehaviour)(header.Type switch
                {
                    CustomOptionType.Header or CustomOptionType.ListHolder => HeaderPrefab,
                    CustomOptionType.Alignment => AlignmentPrefab,
                    CustomOptionType.LayerHeader or CustomOptionType.AlignmentHeader => SpecialHeaderPrefab,
                    _ =>  throw new ArgumentOutOfRangeException($"There's no header prefab for {header.Type}")
                }), parent);
                header.OptionCreated();
                header.Setting.GetComponentsInChildren<PassiveButton>(true).Do(x => x.ClickMask = clickMask);
            }

            foreach (var option in header.GroupMembers.Where(option => !option.Setting))
            {
                option.Setting = UObject.Instantiate((MonoBehaviour)(option.Type switch
                {
                    CustomOptionType.Number => NumberPrefab,
                    CustomOptionType.String => StringPrefab,
                    CustomOptionType.Layer => LayersPrefab,
                    CustomOptionType.Toggle => TogglePrefab,
                    CustomOptionType.Entry or CustomOptionType.MultiSelect => MultiSelectPrefab,
                    _ => throw new ArgumentOutOfRangeException($"There's no prefab for {option.Type}"),
                }), parent);
                option.OptionCreated();

                if (option.Setting is not OptionBehaviour behaviour)
                    continue;

                behaviour.buttons = behaviour.GetComponentsInChildren<PassiveButton>(true).ToArray();
                behaviour.buttons.Do(x => x.ClickMask = clickMask);
            }
        }

        parent.GetComponentsInChildren<UiElement>(true).Do(uiElements.AddUnique);
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

        __instance.GameSettingsTab.gameObject.SetActive(SettingsPage is 0 or 2);
        __instance.RoleSettingsTab.gameObject.SetActive(SettingsPage is 1 or 3 or 4);
        __instance.PresetsTab.gameObject.SetActive(SettingsPage == 5);
        __instance.GameSettingsButton.SelectButton(SettingsPage == 0);
        __instance.RoleSettingsButton.SelectButton(SettingsPage == 1);
        __instance.GamePresetsButton.SelectButton(SettingsPage == 5);
        __instance.GameSettingsTab.MapPicker.gameObject.SetActive(SettingsPage == 0);

        // Gotta love things randomly becoming null

        if (!ReturnButton)
            ReturnButton = __instance.transform.FindChild("ReturnButton")?.gameObject;

        if (ReturnButton)
            ReturnButton.SetActive(SettingsPage is 3 or 4);

        if (!Next)
            Next = __instance.transform.FindChild("NextPageButton")?.gameObject;

        if (Next)
            Next.SetActive(PresetButtons.Count > 24 && SettingsPage == 5);

        if (!Prev)
            Prev = __instance.transform.FindChild("PrevPageButton")?.gameObject;

        if (Prev)
            Prev.SetActive(PresetButtons.Count > 24 && SettingsPage == 5);

        if (!ClientSettingsButton)
            ClientSettingsButton = __instance.transform.Find("LeftPanel")?.Find("ClientSettingsButton")?.GetComponent<PassiveButton>();

        if (ClientSettingsButton)
            ClientSettingsButton.SelectButton(SettingsPage == 2);

        CreateOptions
        (
            SettingsPage is 1 or 3 or 4 ? __instance.RoleSettingsTab.RoleChancesSettings.transform : __instance.GameSettingsTab.settingsContainer,
            SettingsPage is 1 or 3 or 4 ? __instance.RoleSettingsTab.ButtonClickMask : __instance.GameSettingsTab.ButtonClickMask,
            SettingsPage is 1 or 3 or 4 ? __instance.RoleSettingsTab.ControllerSelectable : __instance.GameSettingsTab.ControllerSelectable
        );

        var menu = (MultiMenu)SettingsPage;

        switch (SettingsPage)
        {
            case 0 or 2:
            {
                var y = SettingsPage == 2 ? 2.063f : 0.863f;
                __instance.GameSettingsTab.Children.Clear();

                if (SettingsPage == 0)
                    __instance.GameSettingsTab.Children.Add(__instance.GameSettingsTab.MapPicker);

                foreach (var header in Option.SortedOptions)
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

                    var flag = header.Menu == menu && header.Active() && header.GroupMembers.Any(x => x.PartiallyActive());
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

                    if (!header.Value)
                        y += 0.2f;

                    foreach (var option in header.GroupMembers.Where(option => option.Setting))
                    {
                        var flag2 = option.Active();
                        option.Setting.gameObject.SetActive(flag2);
                        option.Update();

                        if (!flag2)
                            continue;

                        option.Setting.transform.localPosition = new(0.952f, y, -2f);
                        y -= 0.45f;

                        if (option.Setting is OptionBehaviour setting)
                            __instance.GameSettingsTab.Children.Add(setting);

                        if (option is not IMultiSelectOption multiSelect || !multiSelect.Options.Any())
                            continue;

                        y -= 0.2f;

                        foreach (var (i, button) in multiSelect.Options.Indexed())
                        {
                            var col = i % 3;
                            button.transform.localPosition = new(-1f + (2f * col), y, -2f);

                            if (col == 2)
                                y -= 0.65f;
                        }

                        y -= 0.6f;
                    }
                }

                __instance.GameSettingsTab.scrollBar.SetYBoundsMax(-1.65f - y);
                __instance.GameSettingsTab.InitializeControllerNavigation();
                break;
            }
            case 1 or 3 or 4:
            {
                var y = SettingsPage is 3 or 4 ? 1.515f : 1.36f;
                RolesButton.gameObject.SetActive(SettingsPage == 1);
                AlignmentsButton.gameObject.SetActive(SettingsPage == 1);
                __instance.RoleSettingsTab.advancedSettingChildren.Clear();

                foreach (var header in Option.SortedOptions)
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

                    var flag = header.Menu == menu && (SelectedSubOptions is null || header == SelectedSubOptions) && header.Active() && header.GroupMembers.Any(x => x.PartiallyActive());
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

                    var isHeader = header is HeaderOption;
                    var isLayerHeader = header is LayerHeaderOption or AlignmentHeaderOption;

                    if (!isHeader)
                        y -= 0.1f;

                    header.Setting.transform.localPosition = new(isHeader ? -0.903f : (isLayerHeader ? 3.986f : 4.986f), y, -2f);
                    y -= isHeader ? 0.53f : 0.525f;
                    var get = header.Value;

                    if (isLayerHeader)
                        y += get ? -0.5f : 0.3f;
                    else if (isHeader && !get)
                        y += 0.2f;

                    foreach (var option in header.GroupMembers.Where(option => option.Setting))
                    {
                        var flag2 = option.Active();
                        option.Setting.gameObject.SetActive(flag2);
                        option.Update();

                        if (!flag2)
                            continue;

                        option.Setting.transform.localPosition = new(isHeader || isLayerHeader ? 0.952f : -0.15f, y, -2f);
                        y -= isHeader || isLayerHeader ? 0.45f : 0.43f;

                        if (option.Setting is OptionBehaviour setting)
                            __instance.RoleSettingsTab.advancedSettingChildren.Add(setting);

                        if (option is not IMultiSelectOption multiSelect || !multiSelect.Options.Any())
                            continue;

                        y -= 0.2f;

                        foreach (var (i, button) in multiSelect.Options.Indexed())
                        {
                            var col = i % 3;
                            button.transform.localPosition = new(-1f + (2f * col), y, -2f);

                            if (col == 2)
                                y -= 0.65f;
                        }

                        y -= 0.6f;
                    }
                }

                __instance.RoleSettingsTab.scrollBar.SetYBoundsMax(-y + (SettingsPage >= 5 ? -1.65f : -1.2f));
                __instance.RoleSettingsTab.InitializeControllerNavigation();
                break;
            }
        }
    }

    private static void NextPage(bool increment)
    {
        SettingsPage2 = CycleInt(PresetButtons.Count / 24, 0, SettingsPage2, increment);
        OnPageChanged();
    }

    private static GameObject ReturnButton;

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
        SettingsPage = 3;
        CachedPage = 1;
        var scrollbar = GameSettingMenu.Instance.RoleSettingsTab.scrollBar;
        ScrollerLocation = scrollbar.Inner.transform.localPosition;
        scrollbar.ScrollToTop();
        OnValueChanged();
    }

    private static void AllAlignments()
    {
        SettingsPage = 4;
        CachedPage = 1;
        var scrollbar = GameSettingMenu.Instance.RoleSettingsTab.scrollBar;
        ScrollerLocation = scrollbar.Inner.transform.localPosition;
        scrollbar.ScrollToTop();
        OnValueChanged();
    }

    [HarmonyPatch(typeof(PlayerControl._Start_d__82), nameof(PlayerControl._Start_d__82.MoveNext))]
    public static class PlayerJoinPatch
    {
        private static bool SentOnce;

        public static void Prefix(PlayerControl._Start_d__82 __instance, ref bool __result)
        {
            if (__result || !AmongUsClient.Instance || !LocalPlayer || IsFreePlay() || IsHnS())
                return;

            var player = __instance.__4__this;

            if (!player)
                return;

            player.GetComponent<PlayerControlHandler>().UpdateCurrent();

            Holders.EnsureCount();

            OnValueChanged();
            OnValueChangedView();

            GameStartManagerPatches.PlayersReady[player.PlayerId] = TownOfUsReworked.MciActive || player.PlayerId == 0;

            if (player.AmOwner)
            {
                if (SentOnce || ClientOptions.NoWelcome)
                    return;

                Run("<#5411F8FF>人 Welcome! 人</color>", "Welcome to Town Of Us Reworked! Type /help to get started!");
                SentOnce = true;
                return;
            }

            if (GameData.Instance.PlayerCount < 2 || !AmongUsClient.Instance.AmHost || TownOfUsReworked.MciActive)
                return;

            SendOptionRPC(targetClientId: player.OwnerId);
            var writer = CreateWriter(MiscRpc.PlayerJoinSync, MapSettings.Map);
            var cache = Summary is not null;
            writer.Write(value: cache);

            if (cache)
                writer.Write(Summary);

            cache = CachedFirstDead is not null;
            writer.Write(value: cache);

            if (cache)
                writer.Write(CachedFirstDead);

            writer.SendLate(player.OwnerId);
        }
    }

    [HarmonyPatch(typeof(GameOptionsMapPicker))]
    public static class GameOptionsMapPickerPatches
    {
        [HarmonyPatch(nameof(GameOptionsMapPicker.SetupMapButtons))]
        public static bool Prefix(GameOptionsMapPicker __instance, int maskLayer)
        {
            if (!__instance.AllMapIcons.Any(x => x.Name == MapNames.Dleks))
            {
                var skeld = __instance.AllMapIcons.Find(x => x.Name == MapNames.Skeld);
                __instance.AllMapIcons.Add(new()
                {
                    Name = MapNames.Dleks,
                    MapImage = skeld.MapImage,
                    MapIcon = skeld.MapIcon,
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

            if (__instance.mapButtons is not null)
            {
                __instance.mapButtons.ForEach(x => x.gameObject.Destroy());
                __instance.mapButtons.Clear();
            }
            else
                __instance.mapButtons = new();

            __instance.transform.GetChild(1).localPosition = new(-1.134f, 0.733f, -1);

            for (var k = 0; k < __instance.AllMapIcons.Count; k++)
            {
                var thisVal = __instance.AllMapIcons._items[k];
                var mapButton = UObject.Instantiate(__instance.MapButtonOrigin, Vector3.zero, Quaternion.identity, __instance.transform);
                mapButton.SetImage(thisVal.MapIcon, maskLayer);
                mapButton.MapIcon.Do(x => x.flipX = thisVal.Name == MapNames.Dleks);
                mapButton.transform.localPosition = new(__instance.StartPosX + (k * __instance.SpacingX) - 0.7f, 0.74f, -2f);
                mapButton.name = $"{thisVal.Name}";
                mapButton.MapID = (int)thisVal.Name;
                mapButton.Button.ClickMask = __instance.ButtonClickMask;
                mapButton.Button.OverrideOnClickListeners(() =>
                {
                    if (IsInGame() || !AmongUsClient.Instance.AmHost)
                        return;

                    __instance?.selectedButton?.Button?.SelectButton(false);
                    __instance.selectedButton = mapButton;
                    __instance.selectedButton.Button?.SelectButton(true);
                    __instance.SelectMap(thisVal);
                    CallRpc(MiscRpc.SyncMap, MapSettings.Map);
                });

                if (k > 0)
                {
                    var button = __instance.mapButtons._items[k - 1].Button;
                    mapButton.Button.ControllerNav.selectOnLeft = button;
                    button.ControllerNav.selectOnRight = mapButton.Button;
                }

                __instance.mapButtons.Add(mapButton);

                if (thisVal.Name != (MapNames)MapSettings.Map)
                    continue;

                mapButton.Button.SelectButton(true);
                __instance.SelectMap(mapButton.MapID);
                __instance.selectedButton = mapButton;
            }

            return false;
        }

        [HarmonyPatch(nameof(GameOptionsMapPicker.SelectMap), typeof(int)), HarmonyPrefix]
        public static bool SetMapPrefix(GameOptionsMapPicker __instance, int mapId)
        {
            var mapInfo = __instance.AllMapIcons.Find(m => m.Name == (MapNames)mapId);

            if (mapInfo is not null)
                __instance.SelectMap(mapInfo);
            else
            {
                SetMap((Map)mapId);
                __instance.selectedMapId = mapId;
            }

            return false;
        }

        [HarmonyPatch(nameof(GameOptionsMapPicker.SelectMap), typeof(MapIconByName))]
        public static bool Prefix(GameOptionsMapPicker __instance, MapIconByName mapInfo)
        {
            SetMap((Map)mapInfo.Name);
            __instance.selectedMapId = (int)mapInfo.Name;

            if (__instance.MapImage)
            {
                __instance.MapImage.sprite = mapInfo.MapImage;
                __instance.MapImage.flipX = __instance.selectedMapId == 3;
            }

            if (__instance.MapName)
                __instance.MapName.sprite = mapInfo.NameImage;

            __instance.UpdateValue();
            __instance.OnValueChanged?.Invoke(__instance);
            return false;
        }
    }

    private static readonly string[] Maps = [ "The Skeld", "Mira HQ", "Polus", "ehT dlekS", "Airship", "Fungle", "Submerged", "Level Impostor", "Random" ];
    private static LobbyNotificationMessage MapChangeNotif;

    public static void SetMap(Map map)
    {
        if (IsInGame() || !LocalPlayer || MapSettings.Map == map)
            return;

        MapSettings.Map = map;
        OnValueChanged();

        if (!HudManager.InstanceExists)
            return;

        var changed = $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">Game Map</font> set to <font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{Maps[(int)map]}</font>";

        if (MapChangeNotif)
            MapChangeNotif.UpdateMessage(changed);
        else
        {
            var hud = HUD().Notifier;
            MapChangeNotif = PopNotif(changed, hud.settingsChangeColor, hud.settingsChangeSprite);
        }
    }

    private static void CreateViewOptions(Transform parent, int page = -1)
    {
        if (page == -1)
            page = SettingsPage3;

        var type = (MultiMenu)page;

        foreach (var header in Option.SortedOptions.Where(x => x.Menu == type))
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
                    option.ViewSetting = UObject.Instantiate(option.Type == CustomOptionType.Layer ? LayerViewPrefab : GenericViewPrefab, parent);
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

            var ml = GameObject.Find("ModeLabel")?.transform?.GetChild(1);

            if (!ml)
                return;

            var tmp2 = ml.GetComponent<TextMeshPro>();
            var translation = TranslationManager.Translate($"CustomOption.Mode.{GameModeSettings.GameMode}");

            if (tmp2.text != translation)
                tmp2.text = translation;
        }
    }

    public static void OnValueChangedView(LobbyViewSettingsPane __instance = null)
    {
        if (IsHnS())
            return;

        __instance ??= LobbyInfoPane.Instance.LobbyViewSettingsPane;

        if (!__instance || !ClientOptionsButton)
            return;

        __instance.taskTabButton.SelectButton(SettingsPage3 == 0);
        __instance.rolesTabButton.SelectButton(SettingsPage3 == 1);
        ClientOptionsButton.SelectButton(SettingsPage3 == 2);

        var y = 1.4f;
        CreateViewOptions(__instance.settingsContainer);

        switch (SettingsPage3)
        {
            case 0 or 3:
            {
                var menu = (MultiMenu)SettingsPage3;

                foreach (var option in Option.GetHeaderOptions<HeaderOption>())
                {
                    if (!option.ViewSetting)
                        continue;

                    var flag = option.Menu == menu && option.Active() && option.GroupMembers.Any(x => x.PartiallyActive());
                    option.ViewSetting.gameObject.SetActive(flag);
                    option.ViewUpdate();

                    if (!flag)
                    {
                        option.GroupMembers?.ForEach(x =>
                        {
                            if (x.ViewSetting)
                                x.ViewSetting.gameObject.SetActive(false);
                        });
                        continue;
                    }

                    option.ViewSetting.transform.localPosition = new(-9.77f, y, -2f);
                    __instance.settingsInfo.Add(option.ViewSetting.gameObject);
                    y -= 0.06f;
                    var members = option.GroupMembers?.Where(x =>
                    {
                        if (!x.ViewSetting)
                            return false;

                        var flag2 = x.Active();
                        x.ViewUpdate();
                        x.ViewSetting.gameObject.SetActive(flag2);
                        return flag2;
                    }).ToArray();

                    for (var i = 0; i < members!.Length; i++)
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

                break;
            }
            case 1:
            {
                CreateViewOptions(__instance.settingsContainer, 4);
                CreateViewOptions(__instance.settingsContainer, 3);

                foreach (var option in Option.GetHeaderOptions<AlignmentOption>())
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

                    if (option.GroupHeader is not null)
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

                        if (members!.Length > 0)
                            y += 0.2f;

                        for (var i = 0; i < members!.Length; i++)
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

                    foreach (var layer in option.GroupMembers?.Cast<LayerOption>()!)
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

                        if (layer.GroupHeader is not null)
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

                            for (var i = 0; i < members!.Length; i++)
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

                break;
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

                    SettingsPage3 = 2;
                    OnValueChangedView(__instance);
                });
            }

            ClientOptionsButton?.gameObject?.SetActive(!IsHnS());

            if (!HeaderViewPrefab)
            {
                HeaderViewPrefab = UObject.Instantiate(__instance.categoryHeaderOrigin, null).DontDestroy();
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
                LayerViewPrefab = UObject.Instantiate(__instance.infoPanelRoleOrigin, null).DontDestroy();
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
                GenericViewPrefab = UObject.Instantiate(__instance.infoPanelOrigin, null).DontDestroy();
                GenericViewPrefab.name = "GenericViewPrefab";
                GenericViewPrefab.labelBackground.gameObject.SetActive(false);

                prefabs.Add(GenericViewPrefab);
            }

            if (LayersSet || !prefabs.Any())
                return;

            foreach (var mono in prefabs)
            {
                foreach (var obj in mono.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    obj.material.SetInt(PlayerMaterial.MaskLayer, 61);
                    obj.material.SetFloat(StencilComp, 3f);
                    obj.material.SetFloat(Stencil, 61);
                    obj.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }

                foreach (var obj in mono.GetComponentsInChildren<TextMeshPro>(true))
                {
                    obj.fontMaterial.SetFloat(StencilComp, 3f);
                    obj.fontMaterial.SetFloat(Stencil, 61);
                }

                mono.gameObject.SetActive(false);
            }

            LayersSet = true;
        }

        [HarmonyPatch(nameof(LobbyViewSettingsPane.DrawNormalTab)), HarmonyPrefix]
        public static bool DrawNormalTabPrefix(LobbyViewSettingsPane __instance) => TabPrefix(__instance, 0);

        [HarmonyPatch(nameof(LobbyViewSettingsPane.DrawRolesTab)), HarmonyPrefix]
        public static bool DrawRolesTabPrefix(LobbyViewSettingsPane __instance) => TabPrefix(__instance, 1);

        private static bool TabPrefix(LobbyViewSettingsPane __instance, int page)
        {
            if (IsHnS())
                return true;

            SettingsPage3 = page;
            OnValueChangedView(__instance);
            return false;
        }
    }

    private static readonly List<PassiveButton> PresetButtons = [];
    private static GameObject Prev;
    private static GameObject Next;
    public static PassiveButton Save;
    private static PassiveButton Overwrite;
    public static bool Overwriting;

    [HarmonyPatch(typeof(GamePresetsTab), nameof(GamePresetsTab.Start))]
    public static class GamePresetsStart
    {
        public static bool Prefix(GamePresetsTab __instance)
        {
            __instance.StandardRulesSprites.Do(x => x.gameObject.SetActive(false));
            __instance.AlternateRulesSprites.Do(x => x.gameObject.SetActive(false));
            __instance.SpritesToDesaturate.ForEach(x => x.gameObject.SetActive(false));
            __instance.StandardPresetButton.gameObject.SetActive(false);
            __instance.StandardRulesText.gameObject.SetActive(false);
            __instance.AlternateRulesText.gameObject.SetActive(false);
            __instance.SecondPresetButton.gameObject.SetActive(false);
            __instance.PresetDescriptionText.gameObject.SetActive(false);

            if (!Save)
            {
                Save = UObject.Instantiate(ButtonPrefab, __instance.StandardPresetButton.transform.parent);
                Save.OverrideOnClickListeners(Option.SaveSettings);
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

            Directory.GetFiles(TownOfUsReworked.Options).Where(x => x.EndsWith(".txt")).Select(x => x.SanitisePath()).Do(CreatePresetButton);

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
        presetButton.OverrideOnClickListeners(() => Option.HandlePreset(presetName, presetButton.buttonText));
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

    [HarmonyPatch(typeof(LogicOptions))]
    public static class LogicPatches
    {
        [HarmonyPatch(nameof(LogicOptions.MapId), MethodType.Getter)]
        public static bool Prefix(ref byte __result)
        {
            __result = (byte)MapSettings.Map;
            return false;
        }

        [HarmonyPatch(nameof(LogicOptions.MaxPlayers), MethodType.Getter)]
        public static bool Prefix(ref int __result)
        {
            __result = GameOptions.LobbySize.Value;
            return false;
        }
    }
}