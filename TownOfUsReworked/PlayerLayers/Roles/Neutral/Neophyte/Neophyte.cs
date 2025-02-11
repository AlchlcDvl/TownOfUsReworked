namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Neophyte : Neutral
{
    public List<byte> Members { get; } = [];

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Neophyte;
        Members.Clear();
        Members.Add(PlayerId);
    }

    protected override void Deinit()
    {
        base.Deinit();
        Members.Clear();
    }
}