namespace TownOfUsReworked.Options.Attributes;

public sealed class AlignmentOptionAttribute(ListSlot alignment, bool noParts = false) : BaseHeaderOptionAttribute<AlignmentOption>(MultiMenu.Layer)
{
    private readonly ListSlot Alignment = alignment;
    private readonly bool NoParts = noParts;

    protected override AlignmentOption SetUpOption() => new(Alignment, NoParts);
}