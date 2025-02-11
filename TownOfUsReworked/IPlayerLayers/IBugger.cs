namespace TownOfUsReworked.IPlayerLayers;

public interface IBugger : IPlayerLayer
{
    List<LayerEnum> BuggedPlayers { get; }
}