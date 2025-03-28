namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Harbinger : Neutral
{
    public override bool AffectedByLights => NeutralHarbingerSettings.NhHaveImpVision;

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Harbinger;
    }
}

public abstract class Harbinger<TApoc> : Harbinger where TApoc : Apocalypse
{
    protected abstract bool CanTransform();

    private void TurnApocalypse() => Activator.CreateInstance<TApoc>().RoleUpdate(this);

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();

        if ((CanTransform() || NeutralApocalypseSettings.DirectSpawn) && !Dead)
            TurnApocalypse();
    }
}