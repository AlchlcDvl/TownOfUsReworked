namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Allied)]
public sealed class Allied : Disposition
{
    [StringOption<AlliedFaction>]
    public static AlliedFaction AlliedFaction
    {
        get => AlliedFactionBacking;
        set
        {
            if (value == AlliedFaction.Compliance && (!BadGuysSettings.OrderOfCompliance || BadGuysSettings.ComplianceMembers == ComplianceType.Killers))
                value = AlliedFactionBacking < value ? AlliedFaction.Random : (BadGuysSettings.PandoricaOpens ? AlliedFaction.Pandorica : AlliedFaction.Apocalypse);

            AlliedFactionBacking = value switch
            {
                AlliedFaction.Intruder or AlliedFaction.Syndicate or AlliedFaction.Apocalypse when BadGuysSettings.PandoricaOpens => AlliedFaction.Pandorica,
                AlliedFaction.Pandorica when !BadGuysSettings.PandoricaOpens => AlliedFactionBacking < value ? AlliedFaction.Random : AlliedFaction.Apocalypse,
                _ => value
            };
        }
    }
    public static AlliedFaction AlliedFactionBacking;

    public Faction Side
    {
        get;
        set => Handler.CurrentFaction = field = value;
    }

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
    public override Func<string> Description => () => Side == Faction.Outcast ? "- You are conflicted" : "";

    public override void Init() => Side = Faction.Outcast;
}