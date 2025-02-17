namespace TownOfUsReworked.IPlayerLayers;

public interface IDrunkard : ISyndicate
{
    CustomButton ConfuseButton { get; }
    PlayerControl ConfusedPlayer { get; }
}