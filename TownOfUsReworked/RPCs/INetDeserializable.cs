namespace TownOfUsReworked.RPCs;

/// <summary>
/// An interface made to handle the ways in which a custom type can be created and initialised from a network message.
/// </summary>
public interface INetDeserializable
{
    /// <summary>
    /// Populates the internal data from a network reader.
    /// </summary>
    void FromBytes(RpcReader reader);
}