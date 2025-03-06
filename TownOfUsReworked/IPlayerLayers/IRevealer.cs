namespace TownOfUsReworked.IPlayerLayers;

public interface IRevealer : ISovereign
{
    bool Revealed { get; set; }

    /// <summary>
    /// Performs an action to be done when revealing.
    /// </summary>
    void OnReveal();
}