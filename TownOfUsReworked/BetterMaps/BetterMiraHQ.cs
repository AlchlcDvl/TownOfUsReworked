namespace TownOfUsReworked.BetterMaps;

/// <summary>
/// Provides enhanced functionality for the Mira HQ map.
/// </summary>
[HeaderOption(MultiMenu.Main)]
public static class BetterMiraHq
{
    /// <summary>
    /// Enables or disables all BetterMiraHq modifications.
    /// </summary>
    [ToggleOption]
    public static bool EnableBetterMiraHq = true;

    /// <summary>
    /// Enables improved vent connections and positions.
    /// </summary>
    [ToggleOption]
    private static bool MiraHqVentImprovements = false;

    /// <summary>
    /// Time until reactor meltdown occurs during sabotage.<br></br>
    /// Default: <c>60</c>s<br></br>
    /// Range: <c>30</c> to <c>90</c>s<br></br>
    /// Increment: <c>5</c>s
    /// </summary>
    [NumberOption(30f, 90f, 5f, Format.Time)]
    public static Number MiraReactorTimer = 60;

    /// <summary>
    /// Time until oxygen depletion occurs during sabotage.<br></br>
    /// Default: <c>60</c>s<br></br>
    /// Range: <c>30</c> to <c>90</c>s<br></br>
    /// Increment: <c>5</c>s
    /// </summary>
    [NumberOption(30f, 90f, 5f, Format.Time)]
    public static Number MiraO2Timer = 60;

    // Position for the Communications room vent
    private static readonly Vector3 CommsPos = new(14.5f, 3.1f, 2f);

    // Vent references for an improved connection system
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

    // Reference to Communications room
    private static GameObject Comms;

    /// <summary>
    /// Applies all Mira HQ map modifications if enabled.
    /// </summary>
    public static void ApplyChanges()
    {
        if (!EnableBetterMiraHq)
            return;

        FindMiraObjects();
        AdjustMiraHq();
    }

    /// <summary>
    /// Initializes references to map objects and vents.
    /// </summary>
    private static void FindMiraObjects()
    {
        FindRooms();
        FindVents();
    }

    /// <summary>
    /// Locates and caches references to all vents.<br></br>
    /// Creates new Communications vent if not present.
    /// </summary>
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

        if (CommsVent)
            return;

        CommsVent = UObject.Instantiate(YRightVent, Comms.transform);
        CommsVent.Right = null;
        CommsVent.Left = null;
        CommsVent.Center = null;
        CommsVent.name = "CommsVent";
    }

    /// <summary>
    /// Finds and caches reference to Communications room.
    /// </summary>
    private static void FindRooms()
    {
        if (!Comms)
            Comms = AllGameObjects().Find(o => o.name == "Comms");
    }

    /// <summary>
    /// Updates vent positions and connections.
    /// </summary>
    private static void AdjustMiraHq()
    {
        if (!MiraHqVentImprovements || !CommsVent)
            return;

        MoveCommsVent();
        ReconnectVents();
        CommsVent.Id = GetAvailableId();
        var vents = Ship().AllVents.ToList();
        vents.Add(CommsVent);
        Ship().AllVents = vents.ToArray();
    }

    /// <summary>
    /// Creates new vent connection paths between rooms.
    /// </summary>
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

    /// <summary>
    /// Repositions the Communications vent to a specified location.
    /// </summary>
    private static void MoveCommsVent()
    {
        if (CommsVent.transform.position != CommsPos)
            CommsVent.transform.position = CommsPos;
    }
}