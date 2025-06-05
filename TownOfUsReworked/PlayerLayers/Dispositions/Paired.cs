namespace TownOfUsReworked.PlayerLayers.Dispositions;

public abstract class Paired : Disposition
{
    public PlayerControl Other { get; set; }

    protected abstract bool RevealRole { get; }
    protected abstract ChatChannel Channel { get; }

    public override void LateInit() => Handler.Channels |= Channel;

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Other != player)
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
}