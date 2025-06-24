namespace TownOfUsReworked.IPlayerLayers;

public interface ITargeter : IPlayerLayer
{
    PlayerControl TargetPlayer { get; set; }
}