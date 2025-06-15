namespace TownOfUsReworked.Options.Attributes;

public sealed class LayerHeaderOptionAttribute(Layer layer) : BaseHeaderOptionAttribute<LayerHeaderOption>(MultiMenu.LayerSubOptions)
{
    private Layer Layer { get; } = layer;

    protected override LayerHeaderOption SetUpOption() => new(Layer);
}