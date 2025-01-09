namespace TownOfUsReworked.Classes;

public static class RPC
{
    private const byte CustomRPCCallID = 254;

    public static void SendOptionRPC(OptionAttribute setting = null, int targetClientId = -1, bool save = true)
    {
        if (save)
            OptionAttribute.SaveSettings("LastUsed");

        if (TownOfUsReworked.MCIActive || !CustomPlayer.Local)
            return;

        List<OptionAttribute> options;

        if (setting != null)
            options = [ setting ];
        else
            options = [ .. OptionAttribute.AllOptions ];

        options.RemoveAll(x => x is IOptionGroup || x.ClientOnly);
        var split = options.Split(60);
        Info($"Sending {options.Count} options split to {split.Count} sets to {targetClientId}");

        foreach (var list in split)
        {
            var writer = CallTargetedOpenRpc(targetClientId, CustomRPC.Misc, MiscRPC.SyncCustomSettings, (byte)list.Count);

            foreach (var option in list)
            {
                // Info($"Sending {option}");
                writer.Write(option.RpcId.Key);
                writer.Write(option.RpcId.Value);

                if (option is ToggleOptionAttribute toggle)
                    writer.Write(toggle.Value);
                else if (option is NumberOptionAttribute number)
                    writer.Write(number.Value.Value);
                else if (option is StringOptionAttribute stringOpt)
                    writer.Write(stringOpt.Value);
                else if (option is RoleListEntryAttribute entry)
                    writer.Write(entry.Value);
                else if (option is LayerOptionAttribute layer)
                    writer.Write(layer.Value);
            }

            writer.EndRpc();
        }
    }

    public static void ReceiveOptionRPC(MessageReader reader)
    {
        if (TownOfUsReworked.MCIActive)
            return;

        var count = reader.ReadByte();
        Info($"{count} options received:");

        while (count-- > 0)
        {
            var superId = reader.ReadByte();
            var id = reader.ReadByte();
            var customOption = OptionAttribute.GetOption(superId, id);

            if (customOption == null)
            {
                Error($"No option found for id: {id}");
                continue;
            }

            customOption.SetBase(customOption.Type switch
            {
                CustomOptionType.Toggle => reader.ReadBoolean(),
                CustomOptionType.Number => reader.ReadNumber(),
                CustomOptionType.String => reader.ReadEnum(customOption.TargetType),
                CustomOptionType.Layer => reader.ReadRoleOptionData(),
                CustomOptionType.Entry => reader.ReadEnum<LayerEnum>(),
                _ => true
            }, false);
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

    public static CustomButton ReadButton(this MessageReader reader)
    {
        var id = reader.ReadString();
        return CustomButton.AllButtons.Find(x => x.ID == id);
    }

    public static T ReadLayer<T>(this MessageReader reader) where T : PlayerLayer => reader.ReadLayer() as T;

    public static List<byte> ReadByteList(this MessageReader reader) => [ .. reader.ReadBytesAndSize() ];

    public static List<PlayerLayer> ReadLayerList(this MessageReader reader)
    {
        var count = reader.ReadByte();
        var list = new List<PlayerLayer>();

        while (list.Count < count)
            list.Add(reader.ReadLayer());

        return list;
    }

    public static List<T> ReadLayerList<T>(this MessageReader reader) where T : PlayerLayer => [ .. reader.ReadLayerList().Cast<T>() ];

    public static RoleOptionData ReadRoleOptionData(this MessageReader reader) => RoleOptionData.Parse(reader.ReadString());

    public static Version ReadVersion(this MessageReader reader) => new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

    public static object ReadEnum(this MessageReader reader, Type type) => Enum.Parse(type, $"{reader.ReadByte()}");

    public static T ReadEnum<T>(this MessageReader reader) where T : struct, Enum => (T)(object)reader.ReadByte();

    public static Number ReadNumber(this MessageReader reader) => new(reader.ReadSingle());

    public static void Write(this MessageWriter writer, PlayerLayer layer)
    {
        writer.Write(layer.PlayerId);
        writer.Write(layer.Type);
    }

    public static void Write(this MessageWriter writer, RoleOptionData data) => writer.Write($"{data}");

    public static void Write(this MessageWriter writer, Version version)
    {
        writer.Write(version.Major);
        writer.Write(version.Minor);
        writer.Write(version.Build);
        writer.Write(version.Revision);
    }

    public static void Write(this MessageWriter writer, Enum enumVal) => writer.Write(System.Convert.ToByte(enumVal));

    public static void Write(this MessageWriter writer, object item, CustomRPC rpc, int index, Enum subRpc = null)
    {
        if (item is Enum enumVal)
            writer.Write(enumVal);
        else if (item is PlayerControl player)
            writer.Write(player.PlayerId);
        else if (item is DeadBody body)
            writer.Write(body.ParentId);
        else if (item is PlayerVoteArea area)
            writer.Write(area.TargetPlayerId);
        else if (item is Vent vent)
            writer.Write(vent.Id);
        else if (item is PlayerLayer layer2)
            writer.Write(layer: layer2);
        else if (item is bool boolean)
            writer.Write(boolean);
        else if (item is int integer)
            writer.Write(integer);
        else if (item is float Float)
            writer.Write(Float);
        else if (item is string text)
            writer.Write(text);
        else if (item is byte byt)
            writer.Write(byt);
        else if (item is Vector2 vector2)
            writer.Write(vector2);
        else if (item is byte[] array)
            writer.WriteBytesAndSize(array);
        else if (item is List<byte> list)
            writer.WriteBytesAndSize(list.ToArray());
        else if (item is CustomButton button)
            writer.Write(button.ID);
        else if (item is Version version)
            writer.Write(version);
        else if (item is Number num)
            writer.Write(num.Value);
        else if (item is IEnumerable<PlayerLayer> roles)
        {
            writer.Write((byte)roles.Count());
            roles.ForEach(x => writer.Write(layer: x));
        }
        else if (item is null)
            Error($"Data type used in the rpc was null: index - {index}, rpc - {rpc}, sub rpc - {subRpc?.ToString() ?? "None"}");
        else
            Error($"Unknown data type used in the rpc: index - {index}, rpc - {rpc}, sub rpc - {subRpc?.ToString() ?? "None"}, item - {item}, type - {item.GetType()}");
    }

    public static void CallRpc(CustomRPC rpc, params object[] data) => CallOpenRpc(rpc, data)?.EndRpc();

    public static MessageWriter CallOpenRpc(CustomRPC rpc, params object[] data) => CallTargetedOpenRpc(-1, rpc, data);

    public static void CallTargetedRpc(int targetClientId, CustomRPC rpc, params object[] data) => CallTargetedOpenRpc(targetClientId, rpc, data)?.EndRpc();

    public static MessageWriter CallTargetedOpenRpc(int targetClientId, CustomRPC rpc, params object[] data)
    {
        if (TownOfUsReworked.MCIActive || !CustomPlayer.Local)
            return null;

        var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, CustomRPCCallID, SendOption.Reliable, targetClientId);
        writer.Write(rpc);

        if (data.Length > 0)
        {
            // Just to be safe
            if (data[0] is object[] array)
                data = array;

            if (data[0] is Enum @enum)
                data.ForEach((x, y) => writer.Write(x, rpc, y, @enum));
            else
                data.ForEach((x, y) => writer.Write(x, rpc, y));
        }

        return writer;
    }

    public static void EndRpc(this MessageWriter writer)
    {
        if (writer == null)
            Error("RPC writer was null");
        else
            AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
}