namespace TownOfUsReworked.CustomOptions
{
    public class CustomHeaderOption : CustomOption
    {
        public CustomHeaderOption(MultiMenu menu, string name, CustomLayersOption parent = null) : base(-1, menu, name, CustomOptionType.Header, 0, parent) => Format = (_, _) => "";

        public CustomHeaderOption(MultiMenu menu, string name, CustomLayersOption[] parents, bool all = false) : base(-1, menu, name, CustomOptionType.Header, 0, parents, all) => Format =
            (_, _) => "";

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = Name;
        }
    }
}