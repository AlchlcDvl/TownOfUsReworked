using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.DisguiserMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class MorphUnmorph
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Disguiser))
            {
                var disguiser = (Disguiser) role;
                if (disguiser.Disguised)
                    disguiser.Disguise();
                else if (disguiser.DisguisedPlayer) disguiser.Undisguise();
            }
        }
    }
}