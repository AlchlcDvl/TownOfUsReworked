namespace TownOfUsReworked.RPCs;

/// <summary>
/// A message converter for <see cref="PlayerVoteArea"/>s.
/// </summary>
[MessageConverter]
public sealed class PlayerVoteAreaConverter : MessageConverter<PlayerVoteArea>
{
    /// <inheritdoc/>
    public override PlayerVoteArea Read(MessageReader reader, Type objectType) => VoteAreaById(reader.ReadByte());

    /// <inheritdoc/>
    public override void Write(MessageWriter writer, PlayerVoteArea value) => writer.Write(value.TargetPlayerId);
}