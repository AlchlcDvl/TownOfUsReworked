namespace TownOfUsReworked.Pooling;

public interface IPoolable
{
    bool IsPooled { get; set; }

    void Recycle();
}