namespace TownOfUsReworked.PlayerLayers.Roles;

public class GameModeRole : Role
{
    public override void Init()
    {
        base.Init();
        Faction = Faction.GameMode;
    }
}