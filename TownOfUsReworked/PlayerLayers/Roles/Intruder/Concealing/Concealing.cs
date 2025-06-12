namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Concealing : Intruder
{
    protected override Alignment ActualAlignment => Alignment.Concealing;
}