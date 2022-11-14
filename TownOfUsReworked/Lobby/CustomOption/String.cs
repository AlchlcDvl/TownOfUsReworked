using UnityEngine;

namespace TownOfUsReworked.Lobby.CustomOption
{
    public class CustomStringOption : CustomOption
    {
        protected internal CustomStringOption(int id, MultiMenu menu, string name, string[] values) : base(id, menu, name, CustomOptionType.String, 0)
        {
            Values = values;
            Format = value => Values[(int) value];
        }

        protected internal CustomStringOption(bool indent, int id, MultiMenu menu, string name, string[] values) : this(id, menu, name, values)
        {
            Indent = indent;
        }

        protected string[] Values { get; set; }

        protected internal int Get()
        {
            return (int) Value;
        }

        protected internal void Increase()
        {
            Set(Mathf.Clamp(Get() + 1, 0, Values.Length - 1));
        }

        protected internal void Decrease()
        {
            Set(Mathf.Clamp(Get() - 1, 0, Values.Length - 1));
        }

        public override void OptionCreated()
        {
            var str = Setting.Cast<StringOption>();

            str.TitleText.text = Name;
            str.Value = str.oldValue = Get();
            str.ValueText.text = ToString();
        }
    }
}