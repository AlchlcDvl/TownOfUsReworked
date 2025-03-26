namespace TownOfUsReworked.Options;

public sealed class StringOption<T>(params T[] ignore) : Option<T>(CustomOptionType.String) where T : struct, Enum
{
    private int Index { get; set; }
    private IEnumerable<T> Values { get; } = Enum.GetValues<T>().Except(ignore);
    private int Count { get; set; }

    private void Change(bool incrementing)
    {
        Index = CycleInt(Count, 0, Index, incrementing);
        Set(Values.ElementAt(Index));
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

    protected override string Format() => TranslationManager.Translate($"CustomOption.{TargetType.Name}.{ValueString()}");

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        Index = Values.IndexOf(Value);
        Count = Values.Count() - 1;
    }

    public override void ViewUpdate()
    {
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.text = Format();
        viewSettingsInfoPanel.disabledBackground.gameObject.SetActive(false);
    }

    public override void Update()
    {
        Index = Mathf.Clamp(Values.IndexOf(Value), 0, Count);
        Setting.Cast<StringOption>().ValueText.text = Format();
    }

    public override void Debug()
    {
        base.Debug();
        Values.ForEach(x => TranslationManager.DebugId($"CustomOption.{TargetType.Name}.{x}"));
    }

    public override void ReadValueRpc(NetData reader) => Set(reader.Read<T>(), false);

    protected override void ReadValueString(string value) => Set(Enum.Parse<T>(value), false);
}