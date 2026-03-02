using System.Runtime.CompilerServices;
using TownOfUsReworked.Pooling;

namespace TownOfUsReworked.RPCs;

/// <summary>
/// Shared base class for RpcWriter and RpcReader handling memory pooling and state.
/// </summary>
public abstract class RpcBuffer(int starting) : IPoolable
{
    protected byte[] buffer = null!;

    protected byte currentPackedByte;
    protected int currentBitIndex = starting;

    protected int position;

    public bool IsPooled { get; set; }

    private readonly int startingIndex = starting;

    public int Position => position;

    public abstract int DataSize { get; }

    public byte this[int index] => IsPooled
        ? throw new InvalidOperationException("RpcBuffer is already pooled!")
        : (uint)index >= (uint)DataSize
            ? throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the data size.")
            : buffer[index];

    protected virtual void OnRecycle() { }

    public void Recycle()
    {
        OnRecycle();
        buffer = null!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void Clear()
    {
        position = 0;
        currentBitIndex = startingIndex;
        currentPackedByte = 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected void ThrowIfPooled()
    {
        if (IsPooled)
            throw new ObjectDisposedException(nameof(RpcBuffer), "The buffer is already pooled!");
    }

    // Serialisation helpers
    protected const float MinPos = -50f;
    protected const float MaxPos = +50f;
    protected const float Diff = MaxPos - MinPos;
}