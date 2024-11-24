namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Neutral : Role
{
    public override UColor Color => CustomColorManager.Neutral;
    public override Faction BaseFaction => Faction.Neutral;

    public override void Init()
    {
        base.Init();
        Faction = Faction.Neutral;
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (Type == LayerEnum.Jackal)
        {
            var jackal = (Jackal)this;
            team.Add(jackal.Recruit1);
            team.Add(jackal.Recruit2);
        }

        if (HasTarget && Type != LayerEnum.BountyHunter)
            team.Add(Player.GetTarget());

        if (Player.Is(LayerEnum.Allied) && !Player.Is(Faction.Crew))
            team.AddRange(AllPlayers().Where(x => x != Player && x.Is(Faction)));

        return team;
    }
}