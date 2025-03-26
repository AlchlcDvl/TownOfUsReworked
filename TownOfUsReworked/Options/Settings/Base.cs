namespace TownOfUsReworked.Options;

public abstract class Option(CustomOptionType type)
{
    public static readonly List<Option> AllOptions = [];
    public static readonly List<BaseHeaderOption> SortedOptions = [];

    public string ID { get; set; }
    public MonoBehaviour Setting { get; set; }
    public MonoBehaviour ViewSetting { get; set; }
    public CustomOptionType Type { get; } = type;
    public bool All { get; set; }
    public bool ClientOnly { get; set; }
    protected MemberInfo Member { get; private set; }
    public string Name { get; set; } // Not actually the setting text, just the member name :]
    public KeyValuePair<byte, byte> RpcId { get; protected set; }
    public BaseHeaderOption Header { get; set; }

    protected static string LastChangedSetting = "";
    protected static LobbyNotificationMessage LastSettingNotif;

    // Apparently, setting the parents in the attributes doesn't seem to work
    // This one is for those depending on other options
    private static readonly List<(string[], object[])> OptionParents1 =
    [
        ([ "EjectionRevealsRoles", "JestEjectScreen", "ExeEjectScreen" ], [ "ConfirmEjects" ]),
        ([ "InitialCooldowns" ], [ "EnableInitialCds" ]),
        ([ "MeetingCooldowns" ], [ "EnableMeetingCds" ]),
        ([ "FailCooldowns" ], [ "EnableFailCds" ]),
        ([ "WhoSeesFirstKillShield" ], [ "FirstKillShield" ]),
        ([ "WhispersAnnouncement" ], [ "Whispers" ]),
        ([ "KillerReports", "RoleFactionReports", "LocationReports" ], [ "GameAnnouncements" ]),
        ([ "SmallMapHalfVision", "SmallMapDecreasedCooldown", "LargeMapIncreasedCooldown", "SmallMapIncreasedShortTasks", "SmallMapIncreasedLongTasks", "LargeMapDecreasedShortTasks",
            "LargeMapDecreasedLongTasks" ], [ "AutoAdjustSettings" ]),
        ([ "EvilsIgnoreNv" ], [ "NightVision" ]),
        ([ "SkeldVentImprovements" , "SkeldReactorTimer", "SkeldO2Timer" ], [ "EnableBetterSkeld" ]),
        ([ "MiraHQVentImprovements" , "MiraReactorTimer", "MiraO2Timer" ], [ "EnableBetterMiraHq" ]),
        ([ "PolusVentImprovements", "VitalsLab", "ColdTempDeathValley", "WifiChartCourseSwap", "SeismicTimer" ], [ "EnableBetterPolus" ]),
        ([ "SpawnType", "MoveVitals", "MoveFuel", "MoveDivert", "MoveAdmin", "MoveElectrical", "MinDoorSwipeTime", "CrashTimer" ], [ "EnableBetterAirship" ]),
        ([ "FungleReactorTimer", "FungleMixupTimer" ], [ "EnableBetterFungle" ]),
        ([ "CoronerKillerNameTime" ], [ "CoronerReportName" ]),
        ([ "DrunkInterval" ], [ "DrunkControlsSwap" ]),
        ([ "WhisperRateDecrease" ], [ "WhisperRateDecreases" ]),
        ([ "WhisperCdIncrease" ], [ "WhisperCdIncreases" ]),
        ([ "NecroKillCdIncrease" ], [ "NecroKillCdIncreases" ]),
        ([ "JestSwitchVent" ], [ "JesterVent" ]),
        ([ "ExeSwitchVent" ], [ "ExeVent" ]),
        ([ "SurvSwitchVent" ], [ "SurvVent" ]),
        ([ "AmneSwitchVent" ], [ "AmneVent" ]),
        ([ "GASwitchVent" ], [ "GAVent" ]),
        ([ "GuessSwitchVent" ], [ "GuessVent" ]),
        ([ "TrollSwitchVent" ], [ "TrollVent" ]),
        ([ "InteractCd" ], [ "CanInteract" ]),
        ([ "CrewMax", "CrewMin", "NeutralMax", "NeutralMin", "IntruderMax", "IntruderMin", "SyndicateMax", "SyndicateMin" ], [ "not+IgnoreFactionCaps" ]),
        ([ "MaxDispositions", "MinDispositions", "MinAbilities", "MaxAbilities", "MinModifiers", "MaxModifiers" ], [ "not+IgnoreLayerCaps" ]),
        ([ "MaxCI", "MaxCK", "MaxCrP", "MaxCSv", "MaxCS", "MaxNB", "MaxNE", "MaxNH", "MaxNK", "MaxNN", "MaxIC", "MaxID", "MaxIH", "MaxIK", "MaxIS", "MaxSD", "MaxSyK", "MaxSP", "MaxSSu" ], [
            "not+IgnoreAlignmentCaps" ]),
        ([ "Allied", "Allied1" ], [ "not+IlluminatiUnleashed", "not+OrderOfCompliance" ]),
        ([ "PandoricaOpens", "OrderOfCompliance" ], [ "not+IlluminatiUnleashed" ]),
        ([ "RoundOneNoMayorReveal" ], [ "MayorDirectSpawn" ]),
        ([ "ComplianceType" ], [ "OrderOfCompliance" ])
    ];
    // I need a second one because for some dumb reason the game likes crashing
    // This is for everything else
    private static readonly List<(string[], object[])> OptionParents2 =
    [
        ([ "TaskBar" ], [ GameMode.Classic, GameMode.AllAny, GameMode.List, GameMode.Vanilla ]),
        ([ "IgnoreAlignmentCaps", "IgnoreFactionCaps", "IgnoreLayerCaps" ], [ GameMode.Classic ]),
        ([ "HunterCount", "HuntCd", "StartTime", "HunterVent", "HunterVision", "HuntedVision", "HunterSpeedModifier", "HuntedChat", "HunterFlashlight", "HuntedFlashlight", "HnSMode" ], [
            GameMode.HideAndSeek ]),
        ([ "RandomMapSkeld", "RandomMapMira", "RandomMapPolus", "RandomMapdlekS", "RandomMapAirship", "RandomMapFungle" ], [ MapEnum.Random ]),
        ([ "RandomMapSubmerged" ], [ MapEnum.Random, "SubLoaded" ]),
        ([ "RandomMapLevelImpostor" ], [ MapEnum.Random, "LiLoaded" ]),
        ([ "SmallMapHalfVision", "SmallMapDecreasedCooldown", "SmallMapIncreasedShortTasks", "SmallMapIncreasedLongTasks", "OxySlow" ], [ MapEnum.Skeld, MapEnum.dlekS, MapEnum.Random,
            MapEnum.MiraHq, MapEnum.LevelImpostor ]),
        ([ "LargeMapDecreasedShortTasks", "LargeMapDecreasedLongTasks", "LargeMapIncreasedCooldown" ], [ MapEnum.Airship, MapEnum.Submerged, MapEnum.Random, MapEnum.Fungle,
            MapEnum.LevelImpostor ]),
        ([ "BetterSkeld" ], [ MapEnum.Skeld, MapEnum.dlekS, MapEnum.Random ]),
        ([ "BetterMiraHq" ], [ MapEnum.MiraHq, MapEnum.Random ]),
        ([ "BetterPolus" ], [ MapEnum.Polus, MapEnum.Random ]),
        ([ "BetterAirship" ], [ MapEnum.Airship, MapEnum.Random ]),
        ([ "BetterFungle" ], [ MapEnum.Fungle, MapEnum.Random ]),
        ([ "CrewSettings" ], [ GameMode.Classic, GameMode.AllAny, GameMode.Vanilla, GameMode.List ]),
        ([ "CrewMax", "CrewMin", "NeutralMax", "NeutralMin", "IntruderMax", "IntruderMin", "SyndicateMax", "SyndicateMin" ], [ GameMode.Classic, GameMode.AllAny ]),
        ([ "HowIsVigilanteNotified" ], [ VigiOptions.PostMeeting, VigiOptions.PreMeeting ]),
        ([ "RevealerCount", "PhantomCount", "GhoulCount", "BansheeCount", "BanCrewmate", "BanMurderer", "BanImpostor", "BanAnarchist", "RoleEntryList", "ModifierEntryList", "ModifierBanList",
            "DispositionEntryList", "AbilityEntryList", "RoleBanList", "AbilityBanList", "DispositionBanList" ], [ GameMode.List ]),
        ([ "RunnerVision" ], [ GameMode.TaskRace ]),
        ([ "Dispositions", "Modifiers", "Abilities" ], [ GameMode.Classic, GameMode.List, GameMode.AllAny ]),
        ([ "Location1", "Location2", "Location3" ], [ AirshipSpawnType.Fixed ])
    ];
    private static readonly Dictionary<string, bool> MapToLoaded = [];

