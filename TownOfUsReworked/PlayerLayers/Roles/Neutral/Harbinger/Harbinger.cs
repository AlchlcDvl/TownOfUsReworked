namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Harbinger : Neutral
{
    public override void Init()
    {
        base.Init();
        Alignment = Alignment.NeutralHarb;
    }
}

public abstract class Harbinger<Apoc> : Harbinger where Apoc : Apocalypse
{
    public abstract bool CanTransform();

    public void TurnApocalypse() => ((Apoc)Activator.CreateInstance(typeof(Apoc))).RoleUpdate(this, Player);

    public override void UpdatePlayer()
    {
        if ((CanTransform() || NeutralApocalypseSettings.DirectSpawn) && !Dead)
            TurnApocalypse();
    }
}