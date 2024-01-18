namespace TownOfUsReworked.Extensions;

public static class FixExtentions
{
    private static void FixComms() => Ship.RpcUpdateSystem(SystemTypes.Comms, 0);

    private static void FixMiraComms()
    {
        Ship.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
        Ship.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
    }

    private static void FixHeli()
    {
        Ship.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 0);
        Ship.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 1);
    }

    private static void FixReactor(SystemTypes system) => Ship.RpcUpdateSystem(system, 16);

    private static void FixOxygen() => Ship.RpcUpdateSystem(SystemTypes.LifeSupp, 16);

    private static void FixSubOxygen()
    {
        RepairOxygen();
        CallRpc(CustomRPC.Misc, MiscRPC.SubmergedFixOxygen);
    }

    private static void FixLights(SwitchSystem lights)
    {
        CallRpc(CustomRPC.Misc, MiscRPC.FixLights);
        lights.ActualSwitches = lights.ExpectedSwitches;
    }

    private static void FixMixup(MushroomMixupSabotageSystem mixup)
    {
        CallRpc(CustomRPC.Misc, MiscRPC.FixMixup);
        mixup.secondsForAutoHeal = 0.1f;
    }

    public static void Fix()
    {
        try
        {
            var system = Ship.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

            if (system == null || !system.AnyActive)
                return;

            switch (MapPatches.CurrentMap)
            {
                case 1:
                    var comms2 = Ship.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();

                    if (comms2.IsActive)
                        FixMiraComms();

                    var reactor2 = Ship.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                    if (reactor2.IsActive)
                        FixReactor(SystemTypes.Reactor);

                    var oxygen2 = Ship.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (oxygen2.IsActive)
                        FixOxygen();

                    var lights2 = Ship.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights2.IsActive)
                        FixLights(lights2);

                    break;

                case 2:
                    var comms3 = Ship.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms3.IsActive)
                        FixComms();

                    var seismic = Ship.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (seismic.IsActive)
                        FixReactor(SystemTypes.Laboratory);

                    var lights3 = Ship.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights3.IsActive)
                        FixLights(lights3);

                    break;

                case 0 or 3:
                    var comms1 = Ship.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms1.IsActive)
                        FixComms();

                    var reactor1 = Ship.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                    if (reactor1.IsActive)
                        FixReactor(SystemTypes.Reactor);

                    var oxygen1 = Ship.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (oxygen1.IsActive)
                        FixOxygen();

                    var lights1 = Ship.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights1.IsActive)
                        FixLights(lights1);

                    break;

                case 4:
                    var comms4 = Ship.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms4.IsActive)
                        FixComms();

                    var reactor = Ship.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();

                    if (reactor.IsActive)
                        FixHeli();

                    var lights4 = Ship.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights4.IsActive)
                        FixLights(lights4);

                    break;

                case 5:
                    var comms7 = Ship.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms7.IsActive)
                        FixComms();

                    var reactor3 = Ship.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                    if (reactor3.IsActive)
                        FixReactor(SystemTypes.Reactor);

                    var mixup = Ship.Systems[SystemTypes.MushroomMixupSabotage].Cast<MushroomMixupSabotageSystem>();

                    if (mixup.IsActive)
                        FixMixup(mixup);

                    break;

                case 6:
                    if (!SubLoaded)
                        break;

                    var reactor5 = Ship.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                    if (reactor5.IsActive)
                        FixReactor(SystemTypes.Reactor);

                    var lights5 = Ship.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights5.IsActive)
                        FixLights(lights5);

                    var comms5 = Ship.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms5.IsActive)
                        FixComms();

                    if (CustomPlayer.Local.myTasks.Any(x => x.TaskType == RetrieveOxygenMask))
                        FixSubOxygen();

                    break;

                case 7:
                    if (!LILoaded)
                        break;

                    var comms6 = Ship.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                    if (comms6.IsActive)
                        FixComms();

                    var reactor6 = Ship.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (reactor6.IsActive)
                        FixReactor(SystemTypes.Laboratory);

                    var oxygen6 = Ship.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (oxygen6.IsActive)
                        FixOxygen();

                    var lights6 = Ship.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                    if (lights6.IsActive)
                        FixLights(lights6);

                    var mixup2 = Ship.Systems[SystemTypes.MushroomMixupSabotage].Cast<MushroomMixupSabotageSystem>();

                    if (mixup2.IsActive)
                        FixMixup(mixup2);

                    var reactor7 = Ship.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                    if (reactor7.IsActive)
                        FixReactor(SystemTypes.Reactor);

                    break;
            }
        } catch {}
    }
}