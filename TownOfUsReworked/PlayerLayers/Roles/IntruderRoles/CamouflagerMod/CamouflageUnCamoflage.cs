using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class CamouflageUnCamouflage
    {
        public static bool CommsEnabled;
        public static bool CamouflagerEnabled;
        public static bool IsCamoed => CommsEnabled || CamouflagerEnabled;

        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            CamouflagerEnabled = false;
            CommsEnabled = false;

            foreach (Camouflager camouflager in Role.GetRoles(RoleEnum.Camouflager))
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

            foreach (Godfather godfather in Role.GetRoles(RoleEnum.Godfather))
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