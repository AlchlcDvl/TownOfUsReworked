namespace TownOfUsReworked.Options;

public sealed class MultiSelectOptionAttribute<T>(params T[] ignore) : OptionAttribute<MultiSelectOption<T>> where T : struct, Enum
{
    public T NoneValue { get; init; } = (T)(object)(byte)255;
    public T AllValue { get; init; } = (T)(object)(byte)255;
    public int LeastSelected { get; init; }
    private T[] Ignore { get; } = ignore ?? [];

    protected override MultiSelectOption<T> SetUpOption() => new((byte)(object)NoneValue == 255 ? null : NoneValue, (byte)(object)AllValue == 255 ? null : NoneValue, Ignore)
    {
        LeastSelected = LeastSelected
    };
}