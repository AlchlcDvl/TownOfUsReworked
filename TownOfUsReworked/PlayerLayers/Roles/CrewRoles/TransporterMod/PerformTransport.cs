using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TransporterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformTransport
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Transporter, true))
                return false;

            var role = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (role.TransportTimer() != 0f && __instance == role.TransportButton)
                return false;

            if (__instance == role.TransportButton)
            {
                if (role.TransportList == null && role.ButtonUsable)
                {
                    role.PressedButton = true;
                    role.MenuClick = true;
                }

                return false;
            }

            return false;
        }
    }
}