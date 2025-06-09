namespace TownOfUsReworked.Utils;

// FIXME: This does not work...at all
public static class FixUtils
{
    private static void FixComms() => Ship().RpcUpdateSystem(SystemTypes.Comms, 0);

    private static void FixMiraComms()
    {
        var ship = Ship();
        ship.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
        ship.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
    }

    // private static void FixHeli()
    // {
    //     var ship = Ship();
    //     ship.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 0);
    //     ship.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 1);
    // }

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
        FixCritSabotages();
        FixNonCritSabotages();
    }

    public static void FixCritSabotages()
    {
        var ship = Ship();

        if (ship.Systems is null)
            return;

        if (ship.Systems[SystemTypes.Sabotage].TryCast<SabotageSystemType>(out var system) || !system.AnyActive)
            return;

        switch (MapPatches.CurrentMap)
        {
            case >= 1 and <= 5:
            {
                ship.RepairCriticalSabotages();
                break;
            }
            case 6 when SubLoaded:
            {
                if (ship.Systems[SystemTypes.Reactor].TryCast<IActivatable>(out var activatable) && activatable.IsActive)
                    FixReactor(SystemTypes.Reactor);

                if (HasTask(RetrieveOxygenMask))
                    FixSubOxygen();

                break;
            }
            case 7 when LiLoaded:
            {
                if (ship.Systems[SystemTypes.LifeSupp].TryCast<IActivatable>(out var activatable) && activatable.IsActive)
                    FixOxygen();

                if (ship.Systems[SystemTypes.Reactor].TryCast(out activatable) && activatable.IsActive)
                    FixReactor(SystemTypes.Reactor);

                break;
            }
        }
    }

    private static void FixNonCritSabotages()
    {
        var ship = Ship();

        if (ship.Systems is null)
            return;

        if (ship.Systems[SystemTypes.Sabotage].TryCast<SabotageSystemType>(out var system) || !system.AnyActive)
            return;

        switch (MapPatches.CurrentMap)
        {
            case 1:
            {
                if (ship.Systems[SystemTypes.Comms].TryCast<IActivatable>(out var activatable) && activatable.IsActive)
                    FixMiraComms();

                if (ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>(out var lights) && lights.IsActive)
                    FixLights(lights);

                break;
            }
            case 2 or 0 or 3 or 4:
            case 6 when SubLoaded:
            case 7 when LiLoaded:
            {
                FixCommsAndLights();
                break;
            }
            case 5:
            {
                if (ship.Systems[SystemTypes.Comms].TryCast<IActivatable>(out var activatable) && activatable.IsActive)
                    FixComms();

                if (ship.Systems[SystemTypes.MushroomMixupSabotage].TryCast<MushroomMixupSabotageSystem>(out var mixup) && mixup.IsActive)
                    FixMixup(mixup);

                break;
            }
        }
    }

    private static void FixCommsAndLights()
    {
        var ship = Ship();

        if (ship?.Systems is null)
            return;

        if (!ship.Systems[SystemTypes.Sabotage].TryCast<SabotageSystemType>(out var system) || !system.AnyActive)
            return;

        if (ship.Systems[SystemTypes.Comms].TryCast<IActivatable>(out var activatable) && activatable.IsActive)
            FixComms();

        if (ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>(out var lights) && lights.IsActive)
            FixLights(lights);
    }
}