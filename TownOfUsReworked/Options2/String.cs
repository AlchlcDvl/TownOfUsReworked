namespace TownOfUsReworked.Options2;

public class StringOptionAttribute(MultiMenu2 menu, string[] ignoreStrings = null) : OptionAttribute(menu, CustomOptionType.String)
{
    public string[] Values { get; set; }
    public int Index { get; set; }
    public Type TargetType { get; set; }
    private string[] IgnoreStrings { get; } = ignoreStrings;

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

    public override string Format() => TranslationManager.Translate($"{ID}.{GetString()}");

    public override void SetProperty(PropertyInfo property)
    {
        base.SetProperty(property);
        TargetType = property.PropertyType;
        var baseValues = Enum.GetNames(TargetType).ToList();

        if (IgnoreStrings != null)
            baseValues.RemoveAll(IgnoreStrings.Contains);

        Values = [ .. baseValues ];
        Index = (int)Value;
    }

    public override void ViewOptionCreated()
    {
        base.ViewOptionCreated();
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.text = Format();
        viewSettingsInfoPanel.disabledBackground.gameObject.SetActive(false);
    }
}