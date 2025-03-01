namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class NKilling : Neutral
{
    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Killing;
    }
}