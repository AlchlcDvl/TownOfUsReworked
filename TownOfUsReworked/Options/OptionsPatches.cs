namespace TownOfUsReworked.Options;

public static class SettingsPatches
{
    private static readonly string[] Menus = { "Global", "Crew", "Neutral", "Intruder", "Syndicate", "Modifier", "Objectifier", "Ability", "Role List", "Client" };
    private static readonly List<GameObject> MenuG = new();
    private static readonly List<SpriteRenderer> MenuS = new();
    private static GameObject ClientMenu;
    private static SpriteRenderer ClientSprite;

    public static Preset PresetButton;
    public static CustomButtonOption SaveSettings;

    public static int SettingsPage;
    public static int CurrentPage = 1;
    public static bool AllSettings;

    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
    public static class GameOptionsDataPatch
    {
        public static void Postfix(ref string __result)
        {
            if (IsHnS)
                return;

            __result = Settings();
        }
    }

    public static string Settings()
    {
        if (SettingsPage == 10)
            return "";

        var builder = new StringBuilder();

        if (!AllSettings)
            builder.AppendLine($"<b><size=160%>{TranslationManager.Translate($"GameSettings.Page{SettingsPage + 1}")}</size></b>");

        var tobedisplayed = CustomOption.AllOptions.Where(x => (x.Menu == (MultiMenu)SettingsPage || AllSettings) && x.Active).ToList();

        foreach (var option in tobedisplayed)
        {
            if (option.Type == CustomOptionType.Button)
                continue;

            if (option.Menu == MultiMenu.External || (option.Menu == MultiMenu.RoleList && !IsRoleList) || (option.Menu == MultiMenu.Ability && (!CustomGameOptions.EnableAbilities ||
                IsRoleList)) || (option.Menu == MultiMenu.Modifier && (!CustomGameOptions.EnableModifiers || IsRoleList)) || (option.Menu == MultiMenu.Objectifier &&
                (!CustomGameOptions.EnableObjectifiers || IsRoleList)))
            {
                continue;
            }

            var title = $"<b><size=160%>{TranslationManager.Translate($"GameSettings.Page{(int)option.Menu + 1}")}</size></b>";

            if (AllSettings && !builder.ToString().Contains(title))
                builder.AppendLine(title);

            var index = tobedisplayed.IndexOf(option);
            var thing = option is CustomHeaderOption ? "" : (index == tobedisplayed.Count - 1 || tobedisplayed[index + 1].Type == CustomOptionType.Header ? "┗ " : "┣ " );
            builder.AppendLine($"{thing}{option}");
        }

        builder.AppendLine();
        builder.AppendLine(TranslationManager.Translate("GameSettings.CurrentPage").Replace("%page1%", $"{CurrentPage}").Replace("%page2%", $"{MaxPages()}"));
        builder.AppendLine(TranslationManager.Translate("GameSettings.Instructions"));
        builder.AppendLine("Press Shift + Tab To Toggle View All Active Settings");
        return $"<size=1.25>{builder}</size>";
    }

    private static int MaxPages()
    {
        var result = 9;

        if (!CustomGameOptions.EnableAbilities && !IsRoleList)
            result--;

        if (!CustomGameOptions.EnableModifiers && !IsRoleList)
            result--;

        if (!CustomGameOptions.EnableObjectifiers && !IsRoleList)
            result--;

        result--;

        if (IsRoleList)
            result -= 2;

        return result;
    }

