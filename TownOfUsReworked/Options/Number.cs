namespace TownOfUsReworked.Options;

// May I know who the fuck thought it was a good idea not to let int be casted to float explicitly??? Implicit casting bloody works but explicit doesn't seem to
// AD from a couple weeks later: Yeah fuck this, imma just brute force it instead
public class NumberOptionAttribute(float min, float max, float increment, Format format = Format.None, bool allowHalf = true, bool zeroIsInf = false) : OptionAttribute<Number>
    (CustomOptionType.Number)
{
    public float Min { get; } = min;
    public float Max { get; } = max;
    private float Increment { get; } = increment;
    private Format FormatEnum { get; } = format;
    public bool AllowHalf { get; set; } = allowHalf;
    public bool ZeroIsInfinity { get; set; } = zeroIsInf;

    public void Change(bool incrementing) => Set(CycleFloat(Max, Min, Get(), incrementing, Increment / (Input.GetKeyInt(KeyCode.LeftShift) && AllowHalf ? 2f : 1f)));

    private void Increase() => Change(true);

    private void Decrease() => Change(false);

    public override string ValueString() => $"{Value:0.##}";

    public override void OptionCreated()
    {
        base.OptionCreated();
        var number = Setting.Cast<NumberOption>();
        number.ValueText.transform.localPosition += new Vector3(1.05f, 0f, 0f);
        number.TitleText.text = TranslationManager.Translate(ID);
        number.ValidRange = new(Min, Max);
        number.Increment = Increment;
        number.ZeroIsInfinity = ZeroIsInfinity;

        if ((!AmongUsClient.Instance.AmHost || IsInGame()) && !(ClientOnly || TownOfUsReworked.MCIActive))
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

    public override string Format()
    {
        var value = Get().Value;
        var val = value == 0 && ZeroIsInfinity ? "<b>∞</b>" : $"{value:0.##}";
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

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        AllowHalf &= Increment != 1;
    }

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

    public override void ReadValueRpc(MessageReader reader) => Set(reader.ReadNumber());

    public override void WriteValueRpc(MessageWriter writer) => writer.Write(Value);

    public override void ReadValueString(string value) => Set(float.Parse(value), false);
}