using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SurvivorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class VestUnvest
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Survivor))
            {
                var surv = (Survivor)role;

                if (surv.Vesting)
                    surv.Vest();
                else if (surv.Enabled)
                    surv.UnVest();
            }
        }
    }
}