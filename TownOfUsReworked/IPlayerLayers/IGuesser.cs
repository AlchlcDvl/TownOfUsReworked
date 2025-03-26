namespace TownOfUsReworked.IPlayerLayers;

public interface IGuesser : IPlayerLayer
{
    CustomMeeting GuessMenu { get; }
    CustomGuessingMenu GuessingMenu { get; }
}