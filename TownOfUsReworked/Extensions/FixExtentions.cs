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
        var ship = Ship();

        if (ship.Systems == null)
            return;

        if (ship.Systems[SystemTypes.Sabotage].TryCast<SabotageSystemType>(out var system) || !system.AnyActive)
            return;

        switch (MapPatches.CurrentMap)
        {
            case 1:
            {
                if (ship.Systems[SystemTypes.Comms].TryCast<IActivatable>(out var activatable) && activatable.IsActive)
                    FixMiraComms();

                if (ship.Systems[SystemTypes.Reactor].TryCast(out activatable) && activatable.IsActive)
                    FixReactor(SystemTypes.Reactor);

                if (ship.Systems[SystemTypes.LifeSupp].TryCast(out activatable) && activatable.IsActive)
                    FixOxygen();

                if (ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>(out var lights) && lights.IsActive)
                    FixLights(lights);

                break;
            }
            case 2:
            {
                if (ship.Systems[SystemTypes.Comms].TryCast<IActivatable>(out var activatable) && activatable.IsActive)
                    FixComms();

                if (ship.Systems[SystemTypes.Laboratory].TryCast(out activatable) && activatable.IsActive)
                    FixReactor(SystemTypes.Laboratory);

                if (ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>(out var lights) && lights.IsActive)
                    FixLights(lights);

                break;
            }
            case 0 or 3:
            {
                if (ship.Systems[SystemTypes.Comms].TryCast<IActivatable>(out var activatable) && activatable.IsActive)
                    FixComms();

                if (ship.Systems[SystemTypes.Reactor].TryCast(out activatable) && activatable.IsActive)
                    FixReactor(SystemTypes.Reactor);

                if (ship.Systems[SystemTypes.LifeSupp].TryCast(out activatable) && activatable.IsActive)
                    FixOxygen();

                if (ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>(out var lights) && lights.IsActive)
                    FixLights(lights);

                break;
            }
            case 4:
            {
                if (ship.Systems[SystemTypes.Comms].TryCast<IActivatable>(out var activatable) && activatable.IsActive)
                    FixComms();

                if (ship.Systems[SystemTypes.HeliSabotage].TryCast(out activatable) && activatable.IsActive)
                    FixHeli();

                if (ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>(out var lights) && lights.IsActive)
                    FixLights(lights);

                break;
            }
            case 5:
            {
                if (ship.Systems[SystemTypes.Comms].TryCast<IActivatable>(out var activatable) && activatable.IsActive)
                    FixComms();

                if (ship.Systems[SystemTypes.Reactor].TryCast(out activatable) && activatable.IsActive)
                    FixReactor(SystemTypes.Reactor);

                if (ship.Systems[SystemTypes.MushroomMixupSabotage].TryCast<MushroomMixupSabotageSystem>(out var mixup) && mixup.IsActive)
                    FixMixup(mixup);

                break;
            }
            case 6:
            {
                if (!SubLoaded)
                    break;

                if (ship.Systems[SystemTypes.Comms].TryCast<IActivatable>(out var activatable) && activatable.IsActive)
                    FixComms();

                if (ship.Systems[SystemTypes.Reactor].TryCast(out activatable) && activatable.IsActive)
                    FixReactor(SystemTypes.Reactor);

                if (ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>(out var lights) && lights.IsActive)
                    FixLights(lights);

                if (HasTask(RetrieveOxygenMask))
                    FixSubOxygen();

                break;
            }
            case 7:
            {
                if (!LILoaded)
                    break;

                if (ship.Systems[SystemTypes.Comms].TryCast<IActivatable>(out var activatable) && activatable.IsActive)
                    FixComms();

                if (ship.Systems[SystemTypes.LifeSupp].TryCast(out activatable) && activatable.IsActive)
                    FixOxygen();

                if (ship.Systems[SystemTypes.Reactor].TryCast(out activatable) && activatable.IsActive)
                    FixReactor(SystemTypes.Reactor);

                if (ship.Systems[SystemTypes.Laboratory].TryCast(out activatable) && activatable.IsActive)
                    FixReactor(SystemTypes.Laboratory);

                if (ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>(out var lights) && lights.IsActive)
                    FixLights(lights);

                if (ship.Systems[SystemTypes.MushroomMixupSabotage].TryCast<MushroomMixupSabotageSystem>(out var mixup) && mixup.IsActive)
                    FixMixup(mixup);

                break;
            }
        }
    }
}