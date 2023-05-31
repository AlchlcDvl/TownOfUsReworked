namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Allied : Objectifier
    {
        public Faction Side = Faction.Neutral;

        public Allied(PlayerControl player) : base(player)
        {
            Name = "Allied";
            Symbol = "ζ";
            TaskText = () => Side == Faction.Crew ? Role.CrewWinCon : (Side == Faction.Intruder ? Role.IntrudersWinCon : (Side == Faction.Syndicate ? Role.SyndicateWinCon :
                "- You are conflicted"));
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Allied : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Allied;
            Type = LayerEnum.Allied;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}