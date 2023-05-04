using TownOfUsReworked.Data;
using HarmonyLib;

namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public class CustomToggleOption : CustomOption
    {
        public CustomToggleOption(int id, MultiMenu menu, string name, bool value = true) : base(id, menu, name, CustomOptionType.Toggle, value) => Format = val => (bool)val ?
            "True" : "False";

        public bool Get() => (bool)Value;

        public void Toggle() => Set(!Get());

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = Name;
            Setting.Cast<ToggleOption>().CheckMark.enabled = Get();
        }
    }
}