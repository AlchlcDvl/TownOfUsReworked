using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.DisguiserMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Disguiser))
            {
                var role2 = (Disguiser) role;
                role2.DisguiseButton.graphic.sprite = TownOfUsReworked.MeasureSprite;
                role2.MeasuredPlayer = null;
                role2.LastDisguised = DateTime.UtcNow;
            }
        }
    }
}