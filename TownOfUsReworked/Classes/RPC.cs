namespace TownOfUsReworked.Classes;

public static class RPC
{
    private const byte CustomRPCCallID = 254;

    public static void SendOptionRPC(OptionAttribute setting = null, int targetClientId = -1, bool save = true)
    {
        if (save)
            OptionAttribute.SaveSettings("LastUsed");

        if (TownOfUsReworked.MciActive || !CustomPlayer.Local || AllPlayers().Count() == 1)
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
                customOption.ReadValueRpc(reader);
            else
                Failure($"No option found for id pair: {superId}:{id}");
        }

        OptionAttribute.SaveSettings("LastUsed");
    }

    public static PlayerControl ReadPlayer(this MessageReader reader) => PlayerById(reader.ReadByte());

    public static PlayerVoteArea ReadVoteArea(this MessageReader reader) => VoteAreaById(reader.ReadByte());

    public static DeadBody ReadBody(this MessageReader reader) => BodyById(reader.ReadByte());

    public static Vent ReadVent(this MessageReader reader) => VentById(reader.ReadInt32());

    public static PlayerLayer ReadLayer(this MessageReader reader)
    {
        var player = reader.ReadByte();
        var type = reader.ReadEnum<LayerEnum>();
        return PlayerLayer.AllLayers.Find(x => x.PlayerId == player && x.Type == type);
    }

    // public static IPlayerLayer ReadILayer(this MessageReader reader) => reader.ReadLayer();

    public static CustomButton ReadButton(this MessageReader reader)
    {
        var id = reader.ReadString();
        return CustomButton.AllButtons.Find(x => x.ID == id);
    }

    public static T ReadLayer<T>(this MessageReader reader) where T : PlayerLayer => reader.ReadLayer() as T;

    // public static T ReadILayer<T>(this MessageReader reader) where T : IPlayerLayer => (T)reader.ReadILayer();

    public static IEnumerable<byte> ReadByteList(this MessageReader reader) => reader.ReadBytesAndSize();

    private static List<PlayerLayer> ReadLayerList(this MessageReader reader)
    {
        var count = reader.ReadUInt32();
        var list = new List<PlayerLayer>();

        while (list.Count < count)
            list.Add(reader.ReadLayer());

        return list;
    }

    public static List<T> ReadLayerList<T>(this MessageReader reader) where T : PlayerLayer => [ .. reader.ReadLayerList().Cast<T>() ];

    public static RoleOptionData ReadRoleOptionData(this MessageReader reader) => RoleOptionData.Deserialize(reader.ReadBytesAndSize());

    public static T ReadEnum<T>(this MessageReader reader) where T : struct, Enum
    {
        if (typeof(T).GetEnumUnderlyingType() == typeof(byte))
            return (T)(object)reader.ReadByte();

        return (T)(object)reader.ReadInt32();
    }

    public static Number ReadNumber(this MessageReader reader) => new(reader.ReadSingle());

    public static List<T> ReadEnumList<T>(this MessageReader reader) where T : struct, Enum
    {
        var enums = new List<T>();
        var count = reader.ReadUInt32();

        while (count-- > 0)
            enums.Add(reader.ReadEnum<T>());

        return enums;
    }

    private static void Write(this MessageWriter writer, PlayerLayer layer)
    {
        writer.Write(layer.PlayerId);
        writer.Write(layer.Type);
    }

    public static void Write(this MessageWriter writer, RoleOptionData data) => writer.WriteBytesAndSize(data.Serialize());

    private static void Write(this MessageWriter writer, Enum enumVal)
    {
        var enumType = enumVal.GetType();

        if (enumType.GetEnumUnderlyingType() == typeof(byte))
            writer.Write(Convert.ToByte(enumVal));
        else
            writer.Write(Convert.ToInt32(enumVal));
    }

    public static void Write<T>(this MessageWriter writer, T value) where T : struct, Enum => writer.Write((Enum)value);

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
                writer.Write(vent.Id);
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
                writer.Write((uint)layers.Count());
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

    public static void CallRpc(CustomRPC rpc, params object[] data) => CallOpenRpc(rpc, data)?.CloseRpc();

    public static MessageWriter CallOpenRpc(CustomRPC rpc, params object[] data) => CallTargetedOpenRpc(-1, rpc, data);

    public static void CallTargetedRpc(int targetClientId, CustomRPC rpc, params object[] data) => CallTargetedOpenRpc(targetClientId, rpc, data)?.CloseRpc();

    private static MessageWriter CallTargetedOpenRpc(int targetClientId, CustomRPC rpc, params object[] data)
    {
        if (TownOfUsReworked.MciActive || !CustomPlayer.Local)
            return null;

        var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, CustomRPCCallID, SendOption.Reliable, targetClientId);
        writer.Write(rpc);

        if (data.Length > 0)
        {
            // Just to be safe
            if (data[0] is object[] array)
                data = array;

            Enum @enum = null;

            if (data[0] is Enum)
                @enum = data[0] as Enum;

            data.ForEach((x, y) => writer.Write(x, rpc, y, @enum));
        }

        return writer;
    }

    public static void CloseRpc(this MessageWriter writer)
    {
        if (writer == null)
            Failure("RPC writer was null");
        else
            AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
}