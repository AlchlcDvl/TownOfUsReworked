using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CryomaniacMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Cryomaniac))
            {
                var cryo = (Cryomaniac) role;
                cryo.LastDoused = DateTime.UtcNow;
            }
        }
    }
}