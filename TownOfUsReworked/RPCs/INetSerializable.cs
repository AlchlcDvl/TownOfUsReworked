namespace TownOfUsReworked.RPCs;

/// <summary>
/// An interface made to handle the ways in which a custom implemented type can be serialized to bytes for networking.
/// </summary>
public interface INetSerializable
{
    /// <summary>
    /// Serializes the current instance to a sequence of bytes.
    /// </summary>
    /// <returns>A sequence of bytes representing the instance.</returns>
    IEnumerable<byte> GetBytes();
}