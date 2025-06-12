namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class ISupport : Intruder
{
    protected override Alignment ActualAlignment => Alignment.Support;
}