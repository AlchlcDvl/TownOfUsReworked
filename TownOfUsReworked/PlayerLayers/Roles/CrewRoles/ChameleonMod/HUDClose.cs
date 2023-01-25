using System;
using HarmonyLib;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ChameleonMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Chameleon))
            {
                var role2 = (Chameleon) role;
                role2.LastSwooped = DateTime.UtcNow;
            }
        }
    }
}