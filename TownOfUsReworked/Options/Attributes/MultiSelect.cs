namespace TownOfUsReworked.Options.Attributes;

public sealed class MultiSelectOptionAttribute<T>(params T[] ignore) : OptionAttribute<MultiSelectOption<T>> where T : struct, Enum
{
    public T NoneValue { get; init; } = EnumInjector<T>.MaxPossibleValue;
    public T AllValue { get; init; } = EnumInjector<T>.MaxPossibleValue;
    public int LeastSelected { get; init; }

    private readonly T[] Ignore = ignore;

    protected override MultiSelectOption<T> SetUpOption() => new(
        NoneValue.Equals(EnumInjector<T>.MaxPossibleValue) ? null : NoneValue,
        AllValue.Equals(EnumInjector<T>.MaxPossibleValue) ? null : NoneValue,
        Ignore)
    {
        LeastSelected = LeastSelected
    };
}