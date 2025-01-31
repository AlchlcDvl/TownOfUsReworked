namespace TownOfUsReworked.IPlayerLayers;

public interface IAmbusher : IPlayerLayer
{
    CustomButton AmbushButton { get; }
    PlayerControl AmbushedPlayer { get; }
}