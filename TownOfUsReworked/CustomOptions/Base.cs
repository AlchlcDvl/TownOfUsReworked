namespace TownOfUsReworked.CustomOptions;

public class CustomOption
{
    public static readonly List<CustomOption> AllOptions = new();
    public readonly int ID;
    public readonly MultiMenu Menu;
    public Func<object, object, string> Format { get; set; }
    public readonly string Name;
    public object Value { get; set; }
    public object OtherValue { get; set; }
    public OptionBehaviour Setting { get; set; }
    public readonly CustomOptionType Type;
    public object[] Parents { get; set; }
    public readonly bool All;
    public bool Active => All ? Parents.All(IsActive) : Parents.Any(IsActive);

    public CustomOption(int id, MultiMenu menu, string name, CustomOptionType type, object defaultValue, object otherDefault, object[] parent, bool all = false)
    {
        ID = id;
        Menu = menu;
        Name = name;
        Type = type;
        Parents = parent;
        All = all;
        Value = defaultValue;
        OtherValue = otherDefault;

        if (Type != CustomOptionType.Button)
            AllOptions.Add(this);
    }

    public CustomOption(int id, MultiMenu menu, string name, CustomOptionType type, object defaultValue, object parent = null) : this(id, menu, name, type, defaultValue, new[] { parent }) {}

    public CustomOption(int id, MultiMenu menu, string name, CustomOptionType type, object defaultValue, object[] parent, bool all = false) : this(id, menu, name, type, defaultValue, null,
        parent, all) {}

    public CustomOption(int id, MultiMenu menu, string name, CustomOptionType type, object defaultValue, object otherDefault, object parent = null) : this(id, menu, name, type, defaultValue,
        otherDefault, new[] { parent }) {}

    public override string ToString()
    {
        var n = this is CustomHeaderOption ? "\n" : "";
        var colon = this is CustomHeaderOption ? "" : ": ";
        var name = this is CustomHeaderOption ? $"<b>{Name}</b>" : Name;
        return n + name + colon + Format(Value, OtherValue);
    }

    private static bool IsActive(object option)
    {
        if (option == null)
            return true;
        else if (option is CustomToggleOption toggle)
            return toggle.Get() && toggle.Active;
        else if (option is CustomLayersOption layers)
            return layers.GetChance() > 0 && !IsRoleList && !IsVanilla && layers.Active;
        else if (option is CustomHeaderOption header)
            return header.Active;
        else if (option is MapEnum map)
            return CustomGameOptions.Map == map;
        else if (option is GameMode mode)
            return CustomGameOptions.GameMode == mode;
        else if (option is LayerEnum layer)
            return GetOptions<RoleListEntryOption>(CustomOptionType.Entry).Any(x => x.Name.Contains("Entry") && (x.Get() == layer || x.Get() == LayerEnum.Any)) && IsRoleList;
        else if (option is CustomOption custom)
            return custom.Active;

        return false;
    }

    public virtual void OptionCreated()
    {
        Setting.name = Setting.gameObject.name = Name.Replace(" ", "_");
        Setting.Title = (StringNames)999999999;
        Setting.OnValueChanged = new Action<OptionBehaviour>(_ => {});
    }

    public void Set(object value, object otherValue = null)
    {
        Value = value;
        OtherValue = otherValue;

        if (Setting && AmongUsClient.Instance.AmHost)
            SendOptionRPC(this);

        if (Setting is ToggleOption toggle)
        {
            if (Type == CustomOptionType.Entry)
                toggle.TitleText.text = RoleListEntryOption.GetString(Value);
            else
            {
                var newValue = (bool)Value;
                toggle.oldValue = newValue;

                if (toggle.CheckMark != null)
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

    public static void SaveSettings(string fileName)
    {
        var builder = new StringBuilder();

        foreach (var option in AllOptions)
        {
            if (option.Type is CustomOptionType.Button or CustomOptionType.Header)
                continue;

            builder.AppendLine(option.Name.Trim());

            if (option is RoleListEntryOption entry)
                builder.AppendLine(((int)entry.Get()).ToString().Trim());
            else
                builder.AppendLine(option.Value.ToString().Trim());

            if (option.OtherValue != null)
                builder.AppendLine(option.OtherValue?.ToString().Trim());
        }

        SaveText(fileName, builder.ToString());
    }

    public static List<CustomOption> GetOptions(CustomOptionType type) => AllOptions.Where(x => x.Type == type).ToList();

    public static List<T> GetOptions<T>(CustomOptionType type) where T : CustomOption => GetOptions(type).Cast<T>().ToList();
}