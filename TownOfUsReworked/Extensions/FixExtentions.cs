namespace TownOfUsReworked.Extensions;

public static class FixExtentions
{
    private static void FixComms() => Ship().RpcUpdateSystem(SystemTypes.Comms, 0);

    private static void FixMiraComms()
    {
        var ship = Ship();
        ship.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
        ship.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
    }

    private static void FixHeli()
    {
        var ship = Ship();
        ship.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 0);
        ship.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 1);
    }

    private static void FixReactor(SystemTypes system) => Ship().RpcUpdateSystem(system, 16);

    private static void FixOxygen() => Ship().RpcUpdateSystem(SystemTypes.LifeSupp, 16);

    private static void FixSubOxygen()
    {
        Ship().RpcUpdateSystem((SystemTypes)130, 64);
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
            var ship = Ship();

            var system = ship.Systems[SystemTypes.Sabotage].TryCast<SabotageSystemType>();

            if (system == null || !system.AnyActive)
                return;

            switch (MapPatches.CurrentMap)
            {
                case 1:
                {
                    var comms2 = ship.Systems[SystemTypes.Comms].TryCast<IActivatable>();

                    if (comms2.IsActive)
                        FixMiraComms();

                    var reactor2 = ship.Systems[SystemTypes.Reactor].TryCast<IActivatable>();

                    if (reactor2.IsActive)
                        FixReactor(SystemTypes.Reactor);

                    var oxygen2 = ship.Systems[SystemTypes.LifeSupp].TryCast<IActivatable>();

                    if (oxygen2.IsActive)
                        FixOxygen();

                    var lights2 = ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>();

                    if (lights2.IsActive)
                        FixLights(lights2);

                    break;
                }
                case 2:
                {
                    var comms3 = ship.Systems[SystemTypes.Comms].TryCast<IActivatable>();

                    if (comms3.IsActive)
                        FixComms();

                    var seismic = ship.Systems[SystemTypes.Laboratory].TryCast<IActivatable>();

                    if (seismic.IsActive)
                        FixReactor(SystemTypes.Laboratory);

                    var lights3 = ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>();

                    if (lights3.IsActive)
                        FixLights(lights3);

                    break;
                }
                case 0 or 3:
                {
                    var comms1 = ship.Systems[SystemTypes.Comms].TryCast<IActivatable>();

                    if (comms1.IsActive)
                        FixComms();

                    var reactor1 = ship.Systems[SystemTypes.Reactor].TryCast<IActivatable>();

                    if (reactor1.IsActive)
                        FixReactor(SystemTypes.Reactor);

                    var oxygen1 = ship.Systems[SystemTypes.LifeSupp].TryCast<IActivatable>();

                    if (oxygen1.IsActive)
                        FixOxygen();

                    var lights1 = ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>();

                    if (lights1.IsActive)
                        FixLights(lights1);

                    break;
                }
                case 4:
                {
                    var comms4 = ship.Systems[SystemTypes.Comms].TryCast<IActivatable>();

                    if (comms4.IsActive)
                        FixComms();

                    var reactor = ship.Systems[SystemTypes.HeliSabotage].TryCast<IActivatable>();

                    if (reactor.IsActive)
                        FixHeli();

                    var lights4 = ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>();

                    if (lights4.IsActive)
                        FixLights(lights4);

                    break;
                }
                case 5:
                {
                    var comms7 = ship.Systems[SystemTypes.Comms].TryCast<IActivatable>();

                    if (comms7.IsActive)
                        FixComms();

                    var reactor3 = ship.Systems[SystemTypes.Reactor].TryCast<IActivatable>();

                    if (reactor3.IsActive)
                        FixReactor(SystemTypes.Reactor);

                    var mixup = ship.Systems[SystemTypes.MushroomMixupSabotage].TryCast<MushroomMixupSabotageSystem>();

                    if (mixup.IsActive)
                        FixMixup(mixup);

                    break;
                }
                case 6:
                {
                    if (!SubLoaded)
                        break;

                    var reactor5 = ship.Systems[SystemTypes.Reactor].TryCast<IActivatable>();

                    if (reactor5.IsActive)
                        FixReactor(SystemTypes.Reactor);

                    var lights5 = ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>();

                    if (lights5.IsActive)
                        FixLights(lights5);

                    var comms5 = ship.Systems[SystemTypes.Comms].TryCast<IActivatable>();

                    if (comms5.IsActive)
                        FixComms();

                    if (HasTask(RetrieveOxygenMask))
                        FixSubOxygen();

                    break;
                }
                case 7:
                {
                    if (!LILoaded)
                        break;

                    var comms6 = ship.Systems[SystemTypes.Comms].TryCast<IActivatable>();

                    if (comms6.IsActive)
                        FixComms();

                    var oxygen6 = ship.Systems[SystemTypes.LifeSupp].TryCast<IActivatable>();

                    if (oxygen6.IsActive)
                        FixOxygen();

                    var lights6 = ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>();

                    if (lights6.IsActive)
                        FixLights(lights6);

                    var mixup2 = ship.Systems[SystemTypes.MushroomMixupSabotage].TryCast<MushroomMixupSabotageSystem>();

                    if (mixup2.IsActive)
                        FixMixup(mixup2);

                    var reactor7 = ship.Systems[SystemTypes.Reactor].TryCast<IActivatable>();

                    if (reactor7.IsActive)
                        FixReactor(SystemTypes.Reactor);

                    var reactor6 = ship.Systems[SystemTypes.Laboratory].TryCast<IActivatable>();

                    if (reactor6.IsActive)
                        FixReactor(SystemTypes.Laboratory);

                    break;
                }
            }
        } catch {}
    }
}