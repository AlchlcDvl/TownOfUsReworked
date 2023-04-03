using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod.RevivedRoles.Chameleon
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class DoUndo
    {
        public static void Postfix()
        {
            foreach (var role in Role.GetRoles(RoleEnum.Retributionist))
            {
                var ret = (Retributionist)role;

                if (ret.RevivedRole == null)
                    continue;

                var revivedRole = ret.RevivedRole.Type;

                if (revivedRole == RoleEnum.Chameleon)
                {
                    if (ret.IsSwooped)
                        ret.Invis();
                    else if (ret.SwoopEnabled)
                        ret.Uninvis();
                }
                else if (revivedRole == RoleEnum.Veteran)
                {
                    if (ret.OnAlert)
                        ret.Alert();
                    else if (ret.AlertEnabled)
                        ret.UnAlert();
                }
            }
        }
    }
}