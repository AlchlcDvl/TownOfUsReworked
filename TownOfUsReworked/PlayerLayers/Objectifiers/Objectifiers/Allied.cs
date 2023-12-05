namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Allied : Objectifier
{
    public Faction Side { get; set; }

    public override Color Color
    {
        get => Side switch
        {
            Faction.Crew => Colors.Crew,
            Faction.Syndicate => Colors.Syndicate,
            Faction.Intruder => Colors.Intruder,
            _ => ClientGameOptions.CustomObjColors ? Colors.Allied : Colors.Objectifier,
        };
    }
    public override string Name => "Allied";
    public override string Symbol => "ζ";
    public override LayerEnum Type => LayerEnum.Allied;
    public override Func<string> Description => () => Side == Faction.Neutral ? "- You are conflicted" : "";

    public Allied(PlayerControl player) : base(player) => Side = Faction.Neutral;
}