namespace TownOfUsReworked.IPlayerLayers;

public interface IDragger : IPlayerLayer
{
    public DeadBody CurrentlyDragging { get; set; }

    void Drop();
}