namespace TownOfUsReworked.PlayerLayers.Roles;

public class GameModeRole : Role
{
    protected override void Init()
    {
        base.Init();
        Faction = Faction.GameMode;
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        var role = handler.CustomRole;
        name += $"\n{role}";
        color = role.Color;
        revealed = true;
    }
}