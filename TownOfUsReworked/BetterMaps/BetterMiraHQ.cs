namespace TownOfUsReworked.BetterMaps;

[HeaderOption(MultiMenu.Main)]
public static class BetterMiraHQ
{
    [ToggleOption]
    public static bool EnableBetterMiraHQ { get; set; } = true;

    [ToggleOption]
    public static bool MiraHQVentImprovements { get; set; } = false;

    [NumberOption(30f, 90f, 5f, Format.Time)]
    public static Number MiraReactorTimer { get; set; } = 60;

    [NumberOption(30f, 90f, 5f, Format.Time)]
    public static Number MiraO2Timer { get; set; } = 60;

    private static readonly Vector3 CommsPos = new(14.5f, 3.1f, 2f);

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

    public static void ApplyChanges()
    {
        if (EnableBetterMiraHQ)
        {
            FindMiraObjects();
            AdjustMiraHQ();
        }
    }

    private static void FindMiraObjects()
    {
        FindRooms();
        FindVents();
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
    }

    private static void FindRooms()
    {
        if (!Comms)
            Comms = AllGameObjects().Find(o => o.name == "Comms");
    }

    private static void AdjustMiraHQ()
    {
        if (MiraHQVentImprovements)
        {
            MoveCommsVent();
            ReconnectVents();

            if (CommsVent)
            {
                CommsVent.Id = GetAvailableId();
                var vents = Ship().AllVents.ToList();
                vents.Add(CommsVent);
                Ship().AllVents = vents.ToArray();
            }
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