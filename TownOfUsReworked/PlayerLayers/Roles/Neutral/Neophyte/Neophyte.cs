namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Neophyte : Neutral
{
    public List<byte> Members { get; } = [];

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Neophyte;
        Members.Clear();
        Members.Add(PlayerId);
    }

    public override void Deinit()
    {
        base.Deinit();
        Members.Clear();
    }
}