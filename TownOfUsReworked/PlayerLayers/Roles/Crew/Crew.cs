namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Crew : Role
{
    public override UColor Color => CustomColorManager.Crew;
    public override Faction BaseFaction => Faction.Crew;

    public void BaseStart()
    {
        RoleStart();
        Faction = Faction.Crew;
        FactionColor = CustomColorManager.Crew;
        Objectives = () => CrewWinCon;
        Player.SetImpostor(false);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (IsRecruit)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);
            team.AddRange(jackal.GetOtherRecruits(Player));
        }

        if (Player.Is(LayerEnum.Lovers))
            team.Add(Player.GetOtherLover());
        else if (Player.Is(LayerEnum.Rivals))
            team.Add(Player.GetOtherRival());
        else if (Player.Is(LayerEnum.Mafia))
        {
            foreach (var player in AllPlayers())
            {
                if (player != Player && player.Is(LayerEnum.Mafia))
                    team.Add(player);
            }
        }

        return team;
    }
}