using HarmonyLib;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch]
    public class Blocked
    {
        [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public class PerformVent
        {
            public static bool Prefix(VentButton __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !Utils.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data) || !GameStates.IsInGame)
                    return true;

                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role == null)
                    return true;

                return !role.IsBlocked;
            }
        }

        [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public class PerformReport
        {
            public static bool Prefix(ReportButton __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !GameStates.IsInGame)
                    return true;

                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role == null)
                    return true;

                return !role.IsBlocked;
            }
        }

        [HarmonyPatch(typeof(UseButton), nameof(UseButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public class PerformUse
        {
            public static bool Prefix(UseButton __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !GameStates.IsInGame)
                    return true;

                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role == null)
                    return true;

                return !role.IsBlocked;
            }
        }

        [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public class PerformSabotage
        {
            public static bool Prefix(SabotageButton __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !GameStates.IsInGame)
                    return true;

                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role == null)
                    return true;

                return !role.IsBlocked;
            }
        }

        [HarmonyPatch(typeof(AdminButton), nameof(AdminButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public class PerformAdmin
        {
            public static bool Prefix(AdminButton __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !GameStates.IsInGame)
                    return true;

                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role == null)
                    return true;

                return !role.IsBlocked;
            }
        }

        [HarmonyPatch(typeof(PetButton), nameof(PetButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public class PerformPet
        {
            public static bool Prefix(PetButton __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !GameStates.IsInGame)
                    return true;

                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role == null)
                    return true;

                return !role.IsBlocked;
            }
        }

        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public class PerformAbility
        {
            public static bool Prefix(AbilityButton __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !GameStates.IsInGame)
                    return true;

                if (__instance == HudManager.Instance.AbilityButton)
                    return true;

                PlayerControl.LocalPlayer.RegenTask();
                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role == null)
                    return true;

                return !role.IsBlocked;
            }
        }
    }
}