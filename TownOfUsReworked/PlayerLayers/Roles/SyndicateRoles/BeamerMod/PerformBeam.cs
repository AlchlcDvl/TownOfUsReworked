using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BeamerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformBeam
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Beamer))
                return true;

            var role = Role.GetRole<Beamer>(PlayerControl.LocalPlayer);

            if (__instance == role.BeamButton)
            {
                if (role.BeamTimer() != 0f)
                    return false;

                if (role.BeamList == null)
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