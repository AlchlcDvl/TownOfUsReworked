namespace TownOfUsReworked.Options2;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public abstract class OptionAttribute(MultiMenu2 menu, CustomOptionType type) : Attribute
{
    public static readonly List<OptionAttribute> AllOptions = [];
    private static string LastChangedSetting = "";
    public string ID { get; set; }
    public MultiMenu2 Menu { get; } = menu;
    public object Value { get; set; }
    public object DefaultValue { get; set; }
    public MonoBehaviour Setting { get; set; }
    public MonoBehaviour ViewSetting { get; set; }
    public CustomOptionType Type { get; } = type;
    public bool All { get; set; }
    public bool ClientOnly { get; set; }
    public PropertyInfo Property { get; set; }
    // public bool Invert { get; set; }
    // public MethodInfo OnChanged { get; set; }
    // public Type OnChangedType { get; set; }
    // public string OnChangedName { get; set; }

    // Apparently, setting the parents in the attibutes doesn't seem to work
    public static readonly List<(string[], object[])> OptionParents1 =
    [
        ( [ "EjectionRevealsRole" ], [ "ConfirmEjects" ] ),
        ( [ "InitialCooldowns" ], [ "EnableInitialCds" ] ),
        ( [ "MeetingCooldowns" ], [ "EnableMeetingCds" ] ),
        ( [ "FailCooldowns" ], [ "EnableFailCds" ] ),
        ( [ "RLSettings" ], [ GameMode.RoleList ] ),
        ( [ "ClassCustSettings" ], [ GameMode.Classic, GameMode.Custom ] ),
        ( [ "KOSettings" ], [ GameMode.KillingOnly ] ),
        ( [ "HnSSettings" ], [ GameMode.HideAndSeek ] ),
        ( [ "TRSettings" ], [ GameMode.TaskRace ] ),
    ];
    // I need a second one because for some dumb reason the game likes crashing
    public static readonly List<(string[], object[])> OptionParents2 =
    [
        ( [ "TaskBar" ], [ GameMode.Classic, GameMode.Custom, GameMode.AllAny, GameMode.KillingOnly, GameMode.RoleList, GameMode.Vanilla ] ),
    ];

    public void SetProperty(PropertyInfo property)
    {
        Property = property;
        Value = DefaultValue = property.GetValue(null);
        ID = $"CustomOption.{property.Name}";
        // OnChanged = AccessTools.GetDeclaredMethods(OnChangedType).Find(x => x.Name == OnChangedName);
        AllOptions.Add(this);
    }

    public virtual string Format() => "";

    public bool Active()
    {
        var result = true;

        if (OptionParents1.Any(x => x.Item1.Contains(Property.Name)))
        {
            var parents = OptionParents1.Find(x => x.Item1.Contains(Property.Name)).Item2;
            result = parents.Length == 0 || (All ? parents.All(IsActive) : parents.Any(IsActive));

            if (OptionParents2.Any(x => x.Item1.Contains(Property.Name)))
            {
                parents = OptionParents2.Find(x => x.Item1.Contains(Property.Name)).Item2;
                result &= parents.Length == 0 || (All ? parents.All(IsActive) : parents.Any(IsActive));
            }
        }

        return result;
    }

    private bool IsActive(object option)
    {
        var result = false;

        if (option == null)
            result = true;
        else if (option is MapEnum map)
            result = CustomGameOptions2.Map == map;
        else if (option is GameMode mode)
            result = CustomGameOptions2.GameMode == mode;
        // else if (option is LayerEnum layer)
        //     result = GetOptions<RoleListEntryAttribute>().Any(x => x.ID.Contains("Entry") && (x.Get() == layer || x.Get() == LayerEnum.Any)) && IsRoleList;
        // else if (option.GetType() == typeof((string, string)))
        // {
        //     var tuple = ((string, string))option;
        //     var type = AccessTools.GetTypesFromAssembly(TownOfUsReworked.Core).Find(x => x.Name == tuple.Item1);
        //     result = (bool)AccessTools.GetDeclaredProperties(type).Find(x => x.Name == tuple.Item2)?.GetValue(null);
        // }
        else if (option is string id)
        {
            if (id == Property.Name)
                return true; // To prevent accidental stack overflows

            var optionatt = GetOptionFromPropertyName(id);

            if (optionatt != null)
            {
                result = optionatt.Active();

                if (optionatt is ToggleOptionAttribute toggle)
                    result &= toggle.Get();
                else if (optionatt is HeaderOptionAttribute header)
                    result &= header.Get();
                else if (optionatt is LayersOptionAttribute layers)
                    result &= (layers.GetChance() > 0 && (IsCustom || IsCustom)) || (layers.GetActive() && (IsKilling || IsAA));
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

    public void Set(object value, bool rpc = true, bool notify = true)
    {
        Value = value;
        Property.SetValue(null, value);
        // OnChanged.Invoke(value);

        if (AmongUsClient.Instance.AmHost && rpc && !(ClientOnly || !ID.Contains("CustomOption")))
            SendOptionRPC(this);

        if (!Setting)
            return;

        var stringValue = "";

        if (Setting is ToggleOption toggle)
        {
            // if (this is RoleListEntryAttribute)
            //     toggle.TitleText.text = Format();
            // else
            // {
                var newValue = (bool)Value;
                toggle.oldValue = newValue;

                if (toggle.CheckMark)
                    toggle.CheckMark.enabled = newValue;

                stringValue = newValue ? "On" : "Off";
            // }
        }
        else if (Setting is NumberOption number)
        {
            number.Value = number.oldValue = Value is int v ? v : (float)Value; // Part 2 of my mental breakdown
            number.ValueText.text = stringValue = Format();
        }
        else if (Setting is StringOption str)
        {
            var stringOption = (StringOptionAttribute)this;
            str.Value = str.oldValue = stringOption.Index = Mathf.Clamp((int)Value, 0, stringOption.Values.Length - 1);
            str.ValueText.text = stringValue = Format();
        }
        else if (Setting is RoleOptionSetting role)
        {
            var data = (RoleOptionData)Value;
            var layer = (LayersOptionAttribute)this;
            role.chanceText.text = $"{data.Chance}%";
            role.countText.text = $"x{data.Count}";
            layer.UniqueCheck.enabled = data.Unique;
            layer.ActiveCheck.enabled = data.Active;
            stringValue = Format();
        }

        SettingsPatches.OnValueChanged(GameSettingMenu.Instance);

        if (!notify || IsNullEmptyOrWhiteSpace(stringValue))
            return;

        var changed = $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{TranslationManager.Translate(ID)}</font> set to <font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{stringValue}</font>";

        if (LastChangedSetting == ID && HUD.Notifier.activeMessages.Count > 0)
            HUD.Notifier.activeMessages[^1].UpdateMessage(changed);
        else
        {
            LastChangedSetting = ID;
            var newMessage = UObject.Instantiate(HUD.Notifier.notificationMessageOrigin, Vector3.zero, Quaternion.identity, HUD.Notifier.transform);
            newMessage.transform.localPosition = new(0f, 0f, -2f);
            newMessage.SetUp(changed, HUD.Notifier.settingsChangeSprite, HUD.Notifier.settingsChangeColor, (Action)(() => HUD.Notifier.OnMessageDestroy(newMessage)));
            HUD.Notifier.ShiftMessages();
            HUD.Notifier.AddMessageToQueue(newMessage);
        }
    }

    public static string SettingsToString(List<OptionAttribute> list = null)
    {
        list ??= AllOptions;
        var builder = new StringBuilder();

        foreach (var option in list)
        {
            if (option.Type is CustomOptionType.Button or CustomOptionType.Header || option.ClientOnly || !option.ID.Contains("CustomOption"))
                continue;

            builder.AppendLine(option.ID);
            builder.AppendLine(option.Value.ToString());
        }

        return builder.ToString();
    }

    public static void SaveSettings(string fileName) => SaveText(fileName, SettingsToString(), TownOfUsReworked.Options);

    public static void LoadSettings(string settingsData) => Coroutines.Start(CoLoadSettings(settingsData));

    public static IEnumerator CoLoadSettings(string settingsData)
    {
        var splitText = settingsData.Split('\n').ToList();
        splitText.RemoveAll(IsNullEmptyOrWhiteSpace);
        var pos = 0;

        while (splitText.Any())
        {
            pos++;
            var name = splitText[0];
            splitText.RemoveAt(0);
            var option = GetOption(name);

            if (option == null)
            {
                LogWarning($"{name} doesn't exist");

                try
                {
                    splitText.RemoveAt(0);
                } catch {}

                continue;
            }

            var value = splitText[0];
            splitText.RemoveAt(0);

            try
            {
                switch (option.Type)
                {
                    case CustomOptionType.Number:
                        option.Set(float.Parse(value), false);
                        break;

                    case CustomOptionType.Toggle:
                        option.Set(bool.Parse(value), false);
                        break;

                    case CustomOptionType.String:
                        option.Set(int.Parse(value), false);
                        break;

                    case CustomOptionType.Entry:
                        option.Set(Enum.Parse<LayerEnum>(value), false);
                        break;

                    case CustomOptionType.Layers:
                        option.Set(RoleOptionData.Parse(value), false);
                        break;
                }
            }
            catch (Exception e)
            {
                LogError($"Unable to set - {name} : {value}\nException:\n{e}");
            }

            if (pos >= 50)
            {
                pos = 0;
                yield return EndFrame();
            }
        }

        SendOptionRPC(setting: (OptionAttribute)null);
        yield break;
    }

    public static List<OptionAttribute> GetOptions(CustomOptionType type) => AllOptions.Where(x => x.Type == type).ToList();

    public static List<T> GetOptions<T>() where T : OptionAttribute => AllOptions.Where(x => x.GetType() == typeof(T)).Cast<T>().ToList();

    public static OptionAttribute GetOption(string title) => AllOptions.Find(x => x.ID == title);

    public static OptionAttribute GetOptionFromPropertyName(string propertyName) => AllOptions.Find(x => x.Property.Name == propertyName);

    public static T GetOption<T>(string title) where T : OptionAttribute => GetOption(title) as T;
}