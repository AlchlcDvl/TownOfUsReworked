namespace TownOfUsReworked.Options;

public sealed class AlignmentOptionAttribute(ListSlot alignment = ListSlot.None, bool noParts = false, string colorHex = null) : BaseHeaderOptionAttribute<AlignmentOption>(MultiMenu.Layer)
{
    private ListSlot Alignment { get; } = alignment;
    private bool NoParts { get; } = noParts;
    private string ColorHex { get; } = colorHex;

    protected override AlignmentOption SetUpOption() => new(Alignment, NoParts, ColorHex);
}