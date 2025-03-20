namespace TownOfUsReworked.IPlayerLayers;

public interface IIntimidator : IPlayerLayer
{
    PlayerControl Target { get; }
    bool ShookAlready { get; set; }
}