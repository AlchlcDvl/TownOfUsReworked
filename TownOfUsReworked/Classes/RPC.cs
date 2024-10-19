namespace TownOfUsReworked.Classes;

public static class RPC
{
    private const byte CustomRPCCallID = 254;

    public static void SendOptionRPC(OptionAttribute setting = null, int targetClientId = -1, bool save = true)
    {
        if (TownOfUsReworked.MCIActive)
            return;

        List<OptionAttribute> options;

        if (setting != null)
            options = [ setting ];
        else
            options = [ .. OptionAttribute.AllOptions ];

        options.RemoveAll(x => x.Type is CustomOptionType.Header or CustomOptionType.Alignment || x.ClientOnly);
        var split = options.Split(50);
        Info($"Sending {options.Count} options split to {split.Count} sets to {targetClientId}");

        foreach (var list in split)
        {
            var writer = CallTargetedOpenRpc(targetClientId, CustomRPC.Misc, MiscRPC.SyncCustomSettings, list.Count);

            foreach (var option in list)
            {
                // Info($"Sending {option}");
                writer.Write(option.ID);

                if (option is ToggleOptionAttribute toggle)
                    writer.Write(toggle.Value);
                else if (option is NumberOptionAttribute number)
                    writer.Write(number.Value.Value);
                else if (option is StringOptionAttribute stringOpt)
                    writer.Write(stringOpt.Value);
                else if (option is RoleListEntryAttribute entry)
                    writer.Write(entry.Value);
                else if (option is LayersOptionAttribute layer)
                    writer.Write(layer.Value);
            }

            writer.EndRpc();
        }

        CallTargetedRpc(targetClientId, CustomRPC.Misc, MiscRPC.SyncCustomSettings, 1, "Map", MapSettings.Map);

        if (save)
            OptionAttribute.SaveSettings("Last Used");
    }

    public static void ReceiveOptionRPC(MessageReader reader)
    {
        if (TownOfUsReworked.MCIActive)
            return;

        var count = reader.ReadInt32();
        Info($"{count} options received:");

        for (var i = 0; i < count; i++)
        {
            var id = reader.ReadString();

            if (id == "Map")
                SettingsPatches.SetMap(reader.ReadEnum<MapEnum>());
            else
            {
                var customOption = OptionAttribute.AllOptions.Find(option => option.ID == id);
                var value = customOption.Type switch
                {
                    CustomOptionType.Toggle => reader.ReadBoolean(),
                    CustomOptionType.Number => reader.ReadNumber(),
                    CustomOptionType.String => reader.ReadEnum(customOption.TargetType),
                    CustomOptionType.Layers => reader.ReadRoleOptionData(),
                    CustomOptionType.Entry => reader.ReadEnum<LayerEnum>(),
                    _ => true
                };
                customOption.SetBase(value, false);
            }

            // Info(id);
        }

        OptionAttribute.SaveSettings("Last Used");
    }

    public static PlayerVersion ShareGameVersion()
    {
        if (TownOfUsReworked.MCIActive)
            return null;

        var version = new PlayerVersion(TownOfUsReworked.Core.ManifestModule.ModuleVersionId, TownOfUsReworked.VersionFinal, TownOfUsReworked.Version);
        var writer = CallOpenRpc(CustomRPC.Misc, MiscRPC.VersionHandshake, version);
        writer.WritePacked(AmongUsClient.Instance.ClientId);
        writer.EndRpc();
        VersionHandshake(version, AmongUsClient.Instance.ClientId);
        return version;
    }

    public static void VersionHandshake(PlayerVersion version, int clientId) => GameStartManagerPatches.PlayerVersions.TryAdd(clientId, version);

    public static PlayerControl ReadPlayer(this MessageReader reader) => PlayerById(reader.ReadByte());

    public static PlayerVoteArea ReadVoteArea(this MessageReader reader) => VoteAreaById(reader.ReadByte());

    public static DeadBody ReadBody(this MessageReader reader) => BodyById(reader.ReadByte());

    public static Vent ReadVent(this MessageReader reader) => VentById(reader.ReadInt32());

    public static PlayerLayer ReadLayer(this MessageReader reader)
    {
        var player = reader.ReadPlayer();
        var type = reader.ReadEnum<LayerEnum>();
        return PlayerLayer.AllLayers.Find(x => x.Player == player && x.Type == type);
    }

