namespace TownOfUsReworked.Options;

public class StringOptionAttribute(MultiMenu menu, string[] ignoreStrings = null) : OptionAttribute<Enum>(menu, CustomOptionType.String)
{
    public IEnumerable<string> Values { get; set; }
    public int Index { get; set; }
    private string[] IgnoreStrings { get; } = ignoreStrings ?? [];
    private IEnumerable<Enum> EnumValues { get; set; }
    private int Count { get; set; }

    public string GetString() => Values.ElementAt(Index);

    public void Increase()
    {
        Index = CycleInt(Count, 0, Index, true);
        Set(EnumValues.ElementAt(Index));
    }

    public void Decrease()
    {
        Index = CycleInt(Count, 0, Index, false);
        Set(EnumValues.ElementAt(Index));
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var str = Setting.Cast<StringOption>();
        str.TitleText.text = TranslationManager.Translate(ID);
        str.Values = new(0);

        if ((!AmongUsClient.Instance.AmHost || IsInGame()) && !ClientOnly && !TownOfUsReworked.MCIActive)
        {
            str.PlusBtn.gameObject.SetActive(false);
            str.MinusBtn.gameObject.SetActive(false);
        }
    }

    public override string Format() => TranslationManager.Translate($"CustomOption.{TargetType.Name}.{GetString()}");

    public override void PostLoadSetup()
    {
        Values = Enum.GetNames(TargetType).Except(IgnoreStrings);
        EnumValues = Enum.GetValues(TargetType).Cast<Enum>().Where(x => !IgnoreStrings.Contains($"{x}"));
        Index = EnumValues.IndexOf(Value);
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
        str.Value = str.oldValue = Index = Mathf.Clamp(EnumValues.IndexOf(Value), 0, Count);
        str.ValueText.text = Format();
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