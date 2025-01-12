namespace TownOfUsReworked.IPlayerLayers;

public interface ICrusader : ISyndicate
{
    CustomButton CrusadeButton { get; set; }
    PlayerControl CrusadedPlayer { get; set; }
}