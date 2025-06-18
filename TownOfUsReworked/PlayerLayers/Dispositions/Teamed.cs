namespace TownOfUsReworked.PlayerLayers.Dispositions;

public abstract class Teamed : Disposition
{
    protected abstract bool RevealRole { get; }
    public abstract ChatChannel Channel { get; }
    public abstract bool CanChat { get; }

    public override void Init() => Handler.Channels |= Channel;

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (!RoleCondition(player))
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

    public virtual bool RoleCondition(PlayerControl player) => false;
}