using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TransporterMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformTransport
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Transporter))
                return true;

            var role = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);

            if (__instance == role.TransportButton)
            {
                if (role.TransportTimer() != 0f)
                    return false;

                if (role.TransportList == null)
                {
                    role.PressedButton = true;
                    role.MenuClick = true;
                }

                return false;
            }

            return true;
        }
    }
}