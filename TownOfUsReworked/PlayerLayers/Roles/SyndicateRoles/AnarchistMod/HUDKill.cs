using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.AnarchistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDKill
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Anarchist))
                return;

            var role = Role.GetRole<Anarchist>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);
            __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.ChaosDriveKillCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
        }
    }
}