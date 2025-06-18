namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Proselyte : Outcast
{
    public override Alignment Alignment => Alignment.Proselyte;
    public override bool SheriffSeesAsEvil => false;
}