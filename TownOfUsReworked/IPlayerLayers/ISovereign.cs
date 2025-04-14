namespace TownOfUsReworked.IPlayerLayers;

public interface ISovereign : IPlayerLayer
{
    bool Revealed { get; set; }

    /// <summary>
    /// Performs an action to be done when revealing.
    /// </summary>
    void OnReveal() {}
}