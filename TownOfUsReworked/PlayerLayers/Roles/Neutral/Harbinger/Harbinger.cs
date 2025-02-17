namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Harbinger : Neutral
{
    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Harbinger;
    }
}

public abstract class Harbinger<TApoc> : Harbinger where TApoc : Apocalypse
{
    protected abstract bool CanTransform();

    private void TurnApocalypse() => ((TApoc)Activator.CreateInstance(typeof(TApoc))).RoleUpdate(this);

    public override void UpdatePlayer()
    {
        if ((CanTransform() || NeutralApocalypseSettings.DirectSpawn) && !Dead)
            TurnApocalypse();
    }
}