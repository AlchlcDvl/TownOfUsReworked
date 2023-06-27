namespace TownOfUsReworked.CustomOptions
{
    public class CustomOption
    {
        public readonly static List<CustomOption> AllOptions = new();
        public readonly int ID;
        public readonly MultiMenu Menu;
        public Func<object, object, string> Format;
        public string Name;
        public object Value;
        public object OtherValue;
        public OptionBehaviour Setting;
        public CustomOptionType Type;
        public CustomLayersOption Parent;
        public bool Active => Parent == null || Parent.GetChance() > 0;

        public CustomOption(int id, MultiMenu menu, string name, CustomOptionType type, object defaultValue, CustomLayersOption parent = null)
        {
            ID = id;
            Menu = menu;
            Name = name;
            Type = type;
            Parent = parent;
            Set(defaultValue, 0);

            if (Type != CustomOptionType.Button)
                AllOptions.Add(this);
        }

        public override string ToString() => Format(Value, OtherValue);

        public virtual void OptionCreated()
        {
            Setting.name = Setting.gameObject.name = Name;
            Setting.Title = (StringNames)(999999999 - ID);
            Setting.OnValueChanged = new Action<OptionBehaviour>(_ => SettingsPatches.StartPrefix(UObject.FindObjectOfType<GameOptionsMenu>(), out var _));
        }

        public void Set(object value, object otherValue = null)
        {
            Value = value;
            OtherValue = otherValue;

            if (Setting && AmongUsClient.Instance.AmHost)
                RPC.SendOptionRPC(this);

            if (Setting is ToggleOption toggle)
            {
                var newValue = (bool)Value;
                toggle.oldValue = newValue;

                if (toggle.CheckMark != null)
                    toggle.CheckMark.enabled = newValue;
            }
            else if (Setting is NumberOption number)
            {
                var newValue = (float)Value;
                number.Value = number.oldValue = newValue;
                number.ValueText.text = ToString();
            }
            else if (Setting is KeyValueOption str)
            {
                var newValue = (int)Value;
                str.Selected = str.oldValue = newValue;
                str.ValueText.text = ToString();
            }
            else if (Setting is RoleOptionSetting role)
            {
                role.ChanceText.text = $"{value}%";
                role.CountText.text = $"{otherValue}";
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