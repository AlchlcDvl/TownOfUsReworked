using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ShifterMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDShift
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Shifter))
                return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
            var shiftButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            var role = Role.GetRole<Shifter>(PlayerControl.LocalPlayer);

            if (isDead)
                shiftButton.gameObject.SetActive(false);
            else
            {
                shiftButton.gameObject.SetActive(!MeetingHud.Instance);
                shiftButton.SetCoolDown(role.ShifterShiftTimer(), CustomGameOptions.ShifterCd);
                Utils.SetTarget(ref role.ClosestPlayer, shiftButton);
            }
        }
    }
}
