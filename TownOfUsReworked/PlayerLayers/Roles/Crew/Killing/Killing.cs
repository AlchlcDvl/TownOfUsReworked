namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class CKilling : Crew
{
    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Killing;
    }
}