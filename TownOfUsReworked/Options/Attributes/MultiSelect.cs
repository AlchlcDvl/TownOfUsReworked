namespace TownOfUsReworked.Options;

public sealed class MultiSelectOptionAttribute<T>(params T[] ignore) : OptionAttribute<MultiSelectOption<T>> where T : struct, Enum
{
    public T NoneValue { get; set; } = (T)(object)(byte)255;
    public T AllValue { get; set; } = (T)(object)(byte)255;
    public bool ForceAtLeastOne { get; set; }
    private T[] Ignore { get; } = ignore ?? [];

    protected override MultiSelectOption<T> SetUpOption() => new((byte)(object)NoneValue == 255 ? null : NoneValue, (byte)(object)AllValue == 255 ? null : NoneValue, Ignore)
    {
        ForceAtLeastOne = ForceAtLeastOne
    };
}