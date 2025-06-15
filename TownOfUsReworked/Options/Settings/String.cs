namespace TownOfUsReworked.Options.Settings;

public sealed class StringOption<T>(T[] ignore = null, T[] include = null, T defaultValue = default) : Option<T>(CustomOptionType.String, defaultValue), IStringOption where T : struct, Enum
{
    private T[] Values { get; } = [.. (include ?? Enum.GetValues<T>()).Except(ignore ?? [])];
    private int Index { get; set; }

    private void Change(bool incrementing)
    {
        Index = CycleInt(Values.Length - 1, 0, Index, incrementing);
        Set(Values[Index]);
    }

    private void Increase() => Change(true);

    private void Decrease() => Change(false);

    public override void OptionCreated()
    {
        base.OptionCreated();
        var str = Setting.Cast<StringOption>();
        str.TitleText.text = TranslationManager.Translate(ID);
        str.Values = new(0);
        str.ValueText.transform.localPosition += new Vector3(1.05f, 0f, 0f);

        if ((!AmongUsClient.Instance.AmHost || IsInGame()) && !(ClientOnly || TownOfUsReworked.MciActive))
        {
            str.PlusBtn.gameObject.SetActive(false);
            str.MinusBtn.gameObject.SetActive(false);
        }
        else
        {
            str.PlusBtn.OverrideOnClickListeners(Increase);
            str.MinusBtn.OverrideOnClickListeners(Decrease);
        }
    }

    protected override string FormatValue() => TranslationManager.Translate($"CustomOption.{TargetType.Name}.{ValueString()}");

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        Index = Values.IndexOf(Value);
    }

    public override void ViewUpdate()
    {
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.text = FormatValue();
        viewSettingsInfoPanel.disabledBackground.gameObject.SetActive(false);
    }

    public override void Update() => Setting.Cast<StringOption>().ValueText.text = FormatValue();

    public override void Debug()
    {
        base.Debug();
        Values.Do(x => TranslationManager.DebugId($"CustomOption.{TargetType.Name}.{x}"));
    }

    public override bool IsId(string id) => base.IsId(id) || Values.Any(x => id == $"CustomOption.{TargetType.Name}.{x}");

    protected override void ReadValueString(string value) => Set(Enum.Parse<T>(value), false);

    string IStringOption.ValueString() => ValueString();
}

public interface IStringOption
{
    string ValueString();
}