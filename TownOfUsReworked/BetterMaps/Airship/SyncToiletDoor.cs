using HarmonyLib;
using Hazel;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(OpenDoorConsole), nameof(OpenDoorConsole.Use))]
    public static class SyncToiletDoor
    {
        public static void Prefix(OpenDoorConsole __instance)
        {
            var messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.DoorSyncToilet, SendOption.None);
            messageWriter.Write(__instance.MyDoor.Id);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }
    }
}