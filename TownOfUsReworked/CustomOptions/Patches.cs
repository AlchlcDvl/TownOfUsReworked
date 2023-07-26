namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public static class SettingsPatches
    {
        private static readonly string[] Menus = { "Global", "Crew", "Neutral", "Intruder", "Syndicate", "Modifier", "Objectifier", "Ability", "Role List" };
        private static readonly List<GameObject> MenuG = new();
        private static readonly List<SpriteRenderer> MenuS = new();
        //private static readonly List<RolesSettingsMenu> MenuR = new();

        public static Export ExportButton;
        public static Import ImportButton;
        public static Preset PresetButton;

        private static List<OptionBehaviour> CreateOptions(GameOptionsMenu __instance, MultiMenu type)
        {
            var options = new List<OptionBehaviour>();
            var togglePrefab = UObject.FindObjectOfType<ToggleOption>();
            var numberPrefab = UObject.FindObjectOfType<NumberOption>();
            var keyValPrefab = UObject.FindObjectOfType<KeyValueOption>();
            var rolePrefab = UObject.FindObjectOfType<RoleOptionSetting>(true);

            if (type == MultiMenu.main)
            {
                if (ExportButton.Setting != null)
                {
                    ExportButton.Setting.gameObject.SetActive(true);
                    options.Add(ExportButton.Setting);
                }
                else
                {
                    var toggle = UObject.Instantiate(togglePrefab, togglePrefab.transform.parent);
                    toggle.transform.GetChild(2).gameObject.SetActive(false);
                    toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);
                    ExportButton.Setting = toggle;
                    ExportButton.OptionCreated();
                    options.Add(toggle);
                }

                if (ImportButton.Setting != null)
                {
                    ImportButton.Setting.gameObject.SetActive(true);
                    options.Add(ImportButton.Setting);
                }
                else
                {
                    var toggle = UObject.Instantiate(togglePrefab, togglePrefab.transform.parent);
                    toggle.transform.GetChild(2).gameObject.SetActive(false);
                    toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);
                    ImportButton.Setting = toggle;
                    ImportButton.OptionCreated();
                    options.Add(toggle);
                }

                if (PresetButton.Setting != null)
                {
                    PresetButton.Setting.gameObject.SetActive(true);
                    options.Add(PresetButton.Setting);
                }
                else
                {
                    var toggle = UObject.Instantiate(togglePrefab, togglePrefab.transform.parent);
                    toggle.transform.GetChild(2).gameObject.SetActive(false);
                    toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);
                    PresetButton.Setting = toggle;
                    PresetButton.OptionCreated();
                    options.Add(toggle);
                }
            }

            options.AddRange(__instance.Children);

            foreach (var option in CustomOption.AllOptions.Where(x => x.Menu == type))
            {
                if (option.Setting != null)
                {
                    option.Setting.gameObject.SetActive(true);
                    options.Add(option.Setting);
                    continue;
                }

                switch (option.Type)
                {
                    case CustomOptionType.Number:
                        var number = UObject.Instantiate(numberPrefab, numberPrefab.transform.parent);
                        option.Setting = number;
                        options.Add(number);
                        break;

                    case CustomOptionType.String:
                        var str = UObject.Instantiate(keyValPrefab, keyValPrefab.transform.parent);
                        option.Setting = str;
                        options.Add(str);
                        break;

                    case CustomOptionType.Layers:
                        var layer = UObject.Instantiate(rolePrefab, keyValPrefab.transform.parent);
                        layer.transform.GetChild(8).gameObject.SetActive(false);
                        option.Setting = layer;
                        options.Add(layer);
                        break;

                    case CustomOptionType.Toggle:
                    case CustomOptionType.Nested:
                    case CustomOptionType.Button:
                    case CustomOptionType.Header:
                    case CustomOptionType.Entry:
                        var toggle = UObject.Instantiate(togglePrefab, togglePrefab.transform.parent);

                        if (option.Type != CustomOptionType.Toggle)
                            toggle.transform.GetChild(2).gameObject.SetActive(false);

                        if (option.Type == CustomOptionType.Header)
                            toggle.transform.GetChild(1).gameObject.SetActive(false);
                        else if (option.Type is CustomOptionType.Button or CustomOptionType.Nested)
                            toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);

                        option.Setting = toggle;
                        options.Add(toggle);
                        break;
                }

                option.OptionCreated();
            }

            return options;
        }

        private static bool OnEnable(OptionBehaviour opt)
        {
            if (opt == ExportButton.Setting)
            {
                ExportButton.OptionCreated();
                return false;
            }

            if (opt == ImportButton.Setting)
            {
                ImportButton.OptionCreated();
                return false;
            }

            if (opt == PresetButton.Setting)
            {
                PresetButton.OptionCreated();
                return false;
            }

            //Works but may need to change to gameObject.name check
            var customOption = CustomOption.AllOptions.Find(option => option.Setting == opt);

            if (customOption == null)
                customOption = ExportButton.SlotButtons.Find(option => option.Setting == opt);

            if (customOption == null)
                customOption = ImportButton.SlotButtons.Find(option => option.Setting == opt);

            if (customOption == null)
                customOption = PresetButton.SlotButtons.Find(option => option.Setting == opt);

            if (customOption == null)
                customOption = CustomNestedOption.AllCancelButtons.Find(option => option.Setting == opt);

            if (customOption == null)
                customOption = CustomLayersOption.AllRoleOptions.Find(option => option.Setting == opt);

            if (customOption == null)
                customOption = RoleListEntryOption.EntryButtons.Find(option => option.Setting == opt);

            if (customOption == null)
                return true;

            customOption.OptionCreated();
            return false;
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
        public static class OptionsMenuBehaviour_Start
        {
            public static void Postfix(GameSettingMenu __instance)
            {
                if (IsHnS)
                    return;

                var obj = __instance.RolesSettingsHightlight.gameObject.transform.parent.parent;
                var diff = (0.7f * Menus.Length) - 2;
                obj.transform.localPosition = new(obj.transform.localPosition.x - diff, obj.transform.localPosition.y, obj.transform.localPosition.z);
                __instance.GameSettingsHightlight.gameObject.transform.parent.localPosition = new(obj.transform.localPosition.x, obj.transform.localPosition.y,
                    obj.transform.localPosition.z);
                MenuG.Clear();
                MenuS.Clear();

                for (var index = 0; index < Menus.Length; index++)
                {
                    var touSettings = UObject.Instantiate(__instance.RegularGameSettings, __instance.RegularGameSettings.transform.parent);
                    touSettings.SetActive(false);
                    touSettings.name = $"ToU-Rew{Menus[index]}Settings";

                    var gameGroup = touSettings.transform.FindChild("GameGroup");
                    var title = gameGroup?.FindChild("Text");

                    if (title != null)
                    {
                        title.GetComponent<TextTranslatorTMP>().Destroy();
                        title.GetComponent<TextMeshPro>().m_text = $"{Menus[index]} Settings";
                    }

                    var sliderInner = gameGroup?.FindChild("SliderInner");

                    if (sliderInner != null)
                        sliderInner.GetComponent<GameOptionsMenu>().name = $"ToU-Rew{Menus[index]}OptionsMenu";

                    var ourSettingsButton = UObject.Instantiate(obj.gameObject, obj.transform.parent);
                    ourSettingsButton.transform.localPosition = new(obj.localPosition.x + (0.7f * (index + 1)), obj.localPosition.y, obj.localPosition.z);
                    ourSettingsButton.name = $"ToU-Rew{Menus[index]}Tab";

                    var hatButton = ourSettingsButton.transform.GetChild(0); //TODO: Change to FindChild I guess to be sure
                    var hatIcon = hatButton.GetChild(0);
                    var tabBackground = hatButton.GetChild(1);

                    hatIcon.GetComponent<SpriteRenderer>().sprite = GetSprite(GetSettingSprite(index));
                    MenuG.Add(touSettings);
                    MenuS.Add(tabBackground.GetComponent<SpriteRenderer>());

                    var passiveButton = tabBackground.GetComponent<PassiveButton>();
                    passiveButton.OnClick = new();
                    passiveButton.OnClick.AddListener(ToggleButton(index));
                }

                __instance.RegularGameSettings.SetActive(false);
                __instance.RolesSettings.gameObject.SetActive(false);
                __instance.GameSettingsHightlight.gameObject.SetActive(false);
                __instance.RolesSettingsHightlight.gameObject.SetActive(false);
                __instance.GameSettingsHightlight.enabled = false;
                __instance.RolesSettingsHightlight.enabled = false;
                __instance.RolesSettingsHightlight.gameObject.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                __instance.GameSettingsHightlight.gameObject.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                __instance.RolesSettingsHightlight.gameObject.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().gameObject.SetActive(false);
                __instance.GameSettingsHightlight.gameObject.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().gameObject.SetActive(false);
                ToggleButtonVoid(GameSettings.SettingsPage);
            }

            private static string GetSettingSprite(int index) => index switch
            {
                1 => "Crew",
                2 => "Neutral",
                3 => "Intruders",
                4 => "Syndicate",
                5 => "Modifiers",
                6 => "Objectifiers",
                7 => "Abilities",
                8 => "RoleLists",
                _ => "SettingsButton"
            };
        }

        public static Action ToggleButton(int id) => new(() => ToggleButtonVoid(id));

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Update))]
        public static class OptionsMenuBehaviour_Update
        {
            public static void Postfix(GameSettingMenu __instance)
            {
                if (IsHnS)
                    return;

                __instance.RegularGameSettings.SetActive(false);
                __instance.RolesSettings.gameObject.SetActive(false);
                __instance.GameSettingsHightlight.gameObject.SetActive(false);
                __instance.RolesSettingsHightlight.gameObject.SetActive(false);
                __instance.GameSettingsHightlight.enabled = false;
                __instance.RolesSettingsHightlight.enabled = false;
                __instance.RolesSettingsHightlight.gameObject.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                __instance.GameSettingsHightlight.gameObject.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                __instance.RolesSettingsHightlight.gameObject.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().gameObject.SetActive(false);
                __instance.GameSettingsHightlight.gameObject.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().gameObject.SetActive(false);
                var cachedPage = GameSettings.SettingsPage;
                GameSettings.UpdatePageNumber();

                if (cachedPage != GameSettings.SettingsPage)
                    ToggleButtonVoid(GameSettings.SettingsPage);

                if (TownOfUsReworked.NormalOptions.MaxPlayers != CustomGameOptions.LobbySize)
                {
                    TownOfUsReworked.NormalOptions.MaxPlayers = CustomGameOptions.LobbySize;
                    GameStartManager.Instance.LastPlayerCount = CustomGameOptions.LobbySize;
                    GameOptionsManager.Instance.currentNormalGameOptions = TownOfUsReworked.NormalOptions;
                    CustomPlayer.Local.RpcSyncSettings(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameOptionsManager.Instance.currentGameOptions));
                }
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Close))]
        public static class OptionsMenuBehaviour_Close
        {
            public static void Prefix() => CustomOption.SaveSettings("LastUsedSettings");
        }

        public static void ToggleButtonVoid(int id)
        {
            if (MenuG.Count <= id)
                return;

            foreach (var g in MenuG)
            {
                if (!g)
                    continue;

                g.SetActive(MenuG[id] == g);
                MenuS[id].enabled = MenuG[id] == g;
            }

            GameSettings.SettingsPage = id;
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        public static class GameOptionsMenu_Start
        {
            public static bool Prefix(GameOptionsMenu __instance)
            {
                if (IsHnS)
                    return true;

                StartPrefix(__instance, out var notmodded);
                return notmodded;
            }
        }

        public static void StartPrefix(GameOptionsMenu __instance, out bool notmodded)
        {
            notmodded = true;

            for (var index = 0; index < Menus.Length; index++)
            {
                if (__instance.name == $"ToU-Rew{Menus[index]}OptionsMenu")
                {
                    __instance.Children = new(Array.Empty<OptionBehaviour>());
                    var childeren = new Transform[__instance.gameObject.transform.childCount];

                    for (var k = 0; k < childeren.Length; k++)
                        childeren[k] = __instance.gameObject.transform.GetChild(k);

                    //TODO: Make a better fix for this for example caching the options or creating it ourself.
                    var startOption = __instance.gameObject.transform.GetChild(0);
                    var customOptions = CreateOptions(__instance, (MultiMenu)index);
                    var (x, y, z) = (startOption.localPosition.x, startOption.localPosition.y, startOption.localPosition.z);

                    for (var k = 0; k < childeren.Length; k++)
                        childeren[k].gameObject.Destroy();

                    for (var i = 0; i < customOptions.Count; i++)
                        customOptions[i].transform.localPosition = new(x, y - (i * 0.5f), z);

                    __instance.Children = new(customOptions.ToArray());
                    notmodded = false;
                }
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        public static class GameOptionsMenu_Update
        {
            private static float Timer = 1f;

            public static void Postfix(GameOptionsMenu __instance)
            {
                var gameSettingMenu = UObject.FindObjectsOfType<GameSettingMenu>().FirstOrDefault();

                if (gameSettingMenu.RegularGameSettings.active || gameSettingMenu.RolesSettings.gameObject.active || __instance.Children == null || __instance.Children.Length == 0)
                    return;

                Timer += Time.deltaTime;

                if (Timer < 0.1f)
                    return;

                Timer = 0f;
                var y = __instance.GetComponentsInChildren<OptionBehaviour>().Max(option => option.transform.localPosition.y);
                var s = __instance.Children.Length == 1 ? 0 : 1;
                var (x, z) = (__instance.Children[s].transform.localPosition.x, __instance.Children[s].transform.localPosition.z);

                for (var i = 0; i < __instance.Children.Count; i++)
                    __instance.Children[i].transform.localPosition = new(x, y - (i * 0.5f), z);

                /*for (var index = 0; index < Menus.Length; index++)
                {
                    if (__instance.name == $"ToU-Rew{Menus[index]}OptionsMenu")
                    {
                        foreach (var option in CustomOption.AllOptions.Where(x => x.Active))
                        {
                            if (option.Setting != null && option.Setting.gameObject != null)
                            {
                                option.Setting.gameObject.SetActive(true);
                                var cache = option.Setting.transform.localPosition;
                                y -= 0.5f;
                                option.Setting.transform.localPosition = new(cache.x, y, cache.z);
                            }
                        }
                    }
                }*/

                __instance.GetComponentInParent<Scroller>().ContentYBounds.max = (__instance.Children.Length - 6.5f) / 2f;
            }
        }

        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.OnEnable))]
        public static class ToggleOption_OnEnable
        {
            public static bool Prefix(ToggleOption __instance) => OnEnable(__instance);
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.OnEnable))]
        public static class NumberOption_OnEnable
        {
            public static bool Prefix(NumberOption __instance) => OnEnable(__instance);
        }

        [HarmonyPatch(typeof(KeyValueOption), nameof(KeyValueOption.OnEnable))]
        public static class StringOption_OnEnable
        {
            public static bool Prefix(KeyValueOption __instance) => OnEnable(__instance);
        }

        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Toggle))]
        public static class ToggleButtonPatch
        {
            public static bool Prefix(ToggleOption __instance)
            {
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomToggleOption toggle)
                {
                    toggle.Toggle();
                    return false;
                }

                if (option is CustomNestedOption nested)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    nested.ToDo();
                    return false;
                }

                if (option is RoleListEntryOption role)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    role.ToDo();
                    return false;
                }

                if (option is CustomHeaderOption)
                    return false;

                if (__instance == ExportButton.Setting)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    ExportButton.Do();
                    return false;
                }

                if (__instance == ImportButton.Setting)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    ImportButton.Do();
                    return false;
                }

                if (__instance == PresetButton.Setting)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    PresetButton.Do();
                    return false;
                }

                var option2 = ExportButton.SlotButtons.Find(option => option.Setting == __instance);

                if (option2 is CustomButtonOption button)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    button.Do();
                    return false;
                }

                var option3 = ImportButton.SlotButtons.Find(option => option.Setting == __instance);

                if (option3 is CustomButtonOption button2)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    button2.Do();
                    return false;
                }

                var option4 = PresetButton.SlotButtons.Find(option => option.Setting == __instance);

                if (option4 is CustomButtonOption button3)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    button3.Do();
                    return false;
                }

                var option5 = CustomNestedOption.AllCancelButtons.Find(option => option.Setting == __instance);

                if (option5 is CustomButtonOption button4)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    button4.Do();
                    return false;
                }

                var option6 = RoleListEntryOption.EntryButtons.Find(option => option.Setting == __instance);

                if (option6 is CustomButtonOption button5)
                {
                    if (!AmongUsClient.Instance.AmHost)
                        return false;

                    button5.Do();
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
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomNumberOption number)
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
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomNumberOption number)
                {
                    number.Decrease();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(KeyValueOption), nameof(KeyValueOption.Increase))]
        public static class KeyValueOptionPatchIncrease
        {
            public static bool Prefix(KeyValueOption __instance)
            {
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomStringOption str)
                {
                    str.Increase();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(KeyValueOption), nameof(KeyValueOption.Decrease))]
        public static class KeyValueOptionOptionPatchDecrease
        {
            public static bool Prefix(KeyValueOption __instance)
            {
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomStringOption str)
                {
                    str.Decrease();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.SetRole))]
        public static class RoleOptionOptionPatchSetRole
        {
            public static bool Prefix(RoleOptionSetting __instance)
            {
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check
                return option == null;
            }
        }

        [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.ShowRoleDetails))]
        public static class RoleOptionOptionPatchShowRoleDetails
        {
            public static bool Prefix(RoleOptionSetting __instance)
            {
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomLayersOption layer)
                {
                    layer.ShowRoleDetails();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(RoleOptionSetting), nameof(RoleOptionSetting.ShowAdvancedOptions))]
        public static class RoleOptionOptionPatchShowAdvancedOptions
        {
            public static bool Prefix(RoleOptionSetting __instance)
            {
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomLayersOption layer)
                {
                    layer.ShowAdvancedOptions();
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
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomLayersOption layer)
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
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomLayersOption layer)
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
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomLayersOption layer)
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
                var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);
                //Works but may need to change to gameObject.name check

                if (option is CustomLayersOption layer)
                {
                    layer.DecreaseCount();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
        public static class PlayerControlPatch
        {
            public static void Postfix()
            {
                if (CustomPlayer.AllPlayers.Count < 1 || !AmongUsClient.Instance || !CustomPlayer.Local || !AmongUsClient.Instance.AmHost)
                    return;

                SendOptionRPC();
                CustomOption.SaveSettings("LastUsedSettings");

                if (CachedFirstDead != null)
                    CallRpc(CustomRPC.Misc, MiscRPC.SetFirstKilled, CachedFirstDead);
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
        public static class PlayerJoinPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (CustomPlayer.AllPlayers.Count < 1 || !AmongUsClient.Instance || !CustomPlayer.Local)
                    return;

                if (__instance.myPlayer == CustomPlayer.Local)
                    HUD.Chat.AddChat(CustomPlayer.Local, "Welcome to Town Of Us Reworked! Type /help to get started!");

                if (!AmongUsClient.Instance.AmHost)
                    return;

                SendOptionRPC();
                CustomOption.SaveSettings("LastUsedSettings");

                if (CachedFirstDead != null)
                    CallRpc(CustomRPC.Misc, MiscRPC.SetFirstKilled, CachedFirstDead);
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManagerUpdate
        {
            private static float MinX = -5.233334f;
            private const float MinY = 2.9f;
            private static Scroller Scroller;
            private static Vector3 LastPosition;
            private static float LastAspect;
            private static bool SLastPosition;

            public static void Prefix(HudManager __instance)
            {
                if (__instance.GameSettings?.transform == null)
                    return;

                var safeArea = Screen.safeArea;
                var aspect = Mathf.Min(Camera.main.aspect, safeArea.width / safeArea.height);
                var safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
                MinX = 0.1f - (safeOrthographicSize * aspect);

                if (!SLastPosition || aspect != LastAspect)
                {
                    LastPosition = new(MinX, MinY);
                    LastAspect = aspect;
                    SLastPosition = true;

                    if (Scroller)
                        Scroller.ContentXBounds = new(MinX, MinX);
                }

                CreateScroller(__instance);
                Scroller.gameObject.SetActive(__instance.GameSettings.gameObject.activeSelf);

                if (!Scroller.gameObject.active)
                    return;

                var rows = __instance.GameSettings.text.Count(c => c == '\n');
                var maxY = Mathf.Max(MinY, (rows * 0.081f) + ((rows - 30) * 0.081f * (__instance.GameSettings.text.Contains('┣') ? 1.9f : 1f)));

                Scroller.ContentYBounds = new(MinY, maxY);

                //Prevent scrolling when the player is interacting with a menu
                if (CustomPlayer.Local?.CanMove == false)
                {
                    __instance.GameSettings.transform.localPosition = LastPosition;
                    return;
                }

                if (__instance.GameSettings.transform.localPosition.x != MinX || __instance.GameSettings.transform.localPosition.y < MinY)
                    return;

                LastPosition = __instance.GameSettings.transform.localPosition;
            }

            private static void CreateScroller(HudManager __instance)
            {
                if (Scroller)
                    return;

                Scroller = new GameObject("SettingsScroller") { layer = 5 }.AddComponent<Scroller>();
                Scroller.transform.SetParent(__instance.GameSettings.transform.parent);

                Scroller.transform.localScale = Vector3.one;
                Scroller.allowX = false;
                Scroller.allowY = true;
                Scroller.active = true;
                Scroller.velocity = new(0, 0);
                Scroller.ScrollbarYBounds = new(0, 0);
                Scroller.ContentXBounds = new(MinX, MinX);
                Scroller.enabled = true;

                Scroller.Inner = __instance.GameSettings.transform;
                __instance.GameSettings.transform.SetParent(Scroller.transform);
            }
        }
    }
}