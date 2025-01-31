namespace TownOfUsReworked.IPlayerLayers;

public interface IGuesser : IPlayerLayer
{
    CustomMeeting GuessMenu { get; }
    CustomRolesMenu GuessingMenu { get; }
}