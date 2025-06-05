namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class HideAndSeek : GameMode
{
    public override string FactionName => "Hide And Seek";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.HideAndSeek;
    }

    public override List<PlayerControl> Team() => [ .. GetLayers<Hunter>().Select(x => x.Player) ];
}