namespace TownOfUsReworked.Options;

public sealed class NumberOptionAttribute(float min, float max, float increment, Format format = Format.None, bool allowHalf = true, bool zeroIsInf = false) : OptionAttribute<NumberOption>
{
    private float Min { get; } = min;
    private float Max { get; } = max;
    private Format FormatEnum { get; } = format;
    private bool AllowHalf { get; } = allowHalf;
    private float Increment { get; } = increment;
    private bool ZeroIsInfinity { get; } = zeroIsInf;

    protected override NumberOption SetUpOption() => new(Min, Max, Increment, FormatEnum, AllowHalf, ZeroIsInfinity);
}