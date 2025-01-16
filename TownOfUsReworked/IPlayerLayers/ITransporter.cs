namespace TownOfUsReworked.IPlayerLayers;

public interface ITransporter : IPlayerLayer
{
    bool Transporting { get; set; }
}