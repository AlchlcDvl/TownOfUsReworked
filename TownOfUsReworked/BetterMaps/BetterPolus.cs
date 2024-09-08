namespace TownOfUsReworked.BetterMaps;

[HeaderOption(MultiMenu.Main)]
public static class BetterPolus
{
    [ToggleOption(MultiMenu.Main)]
    public static bool EnableBetterPolus { get; set; } = true;

    [ToggleOption(MultiMenu.Main)]
    public static bool PolusVentImprovements { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool VitalsLab { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool ColdTempDeathValley { get; set; } = false;

    [ToggleOption(MultiMenu.Main)]
    public static bool WifiChartCourseSwap { get; set; } = false;

    [NumberOption(MultiMenu.Main, 30f, 90f, 5f, Format.Time)]
    public static float SeismicTimer { get; set; } = 60f;

    private static readonly Vector3 DvdScreenNewPos = new(26.635f, -15.92f, 1f);
    private static readonly Vector3 VitalsNewPos = new(31.275f, -6.45f, 1f);
    private static readonly Vector3 WifiNewPos = new(15.975f, 0.084f, 1f);
    private static readonly Vector3 NavNewPos = new(11.07f, -15.298f, -0.015f);
    private static readonly Vector3 TempColdNewPos = new(25.4f, -6.4f, 1f);
    private static readonly Vector3 TempColdNewPosDV = new(7.772f, -17.103f, -0.017f);
    private static readonly Vector3 SpeciVentPos = new(36.5f, -22f, 0f);

    private static bool IsAdjustmentsDone;
    private static bool IsObjectsFetched;
    private static bool IsRoomsFetched;
    private static bool IsVentsFetched;
    private static bool IsVentModified;

    private static Console WifiConsole;
    private static Console NavConsole;

    private static SystemConsole Vitals;
    private static GameObject DvdScreenOffice;

    private static Vent ElectricBuildingVent;
    private static Vent ElectricalVent;
    private static Vent ScienceBuildingVent;
    private static Vent StorageVent;
    private static Vent LightCageVent;
    private static Vent AdminVent;
    private static Vent SpeciVent;
    private static Vent BathroomVent;

    private static Console TempCold;

    private static GameObject Comms;
    private static GameObject DropShip;
    private static GameObject Outside;
    private static GameObject Science;
    private static GameObject Specimen;
    private static GameObject Office;

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
            if (!IsObjectsFetched || !IsAdjustmentsDone)
                ApplyChanges(__instance);
        }

        public static void Postfix(ShipStatus __instance)
        {
            if (!EnableBetterPolus)
                return;

            if (!IsVentModified && __instance.Type == ShipStatus.MapType.Pb && SpeciVent)
            {
                SpeciVent.Id = GetAvailableId();
                IsVentModified = true;
                var vents = Ship.AllVents.ToList();
                vents.Add(SpeciVent);
                Ship.AllVents = vents.ToArray();
            }
        }
    }

    private static void ApplyChanges(ShipStatus __instance)
    {
        if (!EnableBetterPolus)
            return;

        if (__instance.Type == ShipStatus.MapType.Pb)
        {
            FindPolusObjects();
            AdjustPolus();
        }
    }

    private static void FindPolusObjects()
    {
        FindRooms();
        FindVents();
        FindObjects();
    }

    private static void AdjustPolus()
    {
        if (IsObjectsFetched && IsRoomsFetched)
        {
            if (VitalsLab)
                MoveVitals();

            if (!ColdTempDeathValley && VitalsLab)
                MoveTempCold();

            if (ColdTempDeathValley)
                MoveTempColdDV();

            if (WifiChartCourseSwap)
                SwitchNavWifi();
        }

        if (IsVentsFetched && PolusVentImprovements)
            AdjustVents();

        IsAdjustmentsDone = true;
    }

    private static void FindVents()
    {
        if (ElectricBuildingVent)
            ElectricBuildingVent = AllVents.Find(vent => vent.gameObject.name == "ElectricBuildingVent");

        if (ElectricalVent)
            ElectricalVent = AllVents.Find(vent => vent.gameObject.name == "ElectricalVent");

        if (ScienceBuildingVent)
            ScienceBuildingVent = AllVents.Find(vent => vent.gameObject.name == "ScienceBuildingVent");

        if (StorageVent)
            StorageVent = AllVents.Find(vent => vent.gameObject.name == "StorageVent");

        if (LightCageVent)
            LightCageVent = AllVents.Find(vent => vent.gameObject.name == "ElecFenceVent");

        if (AdminVent)
            AdminVent = AllVents.Find(vent => vent.gameObject.name == "AdminVent");

        if (BathroomVent)
            BathroomVent = AllVents.Find(vent => vent.gameObject.name == "BathroomVent");

        if (SpeciVent)
        {
            SpeciVent = UObject.Instantiate(AdminVent, Specimen.transform);
            SpeciVent.Right = null;
            SpeciVent.Left = null;
            SpeciVent.Center = null;
            SpeciVent.name = "SpeciVent";
        }

        IsVentsFetched = ElectricBuildingVent && ElectricalVent && ScienceBuildingVent && StorageVent && LightCageVent && SpeciVent && BathroomVent && AdminVent;
    }

