namespace TownOfUsReworked.CustomOptions;

public class CustomNumberOption : CustomOption
{
    private readonly float Min;
    private readonly float Max;
    private readonly float Increment;

    public CustomNumberOption(MultiMenu menu, string name, float defaultValue, float min, float max, float increment, Func<object, object, string> format = null) : this(menu, name,
        defaultValue, min, max, increment, format, parent: null) {}

    public CustomNumberOption(MultiMenu menu, string name, float defaultValue, float min, float max, float increment, Func<object, object, string> format, object parent) : this(menu, name,
        defaultValue, min, max, increment, format, new[] { parent }, false) {}

    public CustomNumberOption(MultiMenu menu, string name, float defaultValue, float min, float max, float increment, object parent) : this(menu, name, defaultValue, min, max,
        increment, null, parent) {}

    public CustomNumberOption(MultiMenu menu, string name, float defaultValue, float min, float max, float increment, Func<object, object, string> format, object[] parents, bool all = false)
        : base(menu, name, CustomOptionType.Number, defaultValue, parents, all)
    {
        Min = min;
        Max = max;
        Increment = increment;
        Format = format ?? Blank;
    }

    public CustomNumberOption(MultiMenu menu, string name, float defaultValue, float min, float max, float increment, object[] parents, bool all = false) : this(menu, name,
        defaultValue, min, max, increment, null, parents, all) {}

    private static Func<object, object, string> Blank => (val, _) => $"{val}";

    public float Get() => (float)Value;

    public void Increase() => Set(CycleFloat(Max, Min, Get(), true, Increment / (Input.GetKeyInt(KeyCode.LeftShift) ? 2f : 1f)));

    public void Decrease() => Set(CycleFloat(Max, Min, Get(), false, Increment / (Input.GetKeyInt(KeyCode.LeftShift) ? 2f : 1f)));

    public override void OptionCreated()
    {
        base.OptionCreated();
        var number = Setting.Cast<NumberOption>();
        number.TitleText.text = Name;
        number.ValidRange = new(Min, Max);
        number.Increment = Increment;
        number.Value = number.oldValue = Get();
        number.ValueText.text = Format(Value, OtherValue);
    }

    public static implicit operator float(CustomNumberOption option) => option == null ? 0f : option.Get();

    public static implicit operator int(CustomNumberOption option) => option == null ? 0 : (int)option.Get();
}