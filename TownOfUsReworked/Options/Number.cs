namespace TownOfUsReworked.Options;

// May I know who the fuck thought it was a good idea not to let int be casted to float explicitly??? Implicit casting bloody works but explicit doesn't seem to
// AD from a couple weeks later: Yeah fuck this, imma just brute force it instead
public class NumberOptionAttribute(MultiMenu menu, float min, float max, float increment, Format format = Format.None) : OptionAttribute<Number>(menu, CustomOptionType.Number)
{
    public float Min { get; } = min;
    public float Max { get; } = max;
    private float Increment { get; } = increment;
    private Format FormatEnum { get; } = format;
    public bool AllowHalf { get; set; } = true;
    public bool ZeroIsInfinity { get; set; }

    public void Change(bool incrementing) => Set(new(CycleFloat(Max, Min, Get(), incrementing, Increment / (Input.GetKeyInt(KeyCode.LeftShift) && AllowHalf ? 2f : 1f))));

    public void Increase() => Change(true);

    public void Decrease() => Change(false);

    public override void OptionCreated()
    {
        base.OptionCreated();
        var number = Setting.Cast<NumberOption>();
        number.TitleText.text = TranslationManager.Translate(ID);
        number.ValidRange = new(Min, Max);
        number.Increment = Increment;
        Update();
    }

    public override string Format()
    {
        var value = Get();
        var val = value.Value == 0 && ZeroIsInfinity ? "<b>∞</b>" : $"{value:0.##}";
        return FormatEnum switch
        {
            Data.Format.Time => $"{val}s",
            Data.Format.Distance => $"{val}m",
            Data.Format.Percent => $"{val:0}%",
            Data.Format.Multiplier => $"x{val}",
            _ => $"{val}"
        };
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        ViewUpdate();
    }

    public override void PostLoadSetup() => AllowHalf &= Increment != 1;

    public override void Update()
    {
        var number = Setting.Cast<NumberOption>();
        number.Value = number.oldValue = Get();
        number.ValueText.text = Format();
    }

    public override void ViewUpdate()
    {
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.text = Format();
        viewSettingsInfoPanel.disabledBackground.gameObject.SetActive(false);
    }
}