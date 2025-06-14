namespace TownOfUsReworked.PlayerLayers.Dispositions;

public abstract class Teamed : Disposition
{
    protected abstract bool RevealRole { get; }
    protected abstract ChatChannel Channel { get; }

    public override void Init() => Handler.Channels |= Channel;

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (!RoleCondition(player) && !RoleCondition(handler))
            return;

        name += $" {ColoredSymbol}";

        if (!RevealRole || revealed)
            return;

        var role = handler.CurrentRole;
        color = role.Color;
        name += $"\n{role}";
        revealed = true;
        removeFromConsig = true;
    }

    public virtual bool RoleCondition(LayerHandler handler) => false;

    public virtual bool RoleCondition(PlayerControl player) => false;
}