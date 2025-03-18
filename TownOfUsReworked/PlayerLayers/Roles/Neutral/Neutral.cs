namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Neutral : Role
{
    public override UColor MainColor => CustomColorManager.Neutral;
    public override float VisionRange => NeutralSettings.NeutralVision;
    public override bool AffectedByLights => NeutralSettings.LightsAffectNeutrals;
    public override bool CanVent => NeutralSettings.NeutralsVent;
    public override bool UseMainColor => ClientOptions.CustomNeutColors;

    protected override void Init()
    {
        base.Init();
        Faction = Faction.Neutral;
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (Player.Is<Allied>() && !Player.Is(Faction.Crew))
            team.AddRange(AllPlayers().Where(x => x != Player && x.Is(Faction)));

        return team;
    }
}