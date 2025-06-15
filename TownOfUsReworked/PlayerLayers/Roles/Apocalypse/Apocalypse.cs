namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Apocalypse : Role
{
    public static readonly Dictionary<Layer, Layer> HarbingerToDeityMap = new()
    {
        [Layer.Cultist] = Layer.Void,
        [Layer.Cannibal] = Layer.Gluttony,
        [Layer.Plaguebearer] = Layer.Pestilence
    };

    protected override UColor MainColor => CustomColorManager.Apocalypse;
    protected override UColor LayerColor => CustomColorManager.Apocalypse;
    public override bool CanVent
    {
        get
        {
            var part = Faction switch
            {
                Faction.Pandorica => PandoricaSettings.PandoricaVent,
                Faction.Illuminati => IlluminatiSettings.IlluminatiVent,
                _ => true
            };
            return ApocalypseSettings.ApocalypseVent && part;
        }
    }
    protected override bool UseMainColor => ClientOptions.CustomApocColors;
    public override Faction BaseFaction => BadGuysSettings.IlluminatiUnleashed && BadGuysSettings.IlluminatiMembers == IlluminatiType.Apocalypse
        ? Faction.Illuminati
        : (BadGuysSettings.PandoricaOpens && BadGuysSettings.PandoricaMembers == PandoricaType.Apocalypse
            ? Faction.Pandorica : Faction.Apocalypse);

    protected string CommonAbilities => Player.CanSabotage() ? "\n- You can call sabotages to distract the <#8CFFFFFF>Crew</color>" : "";

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (Player.Is<Allied>() && !Player.Is(Faction.Crew))
            team.AddRange(AllPlayers().Where(x => x != Player && x.Is(Faction)));

        return team;
    }
}