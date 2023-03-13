using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EngineerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformFix
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Engineer))
                return true;

            var role = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);

            if (role.IsBlocked)
                return false;

            if (__instance == role.FixButton)
            {
                if (!Utils.ButtonUsable(role.FixButton))
                    return false;

                if (!role.ButtonUsable)
                    return false;

                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

                if (system == null)
                    return false;

                var dummyActive = system.dummy.IsActive;
                var sabActive = system.specials.ToArray().Any(s => s.IsActive);

                if (!sabActive || dummyActive)
                    return false;

                role.UsesLeft--;
                role.LastFixed = DateTime.UtcNow;

                switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
                {
                    case 1:
                        var comms2 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();

                        if (comms2.IsActive)
                            return FixFunctions.FixMiraComms();

                        var reactor2 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                        if (reactor2.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Reactor);

                        var oxygen2 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                        if (oxygen2.IsActive)
                            return FixFunctions.FixOxygen();

                        var lights2 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights2.IsActive)
                            return FixFunctions.FixLights(lights2);

                        break;

                    case 2:
                        var comms3 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms3.IsActive)
                            return FixFunctions.FixComms();

                        var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                        if (seismic.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Laboratory);

                        var lights3 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights3.IsActive)
                            return FixFunctions.FixLights(lights3);

                        break;

                    case 0:
                    case 3:
                        var comms1 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms1.IsActive)
                            return FixFunctions.FixComms();

                        var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                        if (reactor1.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Reactor);

                        var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                        if (oxygen1.IsActive)
                            return FixFunctions.FixOxygen();

                        var lights1 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights1.IsActive)
                            return FixFunctions.FixLights(lights1);

                        break;

                    case 4:
                        var comms4 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms4.IsActive)
                            return FixFunctions.FixComms();

                        var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();

                        if (reactor.IsActive)
                            return FixFunctions.FixAirshipReactor();

                        var lights4 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights4.IsActive)
                            return FixFunctions.FixLights(lights4);

                        break;

                    case 5:
                        if (!SubmergedCompatibility.Loaded)
                            break;

                        var reactor5 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                        if (reactor5.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Reactor);

                        var lights5 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights5.IsActive)
                            return FixFunctions.FixLights(lights5);

                        var comms5 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms5.IsActive)
                            return FixFunctions.FixComms();

                        foreach (var i in PlayerControl.LocalPlayer.myTasks)
                        {
                            if (i.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                                return FixFunctions.FixSubOxygen();
                        }

                        break;

                    case 6:
                        var comms6 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms6.IsActive)
                            return FixFunctions.FixComms();

                        var reactor6 = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                        if (reactor6.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Laboratory);

                        var oxygen6 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                        if (oxygen6.IsActive)
                            return FixFunctions.FixOxygen();

                        var lights6 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights6.IsActive)
                            return FixFunctions.FixLights(lights6);

                        break;
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.EngineerFix);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }

            return true;
        }
    }
}