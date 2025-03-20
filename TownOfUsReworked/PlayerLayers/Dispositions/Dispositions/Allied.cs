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
            if (value is AlliedFaction.Intruder or AlliedFaction.Syndicate && GameModifiers.PandoricaOpens)
                value = AlliedFaction.Pandorica;

            AlliedFactionPriv = value;
        }
    }
    private static AlliedFaction AlliedFactionPriv;

    public Faction Side { get; set; }

    public override UColor MainColor => Side switch
    {
        Faction.Crew => CustomColorManager.Crew,
        Faction.Syndicate => CustomColorManager.Syndicate,
        Faction.Intruder => CustomColorManager.Intruder,
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