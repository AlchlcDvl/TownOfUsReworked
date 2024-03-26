namespace TownOfUsReworked.PlayerLayers.Roles;

public class Roleless : Role
{
    public override void Init()
    {
        RoleStart();
        Player.SetImpostor(false);
    }
}