namespace TownOfUsReworked.Managers;

/// <summary>
/// Handles Remote Procedure Calls (RPCs) for network synchronization.
/// </summary>
public static class RpcManager
{
    /// <summary>
    /// Custom RPC identifier for TownOfUsReworked mod messages.
    /// </summary>
    private const byte CustomRPCCallID = 254;

    /// <summary>
    /// Sends mod settings to other players.<br/>
    /// Default behavior sends all non-client options to all players.
    /// </summary>
    /// <param name="setting">Specific setting to sync, or <c>null</c> for all settings</param>
    /// <param name="targetClientId">Target player ID, or <c>-1</c> for all players</param>
    /// <param name="save">Whether to save settings after sending</param>
    public static void SendOptionRPC(Option setting = null, int targetClientId = -1, bool save = true)
    {
        if (save)
            Option.SaveSettings("LastUsed");

        if (TownOfUsReworked.MciActive || !CustomPlayer.Local || GameData.Instance.PlayerCount <= 1)
            return;

        var options = setting != null ? [ setting ] : Option.AllOptions.Where(x => !x.ClientOnly && x is not BaseHeaderOption);
        var split = options.Split(70);
        Info($"Sending {options.Count()} options split to {split.Count} sets to {targetClientId}");

        foreach (var list in split)
        {
            var writer = CallTargetedOpenRpc(targetClientId, CustomRPC.Misc, MiscRPC.SyncCustomSettings, (byte)list.Count);

            foreach (var option in list)
            {
                // Info($"Sending {option}");
                writer.Write(option.RpcId.Key);
                writer.Write(option.RpcId.Value);
                option.WriteValueRpc(writer);
            }

            writer.CloseRpc();
        }
    }

    /// <summary>
    /// Processes received mod settings from other players.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    public static void ReceiveOptionRPC(MessageReader reader)
    {
        if (TownOfUsReworked.MciActive)
            return;

        var count = reader.ReadByte();
        Info($"{count} options received:");

        while (count-- > 0)
        {
            var superId = reader.ReadByte();
            var id = reader.ReadByte();
            var customOption = Option.GetOption(superId, id);

            if (customOption != null)
            {
                Info($"Received option: {customOption}");
                customOption.ReadValueRpc(reader);
            }
            else
                Failure($"No option found for id pair: {superId:X2};{id:X2}");
        }

        Option.SaveSettings("LastUsed");
    }

    /// <summary>
    /// Reads a generic value from a network message.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <returns>A value from the message.</returns>
    public static T Read<T>(this MessageReader reader) => (T)reader.Deserialize(typeof(T));

    /// <summary>
    /// Reads a byte array from a network message.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <returns><c>IEnumerable</c> of read bytes</returns>
    public static IEnumerable<byte> ReadByteList(this MessageReader reader) => reader.ReadBytesAndSize();

    /// <summary>
    /// Reads multiple player layers from a network message.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <returns><c>IEnumerable</c> of read PlayerLayers</returns>
    private static IEnumerable<PlayerLayer> ReadLayerList(this MessageReader reader)
    {
        var count = reader.ReadPackedUInt32();

        while (count-- > 0)
            yield return reader.Read<PlayerLayer>();
    }

    /// <summary>
    /// Reads and casts multiple player layers to a specified type.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <typeparam name="T">Type to cast layers to</typeparam>
    /// <returns><c>IEnumerable</c> of PlayerLayers as specified type</returns>
    public static IEnumerable<T> ReadLayers<T>(this MessageReader reader) where T : IPlayerLayer => reader.ReadLayerList().Cast<T>();

    /// <summary>
    /// Writes a value to a network message.
    /// </summary>
    /// <param name="writer">The message writer.</param>
    public static void Write(this MessageWriter writer, object item)
    {
        switch (item)
        {
            case IEnumerable<byte> enumerable:
            {
                writer.WriteBytesAndSize(enumerable.ToArray());
                break;
            }
            case IEnumerable<PlayerLayer> layers:
            {
                writer.WritePacked((uint)layers.Count());
                layers.ForEach(x => writer.Write((object)x));
                break;
            }
            default:
            {
                writer.Serialize(item); // Fallback to the api writer method
                break;
            }
        }
    }

    /// <summary>
    /// Sends an RPC message to all players.
    /// </summary>
    public static void CallRpc(CustomRPC rpc, params object[] data) => CallOpenRpc(rpc, data)?.CloseRpc();

    /// <summary>
    /// Creates an RPC message writer for all players.
    /// </summary>
    /// <returns>MessageWriter for chaining additional data</returns>
    public static MessageWriter CallOpenRpc(CustomRPC rpc, params object[] data) => CallTargetedOpenRpc(-1, rpc, data);

    /// <summary>
    /// Sends an RPC message to a specific player.
    /// </summary>
    public static void CallTargetedRpc(int targetClientId, CustomRPC rpc, params object[] data) => CallTargetedOpenRpc(targetClientId, rpc, data)?.CloseRpc();

    /// <summary>
    /// Creates an RPC message writer for a specific player.
    /// </summary>
    /// <returns>MessageWriter for chaining additional data</returns>
    private static MessageWriter CallTargetedOpenRpc(int targetClientId, CustomRPC rpc, params object[] data)
    {
        if (TownOfUsReworked.MciActive || !CustomPlayer.Local)
            return null;

        var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, CustomRPCCallID, SendOption.Reliable, targetClientId);
        writer.Write(rpc);
        data.ForEach(writer.Write);
        return writer;
    }

    /// <summary>
    /// Closes and sends the rpc.
    /// </summary>
    public static void CloseRpc(this MessageWriter writer)
    {
        if (writer == null)
            Failure("RPC writer was null");
        else
            AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
}