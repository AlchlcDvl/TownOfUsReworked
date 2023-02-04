using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod.RevivedRoles.Chameleon
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class DoUndo
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Retributionist))
            {
                var ret = (Retributionist)role;

                if (ret.RevivedRole.RoleType == RoleEnum.Chameleon)
                {
                    if (ret.IsSwooped)
                        ret.Invis();
                    else if (ret.SwoopEnabled)
                        ret.Uninvis();
                }
            }
        }
    }
}