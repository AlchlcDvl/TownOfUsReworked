using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class CamouflageUnCamouflage
    {
        private static bool CommsEnabled;
        private static bool CamouflagerEnabled;
        public static bool IsCamoed => CommsEnabled || CamouflagerEnabled;

        public static void Postfix()
        {
            CamouflagerEnabled = false;
            CommsEnabled = false;

            foreach (var camouflager in Role.GetRoles(RoleEnum.Camouflager).Cast<Camouflager>())
            {
                if (camouflager.Camouflaged)
                {
                    camouflager.Camouflage();
                    CamouflagerEnabled = true;
                }
                else if (camouflager.Enabled)
                {
                    camouflager.UnCamouflage();
                    CamouflagerEnabled = false;
                }
            }

            foreach (var godfather in Role.GetRoles(RoleEnum.Godfather).Cast<Godfather>())
            {
                if (godfather.Camouflaged)
                {
                    godfather.Camouflage();
                    CamouflagerEnabled = true;
                }
                else if (godfather.CamoEnabled)
                {
                    godfather.UnCamouflage();
                    CamouflagerEnabled = false;
                }
            }

            if (CustomGameOptions.ColourblindComms)
            {
                if (ShipStatus.Instance)
                {
                    switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
                    {
                        case 0:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            HudOverrideSystemType comms5 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                            if (comms5.IsActive)
                            {
                                CommsEnabled = true;
                                Utils.Camouflage();
                                return;
                            }

                            break;

                        case 1:
                            HqHudSystemType comms2 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();

                            if (comms2.IsActive)
                            {
                                CommsEnabled = true;
                                Utils.Camouflage();
                                return;
                            }

                            break;
                    }
                }

                if (CommsEnabled)
                {
                    CommsEnabled = false;
                    CamouflagerEnabled = false;
                    Utils.DefaultOutfitAll();
                }
            }
        }
    }
}