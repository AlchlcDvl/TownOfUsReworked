namespace TownOfUsReworked.IPlayerLayers;

/// <summary>
/// An interface to signify an alerting role.
/// </summary>
public interface IAlerter : IPlayerLayer
{
    /// <summary>
    /// The button that dictates whether or not the effect is active.
    /// </summary>
    CustomButton AlertButton { get; }
}