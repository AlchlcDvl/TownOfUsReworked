namespace TownOfUsReworked.IPlayerLayers;

public interface IAmbusher : IPlayerLayer
{
    CustomButton AmbushButton { get; set; }
    PlayerControl AmbushedPlayer { get; set; }
}