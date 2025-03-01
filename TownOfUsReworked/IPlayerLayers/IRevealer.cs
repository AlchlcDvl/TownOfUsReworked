namespace TownOfUsReworked.IPlayerLayers;

public interface IRevealer : ISovereign
{
    bool Revealed { get; set; }

    void OnReveal() {}
}