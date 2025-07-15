namespace TownOfUsReworked.Options.Attributes;

public sealed class NumberOptionAttribute(float min, float max, float increment, Format format = Format.None, bool allowHalf = true, bool zeroIsInf = false, string customFormat = null) :
    OptionAttribute<ReworkedNumberOption>
{
    private readonly float Min = min;
    private readonly float Max = max;
    private readonly Format Format = format;
    private readonly bool AllowHalf = allowHalf;
    private readonly float Increment = increment;
    private readonly bool ZeroIsInfinity = zeroIsInf;
    private readonly string CustomFormat = customFormat;

    protected override ReworkedNumberOption SetUpOption() => new(Min, Max, Increment, Format, AllowHalf, ZeroIsInfinity, CustomFormat);
}