    public virtual void Set(MemberInfo member, BaseHeaderOption header, bool clientOnly)
    {
        Header = header;
        ClientOnly = clientOnly;
        Member = member;
        Name = member.Name.Replace("Priv", "");
        ID = $"CustomOption.{Name}";
        RpcId = new((byte)(AllOptions.Count / 255), (byte)(AllOptions.Count % 255)); // Gotta love being able to theoretically have 2^16 options
        AllOptions.Add(this);
    }

    protected virtual string Format() => "";

    public virtual void Update() {}

    public virtual void ViewUpdate() {}

    public bool PartiallyActive()
    {
        var result = true;

        if (OptionParents1.TryFindingAll(x => x.Item1.Contains(Name), out var parents))
            result &= parents.AllAnyOrEmpty(x => x.Item2.AllAnyOrEmpty(IsActive, All), All);

        if (OptionParents2.TryFindingAll(x => x.Item1.Contains(Name), out parents))
            result &= parents.AllAnyOrEmpty(x => x.Item2.AllAnyOrEmpty(IsActive, All), All);

        return result && Visible();
    }

    public bool Active() => PartiallyActive() && Header?.Value != false;

    private bool IsActive(object option) => option switch
    {
        MapEnum map => MapSettings.Map == map,
        GameMode mode => GameModeSettings.GameMode == mode,
        VigiOptions vigiOptions => Vigilante.HowDoesVigilanteDie == vigiOptions,
        string id => GetBoolValue(id),
        AirshipSpawnType spawnType => BetterAirship.SpawnType == spawnType,
        LayerEnum layer => RoleGenManager.GetSpawnItem(layer).IsActive(),
        _ => true
    };

