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
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Transporter))
                return false;

            var role = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);

            if (__instance == role.TransportButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.TransportTimer() > 0f)
                    return false;

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