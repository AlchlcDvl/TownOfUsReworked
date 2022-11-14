using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class FreezeUnFreeze
    {
        public static bool FreezeEnabled;
        public static bool IsFrozen => FreezeEnabled;

        public static void Postfix(HudManager __instance)
        {
            FreezeEnabled = false;
            foreach (var role in Role.GetRoles(RoleEnum.TimeMaster))
            {
                var tm = (TimeMaster)role;

                if (tm.Frozen)
                {
                    FreezeEnabled = true;
                    tm.TimeFreeze();
                }
                else if (tm.Enabled)
                {
                    FreezeEnabled = false;
                    tm.Unfreeze();
                }
            }
        }
    }
}