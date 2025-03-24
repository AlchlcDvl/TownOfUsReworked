namespace TownOfUsReworked.Options;

public sealed class MultiSelectOptionAttribute<T> : OptionAttribute<MultiSelectOption<T>> where T : struct, Enum
{
    private T? NoneValue { get; }
    private T? AllValue { get; }
    private T[] Ignore { get; }

    public MultiSelectOptionAttribute(T[] allNone = null, T[] ignore = null)
    {
        Ignore = ignore ?? [];

        try
        {
            NoneValue = allNone[0];
        }
        catch
        {
            NoneValue = null;
        }

        try
        {
            AllValue = allNone[1];
        }
        catch
        {
            AllValue = null;
        }
    }

    protected override MultiSelectOption<T> SetUpOption() => new(NoneValue, AllValue, Ignore);
}