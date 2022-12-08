using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsortMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Consort))
            {
                var role2 = (Consort) role;
                role2.LastBlock = DateTime.UtcNow;
                role2.LastBlock = role2.LastBlock.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ExamineCd);
            }
        }
    }
}