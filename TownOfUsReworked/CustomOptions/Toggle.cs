namespace TownOfUsReworked.CustomOptions;

public class CustomToggleOption : CustomOption
{
    public CustomToggleOption(int id, MultiMenu menu, string name, bool value, object parent = null) : base(id, menu, name, CustomOptionType.Toggle, value, parent) => Format = (val, _) =>
        (bool)val ? "On" : "Off";

    public CustomToggleOption(int id, MultiMenu menu, string name, bool value, object[] parents, bool all = false) : base(id, menu, name, CustomOptionType.Toggle, value, parents, all) =>
        Format = (val, _) => (bool)val ? "On" : "Off";

    public bool Get() => (bool)Value;

    public void Toggle() => Set(!Get());

    public override void OptionCreated()
    {
        base.OptionCreated();
        var toggle = Setting.Cast<ToggleOption>();
        toggle.TitleText.text = Name;
        toggle.CheckMark.enabled = Get();
        toggle.gameObject.GetComponent<BoxCollider2D>().size = new(7.91f, 0.45f);
    }

    public static implicit operator bool(CustomToggleOption option) => option.Get();
}