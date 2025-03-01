namespace TownOfUsReworked.Options;

public class MultiSelectOptionAttribute<T>(T none, T all, params T[] ignore) : BaseMultiSelectOptionAttribute<T>(CustomOptionType.MultiSelect, all, none) where T : struct, Enum
{
    private IEnumerable<T> Values { get; } = Enum.GetValues<T>().Except(ignore);
    private Type InnerType { get; } = typeof(T);

    public override void Debug()
    {
        base.Debug();
        Values.ForEach(x => TranslationManager.DebugId($"CustomOption.{InnerType.Name}.{x}"));
    }

    protected override string Format()
    {
        var result = TranslationManager.Translate($"CustomOption.{InnerType.Name}.{Value.First()}");

        if (Value.Count > 1)
            result += $" + {Value.Count - 1}";

        return result;
    }

    protected override void CreateButtons()
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
}