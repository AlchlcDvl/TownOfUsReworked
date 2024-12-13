namespace TownOfUsReworked.IPlayerLayers;

public interface IIntimidator : IPlayerLayer
{
    PlayerControl Target { get; set; }
}