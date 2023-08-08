namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Allied : Objectifier
{
    public Faction Side { get; set; }

    public override Color32 Color
    {
        get
        {
            if (Side == Faction.Crew)
                return Colors.Crew;
            else if (Side == Faction.Syndicate)
                return Colors.Syndicate;
            else if (Side == Faction.Intruder)
                return Colors.Intruder;
            else
                return ClientGameOptions.CustomObjColors ? Colors.Allied : Colors.Objectifier;
        }
    }
    public override string Name => "Allied";
    public override string Symbol => "ζ";
    public override LayerEnum Type => LayerEnum.Allied;
    public override Func<string> Description => () => Side == Faction.Neutral ? "- You are conflicted" : "";

    public Allied(PlayerControl player) : base(player) => Side = Faction.Neutral;
}