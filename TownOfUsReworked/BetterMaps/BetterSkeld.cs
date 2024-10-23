namespace TownOfUsReworked.BetterMaps;

[HeaderOption(MultiMenu.Main)]
public static class BetterSkeld
{
    [ToggleOption(MultiMenu.Main)]
    public static bool EnableBetterSkeld { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool SkeldVentImprovements { get; set; } = false;

    [NumberOption(MultiMenu.Main, 30f, 90f, 5f, Format.Time)]
    public static Number SkeldReactorTimer { get; set; } = new(60);

    [NumberOption(MultiMenu.Main, 30f, 90f, 5f, Format.Time)]
    public static Number SkeldO2Timer { get; set; } = new(60);

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

    private static void ApplyChanges(ShipStatus __instance)
    {
        if (!EnableBetterSkeld)
            return;

        if (__instance.Type == ShipStatus.MapType.Ship && MapPatches.CurrentMap != 3)
        {
            FindVents();
            AdjustSkeld();
        }
    }

    private static void AdjustSkeld()
    {
        if (IsVentsFetched && SkeldVentImprovements)
            AdjustVents();

        IsAdjustmentsDone = true;
    }

    private static void FindVents()
    {
        var vents = AllVents();

        if (!NavVentSouth)
            NavVentSouth = vents.Find(vent => vent.name == "NavVentSouth");

        if (!NavVentNorth)
            NavVentNorth = vents.Find(vent => vent.name == "NavVentNorth");

        if (!ShieldsVent)
            ShieldsVent = vents.Find(vent => vent.name == "ShieldsVent");

        if (!WeaponsVent)
            WeaponsVent = vents.Find(vent => vent.name == "WeaponsVent");

        if (!REngineVent)
            REngineVent = vents.Find(vent => vent.name == "REngineVent");

        if (!UpperReactorVent)
            UpperReactorVent = vents.Find(vent => vent.name == "UpperReactorVent");

        if (!LEngineVent)
            LEngineVent = vents.Find(vent => vent.name == "LEngineVent");

        if (!ReactorVent)
            ReactorVent = vents.Find(vent => vent.name == "ReactorVent");

        if (!BigYVent)
            BigYVent = vents.Find(vent => vent.name == "BigYVent");

        if (!CafeVent)
            CafeVent = vents.Find(vent => vent.name == "CafeVent");

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