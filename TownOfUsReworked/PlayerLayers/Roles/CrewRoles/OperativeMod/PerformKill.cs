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
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Operative))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Operative>(PlayerControl.LocalPlayer);

            if (!(role.BugTimer() == 0f))
                return false;

            if (!__instance.enabled)
                return false;

            if (!role.ButtonUsable)
                return false;

            role.UsesLeft--;
            role.lastBugged = System.DateTime.UtcNow;
            role.bugs.Add(BugExtentions.CreateBug(PlayerControl.LocalPlayer.GetTruePosition()));

            return false;
        }
    }
}
