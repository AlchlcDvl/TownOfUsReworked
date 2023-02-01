using TownOfUsReworked.Enums;

namespace TownOfUsReworked.Lobby.CustomOption
{
    public class CustomToggleOption : CustomOption
    {
        protected internal CustomToggleOption(int id, MultiMenu menu, string name, bool value = true) : base(id, menu, name, CustomOptionType.Toggle, value)
        {
            Format = val => (bool) val ? "On" : "Off";
        }

        protected internal CustomToggleOption(bool indent, int id, MultiMenu menu, string name, bool value = true) : base(id, menu, name, CustomOptionType.Toggle, value)
        {
            Indent = indent;
        }

        protected internal bool Get()
        {
            return (bool) Value;
        }

        protected internal void Toggle()
        {
            Set(!Get());
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = Name;
            Setting.Cast<ToggleOption>().CheckMark.enabled = Get();
        }
    }
}