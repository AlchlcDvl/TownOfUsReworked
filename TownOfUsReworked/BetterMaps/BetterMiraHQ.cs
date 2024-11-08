namespace TownOfUsReworked.BetterMaps;

[HeaderOption(MultiMenu.Main)]
public static class BetterMiraHQ
{
    [ToggleOption(MultiMenu.Main)]
    public static bool EnableBetterMiraHQ { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool MiraHQVentImprovements { get; set; } = false;

    [NumberOption(MultiMenu.Main, 30f, 90f, 5f, Format.Time)]
    public static Number MiraReactorTimer { get; set; } = new(60);

    [NumberOption(MultiMenu.Main, 30f, 90f, 5f, Format.Time)]
    public static Number MiraO2Timer { get; set; } = new(60);

    private static readonly Vector3 CommsPos = new(14.5f, 3.1f, 2f);

    private static bool IsAdjustmentsDone;
    private static bool IsVentsFetched;
    private static bool IsRoomsFetched;
    private static bool IsVentModified;

    private static Vent SpawnVent;
    private static Vent ReactorVent;
    private static Vent DeconVent;
    private static Vent LockerVent;
    private static Vent LabVent;
    private static Vent LightsVent;
    private static Vent AdminVent;
    private static Vent YRightVent;
    private static Vent O2Vent;
    private static Vent BalcVent;
    private static Vent MedicVent;
    private static Vent CommsVent;

    private static GameObject Comms;

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

        public static void Postfix(ShipStatus __instance)
        {
            if (!EnableBetterMiraHQ)
                return;

            if (!IsVentModified && __instance.Type == ShipStatus.MapType.Hq && CommsVent)
            {
                CommsVent.Id = GetAvailableId();
                IsVentModified = true;
                var vents = Ship().AllVents.ToList();
                vents.Add(CommsVent);
                Ship().AllVents = vents.ToArray();
            }
        }
    }

    private static void ApplyChanges(ShipStatus __instance)
    {
        if (!EnableBetterMiraHQ)
            return;

        if (__instance.Type == ShipStatus.MapType.Hq)
        {
            FindRooms();
            FindVents();
            AdjustMira();
        }
    }

    private static void AdjustMira()
    {
        if (IsVentsFetched && MiraHQVentImprovements && IsRoomsFetched)
            AdjustVents();

        IsAdjustmentsDone = true;
    }

    private static void FindVents()
    {
        var vents = AllVents();

        if (!SpawnVent)
            SpawnVent = vents.Find(vent => vent.name == "LaunchVent");

        if (!BalcVent)
            BalcVent = vents.Find(vent => vent.name == "BalconyVent");

        if (!ReactorVent)
            ReactorVent = vents.Find(vent => vent.name == "ReactorVent");

        if (!LabVent)
            LabVent = vents.Find(vent => vent.name == "LabVent");

        if (!LockerVent)
            LockerVent = vents.Find(vent => vent.name == "LockerVent");

        if (!AdminVent)
            AdminVent = vents.Find(vent => vent.name == "AdminVent");

        if (!LightsVent)
            LightsVent = vents.Find(vent => vent.name == "OfficeVent");

        if (!O2Vent)
            O2Vent = vents.Find(vent => vent.name == "AgriVent");

        if (!DeconVent)
            DeconVent = vents.Find(vent => vent.name == "DeconVent");

        if (!MedicVent)
            MedicVent = vents.Find(vent => vent.name == "MedVent");

        if (!YRightVent)
            YRightVent = vents.Find(vent => vent.name == "YHallRightVent");

        if (!CommsVent)
        {
            CommsVent = UObject.Instantiate(YRightVent, Comms.transform);
            CommsVent.Right = null;
            CommsVent.Left = null;
            CommsVent.Center = null;
            CommsVent.name = "CommsVent";
        }

        IsVentsFetched = SpawnVent && BalcVent && ReactorVent && LabVent && LockerVent && AdminVent && O2Vent && LightsVent && DeconVent && MedicVent && YRightVent && CommsVent;
    }

    private static void FindRooms()
    {
        if (!Comms)
            Comms = AllGameObjects().Find(o => o.name == "Comms");

        IsRoomsFetched = Comms;
    }

    private static void AdjustVents()
    {
        if (IsVentsFetched && IsRoomsFetched)
        {
            MoveCommsVent();
            ReconnectVents();
        }
    }

    private static void ReconnectVents()
    {
        O2Vent.Right = BalcVent;
        O2Vent.Left = MedicVent;
        O2Vent.Center = null;
        MedicVent.Center = O2Vent;
        MedicVent.Right = BalcVent;
        MedicVent.Left = null;
        BalcVent.Left = MedicVent;
        BalcVent.Center = O2Vent;
        BalcVent.Right = null;

        AdminVent.Center = YRightVent;
        AdminVent.Left = null;
        AdminVent.Right = null;
        YRightVent.Center = AdminVent;
        YRightVent.Left = null;
        YRightVent.Right = null;

        LabVent.Right = LightsVent;
        LabVent.Left = null;
        LabVent.Center = null;
        LightsVent.Left = LabVent;
        LightsVent.Right = null;
        LightsVent.Center = null;

        SpawnVent.Center = ReactorVent;
        SpawnVent.Right = null;
        SpawnVent.Left = null;
        ReactorVent.Left = SpawnVent;
        ReactorVent.Right = null;
        ReactorVent.Center = null;

        CommsVent.Left = LockerVent;
        CommsVent.Center = DeconVent;
        LockerVent.Right = CommsVent;
        LockerVent.Center = DeconVent;
        LockerVent.Left = null;
        DeconVent.Left = LockerVent;
        DeconVent.Right = CommsVent;
        DeconVent.Center = null;
    }

    private static void MoveCommsVent()
    {
        if (CommsVent.transform.position != CommsPos)
            CommsVent.transform.position = CommsPos;
    }
}