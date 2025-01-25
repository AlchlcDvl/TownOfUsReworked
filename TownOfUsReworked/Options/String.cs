namespace TownOfUsReworked.Options;

public class StringOptionAttribute<T>(params T[] ignore) : OptionAttribute<T>(CustomOptionType.String) where T : struct, Enum
{
    private int Index { get; set; }
    private IEnumerable<T> Ignore { get; } = ignore;
    private IEnumerable<T> Values { get; set; }
    private int Count { get; set; }

    public void Change(bool incrementing)
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

        if ((!AmongUsClient.Instance.AmHost || IsInGame()) && !(ClientOnly || TownOfUsReworked.MCIActive))
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

    public override string Format() => TranslationManager.Translate($"CustomOption.{TargetType.Name}.{ValueString()}");

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        Values = Enum.GetValues<T>().Except(Ignore);
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
        var str = Setting.Cast<StringOption>();
        str.Value = str.oldValue = Index = Mathf.Clamp(Values.IndexOf(Value), 0, Count);
        str.ValueText.text = Format();
    }

    public override void Debug()
    {
        base.Debug();
        Values.ForEach(x => TranslationManager.DebugId($"CustomOption.{TargetType.Name}.{x}"));
    }

    public override void ReadValueRpc(MessageReader reader) => Set(reader.ReadEnum<T>(), false);

    public override void WriteValueRpc(MessageWriter writer) => writer.Write(Value);

    public override void ReadValueString(string value) => Set(Enum.Parse<T>(value), false);
}