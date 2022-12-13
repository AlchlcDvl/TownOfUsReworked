using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDShoot
    {
        private static KillButton KillButton;

        public static void Postfix(HudManager __instance)
        {
            KillButton = __instance.KillButton;

            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            var flag7 = PlayerControl.AllPlayerControls.Count > 1;

            if (!flag7)
                return;

            var flag8 = PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante);

            if (!flag8)
                return;

            var role = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
            var isDead = PlayerControl.LocalPlayer.Data.IsDead;

            if (isDead)
                KillButton.gameObject.SetActive(false);
            else
            {
                KillButton.gameObject.SetActive(!MeetingHud.Instance);
                KillButton.SetCoolDown(role.KillTimer(), GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                Utils.SetTarget(ref role.ClosestPlayer, KillButton);
            }
        }
    }
}
