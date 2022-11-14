/*using System;
using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject)
                return;
            
            foreach (var role in Role.GetRoles(RoleEnum.Escort))
            {
                var role2 = (Escort) role;
                role2.LastBlocked = DateTime.UtcNow;
            }
        }
    }
}*/