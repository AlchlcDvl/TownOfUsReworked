namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Neutral : Role
{
    public override UColor Color => CustomColorManager.Neutral;
    public override Faction BaseFaction => Faction.Neutral;

    public void BaseStart()
    {
        RoleStart();
        Faction = Faction.Neutral;
        FactionColor = CustomColorManager.Neutral;
        Player.SetImpostor(false);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (IsRecruit)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);

            foreach (var rec in jackal.Recruited)
            {
                if (rec == PlayerId)
                    continue;

                team.Add(PlayerById(rec));
            }
        }
        else if (Type == LayerEnum.Jackal)
        {
            var jackal = (Jackal)this;
            team.Add(jackal.Recruit1);
            team.Add(jackal.Recruit2);
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