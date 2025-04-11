namespace TownOfUsReworked.RPCs;

/// <summary>
/// A message converter for <see cref="NetData"/>s.
/// </summary>
[MessageConverter]
public sealed class NetDataConverter : MessageConverter<NetData>
{
    /// <inheritdoc/>
    public override NetData Read(MessageReader reader, Type objectType) => new(reader.ReadBytes(reader.ReadUInt16()));

    /// <inheritdoc/>
    public override void Write(MessageWriter writer, NetData value) => writer.Write(value.ToBytes());
}