    private static void UpdatePageNumber()
    {
        if (Chat && Chat.IsOpenOrOpening)
            return;

        var cached = SettingsPage;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CurrentPage = CycleInt(MaxPages(), 1, CurrentPage, true);
            SettingsPage++;

            if (SettingsPage == 5 && (!CustomGameOptions.EnableModifiers || IsRoleList))
                SettingsPage = 6;

            if (SettingsPage == 6 && (!CustomGameOptions.EnableObjectifiers || IsRoleList))
                SettingsPage = 7;

            if (SettingsPage == 7 && (!CustomGameOptions.EnableAbilities || IsRoleList))
                SettingsPage = 8;

            if (SettingsPage == 8 && !IsRoleList)
                SettingsPage = 0;

            if (SettingsPage > 8)
                SettingsPage = 0;
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            CurrentPage = CycleInt(MaxPages(), 1, CurrentPage, false);
            SettingsPage--;

            if (SettingsPage < 0)
                SettingsPage = 8;

            if (SettingsPage == 8 && !IsRoleList)
                SettingsPage = 7;

            if (SettingsPage == 7 && (!CustomGameOptions.EnableAbilities || IsRoleList))
                SettingsPage = 6;

            if (SettingsPage == 6 && (!CustomGameOptions.EnableObjectifiers || IsRoleList))
                SettingsPage = 5;

            if (SettingsPage == 5 && (!CustomGameOptions.EnableModifiers || IsRoleList))
                SettingsPage = 4;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            CurrentPage = 1;
            SettingsPage = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            CurrentPage = 2;
            SettingsPage = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            CurrentPage = 3;
            SettingsPage = 2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            CurrentPage = 4;
            SettingsPage = 3;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            CurrentPage = 5;
            SettingsPage = 4;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            if (IsRoleList)
                SettingsPage = 8;
            else if (CustomGameOptions.EnableModifiers)
                SettingsPage = 5;
            else if (CustomGameOptions.EnableObjectifiers)
                SettingsPage = 6;
            else if (CustomGameOptions.EnableAbilities)
                SettingsPage = 7;

            if (SettingsPage is 5 or 6 or 7 or 8)
                CurrentPage = 6;
        }

        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            if (CustomGameOptions.EnableObjectifiers && !IsRoleList)
                SettingsPage = 6;
            else if (CustomGameOptions.EnableAbilities && !IsRoleList)
                SettingsPage = 7;

