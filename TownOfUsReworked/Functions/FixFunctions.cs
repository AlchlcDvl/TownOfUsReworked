using Hazel;
using TownOfUsReworked.Data;
using HarmonyLib;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Functions
{
    [HarmonyPatch]
    public static class FixFunctions
    {
        public static bool FixComms()
        {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 0);
            return false;
        }

        public static bool FixMiraComms()
        {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
            return false;
        }

        public static bool FixAirshipReactor()
        {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 0);
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 1);
            return false;
        }

        public static bool FixReactor(SystemTypes system)
        {
            ShipStatus.Instance.RpcRepairSystem(system, 16);
            return false;
        }

        public static bool FixOxygen()
        {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 16);
            return false;
        }

        public static bool FixSubOxygen()
        {
            SubmergedCompatibility.RepairOxygen();
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SubmergedFixOxygen, SendOption.Reliable);
            writer.Write(PlayerControl.LocalPlayer.NetId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return false;
        }

        public static bool FixLights(SwitchSystem lights)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.FixLights);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            lights.ActualSwitches = lights.ExpectedSwitches;
            return false;
        }
    }
}