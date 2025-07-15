namespace TownOfUsReworked.Options.Attributes;

public sealed class LayerHeaderOptionAttribute(Layer layer) : BaseHeaderOptionAttribute<LayerHeaderOption>(MultiMenu.LayerSubOptions)
{
    private readonly Layer Layer = layer;

    protected override LayerHeaderOption SetUpOption() => new(Layer);
}