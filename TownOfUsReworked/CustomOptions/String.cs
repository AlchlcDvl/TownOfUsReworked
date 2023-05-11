using TownOfUsReworked.Data;
using HarmonyLib;

namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public class CustomStringOption : CustomOption
    {
        public CustomStringOption(int id, MultiMenu menu, string name, string[] values) : base(id, menu, name, CustomOptionType.String, 0)
        {
            Values = values;
            Format = value => Values[(int)value];
        }

        protected string[] Values;

        public int Get() => (int)Value;

        public void Increase()
        {
            if (Get() + 1 >= Values.Length)
                Set(0);
            else
                Set(Get() + 1);
        }

        public void Decrease()
        {
            if (Get() - 1 < 0)
                Set(Values.Length - 1);
            else
                Set(Get() - 1);
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