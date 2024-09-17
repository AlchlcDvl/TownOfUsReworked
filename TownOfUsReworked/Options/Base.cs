namespace TownOfUsReworked.Options;

public abstract class OptionAttribute<T>(MultiMenu menu, CustomOptionType type) : OptionAttribute(menu, type)
{
    private static string LastChangedSetting = "";

    public T Value { get; set; }
    public T DefaultValue { get; set; }

    public T Get() => Value;

    public override void SetProperty(PropertyInfo property)
    {
        base.SetProperty(property);
        Value = DefaultValue = (T)property.GetValue(null);
    }

    public override string ToString() => $"{ID}:{Value}";

    public void Set(T value, bool rpc = true, bool notify = true)
    {
        Value = value;
        Property?.SetValue(null, value);
        // OnChanged.Invoke(value);

        if (AmongUsClient.Instance.AmHost && rpc && !(ClientOnly || !ID.Contains("CustomOption") || Type is CustomOptionType.Header or CustomOptionType.Alignment))
            SendOptionRPC(this);

        if (Setting)
        {
            ModifySetting();
            SettingsPatches.OnValueChanged(GameSettingMenu.Instance);
        }

        if (ViewSetting)
        {
            ModifyViewSetting();
        }

        var stringValue = Format();

        if (!notify || IsNullEmptyOrWhiteSpace(stringValue))
            return;

        var changed = $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{TranslationManager.Translate(ID)}</font> set to <font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{stringValue}</font>";

        if (LastChangedSetting == ID && HUD().Notifier.activeMessages.Count > 0)
            HUD().Notifier.activeMessages[^1].UpdateMessage(changed);
        else
        {
            LastChangedSetting = ID;
            var newMessage = UObject.Instantiate(HUD().Notifier.notificationMessageOrigin, Vector3.zero, Quaternion.identity, HUD().Notifier.transform);
            newMessage.transform.localPosition = new(0f, 0f, -2f);
            newMessage.SetUp(changed, HUD().Notifier.settingsChangeSprite, HUD().Notifier.settingsChangeColor, (Action)(() => HUD().Notifier.OnMessageDestroy(newMessage)));
            HUD().Notifier.ShiftMessages();
            HUD().Notifier.AddMessageToQueue(newMessage);
        }
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public abstract class OptionAttribute(MultiMenu menu, CustomOptionType type) : Attribute
{
    public static readonly List<OptionAttribute> AllOptions = [];
    public string ID { get; set; }
    public MultiMenu Menu { get; set; } = menu;
    public readonly List<MultiMenu> Menus = [ menu ];
    public MonoBehaviour Setting { get; set; }
    public MonoBehaviour ViewSetting { get; set; }
    public CustomOptionType Type { get; } = type;
    public bool All { get; set; }
    public bool ClientOnly { get; set; }
    public PropertyInfo Property { get; set; }
    public string Name { get; set; } // Not actually the setting text, just the property/class name :]
    public Type TargetType { get; set; }
    // public bool Invert { get; set; }
    // public MethodInfo OnChanged { get; set; }
    // public Type OnChangedType { get; set; }
    // public string OnChangedName { get; set; }
    // ^ Code for when I do actually need it :]

    // Apparently, setting the parents in the attibutes doesn't seem to work
    // This one is for those depending on other options
    public static readonly List<(string[], object[])> OptionParents1 =
    [
        ( [ "EjectionRevealsRole" ], [ "ConfirmEjects" ] ),
        ( [ "InitialCooldowns" ], [ "EnableInitialCds" ] ),
        ( [ "MeetingCooldowns" ], [ "EnableMeetingCds" ] ),
        ( [ "FailCooldowns" ], [ "EnableFailCds" ] ),
        ( [ "WhoSeesFirstKillShield" ], [ "FirstKillShield" ] ),
        ( [ "WhispersAnnouncement" ], [ "Whispers" ] ),
        ( [ "KillerReports", "RoleFactionReports", "LocationReports" ], [ "GameAnnouncements" ] ),
        ( [ "SmallMapHalfVision", "SmallMapDecreasedCooldown", "LargeMapIncreasedCooldown", "SmallMapIncreasedShortTasks", "SmallMapIncreasedLongTasks", "LargeMapDecreasedShortTasks",
            "LargeMapDecreasedLongTasks" ], [ "AutoAdjustSettings" ] ),
        ( [ "EvilsIgnoreNV" ], [ "NightVision" ] ),
        ( [ "SkeldVentImprovements" , "SkeldReactorTimer", "SkeldO2Timer" ], [ "EnableBetterSkeld" ] ),
        ( [ "MiraHQVentImprovements" , "MiraReactorTimer", "MiraO2Timer" ], [ "EnableBetterMiraHQ" ] ),
        ( [ "PolusVentImprovements", "VitalsLab", "ColdTempDeathValley", "WifiChartCourseSwap", "SeismicTimer" ], [ "EnableBetterPolus" ] ),
        ( [ "SpawnType", "MoveVitals", "MoveFuel", "MoveDivert", "MoveAdmin", "MoveElectrical", "MinDoorSwipeTime", "CrashTimer" ], [ "EnableBetterAirship" ] ),
        ( [ "FungleReactorTimer", "FungleMixupTimer" ], [ "EnableBetterFungle" ] ),
        ( [ "CoronerKillerNameTime" ], [ "CoronerReportName" ] )
    ];
    // I need a second one because for some dumb reason the game likes crashing
    // This is for everything else
    public static readonly List<(string[], object[])> OptionParents2 =
    [
        ( [ "TaskBar" ], [ GameMode.Classic, GameMode.Custom, GameMode.AllAny, GameMode.KillingOnly, GameMode.RoleList, GameMode.Vanilla ] ),
        ( [ "IgnoreAlignmentCaps", "IgnoreFactionCaps", "IgnoreLayerCaps" ], [ GameMode.Classic, GameMode.Custom ] ),
        ( [ "NeutralsCount", "AddArsonist", "AddCryomaniac", "AddPlaguebearer" ], [ GameMode.KillingOnly ] ),
        ( [ "HnSShortTasks", "HnSCommonTasks", "HnSLongTasks", "HunterCount", "HuntCd", "StartTime", "HunterVent", "HunterVision", "HuntedVision", "HunterSpeedModifier", "HuntedChat",
            "HunterFlashlight", "HuntedFlashlight", "HnSMode" ], [ GameMode.HideAndSeek ] ),
        ( [ "TRShortTasks", "TRCommonTasks", "TRLongTasks" ], [ GameMode.TaskRace ] ),
        ( [ "RandomMapSkeld", "RandomMapMira", "RandomMapPolus", "RandomMapdlekS", "RandomMapAirship", "RandomMapFungle" ], [ MapEnum.Random ] ),
        ( [ "RandomMapSubmerged" ], [ MapEnum.Random, ("ModCompatibility", "SubLoaded") ] ),
        ( [ "RandomMapLevelImpostor" ], [ MapEnum.Random, ("ModCompatibility", "LILoaded") ] ),
        ( [ "SmallMapHalfVision", "SmallMapDecreasedCooldown", "SmallMapIncreasedShortTasks", "SmallMapIncreasedLongTasks", "OxySlow" ], [ MapEnum.Skeld, MapEnum.dlekS, MapEnum.Random,
            MapEnum.MiraHQ, MapEnum.LevelImpostor ] ),
        ( [ "LargeMapDecreasedShortTasks", "LargeMapDecreasedLongTasks", "LargeMapIncreasedCooldown" ], [ MapEnum.Airship, MapEnum.Submerged, MapEnum.Random, MapEnum.Fungle,
            MapEnum.LevelImpostor ] ),
        ( [ "BetterSkeld" ], [ MapEnum.Skeld, MapEnum.dlekS, MapEnum.Random ] ),
        ( [ "BetterMiraHQ" ], [ MapEnum.MiraHQ, MapEnum.Random ] ),
        ( [ "BetterPolus" ], [ MapEnum.Polus, MapEnum.Random ] ),
        ( [ "BetterAirship" ], [ MapEnum.Airship, MapEnum.Random ] ),
        ( [ "BetterFungle" ], [ MapEnum.Fungle, MapEnum.Random ] ),
        ( [ "CrewSettings" ], [ GameMode.Classic, GameMode.AllAny, GameMode.Custom, GameMode.Vanilla, GameMode.KillingOnly, GameMode.RoleList ] ),
        ( [ "CrewMax", "CrewMin" ], [ GameMode.Classic, GameMode.AllAny, GameMode.Custom ] ),
        ( [ "Pestilence" ], [ LayerEnum.Plaguebearer ] ),
        ( [ "Betrayer" ], [ LayerEnum.Traitor, LayerEnum.Fanatic ] ),
        ( [ "Assassin" ], [ LayerEnum.Hitman, LayerEnum.Bullseye, LayerEnum.Sniper, LayerEnum.Slayer ] ),
        ( [ "HowIsVigilanteNotified" ], [ VigiOptions.PostMeeting, VigiOptions.PreMeeting ] ),
        ( [ "RoleListEntries", "RoleListBans" ], [ GameMode.RoleList ] )
    ];
    private static readonly Dictionary<string, bool> MapToLoaded = [];

    public virtual void SetProperty(PropertyInfo property)
    {
        Property = property;
        Name = property.Name.Replace("Priv", "");
        ID = $"CustomOption.{Name}";
        TargetType = property.PropertyType;
        // OnChanged = AccessTools.GetDeclaredMethods(OnChangedType).Find(x => x.Name == OnChangedName);
        // TownOfUsReworked.ModInstance.Harmony.Patch(Property.GetSetMethod(true), null, new(typeof(OptionAttribute), nameof(OptionsPatch)));
        AllOptions.Add(this);
    }

    public virtual string Format() => "";

    public virtual void Update() {}

    public bool Active()
    {
        var result = true;

        if (OptionParents1.TryFinding(x => x.Item1.Contains(Name), out var parents))
            result &= parents.Item2.AllAnyOrEmpty(IsActive, All);

        if (OptionParents2.TryFinding(x => x.Item1.Contains(Name), out parents))
            result &= parents.Item2.AllAnyOrEmpty(IsActive, All);

        return result;
    }

    private bool IsActive(object option)
    {
        var result = option == null;

        if (option is MapEnum map)
            result = MapSettings.Map == map;
        else if (option is GameMode mode)
            result = GameModeSettings.GameMode == mode;
        else if (option is LayerEnum layer)
        {
            AddMenuIndex(6 + (int)layer);
            result = Menus.Any(x => (int)x == SettingsPatches.SettingsPage);
        }
        else if (option is int num)
            result = SettingsPatches.SettingsPage == num;
        else if (option is VigiOptions vigiop)
            result = Vigilante.HowDoesVigilanteDie == vigiop;
        else if (option is (string, string))
        {
            if (!MapToLoaded.TryGetValue(ID, out result))
            {
                var tuple = ((string, string))option;
                var type = AccessTools.GetTypesFromAssembly(TownOfUsReworked.Core).ToList().Find(x => x.Name == tuple.Item1);
                MapToLoaded[ID] = result = (bool)AccessTools.GetDeclaredProperties(type).Find(x => x.Name == tuple.Item2)?.GetValue(null);
            }
        }
        else if (option is string id)
        {
            if (id == Name)
                return true; // To prevent accidental stack overflows, very rudementary because I've already managed to cause several of them even with this line active

            var optionatt = GetOptionFromName(id);

            if (optionatt != null)
            {
                result = optionatt.Active();

                if (optionatt is ToggleOptionAttribute toggle)
                    result &= toggle.Get();
                else if (optionatt is HeaderOptionAttribute header)
                    result &= header.Get();
            }
            else
                result = true;
        }

        // if (Invert && option != null)
        //     result = !result;

        return result;
    }

    public virtual void OptionCreated()
    {
        Setting.name = ID;

        if (Setting is OptionBehaviour option)
        {
            option.Title = (StringNames)999999999;
            option.OnValueChanged = (Action<OptionBehaviour>)BlankVoid; // The cast here is not redundant, idk why the compiler refuses to accept this
        }
    }

    public virtual void ViewOptionCreated()
    {
        ViewSetting.name = ID;

        if (ViewSetting is ViewSettingsInfoPanel viewSettingsInfoPanel)
        {
            viewSettingsInfoPanel.SetMaskLayer(61);
            viewSettingsInfoPanel.titleText.text = TranslationManager.Translate(ID);
            viewSettingsInfoPanel.background.gameObject.SetActive(true);
        }
    }

    public virtual void PostLoadSetup() {}

    public virtual void ModifySetting() {}

    public virtual void ModifyViewSetting() {}

    public virtual void AddMenuIndex(int index)
    {
        var menu = (MultiMenu)index;

        if (!Menus.Contains(menu))
            Menus.Add(menu);
    }

    // public static void OptionsPatch(PropertyInfo __instance, ref object value) => GetOptionFromProperty(__instance).SetBase(value);

    public void SetBase(object value, bool rpc = true, bool notify = true)
    {
        if (this is ToggleOptionAttribute toggle)
            toggle.Set((bool)value, rpc, notify);
        else if (this is NumberOptionAttribute number)
            number.Set((Number)value, rpc, notify);
        else if (this is StringOptionAttribute stringOpt)
            stringOpt.Set((Enum)value, rpc, notify);
        else if (this is RoleListEntryAttribute entry)
            entry.Set((LayerEnum)value, rpc, notify);
        else if (this is LayersOptionAttribute layer)
            layer.Set((RoleOptionData)value, rpc, notify);
    }

    public static string SettingsToString(List<OptionAttribute> list = null)
    {
        list ??= AllOptions;
        var builder = new StringBuilder();

        foreach (var option in list)
        {
            if (option.Type is CustomOptionType.Header or CustomOptionType.Alignment || option.ClientOnly || !option.ID.Contains("CustomOption"))
                continue;

            builder.AppendLine(option.ToString());
        }

        return builder.ToString();
    }

    public static void SaveSettings()
    {
        var fileName = "SavedSettings.txt";
        var filePath = Path.Combine(TownOfUsReworked.Options, fileName);
        var i = 0;

        while (File.Exists(filePath))
        {
            i++;
            fileName = $"SavedSettings{i}.txt";
            filePath = Path.Combine(TownOfUsReworked.Options, fileName);
        }

        SaveSettings(fileName.Replace(".txt", ""));
    }

    public static void SaveSettings(string fileName)
    {
        SaveText($"{fileName}.txt", SettingsToString(), TownOfUsReworked.Options);

        if (!SettingsPatches.Save)
            return;

        var index = SettingsPatches.PresetsButtons.Count;

        var presetButton = UObject.Instantiate(SettingsPatches.Save, GameSettingMenu.Instance.PresetsTab.StandardPresetButton.transform.parent);
        presetButton.transform.localScale = new(0.5f, 0.84f, 1f);
        presetButton.buttonText.transform.localScale = new(1.4f, 0.9f, 1f);
        presetButton.buttonText.alignment = TextAlignmentOptions.Center;
        presetButton.buttonText.GetComponent<TextTranslatorTMP>().Destroy();
        presetButton.buttonText.text = presetButton.name = fileName;
        presetButton.OverrideOnClickListeners(() => LoadPreset(fileName));

        if (index >= (SettingsPatches.SettingsPage2 * 25) && index < ((SettingsPatches.SettingsPage2 + 1) * 25))
        {
            var relativeIndex = index % 25;
            var row = relativeIndex / 4;
            var col = relativeIndex % 4;
            presetButton.transform.localPosition = new(-2.5731f + (col * 1.8911f), 1.7828f - (row * 0.65136f), -2);
        }
        else
            presetButton.gameObject.SetActive(false);

        SettingsPatches.PresetsButtons.Add(presetButton);
        SettingsPatches.Prev.gameObject.SetActive(SettingsPatches.PresetsButtons.Count > 25);
        SettingsPatches.Next.gameObject.SetActive(SettingsPatches.PresetsButtons.Count > 25);
    }

    public static void LoadPreset(string presetName)
    {
        LogMessage($"Loading - {presetName}");
        var text = ReadDiskText($"{presetName}.txt", TownOfUsReworked.Options);

        if (IsNullEmptyOrWhiteSpace(text))
            LogError($"{presetName} no exist");
        else
        {
            CallRpc(CustomRPC.Misc, MiscRPC.LoadPreset, presetName);
            SettingsPatches.CurrentPreset = presetName;
            LoadSettings(text);
        }
    }

    public static void LoadSettings(string settingsData) => Coroutines.Start(CoLoadSettings(settingsData));

    public static IEnumerator CoLoadSettings(string settingsData)
    {
        var splitText = settingsData.Split('\n').ToList();
        splitText.RemoveAll(IsNullEmptyOrWhiteSpace);
        var pos = 0;

        while (splitText.Any())
        {
            pos++;
            var opt = splitText[0];
            splitText.RemoveAt(0);
            var parts = opt.Split(':');
            var name = parts[0];
            var option = AllOptions.Where(x => x is not HeaderOptionAttribute or AlignsOptionAttribute).FirstOrDefault(x => x.ID == name);

            if (option == null)
            {
                LogWarning($"{name} doesn't exist");

                try
                {
                    splitText.RemoveAt(0);
                } catch {}

                continue;
            }

            var value = parts[1];

            try
            {
                switch (option)
                {
                    case NumberOptionAttribute number:
                        number.Set(Number.Parse(value), false);
                        break;

                    case ToggleOptionAttribute toggle:
                        toggle.Set(bool.Parse(value), false);
                        break;

                    case StringOptionAttribute @string:
                        @string.Set((Enum)Enum.Parse(option.TargetType, value), false);
                        break;

                    case RoleListEntryAttribute entry:
                        entry.Set(Enum.Parse<LayerEnum>(value), false);
                        break;

                    case LayersOptionAttribute layer:
                        layer.Set(RoleOptionData.Parse(value), false);
                        break;
                }
            }
            catch (Exception e)
            {
                LogError($"Unable to set - {opt}\nException:\n{e}");
            }

            if (pos >= 50)
            {
                pos = 0;
                yield return EndFrame();
            }
        }

        SendOptionRPC();
        yield break;
    }

    public static List<T> GetOptions<T>() where T : OptionAttribute => AllOptions.Where(x => x.GetType() == typeof(T)).Cast<T>().ToList();

    public static OptionAttribute GetOption(string id) => AllOptions.Find(x => x.ID == id);

    public static T GetOption<T>(string id) where T : OptionAttribute => GetOption(id) as T;

    public static OptionAttribute GetOptionFromName(string name) => GetOption($"CustomOption.{name}");

    public static T GetOptionFromName<T>(string name) where T : OptionAttribute => GetOptionFromName(name) as T;

    // public static OptionAttribute GetOptionFromProperty(PropertyInfo prop) => AllOptions.Find(x => x.Property == prop);
}