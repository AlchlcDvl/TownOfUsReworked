namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Allied)]
public sealed class Allied : Disposition
{
    [StringOption<AlliedFaction>]
    public static AlliedFaction AlliedFaction
    {
        get => alliedFaction;
        set
        {
            if (value == AlliedFaction.Compliance && (!GameModifiers.OrderOfCompliance || GameModifiers.ComplianceMembers == ComplianceType.Killers))
                value = alliedFaction < value ? AlliedFaction.Random : (GameModifiers.PandoricaOpens ? AlliedFaction.Pandorica : AlliedFaction.Apocalypse);

            alliedFaction = value switch
            {
                AlliedFaction.Intruder or AlliedFaction.Syndicate or AlliedFaction.Apocalypse when GameModifiers.PandoricaOpens => AlliedFaction.Pandorica,
                AlliedFaction.Pandorica when !GameModifiers.PandoricaOpens => alliedFaction < value ? AlliedFaction.Random : AlliedFaction.Apocalypse,
                _ => value
            };
        }
    }
    private static AlliedFaction alliedFaction;

    public Faction Side { get; set; }

    protected override UColor MainColor => Side switch
    {
        Faction.Crew => CustomColorManager.Crew,
        Faction.Syndicate => CustomColorManager.Syndicate,
        Faction.Intruder => CustomColorManager.Intruder,
        Faction.Apocalypse => CustomColorManager.Apocalypse,
        Faction.Compliance => CustomColorManager.Compliance,
        Faction.Pandorica => CustomColorManager.Pandorica,
        _ => CustomColorManager.Allied,
    };
    public override string Symbol => "ζ";
    public override LayerEnum Type => LayerEnum.Allied;
    public override Func<string> Description => () => Side == Faction.Neutral ? "- You are conflicted" : "";

    protected override void Init()
    {
        base.Init();
        Side = Faction.Neutral;
    }
}