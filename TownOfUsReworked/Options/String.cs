namespace TownOfUsReworked.Options;

public class StringOptionAttribute(MultiMenu menu, string[] ignoreStrings = null) : OptionAttribute<Enum>(menu, CustomOptionType.String)
{
    public string[] Values { get; set; }
    public int Index { get; set; }
    private string[] IgnoreStrings { get; } = ignoreStrings ?? [];
    private List<Enum> EnumValues { get; set; }

    public int GetInt() => Index;

    public string GetString() => Values[Index];

    public void Increase()
    {
        Index = CycleInt(Values.Length - 1, 0, Index, true);
        Set(EnumValues[Index]);
    }

    public void Decrease()
    {
        Index = CycleInt(Values.Length - 1, 0, Index, false);
        Set(EnumValues[Index]);
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var str = Setting.Cast<StringOption>();
        str.TitleText.text = TranslationManager.Translate(ID);
        str.Values = new(0);
        Update();
    }

    public override string Format() => TranslationManager.Translate($"CustomOption.{TargetType.Name}.{GetString()}");

    public override void PostLoadSetup()
    {
        Values = [ .. Enum.GetNames(TargetType).Where(x => !IgnoreStrings.Contains(x)) ];
        EnumValues = [ .. Enum.GetValues(TargetType).Cast<Enum>() ];
        Index = EnumValues.IndexOf(Value);
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        ViewUpdate();
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
        str.Value = str.oldValue = Index = Mathf.Clamp(EnumValues.IndexOf(Value), 0, Values.Length - 1);
        str.ValueText.text = Format();
    }
}