namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Deity : Apocalypse
{
    public override Defense Defense => Defense.Invincible;
    public override bool AffectedByLights => false;
    public override Alignment Alignment => Alignment.Deity;

    public override void Init()
    {
        if (ApocalypseSettings.PlayersAlerted)
            Flash(Color);
    }
}