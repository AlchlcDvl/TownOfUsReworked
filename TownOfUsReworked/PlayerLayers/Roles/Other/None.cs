namespace TownOfUsReworked.PlayerLayers.Roles;

public class Roleless : Role
{
    public Roleless() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        RoleStart();
        Player.SetImpostor(false);
        return this;
    }
}