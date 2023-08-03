namespace TownOfUsReworked.CustomOptions
{
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
        public readonly CustomOption[] Parent;
        public readonly bool All;
        public bool Active => Parent == null || Parent.Any(x => x == null) || (All ? Parent.All(IsActive) : Parent.Any(IsActive)) || IsRoleList;

        public CustomOption(int id, MultiMenu menu, string name, CustomOptionType type, object defaultValue, object otherDefault, CustomOption[] parent, bool all = false)
        {
            ID = id;
            Menu = menu;
            Name = name;
            Type = type;
            Parent = parent;
            All = all;
            Value = defaultValue;
            OtherValue = otherDefault;

            if (Type != CustomOptionType.Button)
                AllOptions.Add(this);
        }

        public CustomOption(int id, MultiMenu menu, string name, CustomOptionType type, object defaultValue, CustomOption parent = null) : this(id, menu, name, type, defaultValue, new[]
            { parent }) {}

        public CustomOption(int id, MultiMenu menu, string name, CustomOptionType type, object defaultValue, CustomOption[] parent, bool all = false) : this(id, menu, name, type,
            defaultValue, null, parent, all) {}

        public CustomOption(int id, MultiMenu menu, string name, CustomOptionType type, object defaultValue, object otherDefault, CustomOption parent = null) : this(id, menu, name, type,
            defaultValue, otherDefault, new[] { parent }) {}

        public override string ToString()
        {
            var n = Type == CustomOptionType.Header ? "\n" : "";
            var colon = Type == CustomOptionType.Header ? "" : ": ";
            return n + Name + colon + Format(Value, OtherValue);
        }

        private static bool IsActive(CustomOption option)
        {
            if (option == null)
                return false;
            else if (option.Type == CustomOptionType.Toggle)
                return ((CustomToggleOption)option).Get();
            else if (option.Type == CustomOptionType.Layers)
                return ((CustomLayersOption)option).GetChance() > 0;

            return false;
        }

        public virtual void OptionCreated()
        {
            Setting.name = Setting.gameObject.name = Name;
            Setting.Title = (StringNames)(999999999 - ID);
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
                {
                    toggle.TitleText.text = RoleListEntryOption.Alignments.ContainsKey((RoleEnum)(int)Value) ? RoleListEntryOption.Alignments[(RoleEnum)(int)Value] :
                        RoleListEntryOption.Entries[(RoleEnum)(int)Value];
                }
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
                str.Selected = str.oldValue = (int)Value;
                str.ValueText.text = Format(Value, OtherValue);
            }
            else if (Setting is RoleOptionSetting role)
            {
                role.ChanceText.text = $"{Value}%";
                role.CountText.text = $"{OtherValue}";
            }
        }

        public static void SaveSettings(string fileName)
        {
            var builder = new StringBuilder();

            foreach (var option in AllOptions)
            {
                if (option.Type is CustomOptionType.Button or CustomOptionType.Header or CustomOptionType.Nested)
                    continue;

                builder.AppendLine(option.Name.Trim());

                if (option is RoleListEntryOption entry)
                    builder.AppendLine(((int)entry.Get()).ToString());
                else
                    builder.AppendLine(option.Value.ToString());

                if (option.OtherValue != null)
                    builder.AppendLine(option.OtherValue?.ToString());
            }

            var text = Path.Combine(Application.persistentDataPath, $"{fileName}-temp");

            try
            {
                File.WriteAllText(text, builder.ToString());
                var text2 = Path.Combine(Application.persistentDataPath, fileName);
                File.Delete(text2);
                File.Move(text, text2);
            } catch {}
        }

        public static List<CustomOption> GetOptions(CustomOptionType type) => AllOptions.Where(x => x.Type == type).ToList();

        public static List<T> GetOptions<T>(CustomOptionType type) where T : CustomOption => GetOptions(type).Cast<T>().ToList();
    }
}