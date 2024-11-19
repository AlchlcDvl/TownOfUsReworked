namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Harbinger : Neutral
{
    public abstract bool CanTransform { get; }

    public abstract void TurnApocalypse();

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.NeutralHarb;
    }

    public override void UpdatePlayer()
    {
        if ((CanTransform || NeutralApocalypseSettings.DirectSpawn) && !Dead)
            TurnApocalypse();
    }
}