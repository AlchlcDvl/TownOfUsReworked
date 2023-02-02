using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.DisguiserMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class DisguiseUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Disguiser))
            {
                Disguiser disguiser = (Disguiser)role;
                disguiser.DisguiseTick();
            }
        }
    }
}