namespace TownOfUsReworked.Options;

public class CustomHeaderOption : CustomOption
{
    public CustomHeaderOption(MultiMenu menu, string name, object parent = null, bool clientOnly = false) : this(menu, name, new[] { parent }, false, clientOnly) {}

    public CustomHeaderOption(MultiMenu menu, string name, object[] parents, bool all = false, bool clientOnly = false) : base(menu, name, CustomOptionType.Header, null, parents, all, null,
        clientOnly) => Format = (_, _) => "";

    public override void OptionCreated()
    {
        base.OptionCreated();
        Setting.Cast<ToggleOption>().TitleText.text = Name;
        Setting.gameObject.GetComponent<PassiveButton>().Destroy();
    }
}