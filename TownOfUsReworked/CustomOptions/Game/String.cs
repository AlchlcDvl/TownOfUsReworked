namespace TownOfUsReworked.CustomOptions;

public class CustomStringOption : CustomOption
{
    public string[] Values { get; }

    public CustomStringOption(MultiMenu menu, string name, string[] values, object[] parents, bool all = false) : base(menu, name, CustomOptionType.String, 0, parents, all)
    {
        Values = values;
        Format = (val, _) => Values[(int)val];
    }

    public CustomStringOption(MultiMenu menu, string name, string[] values, object parent = null) : this(menu, name, values, new[] { parent }, false) {}

    public int GetInt() => (int)Value;

    public string GetString() => Values[(int)Value];

    public void Increase() => Set(CycleInt(Values.Length - 1, 0, GetInt(), true));

    public void Decrease() => Set(CycleInt(Values.Length - 1, 0, GetInt(), false));

    public override void OptionCreated()
    {
        base.OptionCreated();
        var str = Setting.Cast<KeyValueOption>();
        str.TitleText.text = Name;
        str.Selected = str.oldValue = GetInt();
        str.ValueText.text = GetString();
        var list = new List<ISystem.KeyValuePair<string, int>>();

        for (var i = 0; i < Values.Length; i++)
            list.Add(new(Values[i], i));

        str.Values = list.SystemToIl2Cpp();
    }

    public static implicit operator string(CustomStringOption option) => option == null ? "" : option.GetString();

    public static implicit operator int(CustomStringOption option) => option == null ? 0 : option.GetInt();
}