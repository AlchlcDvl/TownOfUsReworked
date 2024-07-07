namespace TownOfUsReworked.Options;

public class CustomStringOption : CustomOption
{
    public string[] Values { get; }

    public CustomStringOption(MultiMenu menu, string name, string[] values, object parent = null) : this(menu, name, values, [parent], false) {}

    public CustomStringOption(MultiMenu menu, string name, string[] values, object[] parents, bool all = false) : base(menu, name, CustomOptionType.String, 0, parents, all)
    {
        Values = values;
        Format = val => Values[(int)val];
    }

    public int GetInt() => (int)Value;

    public string GetString() => Values[(int)Value];

    public void Increase() => Set(CycleInt(Values.Length - 1, 0, GetInt(), true));

    public void Decrease() => Set(CycleInt(Values.Length - 1, 0, GetInt(), false));

    public override void OptionCreated()
    {
        base.OptionCreated();
        var str = Setting.Cast<StringOption>();
        str.TitleText.text = Name;
        str.Value = str.oldValue = GetInt();
        str.ValueText.text = GetString();
        str.Values = new(0);
    }

    public static implicit operator string(CustomStringOption option) => option == null ? "" : option.GetString();

    public static implicit operator int(CustomStringOption option) => option == null ? 0 : option.GetInt();
}