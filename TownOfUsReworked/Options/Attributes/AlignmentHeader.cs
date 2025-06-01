namespace TownOfUsReworked.Options.Attributes;

public sealed class AlignmentHeaderOptionAttribute(ListSlot alignment = ListSlot.None) : BaseHeaderOptionAttribute<AlignmentHeaderOption>(MultiMenu.AlignmentSubOptions)
{
    private ListSlot Alignment { get; } = alignment;

    protected override AlignmentHeaderOption SetUpOption() => new(Alignment);
}