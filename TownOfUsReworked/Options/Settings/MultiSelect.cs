namespace TownOfUsReworked.Options;

public sealed class MultiSelectOption<T>(T? none, T? all, params T[] ignore) : BaseMultiSelectOption<T>(CustomOptionType.MultiSelect, all, none) where T : struct, Enum
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
            Buttons.Keys.ForEach(x => x.gameObject.Destroy());
            Buttons.Clear();
        }
        else
            Values.ForEach(x => Buttons.Add(CreateButton(x, $"CustomOption.{InnerType.Name}.{x}"), x));

        SettingsPatches.OnValueChanged();
    }

    protected override void TrySetValue(T value, out MultiSelectValue<T> newValue)
    {
        newValue = Value;

        if (value.Equals(AllValue))
        {
            var contained = newValue == value;
            newValue.Clear();
            newValue.Add(contained ? NoneValue : AllValue);
        }
        else if (value.Equals(NoneValue))
        {
            newValue.Clear();
            newValue.Add(NoneValue);
        }
        else
        {
            if (newValue == value)
                newValue.Remove(value);
            else
                newValue.Add(value);

            if (NoneValue != null)
            {
                if (newValue.Count == 0)
                    newValue.Add(NoneValue);
                else
                    newValue.Remove(NoneValue);
            }

            if (AllValue != null)
                newValue.Remove(AllValue);
        }
    }
}