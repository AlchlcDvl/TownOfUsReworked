namespace TownOfUsReworked.IPlayerLayers;

public interface IMorpher : IPlayerLayer
{
    public PlayerControl MorphedPlayer { get; }
}