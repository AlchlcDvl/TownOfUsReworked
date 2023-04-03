using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.BetterMaps.Airship;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Modules;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using System.Collections.Generic;
using TownOfUsReworked.Data;

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
                if (ConstantVariables.Inactive)
                    return true;

                if (!Utils.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data))
                    return false;

                return PlayerLayer.GetLayers(PlayerControl.LocalPlayer).All(x => !x.IsBlocked);
            }
        }

        [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public static class PerformReport
        {
            public static bool Prefix()
            {
                if (ConstantVariables.Inactive)
                    return true;

                return PlayerLayer.GetLayers(PlayerControl.LocalPlayer).All(x => !x.IsBlocked);
            }
        }

        [HarmonyPatch(typeof(UseButton), nameof(UseButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public static class PerformUse
        {
            public static bool Prefix(UseButton __instance)
            {
                if (ConstantVariables.Inactive)
                    return true;

                if (__instance.isActiveAndEnabled && PlayerControl.LocalPlayer && Tasks.NearestTask != null && Tasks.AllCustomPlateform != null)
                {
                    Tasks.NearestTask.Use();
                    return false;
                }

                return PlayerLayer.GetLayers(PlayerControl.LocalPlayer).All(x => !x.IsBlocked);
            }
        }

        [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public static class PerformSabotage
        {
            public static bool Prefix()
            {
                if (ConstantVariables.Inactive)
                    return true;

                return PlayerLayer.GetLayers(PlayerControl.LocalPlayer).All(x => !x.IsBlocked);
            }
        }

        [HarmonyPatch(typeof(AdminButton), nameof(AdminButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public static class PerformAdmin
        {
            public static bool Prefix()
            {
                if (ConstantVariables.Inactive)
                    return true;

                return PlayerLayer.GetLayers(PlayerControl.LocalPlayer).All(x => !x.IsBlocked);
            }
        }

        [HarmonyPatch(typeof(PetButton), nameof(PetButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public static class PerformPet
        {
            public static bool Prefix()
            {
                if (ConstantVariables.Inactive)
                    return true;

                return PlayerLayer.GetLayers(PlayerControl.LocalPlayer).All(x => !x.IsBlocked);
            }
        }

        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
        [HarmonyPriority(Priority.First)]
        public static class PerformAbility
        {
            public static bool Prefix(AbilityButton __instance)
            {
                if (ConstantVariables.Inactive)
                    return true;

                PlayerControl.LocalPlayer.RegenTask();

                if (!CustomButtons.ButtonUsable(__instance))
                    return false;

                return PlayerLayer.GetLayers(PlayerControl.LocalPlayer).All(x => !x.IsBlocked);
            }
        }

        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
        public static class PerformAbility2
        {
            public static bool Prefix(AbilityButton __instance)
            {
                if (ConstantVariables.Inactive)
                    return true;

                PlayerControl.LocalPlayer.RegenTask();

                if (!CustomButtons.ButtonUsable(__instance))
                    return false;

                if (PlayerControl.LocalPlayer.IsBlocked())
                    return false;

                var clicked = false;
                var clickedList = new List<bool>();
                Role.GetRole(PlayerControl.LocalPlayer)?.ButtonClick(__instance, out clicked);
                clickedList.Add(clicked);
                Ability.GetAbility(PlayerControl.LocalPlayer)?.ButtonClick(__instance, out clicked);
                clickedList.Add(clicked);
                Objectifier.GetObjectifier(PlayerControl.LocalPlayer)?.ButtonClick(__instance, out clicked);
                clickedList.Add(clicked);
                return clickedList.All(x => x);
            }
        }
    }
}