namespace TownOfUsReworked.BetterMaps;

public static class PolusShipStatusPatch
{
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
            if (!CustomGameOptions.EnableBetterPolus)
                return;

            if (!IsVentModified && __instance.Type == ShipStatus.MapType.Pb)
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
        if (!CustomGameOptions.EnableBetterPolus)
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
            if (CustomGameOptions.VitalsLab)
                MoveVitals();

            if (!CustomGameOptions.ColdTempDeathValley && CustomGameOptions.VitalsLab)
                MoveTempCold();

            if (CustomGameOptions.ColdTempDeathValley)
                MoveTempColdDV();

            if (CustomGameOptions.WifiChartCourseSwap)
                SwitchNavWifi();
        }

        if (IsVentsFetched && CustomGameOptions.PolusVentImprovements)
            AdjustVents();

        IsAdjustmentsDone = true;
    }

    private static void FindVents()
    {
        if (ElectricBuildingVent == null)
            ElectricBuildingVent = AllVents.Find(vent => vent.gameObject.name == "ElectricBuildingVent");

        if (ElectricalVent == null)
            ElectricalVent = AllVents.Find(vent => vent.gameObject.name == "ElectricalVent");

        if (ScienceBuildingVent == null)
            ScienceBuildingVent = AllVents.Find(vent => vent.gameObject.name == "ScienceBuildingVent");

        if (StorageVent == null)
            StorageVent = AllVents.Find(vent => vent.gameObject.name == "StorageVent");

        if (LightCageVent == null)
            LightCageVent = AllVents.Find(vent => vent.gameObject.name == "ElecFenceVent");

        if (AdminVent == null)
            AdminVent = AllVents.Find(vent => vent.gameObject.name == "AdminVent");

        if (BathroomVent == null)
            BathroomVent = AllVents.Find(vent => vent.gameObject.name == "BathroomVent");

        if (SpeciVent == null)
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
        if (Comms == null)
            Comms = AllGameObjects.Find(o => o.name == "Comms");

        if (DropShip == null)
            DropShip = AllGameObjects.Find(o => o.name == "Dropship");

        if (Outside == null)
            Outside = AllGameObjects.Find(o => o.name == "Outside");

        if (Science == null)
            Science = AllGameObjects.Find(o => o.name == "Science");

        if (Specimen == null)
            Specimen = AllGameObjects.Find(o => o.name == "RightPod");

        if (Office == null)
            Office = AllGameObjects.Find(o => o.name == "Office");

        IsRoomsFetched = Comms && DropShip && Outside && Science && Specimen && Office;
    }

    private static void FindObjects()
    {
        if (WifiConsole == null)
            WifiConsole = AllConsoles.Find(console => console.name == "panel_wifi");

        if (NavConsole == null)
            NavConsole = AllConsoles.Find(console => console.name == "panel_nav");

        if (Vitals == null)
            Vitals = AllSystemConsoles.Find(console => console.name == "panel_vitals");

        if (DvdScreenOffice == null)
        {
            var dvdScreenAdmin = AllGameObjects.Find(o => o.name == "dvdscreen");

            if (dvdScreenAdmin)
            {
                DvdScreenOffice = UObject.Instantiate(dvdScreenAdmin, Office.transform);
                DvdScreenOffice.name = "dvdscreen_office";
                DvdScreenOffice.SetActive(false);
            }
        }

        if (TempCold == null)
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
}

[HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.AppendTaskText))]
public static class NormalPlayerTaskPatches
{
    public static bool Prefix(NormalPlayerTask __instance, ref Il2CppSystem.Text.StringBuilder sb)
    {
        if (!CustomGameOptions.EnableBetterPolus || !Ship || MapPatches.CurrentMap != 2 || __instance.TaskType is not (TaskTypes.RebootWifi or TaskTypes.RecordTemperature or
            TaskTypes.ChartCourse))
        {
            return true;
        }

        var flag = __instance.ShouldYellowText();

        if (flag)
            sb.Append(__instance.IsComplete ? "<color=#00DD00FF>" : "<color=#FFFF00FF>");

        var room = GetUpdatedRoom(__instance);
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

    private static SystemTypes GetUpdatedRoom(NormalPlayerTask task) => task.TaskType switch
    {
        TaskTypes.RecordTemperature => SystemTypes.Outside,
        TaskTypes.RebootWifi => SystemTypes.Dropship,
        TaskTypes.ChartCourse => SystemTypes.Comms,
        _ => task.StartAt
    };
}