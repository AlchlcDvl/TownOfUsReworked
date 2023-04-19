using Hazel;
using TownOfUsReworked.Data;
using HarmonyLib;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Functions
{
    [HarmonyPatch]
    public static class FixFunctions
    {
        public static void FixComms() => ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 0);

        public static void FixMiraComms()
        {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
        }

        public static void FixAirshipReactor()
        {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 0);
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 1);
        }

        public static void FixReactor(SystemTypes system) => ShipStatus.Instance.RpcRepairSystem(system, 16);

        public static void FixOxygen() => ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 16);

        public static void FixSubOxygen()
        {
            SubmergedCompatibility.RepairOxygen();
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SubmergedFixOxygen, SendOption.Reliable);
            writer.Write(PlayerControl.LocalPlayer.NetId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void FixLights(SwitchSystem lights)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.FixLights);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            lights.ActualSwitches = lights.ExpectedSwitches;
        }
    }
}