namespace TownOfUsReworked.BetterMaps.Skeld
{
    [HarmonyPatch(typeof(ShipStatus))]
    public static class SkeldShipStatusPatch
    {
        private static readonly Vector3 ReactorVentNewPos = new(-2.95f, -10.95f, 2f);
        private static readonly Vector3 ShieldsVentNewPos = new(2f, -15f, 2f);
        private static readonly Vector3 BigYVentNewPos = new(5.2f, -4.85f, 2f);
        private static readonly Vector3 NavVentNorthNewPos = new(-11.85f, -11.55f, 2f);
        private static readonly Vector3 CafeVentNewPos = new(-3.9f, 5.5f, 2f);

        private static bool IsAdjustmentsDone;
        private static bool IsObjectsFetched;
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
                if (!IsAdjustmentsDone || !IsObjectsFetched)
                    ApplyChanges(__instance);
            }
        }

        private static void ApplyChanges(ShipStatus instance)
        {
            if (instance.Type == ShipStatus.MapType.Ship)
            {
                FindSkeldObjects();
                AdjustSkeld();
            }
        }

        private static void FindSkeldObjects()
        {
            FindVents();
            FindObjects();
        }

        private static void AdjustSkeld()
        {
            if (IsObjectsFetched && CustomGameOptions.SkeldVentImprovements)
            {
                MoveReactorVent();
                MoveShieldsVent();
                MoveBigYVent();
                MoveNavVentNorth();
                MoveCafeVent();
            }

            if (CustomGameOptions.SkeldVentImprovements)
                AdjustVents();

            IsAdjustmentsDone = true;
        }

        private static void FindVents()
        {
            if (NavVentSouth == null)
                NavVentSouth = Utils.AllVents.Find(vent => vent.gameObject.name == "NavVentSouth");

            if (NavVentNorth == null)
                NavVentNorth = Utils.AllVents.Find(vent => vent.gameObject.name == "NavVentNorth");

            if (ShieldsVent == null)
                ShieldsVent = Utils.AllVents.Find(vent => vent.gameObject.name == "ShieldsVent");

            if (WeaponsVent == null)
                WeaponsVent = Utils.AllVents.Find(vent => vent.gameObject.name == "WeaponsVent");

            if (REngineVent == null)
                REngineVent = Utils.AllVents.Find(vent => vent.gameObject.name == "REngineVent");

            if (UpperReactorVent == null)
                UpperReactorVent = Utils.AllVents.Find(vent => vent.gameObject.name == "UpperReactorVent");

            if (LEngineVent == null)
                LEngineVent = Utils.AllVents.Find(vent => vent.gameObject.name == "LEngineVent");

            if (ReactorVent == null)
                ReactorVent = Utils.AllVents.Find(vent => vent.gameObject.name == "ReactorVent");

            IsVentsFetched = NavVentSouth && NavVentNorth && ShieldsVent && WeaponsVent && REngineVent && UpperReactorVent && LEngineVent && ReactorVent;
        }

        private static void FindObjects()
        {
            if (ReactorVent == null)
                ReactorVent = Utils.AllVents.Find(vent => vent.gameObject.name == "ReactorVent");

            if (ShieldsVent == null)
                ShieldsVent = Utils.AllVents.Find(vent => vent.gameObject.name == "ShieldsVent");

            if (BigYVent == null)
                BigYVent = Utils.AllVents.Find(vent => vent.gameObject.name == "BigYVent");

            if (NavVentNorth == null)
                NavVentNorth = Utils.AllVents.Find(vent => vent.gameObject.name == "NavVentNorth");

            if (CafeVent == null)
                CafeVent = Utils.AllVents.Find(vent => vent.gameObject.name == "CafeVent");

            IsObjectsFetched = ReactorVent && ShieldsVent && BigYVent && NavVentNorth && CafeVent;
        }

        private static void AdjustVents()
        {
            if (IsVentsFetched)
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
        }

        private static void MoveReactorVent()
        {
            if (ReactorVent.transform.position != ReactorVentNewPos)
            {
                var transform = ReactorVent.transform;
                transform.position = ReactorVentNewPos;
            }
        }

        private static void MoveShieldsVent()
        {
            if (ShieldsVent.transform.position != ShieldsVentNewPos)
            {
                var transform = ShieldsVent.transform;
                transform.position = ShieldsVentNewPos;
            }
        }

        private static void MoveBigYVent()
        {
            if (BigYVent.transform.position != BigYVentNewPos)
            {
                var transform = BigYVent.transform;
                transform.position = BigYVentNewPos;
            }
        }

        private static void MoveNavVentNorth()
        {
            if (NavVentNorth.transform.position != NavVentNorthNewPos)
            {
                var transform = NavVentNorth.transform;
                transform.position = NavVentNorthNewPos;
            }
        }

        private static void MoveCafeVent()
        {
            if (CafeVent.transform.position != CafeVentNewPos)
            {
                var transform = CafeVent.transform;
                transform.position = CafeVentNewPos;
            }
        }
    }
}