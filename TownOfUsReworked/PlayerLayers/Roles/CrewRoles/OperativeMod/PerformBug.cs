using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformBug
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Operative, true))
                return false;

            var role = Role.GetRole<Operative>(PlayerControl.LocalPlayer);

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (role.BugTimer() != 0f && __instance == role.BugButton)
                return false;

            if (__instance == role.BugButton)
            {
                role.UsesLeft--;
                role.LastBugged = System.DateTime.UtcNow;
                role.Bugs.Add(BugExtentions.CreateBug(PlayerControl.LocalPlayer.GetTruePosition()));
                return false;
            }

            return false;
        }
    }
}
