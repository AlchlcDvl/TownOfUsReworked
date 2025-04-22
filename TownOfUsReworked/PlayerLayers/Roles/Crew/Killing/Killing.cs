namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class CKilling : Crew
{
    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Killing;
    }
}