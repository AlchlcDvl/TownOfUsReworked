namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Apocalypse : Role
{
    protected override UColor MainColor => CustomColorManager.Apocalypse;
    public override float VisionRange => ApocalypseSettings.ApocalypseVision;
    public override bool CanVent => ApocalypseSettings.ApocalypseVent;
    protected override bool UseMainColor => ClientOptions.CustomApocColors;

    protected string CommonAbilities => Player.CanSabotage() ? "\n- You can call sabotages to distract the <#8CFFFFFF>Crew</color>" : "";

    protected override void Init()
    {
        base.Init();
        Faction = GameModifiers.IlluminatiUnleashed ? Faction.Illuminati : (GameModifiers.PandoricaOpens ? Faction.Pandorica : Faction.Apocalypse);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (Player.Is<Allied>() && !Player.Is(Faction.Crew))
            team.AddRange(AllPlayers().Where(x => x != Player && x.Is(Faction)));

        return team;
    }
}