            if (SettingsPage is 6 or 7)
                CurrentPage = 7;
        }

        if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            if (CustomGameOptions.EnableAbilities && !IsRoleList)
                SettingsPage = 7;

            if (SettingsPage == 7)
                CurrentPage = 8;
        }

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Tab))
            AllSettings = !AllSettings;

        if (cached != SettingsPage && GameSettingMenu.Instance)
            ToggleButtonVoid(SettingsPage, CurrentPage);
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    public static class OptionsMenuBehaviour_Start
    {
        public static void Postfix(GameSettingMenu __instance)
        {
            MenuG.Clear();
            MenuS.Clear();
            ClientMenu = null;
            ClientSprite = null;

            if (IsHnS)
                return;

            var obj = __instance.RolesSettingsHightlight.gameObject.transform.parent.parent;

            for (var index = 0; index < Menus.Length; index++)
            {
                if ((index == 9 && !LobbyConsole.ClientOptionsActive) || (index != 9 && LobbyConsole.ClientOptionsActive))
                    continue;

                CreateOptionsMenu(__instance, index, obj);
            }

            DisableVanillaMenus(__instance);
            ToggleButtonVoid(SettingsPage);
        }

        private static void CreateOptionsMenu(GameSettingMenu __instance, int index, Transform obj)
        {
            var touSettings = UObject.Instantiate(__instance.RegularGameSettings, __instance.RegularGameSettings.transform.parent);
            touSettings.SetActive(false);
            touSettings.name = $"{Menus[index].Replace(" ", "")}Settings";

            //Derived this from Town Of Host Edited
            touSettings.transform.FindChild("BackPanel").transform.localScale = new(1.6f, 1f, 1f);
            touSettings.transform.FindChild("Bottom Gradient").transform.localScale = new(1.6f, 1f, 1f);
            touSettings.transform.FindChild("BackPanel").transform.localPosition += new Vector3(0.2f, 0f, 0f);
            touSettings.transform.FindChild("Bottom Gradient").transform.localPosition += new Vector3(0.2f, 0f, 0f);
            touSettings.transform.FindChild("Background").transform.localScale = new(1.8f, 1f, 1f);
            touSettings.transform.FindChild("UI_Scrollbar").transform.localPosition += new Vector3(1.4f, 0f, 0f);
            touSettings.transform.FindChild("UI_ScrollbarTrack").transform.localPosition += new Vector3(1.4f, 0f, 0f);
            touSettings.transform.FindChild("GameGroup/SliderInner").transform.localPosition += new Vector3(-0.3f, 0f, 0f);

            var gameGroup = touSettings.transform.FindChild("GameGroup");
            var title = gameGroup?.FindChild("Text");
            var color = GetSettingColor(index);

            if (title)
            {
                title.localPosition += new Vector3(1.3f, 0f, 0f);
                title.GetComponent<TextTranslatorTMP>().Destroy();
                title.GetComponent<TextMeshPro>().m_text = $"{Menus[index]} Settings";
                title.GetComponent<TextMeshPro>().color = color;
            }

            var sliderInner = gameGroup?.FindChild("SliderInner");

            if (sliderInner)
                sliderInner.GetComponent<GameOptionsMenu>().name = $"{Menus[index].Replace(" ", "")}OptionsMenu";

            var ourSettingsButton = UObject.Instantiate(obj.gameObject, obj.transform.parent);
            ourSettingsButton.transform.localPosition = new(-4.3f + (0.7f * ((index == 9 ? 4 : index) + 1)), 0f, -5f);
            ourSettingsButton.name = $"{Menus[index].Replace(" ", "")}Tab";

            var hatButton = ourSettingsButton.transform.GetChild(0); //TODO: Change to FindChild I guess to be sure
            hatButton.GetChild(0).GetComponent<SpriteRenderer>().sprite = GetSprite(GetSettingSprite(index));

            var tabBackground = hatButton.GetChild(1);

            var rend = tabBackground.GetComponent<SpriteRenderer>();
            rend.color = color;
            color.a = 0.5f;
            rend.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;

            if (index != 9)
            {
                MenuS.Add(rend);
                MenuG.Add(touSettings);
            }
            else
            {
                ClientMenu = touSettings;
                ClientSprite = rend;
            }

            var passiveButton = tabBackground.GetComponent<PassiveButton>();
            passiveButton.OnClick = new();
            passiveButton.OnClick.AddListener((Action)(() => ToggleButtonVoid(index)));
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
            9 => "ClientSetting",
            0 or _ => "SettingsButton"
        };

        private static UColor GetSettingColor(int index) => index switch
        {
            1 => CustomColorManager.Crew,
            2 => CustomColorManager.Neutral,
            3 => CustomColorManager.Intruder,
            4 => CustomColorManager.Syndicate,
            5 => CustomColorManager.Modifier,
            6 => CustomColorManager.Objectifier,
            7 => CustomColorManager.Abilities,
            8 => CustomColorManager.RoleList,
            9 => CustomColorManager.What,
            _ => UColor.white
        };
    }

    private static void DisableVanillaMenus(GameSettingMenu __instance)
    {
        __instance.RegularGameSettings.SetActive(false);
        __instance.RolesSettings.gameObject.SetActive(false);
        __instance.GameSettingsHightlight.gameObject.SetActive(false);
        __instance.RolesSettingsHightlight.gameObject.SetActive(false);
        __instance.GameSettingsHightlight.enabled = false;
        __instance.RolesSettingsHightlight.enabled = false;
        __instance.RolesSettingsHightlight.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        __instance.GameSettingsHightlight.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        __instance.RolesSettingsHightlight.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().gameObject.SetActive(false);
        __instance.GameSettingsHightlight.transform.parent.parent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().gameObject.SetActive(false);
    }

    private static void ToggleButtonVoid(int id, int current = 0)
    {
        if (id < 9 && !LobbyConsole.ClientOptionsActive)
            MenuG.ForEach(x => x?.SetActive(MenuG[id] == x));
        else
            ClientMenu.SetActive(true);

        SettingsPage = id;
        CurrentPage = current > 0 ? current : SettingsPage + 1;

        foreach (var slot in PresetButton.SlotButtons)
        {
            if (slot.Setting && slot.Setting.gameObject)
                slot.Setting.gameObject.Destroy();
        }

        PresetButton.SlotButtons.Clear();
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Update))]
    public static class OptionsMenuBehaviour_Update
    {
        public static void Postfix(GameSettingMenu __instance)
        {
            if (IsHnS)
                return;

            DisableVanillaMenus(__instance);

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
        public static void Prefix()
        {
            if (SettingsPage is 9 or 10 || (SettingsPage == 8 && !IsRoleList) || (SettingsPage == 7 && !CustomGameOptions.EnableAbilities) || (SettingsPage == 6 &&
                !CustomGameOptions.EnableObjectifiers) || (SettingsPage == 5 && !CustomGameOptions.EnableModifiers))
            {
                SettingsPage = 0;
                CurrentPage = 1;
            }

            MenuG.Clear();
            MenuS.Clear();
            ClientMenu = null;
            ClientSprite = null;
            PresetButton.SlotButtons.Clear();
            LobbyConsole.ClientOptionsActive = false;
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
    public static class GameSettingMenuOnEnable
    {
        public static void Prefix(GameSettingMenu __instance) => __instance.HideForOnline = new(0);
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
    public static class GameOptionsMenu_Start
    {
        private static bool ValuesSet;
        private static float X;
        private static float Y;
        private static float Z;
        private static RoleOptionSetting RolePrefab;

        public static bool Prefix(GameOptionsMenu __instance)
        {
            if (IsHnS)
                return true;

            __instance.Children = Array.Empty<OptionBehaviour>();
            var children = new Transform[__instance.gameObject.transform.childCount];

            for (var k = 0; k < children.Length; k++)
                children[k] = __instance.gameObject.transform.GetChild(k);

            //TODO: Make a better fix for this for example caching the options or creating it ourself.
            //AD Says: Done, kinda.
            if (!ValuesSet)
            {
                ValuesSet = true;
                var startOptionPos = __instance.gameObject.transform.GetChild(0).localPosition;
                (X, Y, Z) = (startOptionPos.x, startOptionPos.y, startOptionPos.z);
            }

            var allOptions = CreateOptions();
            var (customOptions, behaviours) = (allOptions.Keys.ToArray(), allOptions.Values.ToArray());
            var (x, y, z) = (X, Y, Z);

            for (var k = 0; k < children.Length; k++)
                children[k].gameObject.Destroy();

            for (var i = 0; i < allOptions.Count; i++)
            {
                if (i > 0)
                    y -= customOptions[i] is CustomHeaderOption ? 0.6f : 0.5f;

                behaviours[i].transform.localPosition = new(x, y, z);
                behaviours[i].gameObject.SetActive(true);
            }

            __instance.Children = behaviours;
            return false;
        }

        private static Dictionary<CustomOption, OptionBehaviour> CreateOptions()
        {
            var options = new Dictionary<CustomOption, OptionBehaviour>();

            if (SettingsPage == 10)
                return options;

            if (RolePrefab == null)
            {
                //Background = 0, Title = 1, Count - = 2, Count Val = 3, Count + = 4, Chance - = 5, Chance Val = 6, Chance + = 7
                RolePrefab = UObject.Instantiate(UObject.FindObjectOfType<RoleOptionSetting>(true), null);
                RolePrefab.name = "CustomLayersOptionPrefab";
                RolePrefab.transform.GetChild(0).localScale = new(1.6f, 1f, 1f);
                RolePrefab.transform.GetChild(1).localPosition = new(-1.05f, 0f, 0f);
                RolePrefab.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new(5.5f, 0.37f);
                RolePrefab.transform.GetChild(2).localPosition += new Vector3(0.4f, 0f, 0f);
                RolePrefab.transform.GetChild(3).localPosition += new Vector3(0.6f, 0f, 0f);
                RolePrefab.transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new(1.6f, 0.26f);
                RolePrefab.transform.GetChild(4).localPosition += new Vector3(0.8f, 0f, 0f);
                RolePrefab.transform.GetChild(5).localPosition += new Vector3(1, 0f, 0f);
                RolePrefab.transform.GetChild(6).localPosition += new Vector3(1.2f, 0f, 0f);
                RolePrefab.transform.GetChild(6).GetComponent<RectTransform>().sizeDelta = new(1.6f, 0.26f);
                RolePrefab.transform.GetChild(7).localPosition += new Vector3(1.4f, 0f, 0f);
                RolePrefab.transform.GetChild(8).gameObject.SetActive(false);
                RolePrefab.gameObject.SetActive(false);
            }

            var togglePrefab = UObject.FindObjectOfType<ToggleOption>();
            var numberPrefab = UObject.FindObjectOfType<NumberOption>();
            var keyValPrefab = UObject.FindObjectOfType<KeyValueOption>();
            var type = (MultiMenu)SettingsPage;

            if (type == MultiMenu.Main)
            {
                if (SaveSettings.Setting == null)
                {
                    SaveSettings.Setting = CustomButtonOption.CreateButton();
                    SaveSettings.OptionCreated();
                }

                if (PresetButton.Setting == null)
                {
                    PresetButton.Setting = CustomButtonOption.CreateButton();
                    PresetButton.OptionCreated();
                }

                options.Add(SaveSettings, SaveSettings.Setting);
                options.Add(PresetButton, PresetButton.Setting);
            }

            foreach (var option in CustomOption.AllOptions.Where(x => x.Menu == type))
            {
                if (option.Setting == null)
                {
                    OptionBehaviour setting = null;

                    switch (option.Type)
                    {
                        case CustomOptionType.Number: //Background = 0, + = 1, - = 2, Title = 3, Val = 4
                            setting = UObject.Instantiate(numberPrefab, numberPrefab.transform.parent);
                            setting.transform.GetChild(0).localScale = new(1.6f, 1f, 1f);
                            setting.transform.GetChild(1).localPosition += new Vector3(1.4f, 0f, 0f);
                            setting.transform.GetChild(2).localPosition += new Vector3(1, 0f, 0f);
                            setting.transform.GetChild(3).localPosition = new(-1.05f, 0f, 0f);
                            setting.transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new(5.5f, 0.37f);
                            setting.transform.GetChild(4).localPosition += new Vector3(1.2f, 0f, 0f);
                            setting.transform.GetChild(4).GetComponent<RectTransform>().sizeDelta = new(1.6f, 0.26f);
                            break;

                        case CustomOptionType.String: //Background = 0, + = 1, - = 2, Title = 3, Val = 4
                            setting = UObject.Instantiate(keyValPrefab, keyValPrefab.transform.parent);
                            setting.transform.GetChild(0).localScale = new(1.6f, 1f, 1f);
                            setting.transform.GetChild(1).localPosition += new Vector3(1.4f, 0f, 0f);
                            setting.transform.GetChild(2).localPosition += new Vector3(1, 0f, 0f);
                            setting.transform.GetChild(3).localPosition = new(-1.05f, 0f, 0f);
                            setting.transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new(5.5f, 0.37f);
                            setting.transform.GetChild(4).localPosition += new Vector3(1.2f, 0f, 0f);
                            setting.transform.GetChild(4).GetComponent<RectTransform>().sizeDelta = new(1.6f, 0.26f);
                            break;

                        case CustomOptionType.Layers:
                            setting = UObject.Instantiate(RolePrefab, keyValPrefab.transform.parent);
                            setting.transform.GetChild(2).gameObject.SetActive(IsCustom);
                            setting.transform.GetChild(3).gameObject.SetActive(IsCustom);
                            setting.transform.GetChild(4).gameObject.SetActive(IsCustom);
                            break;

                        case CustomOptionType.Toggle or CustomOptionType.Header or CustomOptionType.Entry: //Title = 0, Background = 1, Check + Box = 2
                            setting = UObject.Instantiate(togglePrefab, togglePrefab.transform.parent);
                            setting.transform.GetChild(0).localPosition = new(-1.05f, 0f, 0f);
                            setting.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new(5.5f, 0.37f);
                            setting.transform.GetChild(1).localScale = new(1.6f, 1f, 1f);
                            setting.transform.GetChild(2).localPosition = new(2.72f, 0f, 0f);
                            setting.gameObject.GetComponent<BoxCollider2D>().size = new(option is CustomHeaderOption ? 0f : 7.91f, 0.45f);

                            if (option.Type != CustomOptionType.Toggle)
                                setting.transform.GetChild(2).gameObject.SetActive(false);

                            if (option is CustomHeaderOption)
                                setting.transform.GetChild(1).gameObject.SetActive(false);

                            break;
                    }

                    option.Setting = setting;
                    option.OptionCreated();
                }

                options.Add(option, option.Setting);
            }

            return options;
        }
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.ValueChanged))]
    public static class GettingRidOfOno
    {
        public static bool Prefix(GameOptionsMenu __instance)
        {
            if (!AmongUsClient.Instance || !AmongUsClient.Instance.AmHost)
                return false;

            if (IsHnS)
                return true;

            GameManager.Instance.LogicOptions.SyncOptions();
            __instance.RefreshChildren();
            return false;
        }
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
    public static class GameOptionsMenu_Update
    {
        private static bool ValuesSet;
        private static float X;
        private static float Y;
        private static float Z;
        private static float Timer;

        public static bool Prefix(GameOptionsMenu __instance)
        {
            var gameSettingMenu = UObject.FindObjectsOfType<GameSettingMenu>().FirstOrDefault();

            if (gameSettingMenu.RegularGameSettings.active || gameSettingMenu.RolesSettings.gameObject.active || __instance.Children == null || __instance.Children.Length == 0)
                return true;

            Timer += Time.deltaTime;

            if (Timer < 0.1f)
                return false;

            Timer = 0f;

            if (!ValuesSet)
            {
                ValuesSet = true;
                var startOptionPos = __instance.Children[0].transform.localPosition;
                (X, Y, Z) = (startOptionPos.x, startOptionPos.y, startOptionPos.z);
            }

            try
            {
                for (var i = 0; i < MenuS.Count; i++)
                    MenuS[i].enabled = i == SettingsPage;

                ClientSprite.enabled = SettingsPage == 9;
            } catch {}

            if (SettingsPage != 10)
            {
                var (x, y, z) = (X, Y + 0.6f, Z);
                var list = new List<OptionBehaviour>();

                if (SettingsPage == 0 && IsLobby && !LobbyConsole.ClientOptionsActive)
                {
                    y = PresetButton.Setting.gameObject.transform.localPosition.y;
                    list.Add(SaveSettings.Setting);
                    list.Add(PresetButton.Setting);
                }

                if (SaveSettings != null && SaveSettings.Setting && SaveSettings.Setting.gameObject)
                    SaveSettings.Setting.gameObject.SetActive(SettingsPage == 0);

                if (PresetButton != null && PresetButton.Setting && PresetButton.Setting.gameObject)
                    PresetButton.Setting.gameObject.SetActive(SettingsPage == 0);

                foreach (var option in CustomOption.AllOptions)
                {
                    if (option != null && option.Setting && option.Setting.gameObject)
                    {
                        if (option.Menu != (MultiMenu)SettingsPage || !option.Active)
                        {
                            option.Setting.gameObject.SetActive(false);
                            continue;
                        }

                        option.Setting.gameObject.SetActive(true);
                        y -= option is CustomHeaderOption ? 0.6f : 0.5f;
                        option.Setting.transform.localPosition = new(x, y, z);
                        list.Add(option.Setting);

                        if (option is CustomLayersOption)
                        {
                            option.Setting.transform.GetChild(2).gameObject.SetActive(IsCustom);
                            option.Setting.transform.GetChild(3).gameObject.SetActive(IsCustom);
                            option.Setting.transform.GetChild(4).gameObject.SetActive(IsCustom);
                        }
                    }
                }

                __instance.Children = list.ToArray();
            }

            __instance.GetComponentInParent<Scroller>().ContentYBounds.max = __instance.Children.Length / 2f;
            return false;
        }
    }

    private static bool OnEnable(OptionBehaviour opt)
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

        var customOption = CustomOption.AllOptions.Find(option => option.Setting == opt);

        if (customOption == null)
            customOption = PresetButton.SlotButtons.Find(option => option.Setting == opt);

        if (customOption == null)
            customOption = RoleListEntryOption.EntryButtons.Find(option => option.Setting == opt);

        if (customOption == null)
            return true;

        customOption.OptionCreated();
        return false;
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
    public static class KeyValueOption_OnEnable
    {
        public static bool Prefix(KeyValueOption __instance) => OnEnable(__instance);
    }

    [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Toggle))]
    public static class ToggleButtonPatch
    {
        public static bool Prefix(ToggleOption __instance)
        {
            var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);

            if (option is CustomToggleOption toggle)
            {
                toggle.Toggle();
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

            if (__instance == PresetButton.Setting)
            {
                if (!AmongUsClient.Instance.AmHost)
                    return false;

                PresetButton.Do();
                return false;
            }

            if (__instance == SaveSettings.Setting)
            {
                if (!AmongUsClient.Instance.AmHost)
                    return false;

                SaveSettings.Do();
                return false;
            }

            var option1 = PresetButton.SlotButtons.Find(option => option.Setting == __instance);

            if (option1 is CustomButtonOption button)
            {
                if (!AmongUsClient.Instance.AmHost)
                    return false;

                button.Do();
                return false;
            }

            var option2 = RoleListEntryOption.EntryButtons.Find(option => option.Setting == __instance);

            if (option2 is CustomButtonOption button1)
            {
                if (!AmongUsClient.Instance.AmHost)
                    return false;

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
            var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);

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

            if (option is CustomStringOption str)
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
            var option = CustomOption.AllOptions.Find(option => option.Setting == __instance);

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

            if (option is CustomLayersOption layer)
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

            SendOptionRPC();
            CallRpc(CustomRPC.Misc, MiscRPC.SyncSummary, ReadDiskText("Summary", TownOfUsReworked.Other));

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
            if (IsLobby || CustomPlayer.Local.HasDied())
                __instance?.ReportButton?.gameObject?.SetActive(false);

            UpdatePageNumber();

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