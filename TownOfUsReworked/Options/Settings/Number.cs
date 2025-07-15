namespace TownOfUsReworked.Options.Settings;

// May I know who the fuck thought it was a good idea not to let int be cast to float explicitly??? Implicit casting bloodily works, but explicit doesn't seem to
// AD from a couple of weeks later: Yeah, fuck this, imma just brute force it instead
public sealed class ReworkedNumberOption(float min, float max, float increment, Format format = Format.None, bool allowHalf = true, bool zeroIsInf = false, string customFormat = null, float
    defaultValue = 0f) : Option<Number>(CustomOptionType.Number, defaultValue)
{
    private readonly float Min = min;
    private readonly float Max = max;
    private readonly float Increment = increment;
    private readonly Format Format = format;
    private readonly bool AllowHalf = allowHalf && increment != 1;
    private readonly bool ZeroIsInfinity = zeroIsInf;
    private readonly string CustomFormat = customFormat;

    private void Change(bool incrementing) => Set(CycleFloat(Max, Min, Value, incrementing, Increment / (Input.GetKeyInt(KeyCode.LeftShift) && AllowHalf ? 2f : 1f)));

    private void Increase() => Change(true);

    private void Decrease() => Change(false);

    protected override string ValueString() => $"{Value:0.##}";

    public override void OptionCreated()
    {
        base.OptionCreated();
        var number = Setting.Cast<NumberOption>();
        number.ValueText.transform.localPosition += new Vector3(1.05f, 0f, 0f);
        number.TitleText.text = TranslationManager.Translate(ID);
        number.ValidRange = new(Min, Max);
        number.Increment = Increment;
        number.ZeroIsInfinity = ZeroIsInfinity;

        if ((!AmongUsClient.Instance.AmHost || IsInGame()) && !(ClientOnly || TownOfUsReworked.MciActive))
        {
            number.PlusBtn.gameObject.SetActive(false);
            number.MinusBtn.gameObject.SetActive(false);
        }
        else
        {
            number.PlusBtn.OverrideOnClickListeners(Increase);
            number.MinusBtn.OverrideOnClickListeners(Decrease);
        }
    }

    protected override string FormatValue()
    {
        var value = Value;
        var val = value == 0 && ZeroIsInfinity ? "<b>∞</b>" : $"{value:0.##}";
        return Format switch
        {
            Format.Time => $"{val}s",
            Format.Distance => $"{val}m",
            Format.Percent => $"{val:0}%",
            Format.Multiplier => $"x{val}",
            Format.Custom when CustomFormat is not null => string.Format(CustomFormat, val),
            _ => $"{val}"
        };
    }

    public override void Update() => Setting.Cast<NumberOption>().ValueText.text = FormatValue();

    public override void ViewUpdate()
    {
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.text = FormatValue();
        viewSettingsInfoPanel.disabledBackground.gameObject.SetActive(false);
    }

    protected override void ReadValueString(string value) => Set(float.Parse(value), false);

    public bool IsValid(string value)
    {
        var values = value.TrueSplit(',');
        var value2 = float.Parse(values[0]);
        var min = Min;
        var max = Max;
        var useMin = true;
        var useMax = true;

        for (var i = 1; i < values.Length; i++)
        {
            var temp = values[i];
            var valuePart = temp[4..];

            if (float.TryParse(valuePart, out var value3))
            {
                if (temp.StartsWith("min:"))
                    min = value3;
                else if (temp.StartsWith("max:"))
                    max = value3;
            }
            else if (bool.TryParse(valuePart, out var value4))
            {
                if (temp.StartsWith("min:"))
                    useMin = value4;
                else if (temp.StartsWith("max:"))
                    useMax = value4;
            }
        }

        return value2.IsInRange(min, max, useMin, useMax);
    }
}