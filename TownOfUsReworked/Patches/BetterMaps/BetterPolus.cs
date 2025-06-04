namespace TownOfUsReworked.Patches.BetterMaps;

/// <summary>
/// Provides enhanced functionality and customization options for the Polus map.<br/>
/// Modifies vent connections, task locations, and sabotage timers.
/// </summary>
[HeaderOption(MultiMenu.Main), Sorted(0)]
public static class BetterPolus
{
    /// <summary>
    /// Enables or disables all BetterPolus modifications.
    /// </summary>
    [ToggleOption]
    public static bool EnableBetterPolus = true;

    /// <summary>
    /// Enables improved vent connections and positions.
    /// </summary>
    [ToggleOption]
    private static bool PolusVentImprovements = false;

    /// <summary>
    /// Controls the Temperature task location.<br/>
    /// Options: <c>DontMove</c>, <c>SwappedWithVitals</c>, <c>DeathValley</c>
    /// </summary>
    [StringOption<TempLocation>]
    private static TempLocation TempLocation = TempLocation.DontMove;

    /// <summary>
    /// When enabled, swaps Wi-Fi and Chart Course task locations.
    /// </summary>
    [ToggleOption]
    private static bool WifiChartCourseSwap = false;

    /// <summary>
    /// Time until seismic sabotage completes.
    /// </summary>
    /// <remarks>
    /// Default: <c>60</c>s<br/>
    /// Range: <c>30</c> to <c>90</c>s<br/>
    /// Increment: <c>5</c>s
    /// </remarks>
    [NumberOption(30f, 90f, 5f, Format.Time)]
    public static Number SeismicTimer = 60;

    // New positions for relocated objects
    private static readonly Vector3 DvdScreenNewPos = new(26.635f, -15.92f, 1f);
    private static readonly Vector3 VitalsNewPos = new(31.275f, -6.45f, 1f);
    private static readonly Vector3 WifiNewPos = new(15.975f, 0.084f, 1f);
    private static readonly Vector3 NavNewPos = new(11.07f, -15.298f, -0.015f);
    private static readonly Vector3 TempColdNewPos = new(25.4f, -6.4f, 1f);
    private static readonly Vector3 TempColdNewPosDv = new(7.772f, -17.103f, -0.017f);
    private static readonly Vector3 SpeciVentPos = new(36.5f, -22f, 0f);

    // Task console references
    private static Console WifiConsole;
    private static Console NavConsole;
    private static Console TempCold;

    // Vent references
    private static SystemConsole Vitals;
    private static GameObject DvdScreenOffice;

    // Vent references
    private static Vent ElectricBuildingVent;
    private static Vent ElectricalVent;
    private static Vent ScienceBuildingVent;
    private static Vent StorageVent;
    private static Vent LightCageVent;
    private static Vent AdminVent;
    private static Vent SpeciVent;
    private static Vent BathroomVent;

    // Room references
    private static GameObject Comms;
    private static GameObject DropShip;
    private static GameObject Outside;
    private static GameObject Science;
    private static GameObject Specimen;
    private static GameObject Office;

    /// <summary>
    /// Applies all Polus map modifications if enabled.
    /// </summary>
    public static void ApplyChanges()
    {
        if (!EnableBetterPolus)
            return;

        FindPolusObjects();
        AdjustPolus();
    }

    /// <summary>
    /// Updates task locations and vent connections based on settings.
    /// </summary>
    private static void AdjustPolus()
    {
        switch (TempLocation)
        {
            case TempLocation.SwappedWithVitals:
            {
                MoveVitals();
                MoveTempCold();
                break;
            }
            case TempLocation.DeathValley:
            {
                MoveTempColdDv();
                break;
            }
        }

        if (WifiChartCourseSwap)
            SwitchNavWifi();

        AdjustVents();
    }

    /// <summary>
    /// Finds and caches references to map objects.
    /// </summary>
    private static void FindPolusObjects()
    {
        FindRooms();
        FindVents();
        FindObjects();
    }

