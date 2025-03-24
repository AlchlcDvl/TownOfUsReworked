namespace TownOfUsReworked.RPCs;

/// <summary>
/// A message converter for <see cref="RoleOptionData"/>s.
/// </summary>
[MessageConverter]
public sealed class RoleOptionDataConverter : MessageConverter<RoleOptionData>
{
    /// <inheritdoc/>
    public override RoleOptionData Read(MessageReader reader, Type objectType) => RoleOptionData.Deserialize(reader);

    /// <inheritdoc/>
    public override void Write(MessageWriter writer, RoleOptionData value) => value.Serialize(writer);
}