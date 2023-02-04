using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EngineerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformFix
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Engineer))
                return false;

            var role = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);

            if (__instance == role.FixButton)
            {
                if (!role.ButtonUsable)
                    return false;

                if (!__instance.isActiveAndEnabled)
                    return false;

                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var specials = system.specials.ToArray();
                var dummyActive = system.dummy.IsActive;
                var sabActive = specials.Any(s => s.IsActive);

                if (!sabActive || dummyActive)
                    return false;

                role.UsesLeft--;

                var camouflager = Role.GetRoleValue(RoleEnum.Camouflager);
                var camo = (Camouflager)camouflager;
                var concealer = Role.GetRoleValue(RoleEnum.Concealer);
                var conc = (Concealer)concealer;
                var shapeshifter = Role.GetRoleValue(RoleEnum.Shapeshifter);
                var ss = (Shapeshifter)shapeshifter;

                switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
                {
                    case 0:
                    case 1:
                        var comms2 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();

                        if (comms2.IsActive)
                            return FixMiraComms();

                        var reactor2 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                        if (reactor2.IsActive)
                            return FixReactor(SystemTypes.Reactor);

                        var oxygen2 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                        if (oxygen2.IsActive)
                            return FixOxygen();

                        var lights2 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights2.IsActive)
                            return FixLights(lights2);

                        if (camo.Camouflaged)
                            return FixCamo();

                        if (conc.Concealed)
                            return FixCamo();

                        if (ss.Shapeshifted)
                            return FixCamo();

                        break;

                    case 2:
                        var comms3 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms3.IsActive)
                            return FixComms();

                        var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                        if (seismic.IsActive)
                            return FixReactor(SystemTypes.Laboratory);

                        var lights3 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights3.IsActive)
                            return FixLights(lights3);

                        if (camo.Camouflaged)
                            return FixCamo();

                        if (conc.Concealed)
                            return FixCamo();

                        if (ss.Shapeshifted)
                            return FixCamo();

                        break;

                    case 3:
                        var comms1 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms1.IsActive)
                            return FixComms();

                        var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                        if (reactor1.IsActive)
                            return FixReactor(SystemTypes.Reactor);

                        var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                        if (oxygen1.IsActive)
                            return FixOxygen();

                        var lights1 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights1.IsActive)
                            return FixLights(lights1);

                        if (camo.Camouflaged)
                            return FixCamo();

                        if (conc.Concealed)
                            return FixCamo();

                        if (ss.Shapeshifted)
                            return FixCamo();

                        break;

                    case 4:
                        var comms4 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms4.IsActive)
                            return FixComms();

                        var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();

                        if (reactor.IsActive)
                            return FixAirshipReactor();

                        var lights4 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights4.IsActive)
                            return FixLights(lights4);

                        if (camo.Camouflaged)
                            return FixCamo();

                        if (conc.Concealed)
                            return FixCamo();

                        if (ss.Shapeshifted)
                            return FixCamo();

                        break;

                    case 5:
                        var reactor5 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                        if (reactor5.IsActive)
                            return FixReactor(SystemTypes.Reactor);

                        var lights5 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights5.IsActive)
                            return FixLights(lights5);

                        var comms5 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms5.IsActive)
                            return FixComms();

                        foreach (PlayerTask i in PlayerControl.LocalPlayer.myTasks)
                        {
                            if (i.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                                return FixSubOxygen();
                        }

                        if (camo.Camouflaged)
                            return FixCamo();

                        if (conc.Concealed)
                            return FixCamo();

                        if (ss.Shapeshifted)
                            return FixCamo();

                        break;
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.EngineerFix);
                writer.Write(PlayerControl.LocalPlayer.NetId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.FixSound, false, 1f);
                } catch {}

                return false;
            }

            return false;
        }

        private static bool FixComms()
        {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 0);
            return false;
        }

        private static bool FixMiraComms()
        {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
            return false;
        }

        private static bool FixAirshipReactor()
        {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 0);
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 1);
            return false;
        }

        private static bool FixReactor(SystemTypes system)
        {
            ShipStatus.Instance.RpcRepairSystem(system, 16);
            return false;
        }

        private static bool FixOxygen()
        {
            ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 16);
            return false;
        }

        private static bool FixCamo()
        {
            Utils.DefaultOutfitAll();
            return false;
        }

        private static bool FixConceal()
        {
            Utils.DefaultOutfitAll();
            return false;
        }

        private static bool FixShapeshift()
        {
            Utils.DefaultOutfitAll();
            return false;
        }

        private static bool FixSubOxygen()
        {
            SubmergedCompatibility.RepairOxygen();
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SubmergedFixOxygen, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.NetId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return false;
        }

        private static bool FixLights(SwitchSystem lights)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer.Write((byte)ActionsRPC.FixLights);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            lights.ActualSwitches = lights.ExpectedSwitches;
            return false;
        }
    }
}