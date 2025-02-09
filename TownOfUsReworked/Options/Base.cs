namespace TownOfUsReworked.Options;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public abstract class OptionAttribute(MultiMenu menu, CustomOptionType type, int priority = -1) : Attribute
{
    public static readonly List<OptionAttribute> AllOptions = [];
    public static readonly List<OptionAttribute> SortedOptions = [];
    public string ID { get; set; }
    public readonly List<MultiMenu> Menus = [ menu ];
    public MonoBehaviour Setting { get; set; }
    public MonoBehaviour ViewSetting { get; set; }
    public CustomOptionType Type { get; } = type;
    public bool All { get; set; }
    public bool ClientOnly { get; set; }
    public PropertyInfo Property { get; set; }
    public string Name { get; set; } // Not actually the setting text, just the property/class name :]
    public Type TargetType { get; set; }
    public int Priority { get; set; } = priority;

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
        ( [ "CoronerKillerNameTime" ], [ "CoronerReportName" ] ),
        ( [ "DrunkInterval" ], [ "DrunkControlsSwap" ] ),
        ( [ "WhisperRateDecrease" ], [ "WhisperRateDecreases" ] ),
        ( [ "WhisperCdIncrease" ], [ "WhisperCdIncreases" ] ),
        ( [ "NecroKillCdIncrease" ], [ "NecroKillCdIncreases" ] ),
        ( [ "ResurrectCdIncrease" ], [ "ResurrectCdIncreases" ] ),
        ( [ "JestSwitchVent" ], [ "JesterVent" ] ),
        ( [ "ExeSwitchVent" ], [ "ExeVent" ] ),
        ( [ "SurvSwitchVent" ], [ "SurvVent" ] ),
        ( [ "AmneSwitchVent" ], [ "AmneVent" ] ),
        ( [ "GASwitchVent" ], [ "GAVent" ] ),
        ( [ "GuessSwitchVent" ], [ "GuessVent" ] ),
        ( [ "TrollSwitchVent" ], [ "TrollVent" ] ),
        ( [ "ActSwitchVent" ], [ "ActorVent" ] )
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
        ( [ "RandomMapSubmerged" ], [ MapEnum.Random, "SubLoaded" ] ),
        ( [ "RandomMapLevelImpostor" ], [ MapEnum.Random, "LILoaded" ] ),
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
        ( [ "HowIsVigilanteNotified" ], [ VigiOptions.PostMeeting, VigiOptions.PreMeeting ] ),
        ( [ "RoleListEntries", "RoleListBans" ], [ GameMode.RoleList ] ),
        ( [ "Dispositions", "Modifiers", "Abilities" ], [ GameMode.Classic, GameMode.KillingOnly, GameMode.AllAny, GameMode.Custom ] ),
        ( [ "NoSolo" ], [ NoSolo.SameNKs ] )
    ];
    private static readonly Dictionary<string, bool> MapToLoaded = [];

    public virtual void SetProperty(PropertyInfo property)
    {
        Property = property;
        Name = property.Name.Replace("Priv", "");
        ID = $"CustomOption.{Name}";
        TargetType = property.PropertyType;
        AllOptions.Add(this);
    }

    public virtual string Format() => "";

    public virtual void Update() {}

    public virtual void ViewUpdate() {}

    public bool Active()
    {
        var result = true;

        if (OptionParents1.TryFindingAll(x => x.Item1.Contains(Name), out var parents))
            result &= parents.AllAnyOrEmpty(x => x.Item2.AllAnyOrEmpty(IsActive, All), All);

        if (OptionParents2.TryFindingAll(x => x.Item1.Contains(Name), out parents))
            result &= parents.AllAnyOrEmpty(x => x.Item2.AllAnyOrEmpty(IsActive, All), All);

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
            result = Menus.Any(x => (int)x == SettingsPatches.SettingsPage) || RoleGen.GetSpawnItem(layer).IsActive();
        }
        else if (option is MultiMenu menu)
            result = Menus.Contains(menu);
        else if (option is int num)
            result = SettingsPatches.SettingsPage == num;
        else if (option is VigiOptions vigiop)
            result = Vigilante.HowDoesVigilanteDie == vigiop;
        else if (option is NoSolo noSolo)
            result = NeutralSettings.NoSolo == noSolo;
        else if (option is string id)
        {
            if (id == Name)
                return true; // To prevent accidental stack overflows, very rudementary because I've already managed to cause several of them even with this line active

            var optionatt = GetOption(id);

            if (optionatt != null)
            {
                result = optionatt.Active();

                if (optionatt is OptionAttribute<bool> boolOpt)
                    result &= boolOpt.Get();
            }
            else if (!MapToLoaded.TryGetValue(id, out result))
                MapToLoaded[id] = result = AccessTools.GetDeclaredProperties(typeof(ModCompatibility)).Find(x => x.Name == id).GetValue<bool>(null);
        }

        return result;
    }

    public virtual void OptionCreated()
    {
        Setting.name = ID;

        if (Setting is OptionBehaviour option)
        {
            option.Title = (StringNames)999999999;
            option.OnValueChanged = (Action<OptionBehaviour>)BlankVoid;
        }
    }

    public virtual void ViewOptionCreated()
    {
        ViewSetting.name = ID;

        if (ViewSetting is ViewSettingsInfoPanel viewSettingsInfoPanel)
        {
            viewSettingsInfoPanel.titleText.text = SettingNotif();
            viewSettingsInfoPanel.background.gameObject.SetActive(true);
        }
    }

    public virtual void PostLoadSetup() {}

    public virtual string SettingNotif() => TranslationManager.Translate(ID);

    public virtual void AddMenuIndex(int index)
    {
        var menu = (MultiMenu)index;

        if (!Menus.Contains(menu))
            Menus.Add(menu);
    }

    public void SetBase(object value, bool rpc = true, bool notify = true)
    {
        if (IsInGame() && !ClientOnly)
            return;

        if (this is ToggleOptionAttribute toggle)
            toggle.Set((bool)value, rpc, notify);
        else if (this is NumberOptionAttribute number)
            number.Set((Number)value, rpc, notify);
        else if (this is StringOptionAttribute stringOpt)
            stringOpt.Set((Enum)value, rpc, notify);
        else if (this is RoleListEntryAttribute entry)
            entry.Set((LayerEnum)value, rpc, notify);
        else if (this is LayerOptionAttribute layer)
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

        builder.AppendLine($"Map:{MapSettings.Map}");
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

        if (!SettingsPatches.Save || SettingsPatches.PresetsButtons.Any(x => x.name == fileName))
            return;

        SettingsPatches.CreatePresetButton(fileName);
        SettingsPatches.OnPageChanged();
    }

    public static void LoadPreset(string presetName, TextMeshPro tmp)
    {
        Message($"Loading - {presetName}");
        var text = ReadDiskText($"{presetName}.txt", TownOfUsReworked.Options);

        if (IsNullEmptyOrWhiteSpace(text))
        {
            Error($"{presetName} no exist");
            FlashText(tmp, UColor.red);
        }
        else
        {
            CallRpc(CustomRPC.Misc, MiscRPC.LoadPreset, presetName);
            SettingsPatches.CurrentPreset = presetName;
            LoadSettings(text);
            FlashText(tmp, UColor.green);
        }
    }

    public static void FlashText(TextMeshPro text, UColor color) => Coroutines.Start(CoFlashText(text, color));

    private static IEnumerator CoFlashText(TextMeshPro text, UColor color)
    {
        if (!text)
            yield break;

        var cache = text.color;
        text.color = color;
        yield return Wait(0.5f);
        text.color = cache;
        yield break;
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
            var option = AllOptions.Where(x => x is not IOptionGroup).FirstOrDefault(x => x.ID == name);

            if (option == null && name != "Map")
            {
                Warning($"{opt} doesn't exist");
                continue;
            }

            var value = parts[1];

            try
            {
                if (name == "Map")
                    SettingsPatches.SetMap(Enum.Parse<MapEnum>(value));
                else
                {
                    var val = option.Type switch
                    {
                        CustomOptionType.Toggle => bool.Parse(value),
                        CustomOptionType.Number => Number.Parse(value),
                        CustomOptionType.Layer => RoleOptionData.Parse(value),
                        CustomOptionType.String => Enum.Parse(option.TargetType, value),
                        CustomOptionType.Entry => Enum.Parse<LayerEnum>(value),
                        _ => true
                    };
                    option.SetBase(val, false);
                }
            }
            catch (Exception e)
            {
                Error($"Unable to set - {opt}\nException:\n{e}");
            }

            if (pos >= 50)
            {
                pos = 0;
                yield return EndFrame();
            }
        }

        SendOptionRPC(save: false);
        yield break;
    }

    public static List<T> GetOptions<T>() where T : OptionAttribute => AllOptions.Where(x => x is T).Cast<T>().ToList();

    public static OptionAttribute GetOption(string id) => AllOptions.Find(x => x.ID == $"CustomOption.{id}" || x.Name == id || x.ID == id);

    public static T GetOption<T>(string id) where T : OptionAttribute => GetOption(id) as T;
}