namespace TownOfUsReworked.Options;

public sealed class AlignmentHeaderOptionAttribute(ListSlot alignment = ListSlot.None) : BaseHeaderOptionAttribute<AlignmentHeaderOption>(MultiMenu.LayerSubOptions)
{
    private ListSlot Alignment { get; } = alignment;

    protected override AlignmentHeaderOption SetUpOption() => new(Alignment);
}