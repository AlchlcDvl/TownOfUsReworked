using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject)
                return;
            
            foreach (var role in Role.GetRoles(RoleEnum.Arsonist))
            {
                var role2 = (Arsonist) role;
                role2.LastDoused = DateTime.UtcNow;
                role2.LastIgnited = DateTime.UtcNow;
            }
        }
    }
}