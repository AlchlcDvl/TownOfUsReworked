namespace TownOfUsReworked.Options.Attributes;

public sealed class NumberOptionAttribute(float min, float max, float increment, Format format = Format.None, bool allowHalf = true, bool zeroIsInf = false, string customFormat = null) :
    OptionAttribute<ReworkedNumberOption>
{
    private float Min { get; } = min;
    private float Max { get; } = max;
    private Format Format { get; } = format;
    private bool AllowHalf { get; } = allowHalf;
    private float Increment { get; } = increment;
    private bool ZeroIsInfinity { get; } = zeroIsInf;
    private string CustomFormat { get; } = customFormat;

    protected override ReworkedNumberOption SetUpOption() => new(Min, Max, Increment, Format, AllowHalf, ZeroIsInfinity, CustomFormat);
}