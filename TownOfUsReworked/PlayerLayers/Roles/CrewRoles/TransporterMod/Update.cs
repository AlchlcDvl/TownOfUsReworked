using HarmonyLib;
using InnerNet;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TransporterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        private static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Transporter))
                return;

            var role = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);

            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
            {
                if (role != null)
                {
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                        role.Update(__instance);
                }
            }
        }
    }
}