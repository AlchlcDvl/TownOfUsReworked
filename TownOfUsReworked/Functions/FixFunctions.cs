using Hazel;
using TownOfUsReworked.Data;
using HarmonyLib;
using TownOfUsReworked.Classes;
using System.Linq;

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

        public static void Fix()
        {
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

            if (system == null)
                return;

            var dummyActive = system.dummy.IsActive;
            var sabActive = system.specials.ToArray().Any(s => s.IsActive);

            if (!sabActive || dummyActive)
                return;

            switch (TownOfUsReworked.VanillaOptions.MapId)
            {
                case 1:
                    var comms2 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();

                    if (comms2.IsActive)
                        FixMiraComms();

                    var reactor2 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                    if (reactor2.IsActive)
                        FixReactor(SystemTypes.Reactor);

                    var oxygen2 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (oxygen2.IsActive)
                        FixOxygen();

                    var lights2 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights2.IsActive)
                        FixLights(lights2);

                    break;

                case 2:
                    var comms3 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms3.IsActive)
                        FixComms();

                    var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (seismic.IsActive)
                        FixReactor(SystemTypes.Laboratory);

                    var lights3 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights3.IsActive)
                        FixLights(lights3);

                    break;

                case 0:
                case 3:
                    var comms1 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms1.IsActive)
                        FixComms();

                    var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                    if (reactor1.IsActive)
                        FixReactor(SystemTypes.Reactor);

                    var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (oxygen1.IsActive)
                        FixOxygen();

                    var lights1 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights1.IsActive)
                        FixLights(lights1);

                    break;

                case 4:
                    var comms4 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms4.IsActive)
                        FixComms();

                    var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();

                    if (reactor.IsActive)
                        FixAirshipReactor();

                    var lights4 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights4.IsActive)
                        FixLights(lights4);

                    break;

                case 5:
                    if (!SubmergedCompatibility.Loaded)
                        break;

                    var reactor5 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                    if (reactor5.IsActive)
                        FixReactor(SystemTypes.Reactor);

                    var lights5 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights5.IsActive)
                        FixLights(lights5);

                    var comms5 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms5.IsActive)
                        FixComms();

                    foreach (var i in PlayerControl.LocalPlayer.myTasks)
                    {
                        if (i.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                            FixSubOxygen();
                    }

                    break;

                case 6:
                    var comms6 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms6.IsActive)
                        FixComms();

                    var reactor6 = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (reactor6.IsActive)
                        FixReactor(SystemTypes.Laboratory);

                    var oxygen6 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (oxygen6.IsActive)
                        FixOxygen();

                    var lights6 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights6.IsActive)
                        FixLights(lights6);

                    break;
            }
        }
    }
}