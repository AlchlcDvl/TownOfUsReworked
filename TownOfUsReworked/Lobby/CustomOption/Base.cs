using System;
using System.Collections.Generic;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.Lobby.CustomOption
{
    public class CustomOption
    {
        public static List<CustomOption> AllOptions = new List<CustomOption>();
        public readonly int ID;
        public readonly MultiMenu Menu;
        public Func<object, string> Format;
        public string Name;
        protected internal object Value { get; set; }
        protected internal OptionBehaviour Setting { get; set; }
        protected internal CustomOptionType Type { get; set; }
        public object DefaultValue { get; set; }
        public static bool LobbyTextScroller { get; set; } = true;
        protected internal bool Indent { get; set; }
        
        protected internal CustomOption(int id, MultiMenu menu, string name, CustomOptionType type, object defaultValue, Func<object, string> format = null)
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

        public override string ToString()
        {
            return Format(Value);
        }

        public virtual void OptionCreated()
        {
            Setting.name = Setting.gameObject.name = Name;
        }

        protected internal void Set(object value, bool SendRpc = true)
        {
            System.Console.WriteLine($"{Name} set to {value}");

            Value = value;

            if (Setting != null && AmongUsClient.Instance.AmHost && SendRpc)
                Rpc.SendRpc(this);

            if (Setting is ToggleOption toggle)
            {
                var newValue = (bool) Value;
                toggle.oldValue = newValue;

                if (toggle.CheckMark != null)
                    toggle.CheckMark.enabled = newValue;
            }
            else if (Setting is NumberOption number)
            {
                var newValue = (float) Value;

                number.Value = number.oldValue = newValue;
                number.ValueText.text = ToString();
            }
            else if (Setting is StringOption str)
            {
                var newValue = (int) Value;

                str.Value = str.oldValue = newValue;
                str.ValueText.text = ToString();
            }
        }
    }
}