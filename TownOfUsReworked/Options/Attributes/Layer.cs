namespace TownOfUsReworked.Options.Attributes;

public sealed class LayerOptionAttribute(string hexCode, Layer layer, bool noParts = false, byte min = 1, byte max = 15, byte change = 1) : OptionAttribute<LayerOption>
{
    private readonly byte Max = max;
    private readonly byte Min = min;
    private readonly bool NoParts = noParts;
    private readonly string HexCode = hexCode;
    private readonly Layer Layer = layer;
    private readonly byte Change = change;

    protected override LayerOption SetUpOption() => new(HexCode, Layer, NoParts, Min, Max, Change);
}