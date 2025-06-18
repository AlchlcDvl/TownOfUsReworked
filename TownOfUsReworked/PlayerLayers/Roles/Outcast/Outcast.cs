namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Outcast : Role
{
    protected override UColor MainColor => CustomColorManager.Outcast;
    protected override UColor LayerColor => CustomColorManager.Outcast;
    public override bool AffectedByLights => OutcastSettings.LightsAffectOutcasts;
    public override bool CanVent => OutcastSettings.OutcastsVent;
    protected override bool UseMainColor => ClientOptions.CustomNeutColors;
    public override string FactionName => "Outcast";
    public override Faction BaseFaction => Faction.Outcast;

    public abstract bool SheriffSeesAsEvil { get; }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (Player.Is<Allied>() && !Player.Is(Faction.Crew))
            team.AddRange(AllPlayers().Where(x => x != Player && x.Is(Handler.CurrentFaction)));

        return team;
    }
}