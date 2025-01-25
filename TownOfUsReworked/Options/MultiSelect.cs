namespace TownOfUsReworked.Options;

public class MultiSelectOptionAttribute<T>(T none, T all, params T[] ignore) : BaseMultiSelectOptionAttribute<T>(CustomOptionType.MultiSelect, none, all) where T : struct, Enum
{
    public IEnumerable<T> Values { get; set; }
    private Type InnerType { get; } = typeof(T);
    private IEnumerable<T> Ignore { get; } = ignore;

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        Values = Enum.GetValues<T>().Except(Ignore);
    }

    public override void Debug()
    {
        base.Debug();
        Values.ForEach(x => TranslationManager.DebugId($"CustomOption.{InnerType.Name}.{x}"));
    }

    public override string Format()
    {
        var result = TranslationManager.Translate($"CustomOption.{InnerType.Name}.{Value[0]}");

        if (Value.Count > 1)
            result += $" + {Value.Count - 1}";

        return result;
    }

    public override void CreateButtons()
    {
        if (Buttons.Any())
        {
            Buttons.Keys.ForEach(x => x.gameObject.Destroy());
            Buttons.Clear();
        }
        else
            Values.ForEach(x => Buttons.Add(CreateButton(x, $"CustomOption.{InnerType.Name}.{x}"), x));

        SettingsPatches.OnValueChanged();
    }

    public override List<T> Parse(string value) => [ .. value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(Enum.Parse<T>) ];
}