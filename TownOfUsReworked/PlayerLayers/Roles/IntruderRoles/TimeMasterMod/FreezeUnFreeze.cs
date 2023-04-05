using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class FreezeUnFreeze
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var tm in Role.GetRoles<TimeMaster>(RoleEnum.TimeMaster))
            {
                if (tm.Frozen)
                    tm.TimeFreeze();
                else if (tm.Enabled)
                    tm.Unfreeze();
            }
        }
    }
}