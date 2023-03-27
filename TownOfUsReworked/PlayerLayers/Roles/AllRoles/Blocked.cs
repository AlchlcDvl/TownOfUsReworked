using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.BetterMaps.Airship;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch]
    public static class Blocked
    {
        [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public static class PerformVent
        {
            public static bool Prefix()
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !ConstantVariables.IsInGame)
                {
                    return true;
                }

                if (!Utils.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data))
                    return false;

                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role == null)
                    return true;

                return !role.IsBlocked;
            }
        }

        [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public static class PerformReport
        {
            public static bool Prefix()
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !ConstantVariables.IsInGame)
                {
                    return true;
                }

                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role == null)
                    return true;

                return !role.IsBlocked;
            }
        }

        [HarmonyPatch(typeof(UseButton), nameof(UseButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public static class PerformUse
        {
            public static bool Prefix(UseButton __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !ConstantVariables.IsInGame)
                {
                    return true;
                }

                if (__instance.isActiveAndEnabled && PlayerControl.LocalPlayer && Tasks.NearestTask != null && Tasks.AllCustomPlateform != null)
                {
                    Tasks.NearestTask.Use();
                    return false;
                }

                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role == null)
                    return true;

                return !role.IsBlocked;
            }
        }

        [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public static class PerformSabotage
        {
            public static bool Prefix()
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !ConstantVariables.IsInGame)
                {
                    return true;
                }

                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role == null)
                    return true;

                return !role.IsBlocked;
            }
        }

        [HarmonyPatch(typeof(AdminButton), nameof(AdminButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public static class PerformAdmin
        {
            public static bool Prefix()
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !ConstantVariables.IsInGame)
                {
                    return true;
                }

                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role == null)
                    return true;

                return !role.IsBlocked;
            }
        }

        [HarmonyPatch(typeof(PetButton), nameof(PetButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public static class PerformPet
        {
            public static bool Prefix()
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !ConstantVariables.IsInGame)
                {
                    return true;
                }

                var role = Role.GetRole(PlayerControl.LocalPlayer);

                if (role == null)
                    return true;

                return !role.IsBlocked;
            }
        }

        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public static class PerformAbility
        {
            public static bool Prefix(AbilityButton __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.CanMove ||
                    !ConstantVariables.IsInGame)
                {
                    return true;
                }

                if (!CustomButtons.ButtonUsable(__instance))
                    return false;

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