namespace TownOfUsReworked.PlayerLayers.Dispositions;

public abstract class Paired : Teamed
{
    public PlayerControl Other;

    public override bool RoleCondition(PlayerControl player) => player == Other;
}