    private static void FindRooms()
    {
        if (!Comms)
            Comms = AllGameObjects.Find(o => o.name == "Comms");

        if (!DropShip)
            DropShip = AllGameObjects.Find(o => o.name == "Dropship");

        if (!Outside)
            Outside = AllGameObjects.Find(o => o.name == "Outside");

        if (!Science)
            Science = AllGameObjects.Find(o => o.name == "Science");

        if (!Specimen)
            Specimen = AllGameObjects.Find(o => o.name == "RightPod");

        if (!Office)
            Office = AllGameObjects.Find(o => o.name == "Office");

        IsRoomsFetched = Comms && DropShip && Outside && Science && Specimen && Office;
    }

    private static void FindObjects()
    {
        if (!WifiConsole)
            WifiConsole = AllConsoles.Find(console => console.name == "panel_wifi");

        if (!NavConsole)
            NavConsole = AllConsoles.Find(console => console.name == "panel_nav");

        if (!Vitals)
            Vitals = AllSystemConsoles.Find(console => console.name == "panel_vitals");

        if (!DvdScreenOffice)
        {
            var dvdScreenAdmin = AllGameObjects.Find(o => o.name == "dvdscreen");

            if (dvdScreenAdmin)
            {
                DvdScreenOffice = UObject.Instantiate(dvdScreenAdmin, Office.transform);
                DvdScreenOffice.name = "dvdscreen_office";
                DvdScreenOffice.SetActive(false);
            }
        }

        if (!TempCold)
            TempCold = AllConsoles.Find(console => console.name == "panel_tempcold");

        IsObjectsFetched = WifiConsole && NavConsole && Vitals && DvdScreenOffice && TempCold;
    }

    private static void AdjustVents()
    {
        if (IsVentsFetched)
        {
            MoveSpeciVent();
            ReconnectVents();
        }
    }

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
        if (TempCold.transform.position != TempColdNewPos)
        {
            var tempColdTransform = TempCold.transform;
            tempColdTransform.parent = Outside.transform;
            tempColdTransform.position = TempColdNewPos;
            var collider = TempCold.GetComponent<BoxCollider2D>();
            collider.isTrigger = false;
            collider.size += new Vector2(0f, -0.3f);
        }
    }

    private static void MoveTempColdDV()
    {
        if (TempCold.transform.position != TempColdNewPosDV)
        {
            var tempColdTransform = TempCold.transform;
            tempColdTransform.parent = Outside.transform;
            tempColdTransform.position = TempColdNewPosDV;
            var collider = TempCold.GetComponent<BoxCollider2D>();
            collider.isTrigger = false;
            collider.size += new Vector2(0f, -0.3f);
        }
    }

    private static void SwitchNavWifi()
    {
        if (WifiConsole.transform.position != WifiNewPos)
        {
            var wifiTransform = WifiConsole.transform;
            wifiTransform.parent = DropShip.transform;
            wifiTransform.position = WifiNewPos;
        }

        if (NavConsole.transform.position != NavNewPos)
        {
            var navTransform = NavConsole.transform;
            navTransform.parent = Comms.transform;
            navTransform.position = NavNewPos;
            NavConsole.checkWalls = true;
        }
    }

    private static void MoveVitals()
    {
        if (Vitals.transform.position != VitalsNewPos)
        {
            var vitalsTransform = Vitals.gameObject.transform;
            vitalsTransform.parent = Science.transform;
            vitalsTransform.position = VitalsNewPos;
        }

        if (DvdScreenOffice.transform.position != DvdScreenNewPos)
        {
            var dvdScreenTransform = DvdScreenOffice.transform;
            dvdScreenTransform.position = DvdScreenNewPos;
            var localScale = dvdScreenTransform.localScale;
            localScale = new(0.75f, localScale.y, localScale.z);
            dvdScreenTransform.localScale = localScale;
            DvdScreenOffice.SetActive(true);
        }
    }

    [HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.AppendTaskText))]
    public static class NormalPlayerTaskPatches
    {
        public static bool Prefix(NormalPlayerTask __instance, Il2CppSystem.Text.StringBuilder sb)
        {
            if (!EnableBetterPolus || !Ship || MapPatches.CurrentMap != 2 || (int)__instance.TaskType is not (42 or 41 or 3))
                return true;

            var flag = __instance.ShouldYellowText();

            if (flag)
                sb.Append(__instance.IsComplete ? "<color=#00DD00FF>" : "<color=#FFFF00FF>");

            var room = __instance.TaskType switch
            {
                TaskTypes.RecordTemperature => SystemTypes.Outside,
                TaskTypes.RebootWifi => SystemTypes.Dropship,
                TaskTypes.ChartCourse => SystemTypes.Comms,
                _ => __instance.StartAt
            };
            sb.Append(TranslationController.Instance.GetString(room));
            sb.Append(": ");
            sb.Append(TranslationController.Instance.GetString(__instance.TaskType));

            if (__instance is { ShowTaskTimer: true, TimerStarted: NormalPlayerTask.TimerState.Started })
            {
                sb.Append(" (");
                sb.Append(TranslationController.Instance.GetString(StringNames.SecondsAbbv, (int)__instance.TaskTimer));
                sb.Append(')');
            }
            else if (__instance.ShowTaskStep)
            {
                sb.Append(" (");
                sb.Append(__instance.taskStep);
                sb.Append('/');
                sb.Append(__instance.MaxStep);
                sb.Append(')');
            }

            if (flag)
                sb.Append("</color>");

            sb.AppendLine();
            return false;
        }
    }
}