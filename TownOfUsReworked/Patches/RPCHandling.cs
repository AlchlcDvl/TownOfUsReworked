namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
public static class RPCHandling
{
    public static void Postfix(byte callId, MessageReader reader)
    {
        if (callId != CustomRPCCallID)
            return;

        using var data = new NetData(reader.ReadBytesAndSize());
        HandleRpc(data);
    }
}