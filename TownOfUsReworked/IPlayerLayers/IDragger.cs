namespace TownOfUsReworked.IPlayerLayers;

public interface IDragger : IRole
{
    DeadBody CurrentlyDragging { get; set; }

    void Drop();
}