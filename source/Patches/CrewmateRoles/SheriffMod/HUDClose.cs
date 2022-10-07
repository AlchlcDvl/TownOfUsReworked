using System;
using HarmonyLib;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.SheriffMod
{
    public enum InterrogatePer
    {
        Round,
        Game
    }
    
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            foreach (var role in Role.GetRoles(RoleEnum.Sheriff))
            {
                var sheriff = (Sheriff) role;
                sheriff.LastInterrogated = DateTime.UtcNow;
                if (CustomGameOptions.SheriffFixPer == InterrogatePer.Round) sheriff.UsedThisRound = false;
            }
        }
    }
}