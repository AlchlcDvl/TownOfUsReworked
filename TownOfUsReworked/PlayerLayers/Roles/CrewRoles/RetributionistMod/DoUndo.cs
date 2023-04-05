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
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded)
                return;

            foreach (var ret in Role.GetRoles<Retributionist>(RoleEnum.Retributionist))
            {
                if (ret.RevivedRole == null)
                    continue;

                var revivedRole = ret.RevivedRole.RoleType;

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