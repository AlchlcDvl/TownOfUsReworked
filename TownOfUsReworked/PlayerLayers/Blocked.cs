using HarmonyLib;
using TownOfUsReworked.BetterMaps.Airship;
using TownOfUsReworked.Extensions;
using System.Linq;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
    [HarmonyPriority(Priority.First)]
    public static class PerformVent
    {
        public static bool Prefix()
        {
            if (ConstantVariables.Inactive)
                return true;

            if (!PlayerControl.LocalPlayer.CanVent())
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

            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Coward))
                return false;

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

            if (__instance.isActiveAndEnabled && PlayerControl.LocalPlayer && Tasks.NearestTask != null && Tasks.AllCustomPlateform != null &&
                PlayerLayer.GetLayers(PlayerControl.LocalPlayer).All(x => !x.IsBlocked))
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

            return PlayerLayer.GetLayers(PlayerControl.LocalPlayer).All(x => !x.IsBlocked); //No petting for you lmao
        }
    }
}