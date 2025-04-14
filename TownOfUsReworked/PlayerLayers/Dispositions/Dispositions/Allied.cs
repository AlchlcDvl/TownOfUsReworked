namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Allied)]
public sealed class Allied : Disposition
{
    [StringOption<AlliedFaction>]
    public static AlliedFaction AlliedFaction
    {
        get => AlliedFactionPriv;
        set
        {
            if (value == AlliedFaction.Compliance && (!GameModifiers.OrderOfCompliance || GameModifiers.ComplianceType == ComplianceType.Killers))
                value = AlliedFactionPriv < value ? AlliedFaction.Random : (GameModifiers.PandoricaOpens ? AlliedFaction.Pandorica : AlliedFaction.Apocalypse);

            if (value is AlliedFaction.Intruder or AlliedFaction.Syndicate or AlliedFaction.Apocalypse && GameModifiers.PandoricaOpens)
                value = AlliedFaction.Pandorica;
            else if (value is AlliedFaction.Pandorica && !GameModifiers.PandoricaOpens)
                value = AlliedFactionPriv < value ? AlliedFaction.Random : AlliedFaction.Apocalypse;

            AlliedFactionPriv = value;
        }
    }
    private static AlliedFaction AlliedFactionPriv;

    public Faction Side { get; set; }

    protected override UColor MainColor => Side switch
    {
        Faction.Crew => CustomColorManager.Crew,
        Faction.Syndicate => CustomColorManager.Syndicate,
        Faction.Intruder => CustomColorManager.Intruder,
        _ => CustomColorManager.Allied,
    };
    public override string Symbol => "ζ";
    public override LayerEnum Type { get; } = LayerEnum.Allied;
    public override Func<string> Description => () => Side == Faction.Neutral ? "- You are conflicted" : "";

    protected override void Init()
    {
        base.Init();
        Side = Faction.Neutral;
    }
}