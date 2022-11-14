using HarmonyLib;
using TownOfUsReworked.Enums;
using Object = UnityEngine.Object;
using System;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Cannibal))
            {
                var role2 = (Cannibal) role;
                role2.LastEaten = DateTime.UtcNow;
            }
        }
    }
}