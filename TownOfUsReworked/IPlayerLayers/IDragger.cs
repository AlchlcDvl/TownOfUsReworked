namespace TownOfUsReworked.IPlayerLayers;

public interface IDragger : IRole
{
    DeadBodyHandler CurrentlyDragging { get; set; }

    void Drop();
}