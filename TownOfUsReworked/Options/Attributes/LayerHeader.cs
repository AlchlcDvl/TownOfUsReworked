namespace TownOfUsReworked.Options;

public sealed class LayerHeaderOptionAttribute(LayerEnum layer) : BaseHeaderOptionAttribute<LayerHeaderOption>(MultiMenu.LayerSubOptions)
{
    private LayerEnum Layer { get; } = layer;

    protected override LayerHeaderOption SetUpOption() => new(Layer);
}