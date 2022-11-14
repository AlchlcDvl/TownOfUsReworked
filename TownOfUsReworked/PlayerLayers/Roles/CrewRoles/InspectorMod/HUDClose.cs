using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InspectorMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Inspector))
            {
                var role2 = (Inspector) role;
                role2.LastExamined = DateTime.UtcNow;
                role2.LastExamined = role2.LastExamined.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InspectCooldown);
            }
        }
    }
}