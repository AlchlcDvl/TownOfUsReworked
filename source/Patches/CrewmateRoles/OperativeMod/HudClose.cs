using HarmonyLib;
using TownOfUs.Roles;
using Object = UnityEngine.Object;
using System;

namespace TownOfUs.CrewmateRoles.OperativeMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            foreach (var role in Role.GetRoles(RoleEnum.Operative))
            {
                var op = (Operative)role;
                op.lastBugged = DateTime.UtcNow;
                op.buggedPlayers.Clear();
                if (CustomGameOptions.BugsRemoveOnNewRound)
                {
                    op.bugs.ClearBugs();
                }
            }
        }
    }
}
