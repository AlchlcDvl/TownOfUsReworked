namespace TownOfUsReworked.Options.Settings;

public abstract class Option(CustomOptionType type)
{
    public static readonly List<Option> AllOptions = [];
    public static readonly List<BaseHeaderOption> SortedOptions = [];

    public string ID { get; set; }
    public MonoBehaviour Setting { get; set; }
    public MonoBehaviour ViewSetting { get; set; }
    public CustomOptionType Type => type;
    public bool All { get; set; }
    public bool ClientOnly { get; set; }
    protected bool SelfMember { get; private set; }
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
        (["EjectionRevealsRoles", "JestEjectScreen", "ExeEjectScreen"], ["ConfirmEjects"]),
        (["InitialCooldowns"], ["EnableInitialCds"]),
        (["MeetingCooldowns"], ["EnableMeetingCds"]),
        (["FailCooldowns"], ["EnableFailCds"]),
        (["WhoSeesFirstKillShield"], ["FirstKillShield"]),
        (["WhispersAnnouncement"], ["Whispers"]),
        (["KillerReports", "RoleFactionReports", "LocationReports"], ["GameAnnouncements"]),
        (["SmallMapHalfVision", "SmallMapDecreasedCooldown", "LargeMapIncreasedCooldown", "SmallMapIncreasedShortTasks", "SmallMapIncreasedLongTasks", "LargeMapDecreasedShortTasks",
            "LargeMapDecreasedLongTasks"], ["AutoAdjustSettings"]),
        (["EvilsIgnoreNv"], ["NightVision"]),
        (["SkeldVentImprovements" , "SkeldReactorTimer", "SkeldO2Timer"], ["EnableBetterSkeld"]),
        (["MiraHQVentImprovements" , "MiraReactorTimer", "MiraO2Timer"], ["EnableBetterMiraHq"]),
        (["PolusVentImprovements", "VitalsLab", "ColdTempDeathValley", "WifiChartCourseSwap", "SeismicTimer"], ["EnableBetterPolus"]),
        (["SpawnType", "MoveVitals", "MoveFuel", "MoveDivert", "MoveAdmin", "MoveElectrical", "MinDoorSwipeTime", "CrashTimer"], ["EnableBetterAirship"]),
        (["FungleReactorTimer", "FungleMixupTimer"], ["EnableBetterFungle"]),
        (["CoronerKillerNameTime"], ["CoronerReportName"]),
        (["DrunkInterval"], ["DrunkControlsSwap"]),
        (["WhisperRateDecrease"], ["WhisperRateDecreases"]),
        (["WhisperCdIncrease"], ["WhisperCdIncreases"]),
        (["NecroKillCdIncrease"], ["NecroKillCdIncreases"]),
        (["JestSwitchVent"], ["JesterVent"]),
        (["ExeSwitchVent"], ["ExeVent"]),
        (["SurvSwitchVent"], ["SurvVent"]),
        (["AmneSwitchVent"], ["AmneVent"]),
        (["GASwitchVent"], ["GAVent"]),
        (["GuessSwitchVent"], ["GuessVent"]),
        (["TrollSwitchVent"], ["TrollVent"]),
        (["InteractCd"], ["CanInteract"]),
        (["CrewMax", "CrewMin", "OutcastMax", "OutcastMin", "IntruderMax", "IntruderMin", "SyndicateMax", "SyndicateMin", "ApocalypseMax", "ApocalypseMin", "ComplianceMax", "ComplianceMin",
            "PandoricaMax", "PandoricaMin", "IlluminatiMax", "IlluminatiMin"], ["not+IgnoreFactionCaps"]),
        (["MaxDispositions", "MinDispositions", "MinAbilities", "MaxAbilities", "MinModifiers", "MaxModifiers"], ["not+IgnoreLayerCaps"]),
        (["MaxCi", "MaxCk", "MaxCrP", "MaxCSv", "MaxCs", "MaxNb", "MaxNe", "MaxNh", "MaxAh", "MaxNn", "MaxNk", "MaxIc", "MaxID", "MaxIh", "MaxIs", "MaxSD", "MaxSyK", "MaxSh", "MaxSSu"], [
            "not+IgnoreAlignmentCaps"]),
        (["Allied", "Allied1"], ["not+IlluminatiUnleashed", "not+OrderOfCompliance"]),
        (["PandoricaOpens", "OrderOfCompliance"], ["not+IlluminatiUnleashed"]),
        (["RoundOneNoMayorReveal"], ["MayorDirectSpawn"]),
        (["AssassinChances"], ["AssassinChance"]),
        (["FinalTwoDisableVenting"], ["not+WhoCanVent+NoOne"]),
        (["ComplianceMembers", "ComplianceSettings"], ["OrderOfCompliance"]),
        (["PandoricaMembers", "PandoricaSettings"], ["PandoricaOpens"]),
        (["IlluminatiMembers", "IlluminatiSettings"], ["IlluminatiUnleashed"]),
        (["GameTimer", "DuringMeetings", "TimeLeft", "PlayersLeft", "KillsExtendTimer", "TasksExtendTimer"], ["EnableGameTimer"]),
        (["TimerExtension"], ["EnableGameTimer"]),
    ];
    // I need a second one because for some dumb reason the game likes crashing
    // This is for everything else
    private static readonly List<(string[], object[])> OptionParents2 =
    [
        (["TaskBar"], [Mode.Classic, Mode.AllAny, Mode.List, Mode.Vanilla]),
        (["IgnoreAlignmentCaps", "IgnoreFactionCaps", "IgnoreLayerCaps"], [Mode.Classic]),
        (["HunterCount", "HuntCd", "StartTime", "HunterVent", "HunterVision", "HuntedVision", "HunterSpeedModifier", "HuntedChat", "HunterFlashlight", "HuntedFlashlight", "HnSMode"], [
            Mode.HideAndSeek]),
        (["RandomMapSkeld", "RandomMapMira", "RandomMapPolus", "RandomMapdlekS", "RandomMapAirship", "RandomMapFungle"], [Data.Enums.Map.Random]),
        (["RandomMapSubmerged"], [Data.Enums.Map.Random, "SubLoaded"]),
        (["RandomMapLevelImpostor"], [Data.Enums.Map.Random, "LiLoaded"]),
        (["SmallMapHalfVision", "SmallMapDecreasedCooldown", "SmallMapIncreasedShortTasks", "SmallMapIncreasedLongTasks", "OxySlow"], [Data.Enums.Map.Skeld, Data.Enums.Map.dlekS, Data.Enums.Map.Random,
            Data.Enums.Map.MiraHq, Data.Enums.Map.LevelImpostor]),
        (["LargeMapDecreasedShortTasks", "LargeMapDecreasedLongTasks", "LargeMapIncreasedCooldown"], [Data.Enums.Map.Airship, Data.Enums.Map.Submerged, Data.Enums.Map.Random, Data.Enums.Map.Fungle,
            Data.Enums.Map.LevelImpostor]),
        (["BetterSkeld"], [Data.Enums.Map.Skeld, Data.Enums.Map.dlekS, Data.Enums.Map.Random]),
        (["BetterMiraHq"], [Data.Enums.Map.MiraHq, Data.Enums.Map.Random]),
        (["BetterPolus"], [Data.Enums.Map.Polus, Data.Enums.Map.Random]),
        (["BetterAirship"], [Data.Enums.Map.Airship, Data.Enums.Map.Random]),
        (["BetterFungle"], [Data.Enums.Map.Fungle, Data.Enums.Map.Random]),
        (["CrewSettings"], [Mode.Classic, Mode.AllAny, Mode.Vanilla, Mode.List]),
        (["CrewMax", "CrewMin", "OutcastMax", "OutcastMin", "IntruderMax", "IntruderMin", "SyndicateMax", "SyndicateMin"], [Mode.Classic, Mode.AllAny]),
        (["HowIsVigilanteNotified"], [VigiOptions.PostMeeting, VigiOptions.PreMeeting]),
        (["RevealerCount", "PhantomCount", "GhoulCount", "BansheeCount", "BanCrewmate", "BanMurderer", "BanImpostor", "BanAnarchist", "RoleEntryList", "ModifierEntryList", "ModifierBanList",
            "DispositionEntryList", "AbilityEntryList", "RoleBanList", "AbilityBanList", "DispositionBanList", "BanCultist", "BanZealot"], [Mode.List]),
        (["RunnerVision"], [Mode.TaskRace]),
        (["Dispositions", "Modifiers", "Abilities"], [Mode.Classic, Mode.List, Mode.AllAny]),
        (["Runner", "Runner1"], [Mode.TaskRace]),
        (["Hunted1", "Hunted", "Hunter", "Hunter1"], [Mode.HideAndSeek]),
        (["Location1", "Location2", "Location3"], [AirshipSpawnType.Fixed]),
        (["TimeLeft"], [DuringMeeting.TimeRemaining]),
        (["PlayersLeft"], [DuringMeeting.PeopleRemaining]),
    ];
    private static readonly Dictionary<string, bool> MapToLoaded = [];

    public virtual void Set(MemberInfo member, BaseHeaderOption header, bool clientOnly, bool selfMember)
    {
        Header = header;
        ClientOnly = clientOnly;
        Member = member;
        Name = member.Name;
        ID = $"CustomOption.{Name}";
        SelfMember = selfMember;
        RpcId = new((byte)(AllOptions.Count / 255), (byte)(AllOptions.Count % 255)); // Gotta love being able to theoretically have 2^16 options
        AllOptions.Add(this);
    }

    protected virtual string FormatValue() => "";

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
        Map map => MapSettings.Map == map,
        Mode mode => GameModeSettings.GameMode == mode,
        VigiOptions vigiOptions => Vigilante.HowDoesVigilanteDie == vigiOptions,
        AirshipSpawnType spawnType => BetterAirship.SpawnType == spawnType,
        Layer layer => RoleGenManager.GetSpawnItem(layer).IsActive(),
        DuringMeeting meetingTime => GameModifiers.DuringMeetings == meetingTime,
        string id => GetBoolValue(id),
        _ => true
    };

    private bool GetBoolValue(string id)
    {
        if (id == Name)
            return true; // To prevent accidental stack overflows, very rudimentary because I've already managed to cause several of them even with this line active

        var parts = id.TrueSplit('+');
        var invertVal = parts[0] == "not";
        id = parts[invertVal ? 1 : 0];
        bool result;

        if (TryGetOption(id, out var optionatt))
        {
            result = optionatt.PartiallyActive() && optionatt switch
            {
                Option<bool> boolOpt => invertVal ? !boolOpt.Value : boolOpt.Value,
                ReworkedNumberOption numOpt => invertVal ? !numOpt.IsValid(parts[2]) : numOpt.IsValid(parts[1]),
                IMultiSelectOption multiOpt => invertVal ? !multiOpt.Contains(parts[2]) : multiOpt.Contains(parts[1]),
                IStringOption stringOpt => invertVal ? !parts[2].Contains(stringOpt.ValueString()) : parts[1].Contains(stringOpt.ValueString()),
                _ => true
            };
        }
        else if (id == "Map")
            result = invertVal ? !parts[2].Contains(MapSettings.Map.ToString()) : parts[1].Contains(MapSettings.Map.ToString());
        else if (!MapToLoaded.TryGetValue(id, out result))
            MapToLoaded[id] = result = AccessTools.GetDeclaredProperties(typeof(ModCompatibilityManager)).Find(x => x.Name == id).GetValue<bool>(null);

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

    public virtual bool IsId(string id) => ID == id;

    public virtual void ReadValueRpc(RpcReader reader) {}

    public virtual void WriteValueRpc(RpcWriter writer) {}

    protected virtual void ReadValueString(string value) {}

    protected virtual bool Visible() => true;

    private static string SettingsToString()
    {
        var builder = new StringBuilder();
        AllOptions.Where(option => option is not BaseHeaderOption && !option.ClientOnly && option.ID.Contains("CustomOption")).Do(x => builder.AppendLine($"{x}"));
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
            CallRpc(ReworkedRpc.Misc, MiscRpc.LoadPreset, presetName);
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
                SettingsPatches.SetMap(Enum.Parse<Map>(value));
                continue;
            }

            var option = GetOption(name);

            if (option is null)
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
                yield return null;
        }

        SendOptionRPC(save: false);
        CallRpc(ReworkedRpc.Misc, MiscRpc.SyncMap, MapSettings.Map);
    }

    public static IEnumerable<T> GetOptions<T>() where T : Option => AllOptions.OfType<T>();

    public static IEnumerable<T> GetHeaderOptions<T>() where T : BaseHeaderOption => SortedOptions.OfType<T>();

    private static Option GetOption(string id) => AllOptions.Find(x => x.ID.IsAny($"customOption.{id}", id) || x.Name == id);

    private static bool TryGetOption(string id, out Option option) => AllOptions.TryFinding(x => x.ID.IsAny($"customOption.{id}", id) || x.Name == id, out option);

    public static Option GetOption(byte superId, byte id) => AllOptions.Find(x => x.RpcId.Key == superId && x.RpcId.Value == id);

    public static T GetOption<T>(string id) where T : Option => GetOption(id) as T;
}