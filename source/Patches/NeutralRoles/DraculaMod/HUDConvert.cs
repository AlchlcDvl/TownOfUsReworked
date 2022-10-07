using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.DraculaMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDShift
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateShiftButton(__instance);
        }

        public static void UpdateShiftButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Dracula)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            var biteButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<Dracula>(PlayerControl.LocalPlayer);

            if (isDead)
            {
                biteButton.gameObject.SetActive(false);
             //   shiftButton.isActive = false;
            }
            else
            {
                biteButton.gameObject.SetActive(!MeetingHud.Instance);
               // shiftButton.isActive = !MeetingHud.Instance;
                biteButton.SetCoolDown(role.ConvertTimer(), CustomGameOptions.BiteCd);

                Utils.SetTarget(ref role.ClosestPlayer, biteButton);
            }
        }
    }
}
