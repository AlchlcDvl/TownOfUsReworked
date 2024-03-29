namespace TownOfUsReworked.Classes;

public static class RPC
{
    private const byte CustomRPCCallID = 254;

    public static void SendOptionRPC(CustomOption setting = null, int targetClientId = -1)
    {
        if (TownOfUsReworked.MCIActive)
            return;

        List<CustomOption> options;

        if (setting != null)
            options = [setting];
        else
            options = [..CustomOption.AllOptions];

        options.RemoveAll(x => x.Type is CustomOptionType.Header or CustomOptionType.Button || x.ClientOnly);
        options.Reverse();
        var split = options.Split(50);
        LogInfo($"Sending {options.Count} options to {targetClientId} split to {split.Count} sets");

        foreach (var list in split)
        {
            var writer = CallTargetedOpenRpc(targetClientId, CustomRPC.Misc, MiscRPC.SyncCustomSettings, list.Count);

            foreach (var option in list)
            {
                writer.Write(option.ID);

                if (option.Type == CustomOptionType.Toggle)
                    writer.Write((bool)option.Value);
                else if (option.Type == CustomOptionType.Number)
                    writer.Write((float)option.Value);
                else if (option.Type == CustomOptionType.String)
                    writer.Write((int)option.Value);
                else if (option.Type == CustomOptionType.Entry)
                    writer.Write((LayerEnum)option.Value);
                else if (option.Type == CustomOptionType.Layers)
                {
                    writer.Write((int)option.Value);
                    writer.Write((int)option.OtherValue);
                }

                LogInfo($"Sending {option}");
            }

            writer.EndRpc();
        }

        CustomOption.SaveSettings("Last Used");
    }

    public static void ReceiveOptionRPC(MessageReader reader)
    {
        if (TownOfUsReworked.MCIActive)
            return;

        var count = reader.ReadInt32();
        LogInfo($"{count} options received:");

        for (var i = 0; i < count; i++)
        {
            var id = reader.ReadInt32();
            var customOption = CustomOption.AllOptions.Find(option => option.ID == id);
            object value = null;
            object val = null;

            if (customOption.Type == CustomOptionType.Toggle)
                value = reader.ReadBoolean();
            else if (customOption.Type == CustomOptionType.Number)
                value = reader.ReadSingle();
            else if (customOption.Type == CustomOptionType.String)
                value = reader.ReadInt32();
            else if (customOption.Type == CustomOptionType.Entry)
                value = (LayerEnum)reader.ReadByte();
            else if (customOption.Type == CustomOptionType.Layers)
            {
                value = reader.ReadInt32();
                val = reader.ReadInt32();
            }

            customOption.Set(value, val);
            LogInfo(customOption);
        }

        CustomOption.SaveSettings("Last Used");
    }

    public static void ShareGameVersion()
    {
        if (TownOfUsReworked.MCIActive)
            return;

        var writer = CallOpenRpc(CustomRPC.Misc, MiscRPC.VersionHandshake, TownOfUsReworked.Version.Major, TownOfUsReworked.Version.Minor, TownOfUsReworked.Version.Build,
            TownOfUsReworked.Version.Revision, TownOfUsReworked.IsDev, TownOfUsReworked.DevBuild, TownOfUsReworked.IsStream, TownOfUsReworked.VersionFinal,
            TownOfUsReworked.Core.ManifestModule.ModuleVersionId.ToByteArray());
        writer.WritePacked(AmongUsClient.Instance.ClientId);
        writer.EndRpc();
        VersionHandshake(TownOfUsReworked.Version.Major, TownOfUsReworked.Version.Minor, TownOfUsReworked.Version.Build, TownOfUsReworked.Version.Revision, TownOfUsReworked.IsDev,
            TownOfUsReworked.DevBuild, TownOfUsReworked.IsStream, TownOfUsReworked.VersionFinal, TownOfUsReworked.Core.ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
    }

    public static void VersionHandshake(int major, int minor, int build, int revision, bool dev, int devBuild, bool stream, string version, Guid guid, int clientId) =>
        GameStartManagerPatch.PlayerVersions.TryAdd(clientId, new(new(major, minor, build, revision), dev, devBuild, stream, guid, version));

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
        return AllButtons.Find(x => x.ID == id);
    }

    public static T ReadLayer<T>(this MessageReader reader) where T : PlayerLayer => reader.ReadLayer() as T;

    public static List<byte> ReadByteList(this MessageReader reader) => [..reader.ReadBytesAndSize()];

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

    public static void Write(this MessageWriter writer, PlayerLayer layer)
    {
        writer.Write(layer.PlayerId);
        writer.Write(layer.Type);
    }

    public static void Write(this MessageWriter writer, LayerEnum layer) => writer.Write((byte)layer);

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
            writer.Write(layer: layer2);
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
        else if (item is LayerEnum layerEnum)
            writer.Write(layerEnum);
        else if (item is ActionsRPC action)
            writer.Write((byte)action);
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
        else if (item is MedicActionsRPC medicAction)
            writer.Write((byte)medicAction);
        else if (item is GlitchActionsRPC glitchAction)
            writer.Write((byte)glitchAction);
        else if (item is ThiefActionsRPC thiefAction)
            writer.Write((byte)thiefAction);
        else if (item is PoliticianActionsRPC polAction)
            writer.Write((byte)polAction);
        else if (item is TrapperActionsRPC trapAction)
            writer.Write((byte)trapAction);
        else if (item is MiscRPC misc)
            writer.Write((byte)misc);
        else if (item is VanillaRPC vanilla)
            writer.Write((byte)vanilla);
        else if (item is CustomButton button)
            writer.Write(button.ID);
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
            LogError($"Unknown data type used in the rpc: index - {data.ToList().IndexOf(item) + 1}, rpc - {data[data.Length == 1 ? 0 : 1]}");
    }

    public static void CallRpc(params object[] data) => CallOpenRpc(data)?.EndRpc();

    public static MessageWriter CallOpenRpc(params object[] data)
    {
        if (TownOfUsReworked.MCIActive)
            return null;

        // Just to be safe
        if (data[0] is object[] array)
            data = array;

        if (data[0] is not CustomRPC)
        {
            LogError("The first parameter must be CustomRPC");
            return null;
        }

        var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, CustomRPCCallID, SendOption.Reliable);
        data.ForEach(x => writer.Write(x, data));
        return writer;
    }

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
            LogError("The first parameter must be CustomRPC");
            return null;
        }

        var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, CustomRPCCallID, SendOption.Reliable, targetClientId);
        data.ForEach(x => writer.Write(x, data));
        return writer;
    }

    public static void EndRpc(this MessageWriter writer)
    {
        if (writer == null)
        {
            LogError("RPC writer was null");
            return;
        }

        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }
}