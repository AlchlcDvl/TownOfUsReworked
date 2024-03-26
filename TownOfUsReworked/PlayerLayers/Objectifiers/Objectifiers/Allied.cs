namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Allied : Objectifier
{
    public Faction Side { get; set; }

    public override UColor Color => Side switch
    {
        Faction.Crew => CustomColorManager.Crew,
        Faction.Syndicate => CustomColorManager.Syndicate,
        Faction.Intruder => CustomColorManager.Intruder,
        _ => ClientGameOptions.CustomObjColors ? CustomColorManager.Allied : CustomColorManager.Objectifier,
    };
    public override string Name => "Allied";
    public override string Symbol => "ζ";
    public override LayerEnum Type => LayerEnum.Allied;
    public override Func<string> Description => () => Side == Faction.Neutral ? "- You are conflicted" : "";

    public override void Init() => Side = Faction.Neutral;
}