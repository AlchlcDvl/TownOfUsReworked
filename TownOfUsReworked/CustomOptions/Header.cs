namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public class CustomHeaderOption : CustomOption
    {
        public CustomHeaderOption(int id, MultiMenu menu, string name) : base(id, menu, name, CustomOptionType.Header, 0) {}

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = Name;
        }
    }
}