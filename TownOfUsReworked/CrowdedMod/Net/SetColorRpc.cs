﻿using Hazel;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.CrowdedMod.Net
{
    [RegisterCustomRpc((uint)CustomRPC.SetColor)]
    public class SetColorRpc : PlayerCustomRpc<TownOfUsReworked, byte>
    {
        public SetColorRpc(TownOfUsReworked plugin, uint id) : base(plugin, id) {}

        public override RpcLocalHandling LocalHandling => RpcLocalHandling.After;

        public override void Write(MessageWriter writer, byte data) => writer.Write(data);

        public override byte Read(MessageReader reader) => reader.ReadByte();

        public override void Handle(PlayerControl player, byte data) => player.SetColor(data);
    }
}