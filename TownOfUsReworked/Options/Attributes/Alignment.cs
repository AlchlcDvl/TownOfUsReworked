namespace TownOfUsReworked.Options;

public sealed class AlignmentOptionAttribute(ListSlot alignment, bool noParts = false) : BaseHeaderOptionAttribute<AlignmentOption>(MultiMenu.Layer)
{
    private ListSlot Alignment { get; } = alignment;
    private bool NoParts { get; } = noParts;

    protected override AlignmentOption SetUpOption() => new(Alignment, NoParts);
}