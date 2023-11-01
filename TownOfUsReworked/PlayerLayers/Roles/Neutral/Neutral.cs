namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Neutral : Role
{
    public override Color Color => Colors.Neutral;
    public override Faction BaseFaction => Faction.Neutral;

    protected Neutral(PlayerControl player) : base(player)
    {
        Faction = Faction.Neutral;
        FactionColor = Colors.Neutral;
        Data.SetImpostor(false);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (IsRecruit)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);
            team.Add(Player == jackal.EvilRecruit ? jackal.GoodRecruit : jackal.EvilRecruit);
        }
        else if (Type == LayerEnum.Jackal)
        {
            var jackal = (Jackal)this;
            team.Add(jackal.GoodRecruit);
            team.Add(jackal.EvilRecruit);
        }

        if (HasTarget && Type != LayerEnum.BountyHunter)
            team.Add(Player.GetTarget());

        if (Player.Is(LayerEnum.Lovers))
            team.Add(Player.GetOtherLover());
        else if (Player.Is(LayerEnum.Rivals))
            team.Add(Player.GetOtherRival());
        else if (Player.Is(LayerEnum.Linked))
            team.Add(Player.GetOtherLink());
        else if (Player.Is(LayerEnum.Mafia))
        {
            foreach (var player in CustomPlayer.AllPlayers)
            {
                if (player != Player && player.Is(LayerEnum.Mafia))
                    team.Add(player);
            }
        }
        else if (Player.Is(LayerEnum.Allied) && !Player.Is(Faction.Crew))
        {
            foreach (var player in CustomPlayer.AllPlayers)
            {
                if (player.Is(Faction) && player != Player)
                    team.Add(player);
            }
        }

        return team;
    }
}