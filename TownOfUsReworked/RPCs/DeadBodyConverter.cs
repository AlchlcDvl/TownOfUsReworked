namespace TownOfUsReworked.RPCs;

/// <summary>
/// A message converter for <see cref="DeadBody"/>s.
/// </summary>
[MessageConverter]
public sealed class DeadBodyConverter : MessageConverter<DeadBody>
{
    /// <inheritdoc/>
    public override DeadBody Read(MessageReader reader, Type objectType) => BodyById(reader.ReadByte());

    /// <inheritdoc/>
    public override void Write(MessageWriter writer, DeadBody value) => writer.Write(value.ParentId);
}