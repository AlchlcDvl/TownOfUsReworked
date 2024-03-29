namespace TownOfUsReworked.Options;

public class CustomOption
{
    public static readonly List<CustomOption> AllOptions = [];
    public int ID { get; }
    public MultiMenu Menu { get; }
    public Func<object, object, string> Format { get; set; }
    public string Name { get; }
    public Action<object, object> OnChanged { get; }
    public object Value { get; set; }
    public object OtherValue { get; set; }
    public object DefaultValue { get; }
    public object OtherDefaultValue { get; }
    public OptionBehaviour Setting { get; set; }
    public CustomOptionType Type { get; }
    public object[] Parents { get; set; }
    public bool All { get; }
    public bool Invert { get; }
    public bool ClientOnly { get; }
    public bool Active => All ? Parents.All(IsActive) : Parents.Any(IsActive);

    public CustomOption(MultiMenu menu, string name, CustomOptionType type, object defaultValue, object otherDefault, object[] parent, bool all = false, Action<object, object> onChanged =
        null, bool clientOnly = false)
    {
        Menu = menu;
        Name = name;
        Type = type;
        Parents = parent;
        All = all;
        Value = DefaultValue = defaultValue;
        OtherValue = OtherDefaultValue = otherDefault;
        OnChanged = onChanged ?? BlankVoid;
        ClientOnly = clientOnly;
        ID = Type is CustomOptionType.Header or CustomOptionType.Button || ClientOnly ? -1 : AllOptions.Count(x => x.Type is not (CustomOptionType.Header or CustomOptionType.Button));

        if (Type != CustomOptionType.Button)
            AllOptions.Add(this);
    }

    public CustomOption(MultiMenu menu, string name, CustomOptionType type, object defaultValue, object parent = null, Action<object, object> onChanged = null, bool clientOnly = false) :
        this(menu, name, type, defaultValue, [parent], false, onChanged, clientOnly) {}

    public CustomOption(MultiMenu menu, string name, CustomOptionType type, object defaultValue, object[] parent, bool all = false, Action<object, object> onChanged = null, bool clientOnly =
        false) : this(menu, name, type, defaultValue, null, parent, all, onChanged, clientOnly) {}

    public CustomOption(MultiMenu menu, string name, CustomOptionType type, object defaultValue, object otherDefault, object parent = null, Action<object, object> onChanged = null, bool
        clientOnly = false) : this(menu, name, type, defaultValue, otherDefault, [parent], false, onChanged, clientOnly) {}

    public override string ToString()
    {
        var n = this is CustomHeaderOption ? "\n" : "";
        var colon = this is CustomHeaderOption ? "" : ": ";
        var name = this is CustomHeaderOption ? $"<b>{Name}</b>" : Name;
        return n + name + colon + Format(Value, OtherValue);
    }

    private bool IsActive(object option)
    {
        var result = false;

        if (option == null)
            result = true;
        else if (option is CustomToggleOption toggle)
            result = toggle && toggle.Active;
        else if (option is CustomLayersOption layers)
            result = layers.GetChance() > 0 && !IsRoleList && !IsVanilla && layers.Active;
        else if (option is CustomHeaderOption header)
            result = header.Active;
        else if (option is MapEnum map)
            result = CustomGameOptions.Map == map;
        else if (option is GameMode mode)
            result = CustomGameOptions.GameMode == mode;
        else if (option is LayerEnum layer)
            result = GetOptions<RoleListEntryOption>().Any(x => x.Name.Contains("Entry") && (x.Get() == layer || x.Get() == LayerEnum.Any)) && IsRoleList;
        else if (option is CustomOption custom)
            result = custom.Active;
        else if (option is bool boolean)
            result = boolean;

        if (Invert && option != null)
            result = !result;

        return result;
    }

    public virtual void OptionCreated()
    {
        Setting.name = Setting.gameObject.name = Name.Replace(" ", "_");
        Setting.Title = (StringNames)999999999;
        Setting.OnValueChanged = new Action<OptionBehaviour>(_ => {});
    }

    public void Set(object value, object otherValue = null, bool rpc = true)
    {
        Value = value;
        OtherValue = otherValue;
        OnChanged(Value, OtherValue);

        if (AmongUsClient.Instance.AmHost && rpc && !(ClientOnly || ID == -1 || Type is CustomOptionType.Header or CustomOptionType.Button))
            SendOptionRPC(this);

        if (!Setting)
            return;

        if (Setting is ToggleOption toggle)
        {
            if (this is RoleListEntryOption entry)
                toggle.TitleText.text = entry.GetString(Value);
            else
            {
                var newValue = (bool)Value;
                toggle.oldValue = newValue;

                if (toggle.CheckMark)
                    toggle.CheckMark.enabled = newValue;
            }
        }
        else if (Setting is NumberOption number)
        {
            number.Value = number.oldValue = (float)Value;
            number.ValueText.text = Format(Value, OtherValue);
        }
        else if (Setting is KeyValueOption str)
        {
            Value = Mathf.Clamp((int)Value, 0, ((CustomStringOption)this).Values.Length - 1);
            str.Selected = str.oldValue = (int)Value;
            str.ValueText.text = Format(Value, OtherValue);
        }
        else if (Setting is RoleOptionSetting role)
        {
            role.ChanceText.text = $"{Value}%";
            role.CountText.text = $"x{OtherValue}";
        }
    }

    public static string SettingsToString(List<CustomOption> list = null)
    {
        list ??= AllOptions;
        var builder = new StringBuilder();

        foreach (var option in list)
        {
            if (option.Type is CustomOptionType.Button or CustomOptionType.Header || option.ClientOnly || option.ID == -1)
                continue;

            builder.AppendLine(option.Name.Trim());
            builder.AppendLine(option.Value.ToString().Trim());

            if (option.OtherValue != null)
                builder.AppendLine(option.OtherValue.ToString().Trim());
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
            var name = splitText[0].Trim();
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
                        option.Set(float.Parse(value), null, false);
                        break;

                    case CustomOptionType.Toggle:
                        option.Set(bool.Parse(value), null, false);
                        break;

                    case CustomOptionType.String:
                        option.Set(int.Parse(value), null, false);
                        break;

                    case CustomOptionType.Entry:
                        option.Set(Enum.Parse<LayerEnum>(value), null, false);
                        break;

                    case CustomOptionType.Layers:
                        var value2 = splitText[0];
                        splitText.RemoveAt(0);
                        option.Set(int.Parse(value), int.Parse(value2), false);
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

        SendOptionRPC();
        yield break;
    }

    public static List<CustomOption> GetOptions(CustomOptionType type) => AllOptions.Where(x => x.Type == type).ToList();

    public static List<T> GetOptions<T>() where T : CustomOption => AllOptions.Where(x => x.GetType() == typeof(T)).Cast<T>().ToList();

    public static CustomOption GetOption(string title) => AllOptions.Find(x => x.Name.Trim() == title.Trim());

    public static T GetOption<T>(string title) where T : CustomOption => GetOption(title) as T;
}