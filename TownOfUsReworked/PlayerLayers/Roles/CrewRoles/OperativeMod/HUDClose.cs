using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using Object = UnityEngine.Object;
using System;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Operative))
            {
                var role2 = (Operative) role;
                role2.lastBugged = DateTime.UtcNow;
                role2.buggedPlayers.Clear();
                
                if (CustomGameOptions.BugsRemoveOnNewRound)
                    role2.bugs.ClearBugs();
            }
        }
    }
}
