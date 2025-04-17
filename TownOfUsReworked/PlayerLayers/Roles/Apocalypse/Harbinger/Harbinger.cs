namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Harbinger : Apocalypse
{
    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Harbinger;
    }
}

public abstract class Harbinger<T> : Harbinger where T : Deity
{
    protected abstract bool CanTransform();

    private void TurnApocalypse() => Activator.CreateInstance<T>().RoleUpdate(this);

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();

        if ((CanTransform() || ApocalypseSettings.DirectSpawn) && !Dead)
            TurnApocalypse();
    }
}