    private bool GetBoolValue(string id)
    {
        if (id == Name)
            return true; // To prevent accidental stack overflows, very rudimentary because I've already managed to cause several of them even with this line active

        var invertVal = id.StartsWith("not+");
        id = id.Replace("not+", "");
        bool result;

        if (AllOptions.TryFinding(x => x.Name == id, out var optionatt))
        {
            result = optionatt.PartiallyActive();

            if (optionatt is Option<bool> boolOpt)
                result &= invertVal ? !boolOpt.Value : boolOpt.Value;
        }
        else if (!MapToLoaded.TryGetValue(id, out result))
            MapToLoaded[id] = result = AccessTools.GetDeclaredProperties(typeof(ModCompatibility)).Find(x => x.Name == id).GetValue<bool>(null);

        return result;
    }

    public virtual void OptionCreated()
    {
        Setting.name = ID;

        if (Setting is not OptionBehaviour option)
            return;

        option.Title = TranslationManager.GetOrAddName("Option.Option");
        option.OnValueChanged = (Action<OptionBehaviour>)BlankVoid;
    }

    public virtual void ViewOptionCreated()
    {
        ViewSetting.name = $"{ID}.View";

        if (ViewSetting is not ViewSettingsInfoPanel viewSettingsInfoPanel)
            return;

        viewSettingsInfoPanel.titleText.text = SettingNotif();
        viewSettingsInfoPanel.background.gameObject.SetActive(true);
    }

