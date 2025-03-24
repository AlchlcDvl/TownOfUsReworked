namespace TownOfUsReworked.RPCs;

/// <summary>
/// A message converter for <see cref="PlayerLayer"/>s.
/// </summary>
[MessageConverter]
public sealed class PlayerLayerConverter : MessageConverter<PlayerLayer>
{
    /// <inheritdoc/>
    public override PlayerLayer Read(MessageReader reader, Type objectType)
    {
        var player = reader.ReadByte();
        var type = reader.Read<LayerEnum>();
        return PlayerLayer.AllLayers.Find(x => x.PlayerId == player && x.Type == type);
    }

    /// <inheritdoc/>
    public override void Write(MessageWriter writer, PlayerLayer value)
    {
        writer.Write(value.PlayerId);
        writer.Write(value.Type);
    }
}