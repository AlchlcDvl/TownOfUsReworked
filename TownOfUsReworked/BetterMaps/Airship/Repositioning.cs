using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable))]
    static class Repositioning
    {
        public static void Postfix()
        {
            var AdminTable = Object.FindObjectOfType<MapConsole>();

            if ((byte)CustomGameOptions.MoveAdmin != 0)
            {
                var MapFloating = GameObject.Find("Cockpit/cockpit_mapfloating");

                if ((byte)CustomGameOptions.MoveAdmin == 1)
                {
                    // Admin
                    AdminTable.transform.position = new Vector2(-17.269f, 1.375f);
                    AdminTable.transform.rotation = Quaternion.Euler(new Vector3(0.000f, 0.000f, 350.316f));
                    AdminTable.transform.localScale = new Vector3(1f, 1f, 1f);

                    // Maping Float
                    MapFloating.transform.position = new Vector2(-17.736f, 2.36f);
                    MapFloating.transform.rotation = Quaternion.Euler(new Vector3(0.000f, 0.000f, 350f));
                    MapFloating.transform.localScale = new Vector3(1f, 1f, 1f);
                }
                else if ((byte)CustomGameOptions.MoveAdmin == 2)
                {
                    // New Admin
                    AdminTable.transform.position = new Vector3(5.078f, 3.4f, 1f);
                    AdminTable.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 76.1f));
                    AdminTable.transform.localScale = new Vector3(1.200f, 1.700f, 1f);
                    MapFloating.transform.localScale = new Vector3(0f, 0f, 0f);
                }
            }

            if ((byte)CustomGameOptions.MoveElectrical != 0)
            {
                var Electrical = GameObject.Find("GapRoom/task_lightssabotage (gap)");

                if ((byte)CustomGameOptions.MoveElectrical == 1)
                {
                    // Electical Cargo
                    Electrical.transform.position = new Vector2(-8.818f, 13.184f);
                    Electrical.transform.localScale = new Vector3(0.909f, 0.818f, 1f);

                    // Support
                    var OriginalSupport = GameObject.Find("Vault/cockpit_comms");
                    var SupportElectrical = Object.Instantiate(OriginalSupport, OriginalSupport.transform);
                    SupportElectrical.transform.position = new Vector2(-8.792f, 13.242f);
                    SupportElectrical.transform.localScale = new Vector3(1f, 1f, 1f);
                }
                else if ((byte)CustomGameOptions.MoveElectrical == 2)
                {
                    // Light
                    Electrical.transform.position = new Vector2(19.339f, -3.665f);
                }
            }

            if (CustomGameOptions.MoveVitals)
            {
                // Vitals
                var Vitals = GameObject.Find("Medbay/panel_vitals");
                Vitals.transform.position = new Vector2(24.55f, -4.780f);

                // Download Medbay
                var MedbayDownload = GameObject.Find("Medbay/panel_data");
                MedbayDownload.transform.position = new Vector2(25.240f, -7.938f);
            }

            if (CustomGameOptions.MoveFuel)
            {
                // Cargo gas
                var Fuel = GameObject.Find("Storage/task_gas");
                Fuel.transform.position = new Vector2(36.070f, 1.897f);
            }

            if (CustomGameOptions.MoveDivert)
            {
                // Divert
                var DivertRecieve = GameObject.Find("HallwayMain/DivertRecieve");
                DivertRecieve.transform.position = new Vector2(13.35f, -1.659f);
            }
        }
    }
}