namespace TownOfUsReworked.Options;

// May I know who the fuck thought it was a good idea not to let int be casted to float explicitly??? Implicit casting bloody works but explicit doesn't seem to
// AD from a couple weeks later: Yeah fuck this, imma just brute force it instead
public class NumberOptionAttribute(MultiMenu menu, float min, float max, float increment, Format format = Format.None) : OptionAttribute(menu, CustomOptionType.Number)
{
    public float Min { get; } = min;
    public float Max { get; } = max;
    private float Increment { get; } = increment;
    private Format FormatEnum { get; } = format;
    public bool AllowHalf { get; set; } = true;
    public bool ZeroIsInfinity { get; set; }

    public Number Get() => (Number)Value;

    public void Change(bool incrementing) => Set(new Number(CycleFloat(Max, Min, Get(), incrementing, Increment / (Input.GetKeyInt(KeyCode.LeftShift) && AllowHalf ? 2f : 1f))));

    public void Increase() => Change(true);

    public void Decrease() => Change(false);

    public override void OptionCreated()
    {
        base.OptionCreated();
        var number = Setting.Cast<NumberOption>();
        number.TitleText.text = TranslationManager.Translate(ID);
        number.ValidRange = new(Min, Max);
        number.Increment = Increment;
        number.Value = number.oldValue = Get();
        number.ValueText.text = Format();
    }

    public override string Format()
    {
        if (Get().Value == 0 && ZeroIsInfinity)
        {
            return FormatEnum switch
            {
                Data.Format.Time => "<b>∞</b>s",
                Data.Format.Distance => "<b>∞</b>m",
                Data.Format.Percent => "<b>∞</b>%",
                Data.Format.Multiplier => "x<b>∞</b>",
                _ => "<b>∞</b>"
            };
        }
        else
        {
            return FormatEnum switch
            {
                Data.Format.Time => $"{Value:0.##}s",
                Data.Format.Distance => $"{Value:0.##}m",
                Data.Format.Percent => $"{Value:0}%",
                Data.Format.Multiplier => $"x{Value:0.##}",
                _ => $"{Value:0.##}"
            };
        }
    }

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
        AllowHalf &= Increment != 1;
    }

    public override void ModifySetting(out string stringValue)
    {
        base.ModifySetting(out stringValue);
        var number = Setting.Cast<NumberOption>();
        number.Value = number.oldValue = Get();
        number.ValueText.text = stringValue = Format();
    }
}

// namespace TownOfUsReworked.Options;

// // May I know who the fuck thought it was a good idea not to let int be casted to float explicitly??? Implicit casting bloody works but explicit doesn't seem to
// public class NumberOptionAttribute(MultiMenu menu, float min, float max, float increment, Format format = Format.None) : OptionAttribute(menu, CustomOptionType.Number)
// {
//     public float Min { get; } = min;
//     public float Max { get; } = max;
//     private float Increment { get; } = increment;
//     private Format FormatEnum { get; } = format;
//     public bool AllowHalf { get; set; } = true;
//     public bool ZeroIsInfinity { get; set; }

//     public float GetFloat() => (float)Value;

//     public int GetInt() => (int)Value;

//     public void Change(bool incrementing)
//     {
//         var flag = Value is int;
//         var newVal = CycleFloat(Max, Min, flag ? GetInt() : GetFloat(), incrementing, Increment / (Input.GetKeyInt(KeyCode.LeftShift) && AllowHalf ? 2f : 1f));

//         if (flag)
//             Set((int)newVal);
//         else
//             Set(newVal);
//     }

//     public void Increase() => Change(true);

//     public void Decrease() => Change(false);

//     public override void OptionCreated()
//     {
//         base.OptionCreated();
//         var number = Setting.Cast<NumberOption>();
//         number.TitleText.text = TranslationManager.Translate(ID);
//         number.ValidRange = new(Min, Max);
//         number.Increment = Increment;
//         number.Value = number.oldValue = Value is int v ? v : GetFloat();
//         number.ValueText.text = Format();
//     }

//     public override string Format()
//     {
//         if (Value is int v && v == 0 && ZeroIsInfinity)
//             return "<b>∞</b>";
//         else if (Value is float f && f == 0 && ZeroIsInfinity)
//             return "<b>∞</b>";
//         else
//         {
//             return FormatEnum switch
//             {
//                 Data.Format.Time => $"{Value:0.##}s",
//                 Data.Format.Distance => $"{Value:0.##}m",
//                 Data.Format.Percent => $"{Value:0}%",
//                 Data.Format.Multiplier => $"x{Value:0.##}",
//                 _ => $"{Value:0.##}"
//             };
//         }
//     }

//     public override void ViewOptionCreated()
//     {
//         base.ViewOptionCreated();
//         var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
//         viewSettingsInfoPanel.settingText.text = Format();
//         viewSettingsInfoPanel.disabledBackground.gameObject.SetActive(false);
//     }

//     public override void PostLoadSetup()
//     {
//         base.PostLoadSetup();
//         AllowHalf &= Value is not int;
//     }

//     public override void ModifySetting(out string stringValue)
//     {
//         base.ModifySetting(out stringValue);
//         var number = Setting.Cast<NumberOption>();
//         number.Value = number.oldValue = Value is int v ? v : GetFloat(); // Part 2 of my mental breakdown
//         number.ValueText.text = stringValue = Format();
//     }
// }