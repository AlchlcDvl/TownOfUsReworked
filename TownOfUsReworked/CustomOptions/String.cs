namespace TownOfUsReworked.CustomOptions;

public class CustomStringOption : CustomOption
{
    public readonly string[] Values;

    public CustomStringOption(int id, MultiMenu menu, string name, string[] values, CustomOption parent = null) : base(id, menu, name, CustomOptionType.String, 0, parent)
    {
        Values = values;
        Format = (value, _) => Values[(int)value];
    }

    public CustomStringOption(int id, MultiMenu menu, string name, string[] values, CustomOption[] parents, bool all = false) : base(id, menu, name, CustomOptionType.String, 0, parents,
        all)
    {
        Values = values;
        Format = (value, _) => Values[(int)value];
    }

    public int Get() => (int)Value;

    public void Increase() => Set(CycleInt(Values.Length - 1, 0, Get(), true));

    public void Decrease() => Set(CycleInt(Values.Length - 1, 0, Get(), false));

    public override void OptionCreated()
    {
        base.OptionCreated();
        var str = Setting.Cast<KeyValueOption>();
        str.TitleText.text = Name;
        str.Selected = str.oldValue = Get();
        str.ValueText.text = Format(Value, OtherValue);
    }
}