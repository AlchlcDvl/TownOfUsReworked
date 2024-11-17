namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Neophyte : Neutral
{
    public List<byte> Members { get; set; }

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.NeutralNeo;
        Members = [ Player.PlayerId ];
    }

    public override void Deinit()
    {
        base.Deinit();
        Members.Clear();
    }
}