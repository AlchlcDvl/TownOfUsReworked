using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.JanitorMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject) return;

            foreach (var role in Role.GetRoles(RoleEnum.Janitor))
            {
                var role2 = Role.GetRole<Janitor>(PlayerControl.LocalPlayer);
                role2.LastCleaned = DateTime.UtcNow;
            }
        }
    }
}