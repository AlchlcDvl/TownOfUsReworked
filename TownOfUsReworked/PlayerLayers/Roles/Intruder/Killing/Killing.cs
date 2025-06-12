namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class IKilling : Intruder
{
    protected override Alignment ActualAlignment => Alignment.Killing;
}