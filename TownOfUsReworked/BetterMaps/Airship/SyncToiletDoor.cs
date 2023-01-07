using HarmonyLib;
using Hazel;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(OpenDoorConsole), nameof(OpenDoorConsole.Use))]
    class SyncToiletDoor
    {
        public static void Prefix(OpenDoorConsole __instance)
        {
            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.DoorSyncToilet,
                SendOption.None, -1);
            messageWriter.Write(__instance.MyDoor.Id);
            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
        }
    }
}
