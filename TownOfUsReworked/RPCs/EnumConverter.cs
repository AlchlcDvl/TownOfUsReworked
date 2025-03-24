namespace TownOfUsReworked.RPCs;

/// <summary>
/// A message converter for <see cref="Enum"/>s.
/// </summary>
[MessageConverter]
public sealed class EnumConverter : MessageConverter<Enum>
{
    /// <inheritdoc/>
    public override Enum Read(MessageReader reader, Type objectType) => (Enum)Enum.ToObject(objectType, reader.Deserialize(Enum.GetUnderlyingType(objectType)));

    /// <inheritdoc/>
    public override void Write(MessageWriter writer, Enum value) => writer.Serialize(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())));

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType) => base.CanConvert(objectType) || objectType.IsEnum;
}