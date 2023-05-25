namespace TownOfUsReworked.CustomOptions
{
    [HarmonyPatch]
    public static class RPC
    {
        public static void SendRPC(CustomOption optionn = null)
        {
            List<CustomOption> options;

            if (optionn != null)
                options = new() { optionn };
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

                if (option.Type is CustomOptionType.Header or CustomOptionType.Nested or CustomOptionType.Button)
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

                if (customOption == null)
                    continue;

                // Works but may need to change to gameObject.name check
                object value = null;

                if (customOption.Type == CustomOptionType.Toggle)
                    value = reader.ReadBoolean();
                else if (customOption.Type == CustomOptionType.Number)
                    value = reader.ReadSingle();
                else if (customOption.Type == CustomOptionType.String)
                    value = reader.ReadInt32();

                customOption.Set(value);
                Utils.LogSomething($"{customOption?.Name} : {customOption}");
            }
        }
    }
}