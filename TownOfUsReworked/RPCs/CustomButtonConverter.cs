namespace TownOfUsReworked.RPCs;

/// <summary>
/// A message converter for <see cref="CustomButton"/>s.
/// </summary>
[MessageConverter]
public sealed class CustomButtonConverter : MessageConverter<CustomButton>
{
    /// <inheritdoc/>
    public override CustomButton Read(MessageReader reader, Type objectType)
    {
        var id = reader.ReadString();
        return CustomButton.AllButtons.Find(x => x.ID == id);
    }

    /// <inheritdoc/>
    public override void Write(MessageWriter writer, CustomButton value) => writer.Write(value.ID);
}