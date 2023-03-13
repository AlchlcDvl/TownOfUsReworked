using HarmonyLib;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
    public class PerformVent
    {
        public static bool Prefix(VentButton __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                !Utils.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data) || !GameStates.IsRoaming)
                return true;

            var role = Role.GetRole(PlayerControl.LocalPlayer);
            return !role.IsBlocked;
        }
    }

    [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
    public class PerformReport
    {
        public static bool Prefix(ReportButton __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                !GameStates.IsRoaming)
                return true;

            var role = Role.GetRole(PlayerControl.LocalPlayer);
            return !role.IsBlocked;
        }
    }

    [HarmonyPatch(typeof(UseButton), nameof(UseButton.DoClick))]
    public class PerformUse
    {
        public static bool Prefix(UseButton __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                !GameStates.IsRoaming)
                return true;

            var role = Role.GetRole(PlayerControl.LocalPlayer);
            return !role.IsBlocked;
        }
    }

    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
    public class PerformSabotage
    {
        public static bool Prefix(SabotageButton __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                !GameStates.IsRoaming)
                return true;

            var role = Role.GetRole(PlayerControl.LocalPlayer);
            return !role.IsBlocked;
        }
    }

    [HarmonyPatch(typeof(AdminButton), nameof(AdminButton.DoClick))]
    public class PerformAdmin
    {
        public static bool Prefix(AdminButton __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                !GameStates.IsRoaming)
                return true;

            var role = Role.GetRole(PlayerControl.LocalPlayer);
            return !role.IsBlocked;
        }
    }
}