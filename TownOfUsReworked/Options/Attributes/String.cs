namespace TownOfUsReworked.Options.Attributes;

public sealed class StringOptionAttribute<T>(T[]? ignore = null, T[]? include = null) : OptionAttribute<StringOption<T>> where T : struct, Enum
{
    private readonly T[]? Ignore = ignore;
    private readonly T[]? Include = include;

    protected override StringOption<T> SetUpOption() => new(Ignore, Include);
}