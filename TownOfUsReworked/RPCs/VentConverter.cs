namespace TownOfUsReworked.RPCs;

/// <summary>
/// A message converter for <see cref="Vent"/>s.
/// </summary>
[MessageConverter]
public sealed class VentConverter : MessageConverter<Vent>
{
    /// <inheritdoc/>
    public override Vent Read(MessageReader reader, Type objectType) => VentById(reader.ReadPackedInt32());

    /// <inheritdoc/>
    public override void Write(MessageWriter writer, Vent value) => writer.WritePacked(value.Id);
}