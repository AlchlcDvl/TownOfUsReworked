namespace TownOfUsReworked.BetterMaps;

public static class MiraShipStatusPatch
{
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
            if (!IsVentModified && __instance.Type == ShipStatus.MapType.Hq)
            {
                CommsVent.Id = GetAvailableId();
                IsVentModified = true;
            }
        }
    }

    private static void ApplyChanges(ShipStatus __instance)
    {
        if (__instance.Type == ShipStatus.MapType.Hq)
        {
            FindRooms();
            FindVents();
            AdjustMira();
        }
    }

    private static void AdjustMira()
    {
        if (IsVentsFetched && CustomGameOptions.MiraHQVentImprovements && IsRoomsFetched)
            AdjustVents();

        IsAdjustmentsDone = true;
    }

    private static void FindVents()
    {
        if (SpawnVent == null)
            SpawnVent = AllVents.Find(vent => vent.gameObject.name == "LaunchVent");

        if (BalcVent == null)
            BalcVent = AllVents.Find(vent => vent.gameObject.name == "BalconyVent");

        if (ReactorVent == null)
            ReactorVent = AllVents.Find(vent => vent.gameObject.name == "ReactorVent");

        if (LabVent == null)
            LabVent = AllVents.Find(vent => vent.gameObject.name == "LabVent");

        if (LockerVent == null)
            LockerVent = AllVents.Find(vent => vent.gameObject.name == "LockerVent");

        if (AdminVent == null)
            AdminVent = AllVents.Find(vent => vent.gameObject.name == "AdminVent");

        if (LightsVent == null)
            LightsVent = AllVents.Find(vent => vent.gameObject.name == "OfficeVent");

        if (O2Vent == null)
            O2Vent = AllVents.Find(vent => vent.gameObject.name == "AgriVent");

        if (DeconVent == null)
            DeconVent = AllVents.Find(vent => vent.gameObject.name == "DeconVent");

        if (MedicVent == null)
            MedicVent = AllVents.Find(vent => vent.gameObject.name == "MedVent");

        if (YRightVent == null)
            YRightVent = AllVents.Find(vent => vent.gameObject.name == "YHallRightVent");

        if (CommsVent == null)
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
        if (Comms == null)
            Comms = AllObjects.Find(o => o.name == "Comms");

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

[HarmonyPatch(typeof(ReactorSystemType), nameof(ReactorSystemType.RepairDamage))]
public static class ReactorMira
{
    public static bool Prefix(ReactorSystemType __instance, ref byte opCode)
    {
        if (ShipStatus.Instance.Type == ShipStatus.MapType.Hq && opCode == 128 && !__instance.IsActive)
        {
            __instance.Countdown = CustomGameOptions.MiraReactorTimer;
            __instance.ReactorDuration = CustomGameOptions.MiraReactorTimer;
            __instance.UserConsolePairs.Clear();
            __instance.IsDirty = true;
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(LifeSuppSystemType), nameof(LifeSuppSystemType.RepairDamage))]
public static class O2Mira
{
    public static bool Prefix(LifeSuppSystemType __instance, ref byte opCode)
    {
        if (ShipStatus.Instance.Type == ShipStatus.MapType.Hq && opCode == 128 && !__instance.IsActive)
        {
            __instance.Countdown = CustomGameOptions.MiraO2Timer;
            __instance.LifeSuppDuration = CustomGameOptions.MiraO2Timer;
            __instance.CompletedConsoles.Clear();
            __instance.IsDirty = true;
            return false;
        }

        return true;
    }
}