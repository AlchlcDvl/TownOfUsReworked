using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class FreezeUnFreeze
    {
       public static void Postfix(HudManager __instance)
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