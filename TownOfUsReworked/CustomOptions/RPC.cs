using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.CustomOptions
{
    public static class RPC
    {
        public static void SendRPC(CustomOption optionn = null)
        {
            List<CustomOption> options;

            if (optionn != null)
                options = new List<CustomOption> {optionn};
            else
                options = CustomOption.AllOptions;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyncCustomSettings, SendOption.Reliable);

            foreach (var option in options)
            {
                if (writer.Position > 1000)
                {
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyncCustomSettings, SendOption.Reliable);
                }

                if (option.Type == CustomOptionType.Header || option.Type == CustomOptionType.Nested || option.Type == CustomOptionType.Button)
                    continue;

                writer.Write(option.ID);

                if (option.Type == CustomOptionType.Toggle)
                    writer.Write((bool)option.Value);
                else if (option.Type == CustomOptionType.Number)
                    writer.Write((float)option.Value);
                else if (option.Type == CustomOptionType.String)
                    writer.Write((int)option.Value);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void ReceiveRPC(MessageReader reader)
        {
            Utils.LogSomething("Options received:");

            while (reader.BytesRemaining > 0)
            {
                var id = reader.ReadInt32();
                var customOption = CustomOption.AllOptions.Find(option => option.ID == id);
                // Works but may need to change to gameObject.name check
                var type = customOption?.Type;
                object value = null;

                if (type == CustomOptionType.Toggle)
                    value = reader.ReadBoolean();
                else if (type == CustomOptionType.Number)
                    value = reader.ReadSingle();
                else if (type == CustomOptionType.String)
                    value = reader.ReadInt32();

                customOption?.Set(value);

                Utils.LogSomething($"{customOption?.Name} : {customOption}:");
            }
        }
    }
}