    /// <summary>
    /// Locates and initializes all vent objects.
    /// </summary>
    private static void FindVents()
    {
        var vents = AllVents();

        if (!ElectricBuildingVent)
            ElectricBuildingVent = vents.Find(vent => vent.name == "ElectricBuildingVent");

        if (!ElectricalVent)
            ElectricalVent = vents.Find(vent => vent.name == "ElectricalVent");

        if (!ScienceBuildingVent)
            ScienceBuildingVent = vents.Find(vent => vent.name == "ScienceBuildingVent");

        if (!StorageVent)
            StorageVent = vents.Find(vent => vent.name == "StorageVent");

        if (!LightCageVent)
            LightCageVent = vents.Find(vent => vent.name == "ElecFenceVent");

        if (!AdminVent)
            AdminVent = vents.Find(vent => vent.name == "AdminVent");

        if (!BathroomVent)
            BathroomVent = vents.Find(vent => vent.name == "BathroomVent");

        if (SpeciVent)
            return;

        SpeciVent = UObject.Instantiate(AdminVent, Specimen.transform);
        SpeciVent.Right = null;
        SpeciVent.Left = null;
        SpeciVent.Center = null;
        SpeciVent.name = "SpeciVent";
    }

    /// <summary>
    /// Finds and caches room GameObjects.
    /// </summary>
    private static void FindRooms()
    {
        var gos = AllGameObjects();

        if (!Comms)
            Comms = gos.Find(o => o.name == "Comms");

        if (!DropShip)
            DropShip = gos.Find(o => o.name == "Dropship");

        if (!Outside)
            Outside = gos.Find(o => o.name == "Outside");

        if (!Science)
            Science = gos.Find(o => o.name == "Science");

        if (!Specimen)
            Specimen = gos.Find(o => o.name == "RightPod");

        if (!Office)
            Office = gos.Find(o => o.name == "Office");
    }

    /// <summary>
    /// Locates task consoles and UI elements.
    /// </summary>
    private static void FindObjects()
    {
        var consoles = AllConsoles();

        if (!WifiConsole)
            WifiConsole = consoles.Find(console => console.name == "panel_wifi");

        if (!NavConsole)
            NavConsole = consoles.Find(console => console.name == "panel_nav");

        if (!TempCold)
            TempCold = consoles.Find(console => console.name == "panel_tempcold");

        if (!Vitals)
            Vitals = AllSystemConsoles().Find(console => console.name == "panel_vitals");

        if (DvdScreenOffice)
            return;

        var dvdScreenAdmin = AllGameObjects().Find(o => o.name == "dvdscreen");

        if (!dvdScreenAdmin)
            return;

        DvdScreenOffice = UObject.Instantiate(dvdScreenAdmin, Office.transform);
        DvdScreenOffice.name = "dvdscreen_office";
        DvdScreenOffice.SetActive(false);
    }

    /// <summary>
    /// Updates vent positions and connections if enabled.
    /// </summary>
    private static void AdjustVents()
    {
        if (!PolusVentImprovements)
            return;

        MoveSpeciVent();
        ReconnectVents();

        if (!SpeciVent)
            return;

        SpeciVent.Id = GetAvailableVentId();
        var vents = Ship().AllVents.ToList();
        vents.Add(SpeciVent);
        Ship().AllVents = vents.ToArray();
    }

    /// <summary>
    /// Creates new vent connection paths between rooms.
    /// </summary>
    private static void ReconnectVents()
    {
        ElectricBuildingVent.Left = ElectricalVent;
        ElectricalVent.Center = ElectricBuildingVent;
        ElectricBuildingVent.Center = LightCageVent;
        LightCageVent.Center = ElectricBuildingVent;
        ScienceBuildingVent.Left = StorageVent;
        StorageVent.Center = ScienceBuildingVent;
        AdminVent.Center = SpeciVent;
        SpeciVent.Left = AdminVent;
        SpeciVent.Center = BathroomVent;
        BathroomVent.Left = SpeciVent;
    }

    private static void MoveSpeciVent()
    {
        if (SpeciVent.transform.position != SpeciVentPos)
            SpeciVent.transform.position = SpeciVentPos;
    }

