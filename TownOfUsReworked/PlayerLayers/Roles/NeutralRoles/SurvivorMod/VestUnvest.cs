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
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var surv in Role.GetRoles<Survivor>(RoleEnum.Survivor))
            {
                if (surv.Vesting)
                    surv.Vest();
                else if (surv.Enabled)
                    surv.UnVest();
            }
        }
    }
}