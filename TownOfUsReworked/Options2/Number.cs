namespace TownOfUsReworked.Options2;

public class NumberOptionAttribute(MultiMenu2 menu, float min, float max, float increment, Format format = Format.None) : OptionAttribute(menu,CustomOptionType.Number)
{
    private float Min { get; } = min;
    private float Max { get; } = max;
    private float Increment { get; } = increment;
    private Format FormatEnum { get; } = format;
    public bool IsInt { get; set; } // I have to do some black magic fuckery to make this work
    public bool AllowHalf { get; set; }

    // This whole thing is being held by hopes and dreams, if I or anyone touches this, chances are it's broken....BECAUSE FOR SOME FUCKING REASON IT HAS DECIDED TO CAST INT TO FLOAT AND VICE VERSA ONLY WHEN IT FUCKING WANTS TO

    public float GetFloat() => (float)Value;

    public int GetInt() => (int)Value;

    public void Change(bool incrementing)
    {
        var newVal = CycleFloat(Max, Min, IsInt ? GetInt() : GetFloat(), incrementing, Increment / (Input.GetKeyInt(KeyCode.LeftShift) && AllowHalf ? 2f : 1f));

        if (IsInt)
            Set((int)newVal);
        else
            Set(newVal);
    }

    public void Increase() => Change(true);

    public void Decrease() => Change(false);

    public override void OptionCreated()
    {
        base.OptionCreated();
        var number = Setting.Cast<NumberOption>();
        number.TitleText.text = TranslationManager.Translate(ID);
        number.ValidRange = new(Min, Max);
        number.Increment = Increment;
        number.Value = number.oldValue = IsInt ? GetInt() : GetFloat();
        number.ValueText.text = Format();
    }

    public override string Format() => FormatEnum switch
    {
        Data.Format.Time => $"{Value:0.##}s",
        Data.Format.Distance => $"{Value:0.##}m",
        Data.Format.Percent => $"{Value:0}%",
        Data.Format.Multiplier => $"x{Value:0.##}",
        _ => $"{Value:0.##}"
    };

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.text = Format();
        viewSettingsInfoPanel.disabledBackground.gameObject.SetActive(false);
    }

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        IsInt = Property.PropertyType == typeof(int);
    }
}