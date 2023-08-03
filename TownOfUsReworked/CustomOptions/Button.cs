namespace TownOfUsReworked.CustomOptions
{
    public class CustomButtonOption : CustomOption
    {
        public Action Do { get; set; }

        public CustomButtonOption(MultiMenu menu, string name, Action toDo = null) : base(-1, menu, name, CustomOptionType.Button, 0) => Do = toDo ?? BaseToDo;

        private static void BaseToDo() {}

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = Name;
        }
    }
}