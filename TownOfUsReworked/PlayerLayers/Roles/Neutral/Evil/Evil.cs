namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Evil : Neutral
{
    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Evil;
    }
}