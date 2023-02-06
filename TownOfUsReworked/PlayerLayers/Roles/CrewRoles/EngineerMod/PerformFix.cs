using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
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

                        if (camo.Camouflaged)
                            return FixFunctions.FixCamo();

                        if (conc.Concealed)
                            return FixFunctions.FixCamo();

                        if (ss.Shapeshifted)
                            return FixFunctions.FixCamo();

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

                        if (camo.Camouflaged)
                            return FixFunctions.FixCamo();

                        if (conc.Concealed)
                            return FixFunctions.FixCamo();

                        if (ss.Shapeshifted)
                            return FixFunctions.FixCamo();

                        break;

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

                        if (camo.Camouflaged)
                            return FixFunctions.FixCamo();

                        if (conc.Concealed)
                            return FixFunctions.FixCamo();

                        if (ss.Shapeshifted)
                            return FixFunctions.FixCamo();

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

                        if (camo.Camouflaged)
                            return FixFunctions.FixCamo();

                        if (conc.Concealed)
                            return FixFunctions.FixCamo();

                        if (ss.Shapeshifted)
                            return FixFunctions.FixCamo();

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

                        foreach (PlayerTask i in PlayerControl.LocalPlayer.myTasks)
                        {
                            if (i.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                                return FixFunctions.FixSubOxygen();
                        }

                        if (camo.Camouflaged)
                            return FixFunctions.FixCamo();

                        if (conc.Concealed)
                            return FixFunctions.FixCamo();

                        if (ss.Shapeshifted)
                            return FixFunctions.FixCamo();

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
    }
}