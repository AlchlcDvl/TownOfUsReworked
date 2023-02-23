using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        [HarmonyPriority(Priority.Last)]
        private static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Retributionist))
                return;

            var role = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            if (role.RevivedRole == null)
                return;

            if (role.RevivedRole.RoleType == RoleEnum.Inspector)
            {
                foreach (var player2 in role.Interrogated)
                {
                    var player = Utils.PlayerById(player2);

                    if (Utils.SeemsEvil(player))
                        player.nameText().color = Colors.Intruder;
                    else
                        player.nameText().color = Colors.Glitch;
                }
            }
        }
    }
}