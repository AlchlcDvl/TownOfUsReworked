namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Modifierless : Modifier
{
    public override bool Hidden => true;

    public Modifierless() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        return this;
    }
}