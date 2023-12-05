namespace TownOfUsReworked.CustomOptions;

public class CustomHeaderOption : CustomOption
{
    public CustomHeaderOption(MultiMenu menu, string name, object parent = null) : base(-1, menu, name, CustomOptionType.Header, null, parent, null) => Format = (_, _) => "";

    public CustomHeaderOption(MultiMenu menu, string name, object[] parents, bool all = false) : base(-1, menu, name, CustomOptionType.Header, null, parents, all) => Format = (_, _) => "";

    public override void OptionCreated()
    {
        base.OptionCreated();
        Setting.Cast<ToggleOption>().TitleText.text = Name;
        Setting.gameObject.GetComponent<PassiveButton>().Destroy();
    }
}