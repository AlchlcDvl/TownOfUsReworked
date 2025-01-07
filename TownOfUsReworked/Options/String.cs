namespace TownOfUsReworked.Options;

public class StringOptionAttribute(MultiMenu menu, string[] ignoreStrings = null) : OptionAttribute<Enum>(menu, CustomOptionType.String)
{
    public string[] Values { get; set; }
    public int Index { get; set; }
    private string[] IgnoreStrings { get; } = ignoreStrings ?? [];
    private Enum[] EnumValues { get; set; }

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
        str.TitleText.SetText(TranslationManager.Translate(ID));
        str.Values = new(0);

        if ((!AmongUsClient.Instance.AmHost || IsInGame()) && !ClientOnly)
        {
            str.PlusBtn.gameObject.SetActive(false);
            str.MinusBtn.gameObject.SetActive(false);
        }
    }

    public override string Format() => TranslationManager.Translate($"CustomOption.{TargetType.Name}.{GetString()}");

    public override void PostLoadSetup()
    {
        Values = [ .. Enum.GetNames(TargetType).Except(IgnoreStrings) ];
        EnumValues = [ .. Enum.GetValues(TargetType).Cast<Enum>().Where(x => !IgnoreStrings.Contains($"{x}")) ];
        Index = EnumValues.IndexOf(Value);
    }

    public override void ViewUpdate()
    {
        var viewSettingsInfoPanel = ViewSetting.Cast<ViewSettingsInfoPanel>();
        viewSettingsInfoPanel.settingText.SetText(Format());
        viewSettingsInfoPanel.disabledBackground.gameObject.SetActive(false);
    }

    public override void Update()
    {
        var str = Setting.Cast<StringOption>();
        str.Value = str.oldValue = Index = Mathf.Clamp(EnumValues.IndexOf(Value), 0, Values.Length - 1);
        str.ValueText.SetText(Format());
    }

    public override void Debug()
    {
        base.Debug();

        foreach (var name in Values)
        {
            var id = $"CustomOption.{TargetType.Name}.{name}";

            if (!TranslationManager.IdExists(id))
                Fatal(id);
        }
    }
}