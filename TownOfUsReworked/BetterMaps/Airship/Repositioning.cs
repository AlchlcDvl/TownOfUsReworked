namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable))]
    static class Repositioning
    {
        public static void Postfix()
        {
            var adminTable = UObject.FindObjectOfType<MapConsole>();

            if (CustomGameOptions.MoveAdmin != 0)
            {
                var mapFloating = GameObject.Find("Cockpit/cockpit_mapfloating");

                if ((int)CustomGameOptions.MoveAdmin == 1)
                {
                    adminTable.transform.position = new(-17.269f, 1.375f);
                    adminTable.transform.rotation = Quaternion.Euler(new(0, 0, 350.316f));
                    adminTable.transform.localScale = new(1, 1, 1);

                    mapFloating.transform.position = new(-17.736f, 2.36f);
                    mapFloating.transform.rotation = Quaternion.Euler(new(0, 0, 350));
                    mapFloating.transform.localScale = new(1, 1, 1);
                }
                else if ((int)CustomGameOptions.MoveAdmin == 2)
                {
                    //New Admin
                    adminTable.transform.position = new(5.078f, 3.4f, 1);
                    adminTable.transform.rotation = Quaternion.Euler(new(0, 0, 76.1f));
                    adminTable.transform.localScale = new(1.200f, 1.700f, 1);
                    mapFloating.transform.localScale = new(0, 0, 0);
                }
            }

            if (CustomGameOptions.MoveElectrical != 0)
            {
                var electrical = GameObject.Find("GapRoom/task_lightssabotage (gap)");

                if ((int)CustomGameOptions.MoveElectrical == 1)
                {
                    electrical.transform.position = new(-8.818f, 13.184f);
                    electrical.transform.localScale = new(0.909f, 0.818f, 1);

                    var originalSupport = GameObject.Find("Vault/cockpit_comms");
                    var supportElectrical = UObject.Instantiate(originalSupport, originalSupport.transform);

                    supportElectrical.transform.position = new(-8.792f, 13.242f);
                    supportElectrical.transform.localScale = new(1, 1, 1);
                }
                else if ((int)CustomGameOptions.MoveElectrical == 2)
                    electrical.transform.position = new(19.339f, -3.665f);
            }

            if (CustomGameOptions.MoveVitals)
            {
                GameObject.Find("Medbay/panel_vitals").transform.position = new(24.55f, -4.780f);
                GameObject.Find("Medbay/panel_data").transform.position = new(25.240f, -7.938f);
            }

            if (CustomGameOptions.MoveFuel)
                GameObject.Find("Storage/task_gas").transform.position = new(36.070f, 1.897f);

            if (CustomGameOptions.MoveDivert)
                GameObject.Find("HallwayMain/DivertRecieve").transform.position = new(13.35f, -1.659f);
        }
    }
}