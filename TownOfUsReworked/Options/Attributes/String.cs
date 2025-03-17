namespace TownOfUsReworked.Options;

public sealed class StringOptionAttribute<T>(params T[] ignore) : OptionAttribute<StringOption<T>> where T : struct, Enum
{
    private T[] Ignore { get; } = ignore;

    protected override StringOption<T> SetUpOption() => new(Ignore);
}