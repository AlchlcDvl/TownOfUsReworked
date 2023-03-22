using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PlaguebearerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public class BodyReport
    {
        private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo info)
        {
            if (Utils.NoButton(__instance, RoleEnum.Plaguebearer) || info == null || CustomGameOptions.PlaguebearerOn == 0)
                return;
            
            Utils.Spread(PlayerControl.LocalPlayer, info.Object);
        }
    }
}