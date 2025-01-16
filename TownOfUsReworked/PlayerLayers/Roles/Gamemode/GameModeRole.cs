namespace TownOfUsReworked.PlayerLayers;

public class GameModeRole : Role
{
    public override void Init()
    {
        base.Init();
        Faction = Faction.GameMode;
    }
}