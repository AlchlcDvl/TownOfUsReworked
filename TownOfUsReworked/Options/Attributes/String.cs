namespace TownOfUsReworked.Options.Attributes;

public sealed class StringOptionAttribute<T>(T[] ignore = null, T[] include = null) : OptionAttribute<StringOption<T>> where T : struct, Enum
{
    private T[] Ignore { get; } = ignore;
    private T[] Include { get; } = include;

    protected override StringOption<T> SetUpOption() => new(Ignore, Include);
}