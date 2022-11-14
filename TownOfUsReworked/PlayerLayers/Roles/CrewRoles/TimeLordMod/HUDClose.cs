using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.TimeLord))
            {
                var tl = (TimeLord) role;
                tl.FinishRewind = DateTime.UtcNow;
                tl.StartRewind = DateTime.UtcNow;
                tl.StartRewind = tl.StartRewind.AddSeconds(-10.0f);
            }
        }
    }
}