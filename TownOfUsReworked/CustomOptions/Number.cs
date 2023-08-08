namespace TownOfUsReworked.CustomOptions;

public class CustomNumberOption : CustomOption
{
    private readonly float Min;
    private readonly float Max;
    private readonly float Increment;

    public CustomNumberOption(int id, MultiMenu menu, string name, float defaultValue, float min, float max, float increment, Func<object, object, string> format = null) : base(id, menu,
        name, CustomOptionType.Number, defaultValue)
    {
        Min = min;
        Max = max;
        Increment = increment;
        Format = format ?? Blank;
    }

    public CustomNumberOption(int id, MultiMenu menu, string name, float defaultValue, float min, float max, float increment, Func<object, object, string> format, CustomOption parent) :
        base(id, menu, name, CustomOptionType.Number, defaultValue, parent)
    {
        Min = min;
        Max = max;
        Increment = increment;
        Format = format ?? Blank;
    }

    public CustomNumberOption(int id, MultiMenu menu, string name, float defaultValue, float min, float max, float increment, CustomOption parent) : this(id, menu, name, defaultValue, min,
        max, increment, null, parent) {}

    public CustomNumberOption(int id, MultiMenu menu, string name, float defaultValue, float min, float max, float increment, Func<object, object, string> format, CustomOption[] parents,
        bool all = false) : base(id, menu, name, CustomOptionType.Number, defaultValue, parents, all)
    {
        Min = min;
        Max = max;
        Increment = increment;
        Format = format ?? Blank;
    }

    public CustomNumberOption(int id, MultiMenu menu, string name, float defaultValue, float min, float max, float increment, CustomOption[] parents, bool all = false) : this(id, menu,
        name, defaultValue, min, max, increment, null, parents, all) {}

    private static Func<object, object, string> Blank => (val, _) => $"{val}";

    public float Get() => (float)Value;

    public void Increase() => Set(CycleFloat(Max, Min, Get(), true, Increment / (Input.GetKeyInt(KeyCode.LeftShift) ? 2 : 1)));

    public void Decrease() => Set(CycleFloat(Max, Min, Get(), false, Increment / (Input.GetKeyInt(KeyCode.LeftShift) ? 2 : 1)));

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
}