namespace TownOfUsReworked.IPlayerLayers;

public interface IDragger : IPlayerLayer
{
    DeadBody CurrentlyDragging { get; set; }

    void Drop();
}