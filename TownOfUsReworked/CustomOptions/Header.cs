namespace TownOfUsReworked.CustomOptions
{
    public class CustomHeaderOption : CustomOption
    {
        public CustomHeaderOption(MultiMenu menu, string name) : base(-1, menu, name, CustomOptionType.Header, 0) => Format = (underscore, _) => "";

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = Name;
        }
    }
}