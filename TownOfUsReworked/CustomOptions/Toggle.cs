namespace TownOfUsReworked.CustomOptions
{
    public class CustomToggleOption : CustomOption
    {
        public CustomToggleOption(int id, MultiMenu menu, string name, bool value) : base(id, menu, name, CustomOptionType.Toggle, value) => Format = (val, _) => (bool)val ? "On" : "Off";

        public bool Get() => (bool)Value;

        public void Toggle() => Set(!Get());

        public override void OptionCreated()
        {
            base.OptionCreated();
            var toggle = Setting.Cast<ToggleOption>();
            toggle.TitleText.text = Name;
            toggle.CheckMark.enabled = Get();
        }
    }
}