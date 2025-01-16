namespace TownOfUsReworked.IPlayerLayers;

public interface IGuesser : IPlayerLayer
{
    CustomMeeting GuessMenu { get; set; }
    CustomRolesMenu GuessingMenu { get; set; }
}