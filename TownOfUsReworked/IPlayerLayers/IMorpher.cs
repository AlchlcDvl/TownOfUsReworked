namespace TownOfUsReworked.IPlayerLayers;

public interface IMorpher : IPlayerLayer
{
    PlayerControl MorphedPlayer { get; }
}