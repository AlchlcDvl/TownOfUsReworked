namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Deity : Apocalypse
{
    public override DefenseEnum DefenseVal => DefenseEnum.Invincible;
    public override bool AffectedByLights => false;
    public override Alignment Alignment => Alignment.Deity;

    public override void Init()
    {
        base.Init();

        if (ApocalypseSettings.PlayersAlerted)
            Flash(Color);
    }
}