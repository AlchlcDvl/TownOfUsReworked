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
            foreach (var role in Role.GetRoles(RoleEnum.TimeMaster))
            {
                var tm = (TimeMaster)role;

                if (tm.Frozen)
                    tm.TimeFreeze();
                else if (tm.Enabled)
                    tm.Unfreeze();
            }
        }
    }
}