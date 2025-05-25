namespace TownOfUsReworked.Options;

public sealed class MultiSelectOptionAttribute<T>(params T[] ignore) : OptionAttribute<MultiSelectOption<T>> where T : struct, Enum
{
    private static readonly EnumInjector<T> Injector = new();

    public T NoneValue { get; init; } = Injector.MaxPossibleValue;
    public T AllValue { get; init; } = Injector.MaxPossibleValue;
    public int LeastSelected { get; init; }
    private T[] Ignore { get; } = ignore;

    protected override MultiSelectOption<T> SetUpOption() => new(NoneValue.Equals(Injector.MaxPossibleValue) ? null : NoneValue, AllValue.Equals(Injector.MaxPossibleValue) ? null : NoneValue, Ignore)
    {
        LeastSelected = LeastSelected
    };
}