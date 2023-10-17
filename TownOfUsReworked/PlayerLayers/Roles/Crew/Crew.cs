namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Crew : Role
{
    public override Color Color => Colors.Crew;
    public override Faction BaseFaction => Faction.Crew;

    protected Crew(PlayerControl player) : base(player)
    {
        Faction = Faction.Crew;
        FactionColor = Colors.Crew;
        Objectives = () => CrewWinCon;
        Player.Data.SetImpostor(false);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (IsRecruit)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);
            team.Add(jackal.EvilRecruit);
        }

        if (Player.Is(LayerEnum.Lovers))
            team.Add(Player.GetOtherLover());
        else if (Player.Is(LayerEnum.Rivals))
            team.Add(Player.GetOtherRival());
        else if (Player.Is(LayerEnum.Mafia))
        {
            foreach (var player in CustomPlayer.AllPlayers)
            {
                if (player != Player && player.Is(LayerEnum.Mafia))
                    team.Add(player);
            }
        }

        return team;
    }
}