namespace TownOfUsReworked.Options.Settings;

public sealed class AlignmentHeaderOption(ListSlot alignment = ListSlot.None) : SpecialHeader(MultiMenu.AlignmentSubOptions, CustomOptionType.AlignmentHeader)
{
    public readonly ListSlot Alignment = alignment;
    private AlignmentOption LinkedOption;

    public override void OptionCreated()
    {
        base.OptionCreated();
        Desc.GetComponentInChildren<TextMeshPro>().text = TranslationManager.Translate($"ShortDesc.{Alignment}");
        Label.color = (Alignment switch
        {
            >= ListSlot.CrewSupport and <= ListSlot.CrewUtil => CustomColorManager.Crew,
            >= ListSlot.IntruderSupport and <= ListSlot.IntruderHead => CustomColorManager.Intruder,
            > ListSlot.OutcastPros and <= ListSlot.OutcastNeo => CustomColorManager.Outcast,
            >= ListSlot.SyndicateKill and <= ListSlot.SyndicateUtil => CustomColorManager.Syndicate,
            ListSlot.OutcastBen or ListSlot.OutcastEvil => CustomColorManager.Stalemate,
            ListSlot.OutcastPros => CustomColorManager.Stalemate.Alternate(0.1f),
            ListSlot.ApocDeity or ListSlot.ApocHarb => CustomColorManager.Apocalypse,
            ListSlot.Modifiers => CustomColorManager.Modifier,
            ListSlot.Abilities => CustomColorManager.Ability,
            ListSlot.Dispositions => CustomColorManager.Disposition,
            _ => UColor.white
        }).Alternate(0.3f);
    }

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        LinkedOption = GetHeaderOptions<AlignmentOption>().FirstOrDefault(x => x.Alignment == Alignment);
    }

    protected override bool Visible() => LinkedOption?.PartiallyActive() == true;

    public override void Debug()
    {
        base.Debug();
        TranslationManager.DebugId($"ShortDesc.{Alignment}");
    }
}