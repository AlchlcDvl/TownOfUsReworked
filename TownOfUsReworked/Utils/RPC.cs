namespace TownOfUsReworked.Utils;

/// <summary>
/// Handles Remote Procedure Calls (RPCs) for network synchronization.
/// </summary>
public static class RPC
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
    public static void SendOptionRPC(OptionAttribute setting = null, int targetClientId = -1, bool save = true)
    {
        if (save)
            OptionAttribute.SaveSettings("LastUsed");

        if (TownOfUsReworked.MciActive || !CustomPlayer.Local || GameData.Instance.PlayerCount <= 1)
            return;

        var options = setting != null ? [ setting ] : OptionAttribute.AllOptions.Where(x => !x.ClientOnly && x is not BaseHeaderOptionAttribute);
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
            var customOption = OptionAttribute.GetOption(superId, id);

            if (customOption != null)
            {
                Info($"Received option: {customOption}");
                customOption.ReadValueRpc(reader);
            }
            else
                Failure($"No option found for id pair: {superId}:{id}");
        }

        OptionAttribute.SaveSettings("LastUsed");
    }

    /// <summary>
    /// Reads a player reference from a network message.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <returns><c>PlayerControl</c> instance for the read player ID</returns>
    public static PlayerControl ReadPlayer(this MessageReader reader) => PlayerById(reader.ReadByte());

    /// <summary>
    /// Reads a vote area reference from a network message.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <returns><c>PlayerVoteArea</c> for the read player ID</returns>
    public static PlayerVoteArea ReadVoteArea(this MessageReader reader) => VoteAreaById(reader.ReadByte());

    /// <summary>
    /// Reads a dead body reference from a network message.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <returns><c>DeadBody</c> for the read player ID</returns>
    public static DeadBody ReadBody(this MessageReader reader) => BodyById(reader.ReadByte());

    /// <summary>
    /// Reads a vent reference from a network message.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <returns><c>Vent</c> for the read vent ID</returns>
    public static Vent ReadVent(this MessageReader reader) => VentById(reader.ReadPackedInt32());

    /// <summary>
    /// Reads a player layer from a network message.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <returns><c>PlayerLayer</c> matching the read ID and type</returns>
    public static PlayerLayer ReadLayer(this MessageReader reader)
    {
        var player = reader.ReadByte();
        var type = reader.ReadEnum<LayerEnum>();
        return PlayerLayer.AllLayers.Find(x => x.PlayerId == player && x.Type == type);
    }

    /// <summary>
    /// Reads a custom button from a network message.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <returns><c>CustomButton</c> matching the read ID</returns>
    public static CustomButton ReadButton(this MessageReader reader)
    {
        var id = reader.ReadString();
        return CustomButton.AllButtons.Find(x => x.ID == id);
    }

    /// <summary>
    /// Reads and casts a player layer to a specified type.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <typeparam name="T">Type to cast layer to</typeparam>
    /// <returns>The <c>PlayerLayer</c> as specified type</returns>
    public static T ReadLayer<T>(this MessageReader reader) where T : IPlayerLayer => (T)(object)reader.ReadLayer();

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
            yield return reader.ReadLayer();
    }

    /// <summary>
    /// Reads and casts multiple player layers to a specified type.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <typeparam name="T">Type to cast layers to</typeparam>
    /// <returns><c>IEnumerable</c> of PlayerLayers as specified type</returns>
    public static IEnumerable<T> ReadLayers<T>(this MessageReader reader) where T : IPlayerLayer => reader.ReadLayerList().Cast<T>();

    /// <summary>
    /// Reads role option data from a network message.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <returns>Deserialized <c>RoleOptionData</c></returns>
    public static RoleOptionData ReadRoleOptionData(this MessageReader reader) => RoleOptionData.Deserialize(reader);

    /// <summary>
    /// Reads and converts enum value from a network message.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <typeparam name="T">Enum type to convert to</typeparam>
    /// <returns>Read value as a specified enum type</returns>
    public static T ReadEnum<T>(this MessageReader reader) where T : struct, Enum
    {
        if (typeof(T).GetEnumUnderlyingType() == typeof(byte))
            return (T)(object)reader.ReadByte();

        return (T)(object)reader.ReadPackedInt32();
    }

    /// <summary>
    /// Reads a floating point number from a network message.
    /// </summary>
    /// <param name="reader">Network message containing settings data</param>
    /// <returns><c>Number</c> wrapper for the read float value</returns>
    public static Number ReadNumber(this MessageReader reader) => new(reader.ReadSingle());

    /// <summary>
    /// Writes player layer data to network message.
    /// </summary>
    private static void Write(this MessageWriter writer, PlayerLayer layer)
    {
        writer.Write(layer.PlayerId);
        writer.Write(layer.Type);
    }

    /// <summary>
    /// Writes role option data to network message.
    /// </summary>
    public static void Write(this MessageWriter writer, RoleOptionData data) => data.Serialize(writer);

    /// <summary>
    /// Writes enum value to the network message based on its underlying type.
    /// </summary>
    private static void Write(this MessageWriter writer, Enum enumVal)
    {
        var enumType = enumVal.GetType();

        if (enumType.GetEnumUnderlyingType() == typeof(byte))
            writer.Write(Convert.ToByte(enumVal));
        else
            writer.WritePacked(Convert.ToInt32(enumVal));
    }

    /// <summary>
    /// Generic enum writer for network messages.
    /// </summary>
    /// <typeparam name="T">Enum type to write</typeparam>
    public static void Write<T>(this MessageWriter writer, T value) where T : struct, Enum => writer.Write((Enum)value);

    /// <summary>
    /// Writes various data types to the network message.
    /// </summary>
    private static void Write(this MessageWriter writer, object item, CustomRPC rpc, int index, Enum subRpc = null)
    {
        switch (item)
        {
            case Enum enumVal:
            {
                writer.Write(enumVal);
                break;
            }
            case PlayerControl player:
            {
                writer.Write(player.PlayerId);
                break;
            }
            case DeadBody body:
            {
                writer.Write(body.ParentId);
                break;
            }
            case PlayerVoteArea area:
            {
                writer.Write(area.TargetPlayerId);
                break;
            }
            case Vent vent:
            {
                writer.WritePacked(vent.Id);
                break;
            }
            case PlayerLayer layer2:
            {
                writer.Write(layer: layer2);
                break;
            }
            case bool boolean:
            {
                writer.Write(boolean);
                break;
            }
            case Number num:
            {
                writer.Write(num.Value);
                break;
            }
            case int integer:
            {
                writer.WritePacked(integer);
                break;
            }
            case uint unsignedInt:
            {
                writer.WritePacked(unsignedInt);
                break;
            }
            case float @float:
            {
                writer.Write(@float);
                break;
            }
            case string text:
            {
                writer.Write(text);
                break;
            }
            case byte byt:
            {
                writer.Write(byt);
                break;
            }
            case Vector2 vector2:
            {
                writer.Write(vector2);
                break;
            }
            case IEnumerable<byte> enumerable:
            {
                writer.WriteBytesAndSize(enumerable.ToArray());
                break;
            }
            case CustomButton button:
            {
                writer.Write(button.ID);
                break;
            }
            case IEnumerable<PlayerLayer> layers:
            {
                writer.WritePacked((uint)layers.Count());
                layers.ForEach(x => writer.Write(layer: x));
                break;
            }
            case null:
            {
                Failure($"Data type used in the rpc was null: index - {index}, rpc - {rpc}, sub rpc - {subRpc?.ToString() ?? "None"}");
                break;
            }
            default:
            {
                Failure($"Unknown data type used in the rpc: index - {index}, rpc - {rpc}, sub rpc - {subRpc?.ToString() ?? "None"}, item - {item}, type - {item.GetType().Name}");
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

        if (data.Length > 0)
        {
            Enum @enum = null;

            if (data[0] is Enum)
                @enum = data[0] as Enum;

            data.ForEach((x, y) => writer.Write(y, rpc, x, @enum));
        }

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