namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Allied : Disposition
{
    [StringOption<AlliedFaction>]
    public static AlliedFaction AlliedFaction = AlliedFaction.Random;

    public Faction Side { get; set; }

    public override UColor Color => Side switch
    {
        Faction.Crew => CustomColorManager.Crew,
        Faction.Syndicate => CustomColorManager.Syndicate,
        Faction.Intruder => CustomColorManager.Intruder,
        _ => ClientOptions.CustomDispColors ? CustomColorManager.Allied : CustomColorManager.Disposition,
    };
    public override string Symbol => "ζ";
    public override LayerEnum Type => LayerEnum.Allied;
    public override Func<string> Description => () => Side == Faction.Neutral ? "- You are conflicted" : "";

    public override void Init()
    {
        base.Init();
        Side = Faction.Neutral;
    }
}