    public static CustomButton ReadButton(this MessageReader reader)
    {
        var id = reader.ReadString();
        return AllButtons.Find(x => x.ID == id);
    }

    public static T ReadLayer<T>(this MessageReader reader) where T : PlayerLayer => reader.ReadLayer() as T;

    public static List<byte> ReadByteList(this MessageReader reader) => [ .. reader.ReadBytesAndSize() ];

    public static List<PlayerLayer> ReadLayerList(this MessageReader reader)
    {
        var count = reader.ReadInt32();
        var list = new List<PlayerLayer>();

        while (list.Count < count)
            list.Add(reader.ReadLayer());

        return list;
    }

    public static List<T> ReadLayerList<T>(this MessageReader reader) where T : PlayerLayer => reader.ReadLayerList().Cast<T>().ToList();

    public static RoleOptionData ReadRoleOptionData(this MessageReader reader) => RoleOptionData.Parse(reader.ReadString());

    public static Version ReadVersion(this MessageReader reader) => new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

    public static PlayerVersion ReadPlayerVersion(this MessageReader reader) => new(new(reader.ReadString()), reader.ReadString(), reader.ReadVersion());

    public static object ReadEnum(this MessageReader reader, Type type) => Enum.Parse(type, reader.ReadString());

    public static T ReadEnum<T>(this MessageReader reader) where T : struct => Enum.Parse<T>(reader.ReadString());

    public static Number ReadNumber(this MessageReader reader) => new(reader.ReadSingle());

    public static void Write(this MessageWriter writer, PlayerLayer layer)
    {
        writer.Write(layer.PlayerId);
        writer.Write(layer.Type);
    }

    public static void Write(this MessageWriter writer, RoleOptionData data) => writer.Write(data.ToString());

    public static void Write(this MessageWriter writer, Version version)
    {
        writer.Write(version.Major);
        writer.Write(version.Minor);
        writer.Write(version.Build);
        writer.Write(version.Revision);
    }

    public static void Write(this MessageWriter writer, PlayerVersion pv)
    {
        writer.Write(pv.Guid.ToString());
        writer.Write(pv.VersionFinal);
        writer.Write(pv.Version);
    }

    public static void Write(this MessageWriter writer, Enum enumVal) => writer.Write(enumVal.ToString());

    public static void Write(this MessageWriter writer, object item, object[] data)
    {
        if (item == null)
            Error($"Data type used in the rpc was null: index - {data.ToList().IndexOf(item) + 1}, rpc - {data[data.Length == 1 ? 0 : 1]}");
        else if (item is Enum enumVal)
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
        else if (item is PlayerVersion pv)
            writer.Write(pv);
        else if (item is Version version)
            writer.Write(version);
        else if (item is Number num)
            writer.Write(num.Value);
        else if (item is List<Role> roles)
        {
            writer.Write(roles.Count);
            roles.ForEach(x => writer.Write(layer: x));
        }
        else if (item is List<PlayerLayer> layers)
        {
            writer.Write(layers.Count);
            layers.ForEach(x => writer.Write(layer: x));
        }
        else
            Error($"Unknown data type used in the rpc: index - {data.ToList().IndexOf(item) + 1}, rpc - {data[data.Length == 1 ? 0 : 1]}, item - {item}, type - {item.GetType()}");
    }

    public static void CallRpc(params object[] data) => CallOpenRpc(data)?.EndRpc();

    public static MessageWriter CallOpenRpc(params object[] data) => CallTargetedOpenRpc(-1, data);

    public static void CallTargetedRpc(int targetClientId, params object[] data) => CallTargetedOpenRpc(targetClientId, data)?.EndRpc();

    public static MessageWriter CallTargetedOpenRpc(int targetClientId, params object[] data)
    {
        if (TownOfUsReworked.MCIActive)
            return null;

        // Just to be safe
        if (data[0] is object[] array)
            data = array;

        if (data[0] is not CustomRPC)
        {
            Error("The first parameter must be CustomRPC");
            return null;
        }

        var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, CustomRPCCallID, SendOption.Reliable, targetClientId);
        data.ForEach(x => writer.Write(x, data));
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