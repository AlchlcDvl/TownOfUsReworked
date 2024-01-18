namespace TownOfUsReworked.CustomOptions;

public class CustomToggleOption : CustomOption
{
    public CustomToggleOption(MultiMenu menu, string name, bool value, object parent = null) : this(menu, name, value, new[] { parent }, false) {}

    public CustomToggleOption(MultiMenu menu, string name, bool value, object[] parents, bool all = false) : base(menu, name, CustomOptionType.Toggle, value, parents, all) => Format = (val,
        _) => (bool)val ? "On" : "Off";

    public bool Get() => (bool)Value;

    public void Toggle() => Set(!Get());

    public override void OptionCreated()
    {
        base.OptionCreated();
        var toggle = Setting.Cast<ToggleOption>();
        toggle.TitleText.text = Name;
        toggle.CheckMark.enabled = Get();
    }

    public static implicit operator bool(CustomToggleOption option) => option != null && option.Get();
}