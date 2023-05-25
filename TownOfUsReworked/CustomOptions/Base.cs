namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
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

        public virtual void OptionCreated() => Setting.name = Setting.gameObject.name = Name;

        public void Set(object value)
        {
            Value = value;

            if (Setting != null && AmongUsClient.Instance.AmHost)
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
            else if (Setting is StringOption str)
            {
                var newValue = (int)Value;

                str.Value = str.oldValue = newValue;
                str.ValueText.text = ToString();
            }
        }
    }
}