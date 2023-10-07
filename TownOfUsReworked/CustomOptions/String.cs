namespace TownOfUsReworked.CustomOptions;

public class CustomStringOption : CustomOption
{
    public readonly string[] Values;

    public CustomStringOption(int id, MultiMenu menu, string name, string[] values, object parent = null) : base(id, menu, name, CustomOptionType.String, 0, parent)
    {
        Values = values;
        Format = (value, _) => Values[(int)value];
    }

    public CustomStringOption(int id, MultiMenu menu, string name, string[] values, object[] parents, bool all = false) : base(id, menu, name, CustomOptionType.String, 0, parents, all)
    {
        Values = values;
        Format = (value, _) => Values[(int)value];
    }

    public int GetInt() => (int)Value;

    public string GetString() => Values[GetInt()];

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

    public static implicit operator string(CustomStringOption option) => option.GetString();

    public static implicit operator int(CustomStringOption option) => option.GetInt();
}