    private static void MoveTempCold()
    {
        if (TempCold.transform.position == TempColdNewPos)
            return;

        var tempColdTransform = TempCold.transform;
        tempColdTransform.SetParent(Outside.transform);
        tempColdTransform.position = TempColdNewPos;
        var collider = TempCold.GetComponent<BoxCollider2D>();
        collider.isTrigger = false;
        collider.size += new Vector2(0f, -0.3f);
    }

    private static void MoveTempColdDv()
    {
        if (TempCold.transform.position == TempColdNewPosDv)
            return;

        var tempColdTransform = TempCold.transform;
        tempColdTransform.SetParent(Outside.transform);
        tempColdTransform.position = TempColdNewPosDv;
        var collider = TempCold.GetComponent<BoxCollider2D>();
        collider.isTrigger = false;
        collider.size += new Vector2(0f, -0.3f);
    }

    private static void SwitchNavWifi()
    {
        if (WifiConsole.transform.position != WifiNewPos)
        {
            var wifiTransform = WifiConsole.transform;
            wifiTransform.SetParent(DropShip.transform);
            wifiTransform.position = WifiNewPos;
        }

        if (NavConsole.transform.position == NavNewPos)
            return;

        var navTransform = NavConsole.transform;
        navTransform.SetParent(NavConsole.transform);
        navTransform.position = NavNewPos;
        NavConsole.checkWalls = true;
    }

    private static void MoveVitals()
    {
        if (Vitals.transform.position != VitalsNewPos)
        {
            var vitalsTransform = Vitals.transform;
            vitalsTransform.SetParent(Science.transform);
            vitalsTransform.position = VitalsNewPos;
        }

        if (DvdScreenOffice.transform.position == DvdScreenNewPos)
            return;

        var dvdScreenTransform = DvdScreenOffice.transform;
        dvdScreenTransform.position = DvdScreenNewPos;
        var localScale = dvdScreenTransform.localScale;
        localScale = new(0.75f, localScale.y, localScale.z);
        dvdScreenTransform.localScale = localScale;
        DvdScreenOffice.SetActive(true);
    }

    [HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.AppendTaskText))]
    public static class NormalPlayerTaskPatches
    {
        public static bool Prefix(NormalPlayerTask __instance, IStringBuilder sb)
        {
            // Skip if BetterPolus is disabled, ship not loaded, wrong map, or not a modified task
            // IDs: Temperature (42), WiFi (41), Chart Course (3)
            if (!EnableBetterPolus || !Ship() || MapPatches.CurrentMap != 2 || (int)__instance.TaskType is not (42 or 41 or 3))
                return true;

            var flag = __instance.ShouldYellowText(); // Check if the task text should be colored

            // Add color tags based on task completion
            if (flag)
                sb.Append(__instance.IsComplete ? "<#00DD00FF>" : "<#FFFF00FF>");

            // Determine the correct room name based on task type and settings
            SystemTypes room;

            if (__instance.TaskType == TaskTypes.RecordTemperature && __instance.StartAt != SystemTypes.Outside)
            {
                room = TempLocation switch
                {
                    TempLocation.DeathValley => SystemTypes.Outside,
                    TempLocation.SwappedWithVitals => SystemTypes.Office,
                    _ => __instance.StartAt
                };
            }
            else
            {
                room = __instance.TaskType switch
                {
                    TaskTypes.RebootWifi => SystemTypes.Dropship,
                    TaskTypes.ChartCourse => SystemTypes.Comms,
                    _ => __instance.StartAt
                };
            }

            // Build task text with translated room and task names
            sb.Append($"{TranslationController.Instance.GetString(room)}: {TranslationController.Instance.GetString(__instance.TaskType)}");

            // Add a timer or step counter if needed
            if (__instance is { ShowTaskTimer: true, TimerStarted: NormalPlayerTask.TimerState.Started })
                sb.Append($" ({TranslationController.Instance.GetString(StringNames.SecondsAbbv, (int)__instance.TaskTimer)})");
            else if (__instance.ShowTaskStep)
                sb.Append($" ({__instance.taskStep}/{__instance.MaxStep})");

            // Close color tag if used
            if (flag)
                sb.Append("</color>");

            sb.AppendLine();
            return false;
        }
    }
}