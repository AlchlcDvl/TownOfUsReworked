namespace TownOfUsReworked.Classes;

public static class RPC
{
    public static void SendOptionRPC(CustomOption optionn = null)
    {
        List<CustomOption> options;

        if (optionn != null)
            options = new() { optionn };
        else
            options = CustomOption.AllOptions;

        var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, 254, SendOption.Reliable);
        writer.Write((byte)CustomRPC.Misc);
        writer.Write((byte)MiscRPC.SyncCustomSettings);

        foreach (var option in options)
        {
            if (writer.Position > 1000)
            {
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, 254, SendOption.Reliable);
                writer.Write((byte)CustomRPC.Misc);
                writer.Write((byte)MiscRPC.SyncCustomSettings);
            }

            if (option.Type is CustomOptionType.Header or CustomOptionType.Button)
                continue;

            writer.Write(option.ID);

            if (option.Type == CustomOptionType.Toggle)
                writer.Write((bool)option.Value);
            else if (option.Type == CustomOptionType.Number)
                writer.Write((float)option.Value);
            else if (option.Type is CustomOptionType.String or CustomOptionType.Entry)
                writer.Write((int)option.Value);
            else if (option.Type == CustomOptionType.Layers)
            {
                writer.Write((int)option.Value);
                writer.Write((int)option.OtherValue);
            }
        }

        AmongUsClient.Instance.FinishRpcImmediately(writer);
        CustomOption.SaveSettings("LastUsedSettings");
    }

    public static void ReceiveOptionRPC(MessageReader reader)
    {
        LogInfo("Options received:");

        while (reader.BytesRemaining > 0)
        {
            var id = reader.ReadInt32();
            var customOption = CustomOption.AllOptions.Find(option => option.ID == id);

            if (customOption == null)
                continue;

            object value = null;
            object val = null;

            if (customOption.Type == CustomOptionType.Toggle)
                value = reader.ReadBoolean();
            else if (customOption.Type == CustomOptionType.Number)
                value = reader.ReadSingle();
            else if (customOption.Type == CustomOptionType.String)
                value = reader.ReadInt32();
            else if (customOption.Type == CustomOptionType.Entry)
                value = (LayerEnum)reader.ReadInt32();
            else if (customOption.Type == CustomOptionType.Layers)
            {
                value = reader.ReadInt32();
                val = reader.ReadInt32();
            }

            customOption.Set(value, val);
            LogInfo(customOption);
        }

        CustomOption.SaveSettings("LastUsedSettings");
    }

    public static void ShareGameVersion()
    {
        var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, 254, SendOption.Reliable);
        writer.Write((byte)CustomRPC.Misc);
        writer.Write((byte)MiscRPC.VersionHandshake);
        writer.Write((byte)TownOfUsReworked.Version.Major);
        writer.Write((byte)TownOfUsReworked.Version.Minor);
        writer.Write((byte)TownOfUsReworked.Version.Build);
        writer.Write((byte)TownOfUsReworked.Version.Revision);
        writer.WriteBytesAndSize(TownOfUsReworked.Core.ManifestModule.ModuleVersionId.ToByteArray());
        writer.WritePacked(AmongUsClient.Instance.ClientId);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        VersionHandshake(TownOfUsReworked.Version.Major, TownOfUsReworked.Version.Minor, TownOfUsReworked.Version.Build, TownOfUsReworked.Version.Revision,
            TownOfUsReworked.Core.ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
    }

    public static void VersionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId) => GameStartManagerPatch.PlayerVersions.TryAdd(clientId, new(new(major, minor,
        build, revision), guid));

    public static PlayerControl ReadPlayer(this MessageReader reader) => PlayerById(reader.ReadByte());

    public static PlayerVoteArea ReadVoteArea(this MessageReader reader) => VoteAreaById(reader.ReadByte());

    public static DeadBody ReadBody(this MessageReader reader) => BodyById(reader.ReadByte());

    public static Vent ReadVent(this MessageReader reader) => VentById(reader.ReadInt32());

    public static PlayerLayer ReadLayer(this MessageReader reader)
    {
        var player = reader.ReadPlayer();
        var type = (LayerEnum)reader.ReadByte();
        return PlayerLayer.AllLayers.Find(x => x.Player == player && x.Type == type);
    }

    public static CustomButton ReadButton(this MessageReader reader)
    {
        var id = reader.ReadString();
        return CustomButton.AllButtons.Find(x => x.ID == id);
    }

    public static T ReadLayer<T>(this MessageReader reader) where T : PlayerLayer => reader.ReadLayer() as T;

    public static List<byte> ReadByteList(this MessageReader reader) => reader.ReadBytesAndSize().ToList();

    public static List<PlayerLayer> ReadLayerList(this MessageReader reader)
    {
        var count = reader.ReadInt32();
        var list = new List<PlayerLayer>();

        while (list.Count < count)
            list.Add(reader.ReadLayer());

        return list;
    }

    public static List<T> ReadLayerList<T>(this MessageReader reader) where T : PlayerLayer
    {
        var count = reader.ReadInt32();
        var list = new List<T>();

        while (list.Count < count)
            list.Add(reader.ReadLayer<T>());

        return list;
    }

    public static void Write(this MessageWriter writer, object item, object[] data)
    {
        if (item == null)
            LogError($"Data type used in the rpc was null: index - {data.ToList().IndexOf(item) + 1}, rpc - {data[data.Length == 1 ? 0 : 1]}");
        else if (item is CustomRPC custom)
            writer.Write((byte)custom);
        else if (item is PlayerControl player)
            writer.Write(player.PlayerId);
        else if (item is DeadBody body)
            writer.Write(body.ParentId);
        else if (item is PlayerVoteArea area)
            writer.Write(area.TargetPlayerId);
        else if (item is Vent vent)
            writer.Write(vent.Id);
        else if (item is PlayerLayer layer2)
        {
            writer.Write(layer2.PlayerId);
            writer.Write((byte)layer2.Type);
        }
        else if (item is bool boolean)
            writer.Write(boolean);
        else if (item is int integer)
            writer.Write(integer);
        else if (item is uint uinteger)
            writer.Write(uinteger);
        else if (item is float Float)
            writer.Write(Float);
        else if (item is string text)
            writer.Write(text);
        else if (item is byte Byte)
            writer.Write(Byte);
        else if (item is sbyte sByte)
            writer.Write(sByte);
        else if (item is Vector2 vector2)
            writer.Write(vector2);
        else if (item is ulong Ulong)
            writer.Write(Ulong);
        else if (item is ushort Ushort)
            writer.Write(Ushort);
        else if (item is short Short)
            writer.Write(Short);
        else if (item is long Long)
            writer.Write(Long);
        else if (item is byte[] array)
            writer.WriteBytesAndSize(array);
        else if (item is List<byte> list)
            writer.WriteBytesAndSize(list.ToArray());
        else if (item is TargetRPC target)
            writer.Write((byte)target);
        else if (item is ActionsRPC action)
            writer.Write((byte)action);
        else if (item is TurnRPC turn)
            writer.Write((byte)turn);
        else if (item is Faction faction)
            writer.Write((byte)faction);
        else if (item is Alignment alignment)
            writer.Write((byte)alignment);
        else if (item is SubFaction subfaction)
            writer.Write((byte)subfaction);
        else if (item is PlayerLayerEnum layer)
            writer.Write((byte)layer);
        else if (item is DeathReasonEnum death)
            writer.Write((byte)death);
        else if (item is WinLoseRPC winlose)
            writer.Write((byte)winlose);
        else if (item is RetActionsRPC retAction)
            writer.Write((byte)retAction);
        else if (item is GFActionsRPC gfAction)
            writer.Write((byte)gfAction);
        else if (item is RebActionsRPC rebAction)
            writer.Write((byte)rebAction);
        else if (item is DictActionsRPC dictAction)
            writer.Write((byte)dictAction);
        else if (item is GlitchActionsRPC glitchAction)
            writer.Write((byte)glitchAction);
        else if (item is ThiefActionsRPC thiefAction)
            writer.Write((byte)thiefAction);
        else if (item is PoliticianActionsRPC polAction)
            writer.Write((byte)polAction);
        else if (item is MiscRPC misc)
            writer.Write((byte)misc);
        else if (item is CustomButton button)
            writer.Write(button.ID);
        else if (item is List<PlayerLayer> layers)
        {
            writer.Write(layers.Count);
            layers.ForEach(x => writer.Write(x));
        }
        else if (item is List<Role> roles)
        {
            writer.Write(roles.Count);
            roles.ForEach(x => writer.Write(x));
        }
        else
            LogError($"Unknown data type used in the rpc: index - {data.ToList().IndexOf(item) + 1}, rpc - {data[data.Length == 1 ? 0 : 1]}");
    }

    public static void CallRpc(params object[] data)
    {
        if (data[0] is not CustomRPC)
        {
            LogError("The first parameter must be CustomRPC");
            return;
        }

        var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, 254, SendOption.Reliable);
        data.ForEach(x => writer.Write(x, data));
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
}