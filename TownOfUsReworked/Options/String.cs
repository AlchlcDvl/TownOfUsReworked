namespace TownOfUsReworked.Options;

public class StringOptionAttribute(MultiMenu menu, string[] ignoreStrings = null) : OptionAttribute(menu, CustomOptionType.String)
{
    public string[] Values { get; set; }
    public int Index { get; set; }
    private string[] IgnoreStrings { get; } = ignoreStrings ?? [];

    public int GetInt() => Index;

    public string GetString() => Values[Index];

    public object GetEnumValue() => Enum.Parse(TargetType, $"{Index}");

    public void Increase()
    {
        Index = CycleInt(Values.Length - 1, 0, Index, true);
        Set(Enum.Parse(TargetType, $"{Index}"));
    }

    public void Decrease()
    {
        Index = CycleInt(Values.Length - 1, 0, Index, false);
        Set(Enum.Parse(TargetType, $"{Index}"));
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var str = Setting.Cast<StringOption>();
        str.TitleText.text = TranslationManager.Translate(ID);
        str.Value = str.oldValue = GetInt();
        str.ValueText.text = Format();
        str.Values = new(0);
    }

    public override string Format() => TranslationManager.Translate($"CustomOption.{TargetType.Name}.{GetString()}");

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        Values = [ .. Enum.GetNames(TargetType).Where(x => !IgnoreStrings.Contains(x)) ];
        Index = (int)Value;
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.text = Format();
        viewSettingsInfoPanel.disabledBackground.gameObject.SetActive(false);
    }

    public override void ModifySetting(out string stringValue)
    {
        base.ModifySetting(out stringValue);
        var str = Setting.Cast<StringOption>();
        str.Value = str.oldValue = Index = Mathf.Clamp((int)Value, 0, Values.Length - 1);
        str.ValueText.text = stringValue = Format();
    }
}