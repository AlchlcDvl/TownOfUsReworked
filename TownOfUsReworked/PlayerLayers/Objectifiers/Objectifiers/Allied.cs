namespace TownOfUsReworked.PlayerLayers.Objectifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Allied : Objectifier
{
    [StringOption(MultiMenu.LayerSubOptions)]
    public static AlliedFaction AlliedFaction { get; set; } = AlliedFaction.Random;

    public Faction Side { get; set; }

    public override UColor Color => Side switch
    {
        Faction.Crew => CustomColorManager.Crew,
        Faction.Syndicate => CustomColorManager.Syndicate,
        Faction.Intruder => CustomColorManager.Intruder,
        _ => ClientOptions.CustomObjColors ? CustomColorManager.Allied : CustomColorManager.Objectifier,
    };
    public override string Name => "Allied";
    public override string Symbol => "ζ";
    public override LayerEnum Type => LayerEnum.Allied;
    public override Func<string> Description => () => Side == Faction.Neutral ? "- You are conflicted" : "";

    public override void Init()
    {
        base.Init();
        Side = Faction.Neutral;
    }
}