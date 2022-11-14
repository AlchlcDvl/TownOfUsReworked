using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SheriffMod
{
    public enum InterrogatePer
    {
        Round,
        Game
    }
    
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.Sheriff))
            {
                var role2 = (Sheriff) role;
                role2.LastInterrogated = DateTime.UtcNow;

                if (CustomGameOptions.SheriffFixPer == InterrogatePer.Round)
                    role2.UsedThisRound = false;
            }
        }
    }
}