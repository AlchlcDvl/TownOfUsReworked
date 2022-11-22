using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDKill
    {
        private static KillButton KillButton;

        public static void Postfix(HudManager __instance)
        {
            UpdateKillButton(__instance);
        }

        private static void UpdateKillButton(HudManager __instance)
        {
            KillButton = __instance.KillButton;

            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            var isDead = PlayerControl.LocalPlayer.Data.IsDead;

            if (isDead)
                return;
            
            var role = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
            __instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.VigiKillCd);
            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
        }
    }
}
