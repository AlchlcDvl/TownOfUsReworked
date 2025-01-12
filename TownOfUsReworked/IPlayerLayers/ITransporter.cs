namespace TownOfUsReworked.IPlayerLayers;

public interface ITransporter : IPlayerLayer
{
    PlayerControl TransportPlayer1 { get; set; }
    PlayerControl TransportPlayer2 { get; set; }
    bool Transporting { get; set; }
}