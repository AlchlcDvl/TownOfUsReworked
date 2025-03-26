namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class GameModeRole : Role
{
    protected override bool UseMainColor => ClientOptions.CustomGmColors;

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