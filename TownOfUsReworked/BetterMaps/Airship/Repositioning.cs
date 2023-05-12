namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable))]
    static class Repositioning
    {
        public static void Postfix()
        {
            var AdminTable = UObject.FindObjectOfType<MapConsole>();

            if ((byte)CustomGameOptions.MoveAdmin != 0)
            {
                var MapFloating = GameObject.Find("Cockpit/cockpit_mapfloating");

                if ((byte)CustomGameOptions.MoveAdmin == 1)
                {
                    AdminTable.transform.position = new(-17.269f, 1.375f);
                    AdminTable.transform.rotation = Quaternion.Euler(new(0, 0, 350.316f));
                    AdminTable.transform.localScale = new(1, 1, 1);

                    MapFloating.transform.position = new(-17.736f, 2.36f);
                    MapFloating.transform.rotation = Quaternion.Euler(new(0, 0, 350));
                    MapFloating.transform.localScale = new(1, 1, 1);
                }
                else if ((byte)CustomGameOptions.MoveAdmin == 2)
                {
                    // New Admin
                    AdminTable.transform.position = new(5.078f, 3.4f, 1);
                    AdminTable.transform.rotation = Quaternion.Euler(new(0, 0, 76.1f));
                    AdminTable.transform.localScale = new(1.200f, 1.700f, 1);
                    MapFloating.transform.localScale = new(0, 0, 0);
                }
            }

            if ((byte)CustomGameOptions.MoveElectrical != 0)
            {
                var Electrical = GameObject.Find("GapRoom/task_lightssabotage (gap)");

                if ((byte)CustomGameOptions.MoveElectrical == 1)
                {
                    Electrical.transform.position = new(-8.818f, 13.184f);
                    Electrical.transform.localScale = new(0.909f, 0.818f, 1);

                    var OriginalSupport = GameObject.Find("Vault/cockpit_comms");
                    var SupportElectrical = UObject.Instantiate(OriginalSupport, OriginalSupport.transform);

                    SupportElectrical.transform.position = new(-8.792f, 13.242f);
                    SupportElectrical.transform.localScale = new(1, 1, 1);
                }
                else if ((byte)CustomGameOptions.MoveElectrical == 2)
                    Electrical.transform.position = new(19.339f, -3.665f);
            }

            if (CustomGameOptions.MoveVitals)
            {
                var Vitals = GameObject.Find("Medbay/panel_vitals");
                Vitals.transform.position = new(24.55f, -4.780f);

                var MedbayDownload = GameObject.Find("Medbay/panel_data");
                MedbayDownload.transform.position = new(25.240f, -7.938f);
            }

            if (CustomGameOptions.MoveFuel)
            {
                var Fuel = GameObject.Find("Storage/task_gas");
                Fuel.transform.position = new(36.070f, 1.897f);
            }

            if (CustomGameOptions.MoveDivert)
            {
                var DivertRecieve = GameObject.Find("HallwayMain/DivertRecieve");
                DivertRecieve.transform.position = new(13.35f, -1.659f);
            }
        }
    }
}