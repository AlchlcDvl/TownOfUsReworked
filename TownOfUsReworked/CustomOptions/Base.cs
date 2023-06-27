namespace TownOfUsReworked.CustomOptions
{
    public class CustomOption
    {
        public readonly static List<CustomOption> AllOptions = new();
        public readonly int ID;
        public readonly MultiMenu Menu;
        public Func<object, string> Format;
        public string Name;
        public object Value;
        public OptionBehaviour Setting;
        public CustomOptionType Type;
        public object DefaultValue;

        public CustomOption(int id, MultiMenu menu, string name, CustomOptionType type, object defaultValue, Func<object, string> format = null)
        {
            ID = id;
            Menu = menu;
            Name = name;
            Type = type;
            DefaultValue = Value = defaultValue;
            Format = format ?? (obj => $"{obj}");

            if (Type == CustomOptionType.Button)
                return;

            AllOptions.Add(this);
            Set(Value);
        }

        public override string ToString() => Format(Value);

        public virtual void OptionCreated()
        {
            Setting.name = Setting.gameObject.name = Name;
            Setting.Title = (StringNames)(9999000 - ID);
            Setting.OnValueChanged = new Action<OptionBehaviour>(_ => {});
        }

        public void Set(object value, object otherValue = null)
        {
            Value = value;

            if (Setting && AmongUsClient.Instance.AmHost)
                RPC.SendRPC(this);

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
                role.ChanceText.text = value.ToString() + "%";
                role.CountText.text = otherValue.ToString();
            }
        }

        public static void SaveSettings(string fileName)
        {
            var builder = new StringBuilder();

            foreach (var option in AllOptions)
            {
                if (option.Type is CustomOptionType.Button or CustomOptionType.Header or CustomOptionType.Nested)
                    continue;

                builder.AppendLine(option.Name);
                builder.AppendLine(option.Value.ToString());
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
    }
}