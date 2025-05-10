namespace TownOfUsReworked.Options;

public class AlignmentHeaderOption(ListSlot alignment = ListSlot.None) : HeaderOption(MultiMenu.AlignmentSubOptions, CustomOptionType.AlignmentHeader)
{
    public ListSlot Alignment { get; } = alignment;
    private AlignmentOption LinkedOption { get; set; }

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        LinkedOption = GetOptions<AlignmentOption>().FirstOrDefault(x => x.Alignment == Alignment);
    }

    protected override bool Visible() => LinkedOption?.PartiallyActive() == true;
}