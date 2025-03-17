namespace TownOfUsReworked.Options;

public sealed class MultiSelectOptionAttribute<T>(T none, T all, params T[] ignore) : OptionAttribute<MultiSelectOption<T>>() where T : struct, Enum
{
    private T NoneValue { get; } = none;
    private T AllValue { get; } = all;
    private T[] Ignore { get; } = ignore;

    protected override MultiSelectOption<T> SetUpOption() => new(NoneValue, AllValue, Ignore);
}