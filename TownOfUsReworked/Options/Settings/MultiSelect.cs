namespace TownOfUsReworked.Options.Settings;

public sealed class MultiSelectOption<T>(T? none, T? all, T[] ignore = null, T[] include = null, MultiSelectValue<T> defaultValue = null) : BaseMultiSelectOption<T>(CustomOptionType.MultiSelect,
    all, none, defaultValue) where T : struct, Enum
{
    private IEnumerable<T> Values { get; } = (include ?? Enum.GetValues<T>()).Except(ignore ?? []);
    private Type InnerType { get; } = typeof(T);

    public override void Debug()
    {
        base.Debug();
        Values.Do(x => TranslationManager.DebugId($"CustomOption.{InnerType.Name}.{x}"));
    }

    public override bool IsId(string id) => base.IsId(id) || Values.Any(x => id == $"CustomOption.{InnerType.Name}.{x}");

    protected override string FormatValue()
    {
        if (Value.Count == 0)
            return TranslationManager.Translate("ValueText.None");

        var result = TranslationManager.Translate($"CustomOption.{InnerType.Name}.{Value.First()}");

        if (Value.Count > 1)
            result += $" + {Value.Count - 1}";

        return result;
    }

    protected override void CreateButtons()
    {
        if (Buttons.Any())
        {
            Buttons.Keys.Do(x => x.gameObject.Destroy());
            Buttons.Clear();
        }
        else
            Values.Do(x => Buttons.Add(CreateButton(x, $"CustomOption.{InnerType.Name}.{x}"), x));

        SettingsPatches.OnValueChanged();
    }
}