namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Benign : Outcast
{
    public override Alignment Alignment => Alignment.Benign;
    public override bool SheriffSeesAsEvil => Sheriff.NeutEvilRed;
}