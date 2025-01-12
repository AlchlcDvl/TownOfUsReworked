namespace TownOfUsReworked.IPlayerLayers;

public interface IWarper : IPlayerLayer
{
    PlayerControl WarpPlayer1 { get; set; }
    bool Warping { get; set; }
}