namespace TownOfUsReworked.RPCs;

/// <summary>
/// An interface made to handle the ways in which a custom implemented type can be serialized to bytes for networking.
/// </summary>
public interface INetSerializable
{
    /// <summary>
    /// Serializes the current instance to an array of bytes.
    /// </summary>
    /// <returns>An array of bytes representing the instance.</returns>
    byte[] ToBytes();
}