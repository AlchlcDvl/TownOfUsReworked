using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Operative))
                return false;

            var role = Role.GetRole<Operative>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Operative, null, __instance) || __instance != role.BugButton || !role.ButtonUsable)
                return false;

            if (role.BugTimer() != 0f && __instance == role.BugButton)
                return false;

            role.UsesLeft--;
            role.lastBugged = System.DateTime.UtcNow;
            role.bugs.Add(BugExtentions.CreateBug(PlayerControl.LocalPlayer.GetTruePosition()));
            return false;
        }
    }
}
