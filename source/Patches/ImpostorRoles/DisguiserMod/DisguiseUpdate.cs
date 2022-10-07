using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.Patches.ImpostorRoles.FramerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class DisguiseUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Disguiser))
            {
                Disguiser disguiser = (Disguiser) role;
                disguiser.DisguiseTick();
            }
        }
    }
}