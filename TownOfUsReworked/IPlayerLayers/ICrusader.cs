namespace TownOfUsReworked.IPlayerLayers;

public interface ICrusader : ISyndicate
{
    CustomButton CrusadeButton { get; }
    PlayerControl CrusadedPlayer { get; }
}