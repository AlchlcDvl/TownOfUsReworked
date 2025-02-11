namespace TownOfUsReworked.PlayerLayers.Roles;

public class GameModeRole : Role
{
    protected override void Init()
    {
        base.Init();
        Faction = Faction.GameMode;
    }
}