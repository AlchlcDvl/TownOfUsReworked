namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Objectifierless : Objectifier
{
    public override bool Hidden => true;

    public Objectifierless() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        return this;
    }
}