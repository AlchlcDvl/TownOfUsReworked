namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class NKilling : Neutral
{
    public override void Init()
    {
        base.Init();
        Alignment = Alignment.NeutralKill;
    }
}