namespace TownOfUsReworked.Options.Attributes;

public sealed class LayerOptionAttribute(string hexCode, Layer layer, bool noParts = false, byte min = 1, byte max = 15, byte change = 1) : OptionAttribute<LayerOption>
{
    private byte Max { get; } = max;
    private byte Min { get; } = min;
    private bool NoParts { get; } = noParts;
    private string HexCode { get; } = hexCode;
    private Layer Layer { get; } = layer;
    private byte Change { get; } = change;

    protected override LayerOption SetUpOption() => new(HexCode, Layer, NoParts, Min, Max, Change);
}