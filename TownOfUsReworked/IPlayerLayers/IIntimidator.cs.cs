namespace TownOfUsReworked.IPlayerLayers;

public interface IIntimidator : IPlayerLayer
{
    public PlayerControl Target { get; set; }
}