namespace TownOfUsReworked.IPlayerLayers;

public interface IBugger : IPlayerLayer
{
    List<Layer> BuggedPlayers { get; }
}