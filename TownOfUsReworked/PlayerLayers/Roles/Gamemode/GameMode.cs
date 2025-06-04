namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class GameMode : Role
{
    protected override bool UseMainColor => ClientOptions.CustomGmColors;
    protected override UColor LayerColor => CustomColorManager.GameMode;

    protected override void Init()
    {
        base.Init();
        Faction = Faction.GameMode;
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        var role = handler.CurrentRole;
        name += $"\n{role}";
        color = role.Color;
        revealed = true;
    }
}