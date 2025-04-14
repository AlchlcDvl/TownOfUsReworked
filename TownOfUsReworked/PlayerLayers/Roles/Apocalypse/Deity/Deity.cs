namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Deity : Apocalypse
{
    public override DefenseEnum DefenseVal => DefenseEnum.Invincible;
    public override bool AffectedByLights => false;

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Deity;

        if (ApocalypseSettings.PlayersAlerted)
            Flash(Color);
    }
}