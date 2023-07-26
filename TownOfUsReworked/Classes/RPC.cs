namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class RPC
    {
        public static void SendOptionRPC(CustomOption optionn = null)
        {
            List<CustomOption> options;

            if (optionn != null)
                options = new() { optionn };
            else
                options = CustomOption.AllOptions;

            var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, 255, SendOption.Reliable);
            writer.Write((byte)CustomRPC.Misc);
            writer.Write((byte)MiscRPC.SyncCustomSettings);

            foreach (var option in options)
            {
                if (writer.Position > 1000)
                {
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, 255, SendOption.Reliable);
                    writer.Write((byte)CustomRPC.Misc);
                    writer.Write((byte)MiscRPC.SyncCustomSettings);
                }

                if (option.Type is CustomOptionType.Header or CustomOptionType.Nested or CustomOptionType.Button)
                    continue;

                writer.Write(option.ID);
                writer.Write(option.Name);

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
        }

        public static void ReceiveOptionRPC(MessageReader reader)
        {
            LogSomething("Options received:");

            while (reader.BytesRemaining > 0)
            {
                var id = reader.ReadInt32();
                var name = reader.ReadString();
                var customOption = CustomOption.AllOptions.Find(option => option.ID == id && option.Name == name);

                if (customOption == null)
                    continue;

                //Works but may need to change to gameObject.name check
                object value = null;
                object val = null;

                if (customOption.Type == CustomOptionType.Toggle)
                    value = reader.ReadBoolean();
                else if (customOption.Type == CustomOptionType.Number)
                    value = reader.ReadSingle();
                else if (customOption.Type is CustomOptionType.String or CustomOptionType.Entry)
                    value = reader.ReadInt32();
                else if (customOption.Type == CustomOptionType.Layers)
                {
                    value = reader.ReadInt32();
                    val = reader.ReadInt32();
                }

                customOption.Set(value, val);
                LogSomething($"{customOption}");
            }

            CustomOption.SaveSettings("LastUsedSettings");
        }

        public static void ShareGameVersion()
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, 255, SendOption.Reliable);
            writer.Write((byte)CustomRPC.Misc);
            writer.Write((byte)MiscRPC.VersionHandshake);
            writer.Write((byte)TownOfUsReworked.Version.Major);
            writer.Write((byte)TownOfUsReworked.Version.Minor);
            writer.Write((byte)TownOfUsReworked.Version.Build);
            writer.Write((byte)TownOfUsReworked.Version.Revision);
            writer.WriteBytesAndSize(TownOfUsReworked.Executing.ManifestModule.ModuleVersionId.ToByteArray());
            writer.WritePacked(AmongUsClient.Instance.ClientId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            VersionHandshake(TownOfUsReworked.Version.Major, TownOfUsReworked.Version.Minor, TownOfUsReworked.Version.Build, TownOfUsReworked.Version.Revision,
                TownOfUsReworked.Executing.ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
        }

        public static void VersionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
        {
            if (!GameStartManagerPatch.PlayerVersions.ContainsKey(clientId))
                GameStartManagerPatch.PlayerVersions.Add(clientId, new(new(major, minor, build, revision), guid));
        }

        public static void Write(this MessageWriter writer, Vector3 vector3)
        {
            var vector2 = new Vector2(vector3.x, vector3.y);
            writer.Write(vector2);
            writer.Write(vector3.z);
        }

        public static Vector3 ReadVector3(this MessageReader reader)
        {
            var vector2 = reader.ReadVector2();
            var z = reader.ReadSingle();
            return new(vector2.x, vector2.y, z);
        }

        public static PlayerControl ReadPlayer(this MessageReader reader) => PlayerById(reader.ReadByte());

        public static PlayerVoteArea ReadVoteArea(this MessageReader reader) => VoteAreaById(reader.ReadByte());

        public static DeadBody ReadBody(this MessageReader reader) => BodyById(reader.ReadByte());

        public static PlayerLayer ReadLayer(this MessageReader reader)
        {
            var player = reader.ReadPlayer();
            var type = (LayerEnum)reader.ReadByte();
            return PlayerLayer.AllLayers.Find(x => x.Player == player && x.Type == type);
        }

        public static void CallRpc(params object[] data)
        {
            if (data[0] is not CustomRPC)
                throw new ArgumentException("The first param must be CustomRPC");

            var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, 255, SendOption.Reliable);
            writer.Write((byte)(CustomRPC)data[0]);

            if (data.Length > 1)
            {
                foreach (var item in data[1..])
                {
                    if (writer.Position > 1000)
                    {
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, 255, SendOption.Reliable);
                        writer.Write((byte)(CustomRPC)data[0]);
                    }

                    if (item is bool boolean)
                        writer.Write(boolean);
                    else if (item is int integer)
                        writer.Write(integer);
                    else if (item is uint uinteger)
                        writer.Write(uinteger);
                    else if (item is float Float)
                        writer.Write(Float);
                    else if (item is byte Byte)
                        writer.Write(Byte);
                    else if (item is sbyte sByte)
                        writer.Write(sByte);
                    else if (item is Vector2 vector2)
                        writer.Write(vector2);
                    else if (item is Vector3 vector3)
                        writer.Write(vector3);
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
                    else if (item is TargetRPC target)
                        writer.Write((byte)target);
                    else if (item is ActionsRPC action)
                        writer.Write((byte)action);
                    else if (item is TurnRPC turn)
                        writer.Write((byte)turn);
                    else if (item is Faction faction)
                        writer.Write((byte)faction);
                    else if (item is RoleAlignment alignment)
                        writer.Write((byte)alignment);
                    else if (item is SubFaction subfaction)
                        writer.Write((byte)subfaction);
                    else if (item is PlayerLayerEnum layer)
                        writer.Write((byte)layer);
                    else if (item is InspectorResults results)
                        writer.Write((byte)results);
                    else if (item is DeathReasonEnum death)
                        writer.Write((byte)death);
                    else if (item is WinLoseRPC winlose)
                        writer.Write((byte)winlose);
                    else if (item is RetributionistActionsRPC retAction)
                        writer.Write((byte)retAction);
                    else if (item is GodfatherActionsRPC gfAction)
                        writer.Write((byte)gfAction);
                    else if (item is RebelActionsRPC rebAction)
                        writer.Write((byte)rebAction);
                    else if (item is MiscRPC misc)
                        writer.Write((byte)misc);
                    else if (item is PlayerControl player)
                        writer.Write(player.PlayerId);
                    else if (item is DeadBody body)
                        writer.Write(body.ParentId);
                    else if (item is PlayerVoteArea area)
                        writer.Write(area.TargetPlayerId);
                    else if (item is PlayerLayer layer2)
                    {
                        writer.Write(layer2.PlayerId);
                        writer.Write((byte)layer2.Type);
                    }
                    else if (item == null)
                    {
                        LogSomething($"Data type used in the rpc was null: item - {nameof(item)}, rpc - {data[0]}");
                        break;
                    }
                    else
                    {
                        LogSomething($"Unknown data type used in the rpc: item - {nameof(item)}, rpc - {data[0]}");
                        break;
                    }
                }
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }
}