    public virtual void PostLoadSetup() {}

    protected virtual string SettingNotif() => TranslationManager.Translate(ID);

    public virtual void Debug() => TranslationManager.DebugId(ID);

    public virtual void ReadValueRpc(NetData reader) {}

    public virtual void WriteValueRpc(NetData writer) {}

    protected virtual void ReadValueString(string value) {}

    protected virtual bool Visible() => true;

    private static string SettingsToString()
    {
        var builder = new StringBuilder();
        AllOptions.Where(option => option is not BaseHeaderOption && !option.ClientOnly && option.ID.Contains("CustomOption")).ForEach(x => builder.AppendLine($"{x}"));
        builder.AppendLine($"Map:{MapSettings.Map}");
        return $"{builder}";
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

        SettingsPatches.Overwriting = false;
        SettingsPatches.CreatePresetButton(fileName);
        SettingsPatches.OnPageChanged();
    }

    public static void HandlePreset(string presetName, TextMeshPro tmp)
    {
        if (SettingsPatches.Overwriting)
            SaveSettings(presetName);
        else
            LoadPreset(presetName, tmp);
    }

    public static void LoadPreset(string presetName, TextMeshPro tmp)
    {
        Message($"Loading - {presetName}");
        var text = ReadDiskText($"{presetName}.txt", TownOfUsReworked.Options);

        if (IsNullEmptyOrWhiteSpace(text))
        {
            Failure($"{presetName} no exist");
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

    private static void FlashText(TextMeshPro text, UColor color) => Coroutines.Start(CoFlashText(text, color));

    private static IEnumerator CoFlashText(TextMeshPro text, UColor color)
    {
        if (!text)
            yield break;

        var cache = text.color;
        text.color = color;
        yield return Wait(0.5f);
        text.color = cache;
    }

    private static void LoadSettings(string settingsData) => Coroutines.Start(CoLoadSettings(settingsData));

    private static IEnumerator CoLoadSettings(string settingsData)
    {
        foreach (var (i, opt) in settingsData.TrueSplit('\n').Indexed())
        {
            var parts = opt.TrueSplit(':');
            var name = parts[0];
            string value;

            try
            {
                value = parts[1];
            }
            catch
            {
                value = "";
            }

            if (name == "Map")
            {
                SettingsPatches.SetMap(Enum.Parse<MapEnum>(value));
                continue;
            }

            var option = GetOption(name);

            if (option == null)
            {
                if (name.ContainsAny("Entry", "Ban"))
                    ListHolderOption.CachedValues[name] = value;
                else
                    Warning($"{opt} doesn't exist");

                continue;
            }

            try
            {
                option.ReadValueString(value);
            }
            catch (Exception e)
            {
                Failure($"Unable to set - {name}:{value}\n{e}");
            }

            if (i % 50 == 0)
                yield return EndFrame();
        }

        SendOptionRPC(save: false);
        CallRpc(CustomRPC.Misc, MiscRPC.SyncMap, MapSettings.Map);
    }

    public static IEnumerable<T> GetOptions<T>() where T : Option => AllOptions.OfType<T>();

    private static Option GetOption(string id)
    {
        id = id.ToLower();
        return AllOptions.Find(x => x.ID.ToLower().IsAny($"customOption.{id}", id) || x.Name.ToLower() == id); // To support any case changes
    }

    public static Option GetOption(byte superId, byte id) => AllOptions.Find(x => x.RpcId.Key == superId && x.RpcId.Value == id);

    public static T GetOption<T>(string id) where T : Option => GetOption(id) as T;
}