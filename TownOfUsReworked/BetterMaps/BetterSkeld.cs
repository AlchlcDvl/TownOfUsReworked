namespace TownOfUsReworked.BetterMaps;

public static class SkeldShipStatusPatch
{
    private static readonly Vector3 ReactorVentNewPos = new(-2.95f, -10.95f, 2f);
    private static readonly Vector3 ShieldsVentNewPos = new(2f, -15f, 2f);
    private static readonly Vector3 BigYVentNewPos = new(5.2f, -4.85f, 2f);
    private static readonly Vector3 NavVentNorthNewPos = new(-11.85f, -11.55f, 2f);
    private static readonly Vector3 CafeVentNewPos = new(-3.9f, 5.5f, 2f);

    private static bool IsAdjustmentsDone;
    private static bool IsVentsFetched;

    private static Vent NavVentSouth;
    private static Vent NavVentNorth;
    private static Vent ShieldsVent;
    private static Vent WeaponsVent;
    private static Vent REngineVent;
    private static Vent UpperReactorVent;
    private static Vent LEngineVent;
    private static Vent ReactorVent;
    private static Vent BigYVent;
    private static Vent CafeVent;

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static class ShipStatusBeginPatch
    {
        public static void Prefix(ShipStatus __instance) => ApplyChanges(__instance);
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    public static class ShipStatusAwakePatch
    {
        public static void Prefix(ShipStatus __instance) => ApplyChanges(__instance);
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
    public static class ShipStatusFixedUpdatePatch
    {
        public static void Prefix(ShipStatus __instance)
        {
            if (!IsAdjustmentsDone || !IsVentsFetched)
                ApplyChanges(__instance);
        }
    }

    private static void ApplyChanges(ShipStatus instance)
    {
        if (instance.Type == ShipStatus.MapType.Ship)
        {
            FindVents();
            AdjustSkeld();
        }
    }

    private static void AdjustSkeld()
    {
        if (IsVentsFetched && CustomGameOptions.SkeldVentImprovements)
            AdjustVents();

        IsAdjustmentsDone = true;
    }

    private static void FindVents()
    {
        if (NavVentSouth == null)
            NavVentSouth = AllVents.Find(vent => vent.gameObject.name == "NavVentSouth");

        if (NavVentNorth == null)
            NavVentNorth = AllVents.Find(vent => vent.gameObject.name == "NavVentNorth");

        if (ShieldsVent == null)
            ShieldsVent = AllVents.Find(vent => vent.gameObject.name == "ShieldsVent");

        if (WeaponsVent == null)
            WeaponsVent = AllVents.Find(vent => vent.gameObject.name == "WeaponsVent");

        if (REngineVent == null)
            REngineVent = AllVents.Find(vent => vent.gameObject.name == "REngineVent");

        if (UpperReactorVent == null)
            UpperReactorVent = AllVents.Find(vent => vent.gameObject.name == "UpperReactorVent");

        if (LEngineVent == null)
            LEngineVent = AllVents.Find(vent => vent.gameObject.name == "LEngineVent");

        if (ReactorVent == null)
            ReactorVent = AllVents.Find(vent => vent.gameObject.name == "ReactorVent");

        if (BigYVent == null)
            BigYVent = AllVents.Find(vent => vent.gameObject.name == "BigYVent");

        if (CafeVent == null)
            CafeVent = AllVents.Find(vent => vent.gameObject.name == "CafeVent");

        IsVentsFetched = NavVentSouth && NavVentNorth && ShieldsVent && WeaponsVent && REngineVent && UpperReactorVent && LEngineVent && ReactorVent && BigYVent && CafeVent;
    }

    private static void AdjustVents()
    {
        if (IsVentsFetched)
        {
            MoveVents();
            ReconnectVents();
        }
    }

    private static void MoveVents()
    {
        MoveReactorVent();
        MoveShieldsVent();
        MoveBigYVent();
        MoveNavVentNorth();
        MoveCafeVent();
    }

    private static void ReconnectVents()
    {
        WeaponsVent.Right = NavVentNorth;
        WeaponsVent.Left = NavVentSouth;
        NavVentNorth.Right = ShieldsVent;
        NavVentNorth.Left = WeaponsVent;
        NavVentSouth.Right = ShieldsVent;
        NavVentSouth.Left = WeaponsVent;
        ShieldsVent.Right = NavVentNorth;
        ShieldsVent.Left = NavVentSouth;
        LEngineVent.Right = ReactorVent;
        LEngineVent.Left = UpperReactorVent;
        UpperReactorVent.Right = LEngineVent;
        UpperReactorVent.Left = REngineVent;
        ReactorVent.Right = LEngineVent;
        ReactorVent.Left = REngineVent;
        REngineVent.Right = ReactorVent;
        REngineVent.Left = UpperReactorVent;
    }

    private static void MoveReactorVent()
    {
        if (ReactorVent.transform.position != ReactorVentNewPos)
            ReactorVent.transform.position = ReactorVentNewPos;
    }

    private static void MoveShieldsVent()
    {
        if (ShieldsVent.transform.position != ShieldsVentNewPos)
            ShieldsVent.transform.position = ShieldsVentNewPos;
    }

    private static void MoveBigYVent()
    {
        if (BigYVent.transform.position != BigYVentNewPos)
            BigYVent.transform.position = BigYVentNewPos;
    }

    private static void MoveNavVentNorth()
    {
        if (NavVentNorth.transform.position != NavVentNorthNewPos)
            NavVentNorth.transform.position = NavVentNorthNewPos;
    }

    private static void MoveCafeVent()
    {
        if (CafeVent.transform.position != CafeVentNewPos)
            CafeVent.transform.position = CafeVentNewPos;
    }
}

[HarmonyPatch(typeof(ReactorSystemType), nameof(ReactorSystemType.RepairDamage))]
public static class ReactorSkeld
{
    public static bool Prefix(ReactorSystemType __instance, ref byte opCode)
    {
        if (ShipStatus.Instance.Type == ShipStatus.MapType.Ship && opCode == 128 && !__instance.IsActive)
        {
            __instance.Countdown = CustomGameOptions.SkeldReactorTimer;
            __instance.ReactorDuration = CustomGameOptions.SkeldReactorTimer;
            __instance.UserConsolePairs.Clear();
            __instance.IsDirty = true;
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(LifeSuppSystemType), nameof(LifeSuppSystemType.RepairDamage))]
public static class O2Skeld
{
    public static bool Prefix(LifeSuppSystemType __instance, ref byte opCode)
    {
        if (ShipStatus.Instance.Type == ShipStatus.MapType.Ship && opCode == 128 && !__instance.IsActive)
        {
            __instance.Countdown = CustomGameOptions.SkeldO2Timer;
            __instance.LifeSuppDuration = CustomGameOptions.SkeldO2Timer;
            __instance.CompletedConsoles.Clear();
            __instance.IsDirty = true;
            return false;
        }

        return true;
    }
}